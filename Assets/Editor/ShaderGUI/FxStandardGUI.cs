using System.Collections;
using System;
using UnityEngine;
using UnityEditor;

public class FxStandardGUI : ShaderGUI
{
    private enum BlendMode
    {
        Add,
        Transparent
    }

    private enum FaceMode
    {
        One,
        Two
    }

    private enum ZTestMode
    {
        On,
        Off
    }

    private enum MainTexRGBA
    {
        RGB,
        R,
        G,
        B,
        A
    }

    private enum MaskTexRGBA
    {
        A,
        R,
        G,
        B
        
    }
    private static class Styles
    {
        public static readonly string[] blendNames = Enum.GetNames(typeof(BlendMode));
        public static readonly string[] faceNames = Enum.GetNames(typeof(FaceMode));
        public static readonly string[] ztestNames = Enum.GetNames(typeof(ZTestMode));
        public static GUIContent BlendModeText = new GUIContent("混合模式");
        public static GUIContent FaceModeText = new GUIContent("单双面","双面渲染双倍消耗，尽量选择单面");
        public static GUIContent ZTestModeText = new GUIContent("深度检测");
        public static readonly string[] MaskTexRGBAs = Enum.GetNames(typeof(MaskTexRGBA));

        // public static GUIContent MainTexRGBAText = new GUIContent("MainTex通道");


        public static GUIContent BaseTextureText = new GUIContent("基础贴图");
        public static GUIContent ColorText = new GUIContent("基础颜色");
        //public static GUIContent ColorIntensityText = new GUIContent("颜色强度");
        public static GUIContent AlphaValueText = new GUIContent("Alpha值");
        public static GUIContent USpeedText = new GUIContent("USpeed");
        public static GUIContent VSpeedText = new GUIContent("VSpeed");
        public static GUIContent LerpColorText = new GUIContent("插值颜色");
        public static GUIContent LerpValueText = new GUIContent("插值阈值");
        public static GUIContent DiffuseRotateText = new GUIContent("贴图旋转","性能消耗较大，旋转角度为0时请取消勾选");
        public static GUIContent DiffuseAngleText = new GUIContent("旋转角度");

        public static GUIContent DiffuseMaskText = new GUIContent("基础遮罩");
        public static GUIContent DiffuseMaskTextureText = new GUIContent("遮罩贴图","Mask(A)");
        public static GUIContent MaskUSpeedText = new GUIContent("Mask_USpeed");
        public static GUIContent MaskVSpeedText = new GUIContent("Mask_VSpeed");

        public static string advancedText = "Unity's Advanced Options";
    }

    public static Texture2D WHXSJ_icon;

    MaterialProperty _BlendMode = null;
    MaterialProperty _FaceMode = null;
    MaterialProperty _ZTestMode = null;

    MaterialProperty _Color = null;


    MaterialProperty _MaskTexPopUp = null;

    MaterialProperty _MainTex = null;
    MaterialProperty _AlphaValue = null;
    //MaterialProperty _ColorIntensity = null;
    MaterialProperty _USpeed = null;
    MaterialProperty _VSpeed = null;
    MaterialProperty _LerpColor = null;
    MaterialProperty _LerpValue = null;

    MaterialProperty _DiffuseMask = null;
    MaterialProperty _DiffuseMaskTex = null;
    MaterialProperty _Mask_USpeed = null;
    MaterialProperty _Mask_VSpedd = null;

    // MaterialProperty _DissolutionToggle = null;

    MaterialProperty _Dissolvability = null;
    MaterialProperty _Eclosion = null;

    MaterialProperty _EdgeColor = null;
    MaterialProperty _EdgeWidth = null;

    MaterialEditor m_MaterialEditor;
    bool m_FirstTimeApply = true;

    public void FindProperties(MaterialProperty[] props)
    {
        _BlendMode = FindProperty("_BlendMode", props);
        _FaceMode = FindProperty("_FaceMode", props);
        _ZTestMode = FindProperty("_ZTestMode", props);

        _Color = FindProperty("_Color", props);

        _MaskTexPopUp = FindProperty("_MaskTexPopUp",props);
        _MainTex = FindProperty("_MainTex", props);
        _AlphaValue = FindProperty("_AlphaValue", props);
        //_ColorIntensity = FindProperty("_ColorIntensity", props);
        _USpeed = FindProperty("_USpeed",props);
        _VSpeed = FindProperty("_VSpeed", props);
        _LerpColor = FindProperty("_LerpColor", props);
        _LerpValue = FindProperty("_LerpValue", props);

        _DiffuseMask = FindProperty("_DiffuseMask", props);
        _DiffuseMaskTex = FindProperty("_DiffuseMaskTex", props);
        _Mask_USpeed = FindProperty("_Mask_USpeed", props);
        _Mask_VSpedd = FindProperty("_Mask_VSpeed", props);
    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
    {
        if (WHXSJ_icon== null)
        {
            string[] icons = AssetDatabase.FindAssets("XSJLogo t:Texture2D", null);
            if (icons.Length > 0)
            {
                WHXSJ_icon = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(icons[0]));
            }
        }

        FindProperties(props);
        m_MaterialEditor = materialEditor;
        Material material = materialEditor.target as Material;
        ShaderPropertiesGUI(material);//主要面板显示方法，在后面有定义

        if (m_FirstTimeApply)
        {
            m_FirstTimeApply = false;
        }
    }

