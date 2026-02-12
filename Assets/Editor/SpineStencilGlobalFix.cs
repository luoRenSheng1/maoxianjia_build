using UnityEditor;
using UnityEngine;
using Spine.Unity;
using System.IO;

public class SpineUIPanelStencilFix : EditorWindow
{
    [MenuItem("Tools/Fix UIPanel Spine Materials (Stencil=Equal)")]
    public static void FixUIPanelSpineMaterials()
    {
        string targetFolder = "Assets/Editor Default Resources/UIPanel";
        
        // 1. 修复所有材质文件 (.mat)
        string[] materialGuids = AssetDatabase.FindAssets("t:Material", new[] { targetFolder });
        foreach (string guid in materialGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            
            if (mat != null && mat.shader.name.Contains("Spine"))
            {
                mat.SetInt("_StencilComp", 3); // Equal
                mat.SetInt("_StencilRef", 0);  // FGUI遮罩
                EditorUtility.SetDirty(mat);
                Debug.Log($"Fixed material: {path}");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"✅ 已修复 {targetFolder} 下所有 Spine 材质的 Stencil 参数 (Equal=3)");
    }
}