using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(Transform), true)]
public class TransformInspector : Editor
{
    SerializedProperty mPos;
    SerializedProperty mRot;
    SerializedProperty mScale;

    void OnEnable()
    {
        if (this)
        {
            try
            {
                var so = serializedObject;
                mPos = so.FindProperty("m_LocalPosition");
                mRot = so.FindProperty("m_LocalRotation");
                mScale = so.FindProperty("m_LocalScale");
            }
            catch { }
        }
    }

    static public void SetLabelWidth(float width)
    {
        EditorGUIUtility.labelWidth = width;
    }

    [System.Diagnostics.DebuggerHidden]
    [System.Diagnostics.DebuggerStepThrough]
    static public float WrapAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }

    static public void RegisterUndo(string name, params Object[] objects)
    {
        if (objects != null && objects.Length > 0)
        {
            UnityEditor.Undo.RecordObjects(objects, name);

            foreach (Object obj in objects)
            {
                if (obj == null) continue;
                EditorUtility.SetDirty(obj);
            }
        }
    }

    /// <summary>
    /// Draw the inspector widget.
    /// </summary>

    public override void OnInspectorGUI()
    {
        SetLabelWidth(15f);

        serializedObject.Update();

        DrawPosition();
        DrawRotation();
        DrawScale();
        //DrawTool();

        serializedObject.ApplyModifiedProperties();
    }

    void DrawPosition()
    {
        GUILayout.BeginHorizontal();
        bool reset = GUILayout.Button("P", GUILayout.Width(20f));
        EditorGUILayout.PropertyField(mPos.FindPropertyRelative("x"));
        EditorGUILayout.PropertyField(mPos.FindPropertyRelative("y"));
        EditorGUILayout.PropertyField(mPos.FindPropertyRelative("z"));
        GUILayout.EndHorizontal();

        if (reset) mPos.vector3Value = Vector3.zero;
    }

    void DrawScale()
    {
        GUILayout.BeginHorizontal();
        bool reset = GUILayout.Button("S", GUILayout.Width(20f));
        EditorGUILayout.PropertyField(mScale.FindPropertyRelative("x"));
        EditorGUILayout.PropertyField(mScale.FindPropertyRelative("y"));
        EditorGUILayout.PropertyField(mScale.FindPropertyRelative("z"));
        if (reset) mScale.vector3Value = Vector3.one;
        GUILayout.EndHorizontal();
    }

    void DrawTool()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Copy Int", GUILayout.Width(100f)))
        {
            var vRot = mRot.quaternionValue.eulerAngles;
            string strValue = string.Format("{0:D},{1:D},{2:D}\r\n{3:D},{4:D},{5:D}\r\n{6:D},{7:D},{8:D}\r\n",
                Mathf.RoundToInt(mPos.vector3Value.x),
                Mathf.RoundToInt(mPos.vector3Value.y),
                Mathf.RoundToInt(mPos.vector3Value.z),
                Mathf.RoundToInt(vRot.x),
                Mathf.RoundToInt(vRot.y),
                Mathf.RoundToInt(vRot.z),
                Mathf.RoundToInt(mScale.vector3Value.x),
                Mathf.RoundToInt(mScale.vector3Value.y),
                Mathf.RoundToInt(mScale.vector3Value.z));
            UnityEngine.GUIUtility.systemCopyBuffer = strValue;
        }
        if (GUILayout.Button("Copy Float", GUILayout.Width(100f)))
        {
            var vRot = mRot.quaternionValue.eulerAngles;
            string strValue = string.Format("{0:F4},{1:F4},{2:F4}\r\n{3:F4},{4:F4},{5:F4}\r\n{6:F4},{7:F4},{8:F4}\r\n",
                mPos.vector3Value.x, mPos.vector3Value.y, mPos.vector3Value.z,
                vRot.x, vRot.y, vRot.z,
                mScale.vector3Value.x, mScale.vector3Value.y, mScale.vector3Value.z);
            UnityEngine.GUIUtility.systemCopyBuffer = strValue;
        }
        GUILayout.EndHorizontal();
    }

    enum Axes : int
    {
        None = 0,
        X = 1,
        Y = 2,
        Z = 4,
        All = 7,
    }

    Axes CheckDifference(Transform t, Vector3 original)
    {
        Vector3 next = t.localEulerAngles;

        Axes axes = Axes.None;

        if (Differs(next.x, original.x)) axes |= Axes.X;
        if (Differs(next.y, original.y)) axes |= Axes.Y;
        if (Differs(next.z, original.z)) axes |= Axes.Z;

        return axes;
    }

    Axes CheckDifference(SerializedProperty property)
    {
        Axes axes = Axes.None;

        if (property.hasMultipleDifferentValues)
        {
            Vector3 original = property.quaternionValue.eulerAngles;

            foreach (Object obj in serializedObject.targetObjects)
            {
                axes |= CheckDifference(obj as Transform, original);
                if (axes == Axes.All) break;
            }
        }
        return axes;
    }

    /// <summary>
    /// Draw an editable float field.
    /// </summary>
    /// <param name="hidden">Whether to replace the value with a dash</param>
    /// <param name="greyedOut">Whether the value should be greyed out or not</param>

    static bool FloatField(string name, ref float value, bool hidden, bool greyedOut, GUILayoutOption opt)
    {
        float newValue = value;
        GUI.changed = false;

        if (!hidden)
        {
            if (greyedOut)
            {
                GUI.color = new Color(0.7f, 0.7f, 0.7f);
                newValue = EditorGUILayout.FloatField(name, newValue, opt);
                GUI.color = Color.white;
            }
            else
            {
                newValue = EditorGUILayout.FloatField(name, newValue, opt);
            }
        }
        else if (greyedOut)
        {
            GUI.color = new Color(0.7f, 0.7f, 0.7f);
            float.TryParse(EditorGUILayout.TextField(name, "--", opt), out newValue);
            GUI.color = Color.white;
        }
        else
        {
            float.TryParse(EditorGUILayout.TextField(name, "--", opt), out newValue);
        }

        if (GUI.changed && Differs(newValue, value))
        {
            value = newValue;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Because Mathf.Approximately is too sensitive.
    /// </summary>

    static bool Differs(float a, float b) { return Mathf.Abs(a - b) > 0.0001f; }

    void DrawRotation()
    {
        GUILayout.BeginHorizontal();
        bool reset = GUILayout.Button("R", GUILayout.Width(20f));

        Vector3 visible = (serializedObject.targetObject as Transform).localEulerAngles;

        visible.x = WrapAngle(visible.x);
        visible.y = WrapAngle(visible.y);
        visible.z = WrapAngle(visible.z);

        Axes changed = CheckDifference(mRot);
        Axes altered = Axes.None;

        GUILayoutOption opt = GUILayout.MinWidth(30f);

        if (FloatField("X", ref visible.x, (changed & Axes.X) != 0, false, opt)) altered |= Axes.X;
        if (FloatField("Y", ref visible.y, (changed & Axes.Y) != 0, false, opt)) altered |= Axes.Y;
        if (FloatField("Z", ref visible.z, (changed & Axes.Z) != 0, false, opt)) altered |= Axes.Z;

        if (reset)
        {
            mRot.quaternionValue = Quaternion.identity;
        }
        else if (altered != Axes.None)
        {
            RegisterUndo("Change Rotation", serializedObject.targetObjects);

            foreach (Object obj in serializedObject.targetObjects)
            {
                Transform t = obj as Transform;
                Vector3 v = t.localEulerAngles;

                if ((altered & Axes.X) != 0) v.x = visible.x;
                if ((altered & Axes.Y) != 0) v.y = visible.y;
                if ((altered & Axes.Z) != 0) v.z = visible.z;

                t.localEulerAngles = v;
            }
        }
        GUILayout.EndHorizontal();
    }
}