    public void ShaderPropertiesGUI(Material material)
    {
        EditorGUIUtility.labelWidth = 0f;
        EditorGUI.BeginChangeCheck();
        {
            EditorGUILayout.BeginVertical(GUILayout.MaxWidth(250));
            BlendModePopup ();
            EditorGUI.EndDisabledGroup();

            if (WHXSJ_icon != null)
            {
                Rect iconRect = GUILayoutUtility.GetLastRect();
                iconRect.y -= 5;
                iconRect.height = WHXSJ_icon.height;;
                iconRect.width = WHXSJ_icon.width;
                iconRect.x = EditorGUIUtility.currentViewWidth - iconRect.width - 15;
                iconRect.x = GUILayoutUtility.GetLastRect().xMax > iconRect.x ? GUILayoutUtility.GetLastRect().xMax : iconRect.x;
                GUI.DrawTexture(iconRect, WHXSJ_icon, ScaleMode.StretchToFill);
            }
            FaceModePopup();
            ZTestModePopup();


            EditorGUILayout.EndVertical();

            //diffuse
            m_MaterialEditor.ShaderProperty(_AlphaValue, Styles.AlphaValueText, 0);
            m_MaterialEditor.ShaderProperty(_Color, Styles.ColorText, 0);
            //m_MaterialEditor.ShaderProperty(_ColorIntensity, Styles.ColorIntensityText, 0);

            EditorGUILayout.BeginHorizontal();
            m_MaterialEditor.TexturePropertySingleLine(Styles.BaseTextureText,_MainTex);
            EditorGUILayout.EndHorizontal();

            m_MaterialEditor.TextureScaleOffsetProperty(_MainTex);

            m_MaterialEditor.ShaderProperty(_USpeed, Styles.USpeedText, 0);
            m_MaterialEditor.ShaderProperty(_VSpeed, Styles.VSpeedText, 0);
            m_MaterialEditor.ShaderProperty(_LerpColor, Styles.LerpColorText, 0);
            m_MaterialEditor.ShaderProperty(_LerpValue, Styles.LerpValueText, 0);
        }
        Color bCol = GUI.backgroundColor;

        //diffuse mask
        GUI.backgroundColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        EditorGUILayout.BeginVertical("Button");
        GUI.backgroundColor = bCol;
        {
            EditorGUI.showMixedValue = _DiffuseMask.hasMixedValue;
            float nval;
            EditorGUI.BeginChangeCheck();
            if (_DiffuseMask.floatValue == 1)
            {
                material.EnableKeyword("_DIFFUSEMASK_ON");
                nval = EditorGUILayout.ToggleLeft(Styles.DiffuseMaskText, _DiffuseMask.floatValue == 1, EditorStyles.boldLabel, GUILayout.Width(EditorGUIUtility.currentViewWidth - 60)) ? 1 : 0;
            }
            else
            {
                material.DisableKeyword("_DIFFUSEMASK_ON");
                material.SetTexture("_DiffuseMaskTex", null);
                nval = EditorGUILayout.ToggleLeft(Styles.DiffuseMaskText, _DiffuseMask.floatValue == 1, EditorStyles.boldLabel) ? 1 : 0;

            }

            if (EditorGUI.EndChangeCheck())
            {
                _DiffuseMask.floatValue = nval;
            }
            EditorGUI.showMixedValue = false;
        }

        //setting
        if (_DiffuseMask.floatValue == 1)
        {
            EditorGUILayout.BeginHorizontal();
            m_MaterialEditor.TexturePropertySingleLine(Styles.DiffuseMaskTextureText ,_DiffuseMaskTex);
            MaskTexRGBAPopup(); 
            EditorGUILayout.EndHorizontal();

            m_MaterialEditor.TextureScaleOffsetProperty(_DiffuseMaskTex);
            m_MaterialEditor.ShaderProperty(_Mask_USpeed, Styles.MaskUSpeedText, 0);
            m_MaterialEditor.ShaderProperty(_Mask_VSpedd, Styles.MaskVSpeedText, 0);
        }
        EditorGUILayout.EndVertical();

        //set mode
        if (EditorGUI.EndChangeCheck())
        {
            foreach (var obj in _BlendMode.targets)
            {
                SetupMaterialWithBlendMode((Material)obj, (BlendMode)_BlendMode.floatValue);
            }
            foreach (var obj in _FaceMode.targets)
            {
                SetupMaterialWithCullMode((Material)obj, (FaceMode)_FaceMode.floatValue);
            }
            foreach (var obj in _ZTestMode.targets)
            {
                SetupMaterialWithZTestMode((Material)obj, (ZTestMode)_ZTestMode.floatValue);
            }

            foreach (var obj in _MaskTexPopUp.targets)
            {
                SetupMaterialWithMaskRGBA((Material)obj, (MaskTexRGBA)_MaskTexPopUp.floatValue);
            }
        }

        //other options
        EditorGUILayout.Space();
        GUILayout.Label(Styles.advancedText, EditorStyles.boldLabel);
        m_MaterialEditor.RenderQueueField();
        m_MaterialEditor.EnableInstancingField();
    }

    void BlendModePopup()
    {
        EditorGUI.showMixedValue = _BlendMode.hasMixedValue;
        var mode = (BlendMode)_BlendMode.floatValue;

        EditorGUI.BeginChangeCheck();
        mode = (BlendMode)EditorGUILayout.Popup(Styles.BlendModeText, (int)mode, Styles.blendNames);
        if (EditorGUI.EndChangeCheck())
        {
            m_MaterialEditor.RegisterPropertyChangeUndo("Rendering Mode");
            _BlendMode.floatValue = (float)mode;

        }
        EditorGUI.showMixedValue = false;
    }

    void FaceModePopup()
    {
        EditorGUI.showMixedValue = _FaceMode.hasMixedValue;
        var mode = (FaceMode)_FaceMode.floatValue;

        EditorGUI.BeginChangeCheck();
        mode = (FaceMode)EditorGUILayout.Popup(Styles.FaceModeText, (int)mode, Styles.faceNames);
        if (EditorGUI.EndChangeCheck())
        {
            m_MaterialEditor.RegisterPropertyChangeUndo("Face Mode");
            _FaceMode.floatValue = (float)mode;
        }

        EditorGUI.showMixedValue = false;

    }
    void ZTestModePopup()
    {
        EditorGUI.showMixedValue = _ZTestMode.hasMixedValue;
        var mode = (ZTestMode)_ZTestMode.floatValue;

        EditorGUI.BeginChangeCheck();
        mode = (ZTestMode)EditorGUILayout.Popup(Styles.ZTestModeText, (int)mode, Styles.ztestNames);
        if (EditorGUI.EndChangeCheck())
        {
            m_MaterialEditor.RegisterPropertyChangeUndo("ZTest");
            _ZTestMode.floatValue = (float)mode;
        }

        EditorGUI.showMixedValue = false;
    }


    void MaskTexRGBAPopup()
    {
        EditorGUI.showMixedValue = _MaskTexPopUp.hasMixedValue;
        var mode = (MaskTexRGBA)_MaskTexPopUp.floatValue;

        EditorGUI.BeginChangeCheck();
        mode = (MaskTexRGBA)EditorGUILayout.Popup( (int)mode, Styles.MaskTexRGBAs);
        if (EditorGUI.EndChangeCheck())
        {
            m_MaterialEditor.RegisterPropertyChangeUndo("MaskTexMaskMode");
            _MaskTexPopUp.floatValue = (float)mode;
        }
        EditorGUI.showMixedValue = false;
    }
    static void SetupMaterialWithBlendMode(Material material, BlendMode blendMode)
    {
        switch (blendMode)
        {
            case BlendMode.Add:
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                break;
            case BlendMode.Transparent:
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                break;
        }
    }

    static void SetupMaterialWithCullMode(Material material, FaceMode cullMode)
    {
        switch (cullMode)
        {
            case FaceMode.One:
                material.SetInt("_CullMode", (int)UnityEngine.Rendering.CullMode.Back);
                break;
            case FaceMode.Two:
                material.SetInt("_CullMode", (int)UnityEngine.Rendering.CullMode.Off);
                break;
        }
    }

    static void SetupMaterialWithZTestMode(Material material, ZTestMode ztest)
    {
        switch (ztest)
        {
            case ZTestMode.On:
                material.SetInt("_ZTest", 4);
                break;
            case ZTestMode.Off:
                material.SetInt("_ZTest", 8);
                break;
        }
    }

    static void SetupMaterialWithMaskRGBA(Material material, MaskTexRGBA maskrgba)
    {
        switch (maskrgba)
        {
            case MaskTexRGBA.R:
                material.SetVector("_MaskTexRGBA", new Vector4(1,0,0,0));
                break;
            case MaskTexRGBA.G:
                material.SetVector("_MaskTexRGBA", new Vector4(0,1,0,0));
                break;
            case MaskTexRGBA.B:
                material.SetVector("_MaskTexRGBA", new Vector4(0,0,1,0));
                break;
            case MaskTexRGBA.A:
                material.SetVector("_MaskTexRGBA", new Vector4(0,0,0,1));
                break;
        }
    }
}
