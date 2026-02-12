using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Editor.build
{
    class FindBuildinResources : EditorWindow
    {
        [MenuItem("Tools/T. Tools/检查buildin资源")]
        public static void FindResource()
        {
            GetWindow<FindBuildinResources>().Show();
            GetBuildinResource();
        }

        private Vector3 scrollPos = Vector3.zero;
        private static Dictionary<UnityEngine.Object, Node> res = new Dictionary<UnityEngine.Object, Node>();
        private const string shader = "shader";
        private const string texture = "texture";
        private const string material = "material";
        private const string sprite = "sprite";
        private const string prefab = "prefab";
        private const string renderer = "renderer";
        private const string image = "image";
        private const string builtin = "builtin";

        /// <summary>
        /// 加载 buildin资源
        /// </summary>
        private static void GetBuildinResource()
        {
            res.Clear();
            string path = "Assets/";
            var allfiles = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(
                s => s.EndsWith("mat")
                || s.EndsWith("prefab")
                ).ToArray();
            foreach (var item in allfiles)
            {
                if (item.EndsWith("prefab"))
                {
                    GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(item);
                    if (go)
                    {
                        #region 找到prefab里的 buildin shader & material & texture
                        Renderer[] renders = go.GetComponentsInChildren<Renderer>(true);
                        foreach (var render in renders)
                        {
                            foreach (var mat in render.sharedMaterials)
                            {
                                if (!mat) continue;
                                //判断材质是不是用的builtin的
                                if (AssetDatabase.GetAssetPath(mat).Contains(builtin))
                                {
                                    Node n;
                                    if (res.Keys.Contains(go)) n = res[go];
                                    else
                                    {
                                        n = new Node(go, prefab);
                                        res.Add(go, n);
                                    }
                                    n.Add(render, renderer).Add(mat, material);
                                }
                                //判断shader是不是builtin的
                                if (AssetDatabase.GetAssetPath(mat.shader).Contains(builtin))
                                {
                                    Node n;
                                    if (res.Keys.Contains(go)) n = res[go];
                                    else
                                    {
                                        n = new Node(go, prefab);
                                        res.Add(go, n);
                                    }
                                    n.Add(render, renderer).Add(mat, material).Add(mat.shader, shader);
                                }
                                //判断shader用的贴图是不是用的builtin的
                                for (int i = 0; i < ShaderUtil.GetPropertyCount(mat.shader); i++)
                                {
                                    if (ShaderUtil.GetPropertyType(mat.shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                                    {
                                        string propertyname = ShaderUtil.GetPropertyName(mat.shader, i);
                                        Texture t = mat.GetTexture(propertyname);
                                        if (t && AssetDatabase.GetAssetPath(t).Contains(builtin))
                                        {
                                            Node n;
                                            if (res.Keys.Contains(go)) n = res[go];
                                            else
                                            {
                                                n = new Node(go, prefab);
                                                res.Add(go, n);
                                            }
                                            n.Add(render, renderer).Add(mat, material).Add(t, texture);
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                        #region 找到prefab里的 buildin Sprite
                        Image[] images = go.GetComponentsInChildren<Image>(true);
                        foreach (var img in images)
                        {
                            if (AssetDatabase.GetAssetPath(img.sprite).Contains(builtin))
                            {
                                Node n;
                                if (res.Keys.Contains(go)) n = res[go];
                                else
                                {
                                    n = new Node(go, prefab);
                                    res.Add(go, n);
                                }
                                n.Add(img, "image").Add(img.sprite, sprite);
                            }
                        }

                        #endregion
                        #region 找到prefab 里的Texture
                        RawImage[] rawimgs = go.GetComponentsInChildren<RawImage>(true);
                        foreach (var rawimg in rawimgs)
                        {
                            if (rawimg.texture && AssetDatabase.GetAssetPath(rawimg.texture).Contains(builtin))
                            {
                                Node n;
                                if (res.Keys.Contains(go)) n = res[go];
                                else
                                {
                                    n = new Node(go, prefab);
                                    res.Add(go, n);
                                }
                                n.Add(rawimg, "rawimage").Add(rawimg.texture, texture);
                            }
                        }
                        #endregion
                    }
                }
                else if (item.EndsWith("mat"))
                {
                    #region 找到material里的 shader
                    Material mt = AssetDatabase.LoadAssetAtPath<Material>(item);
                    if (!mt) continue;
                    if (AssetDatabase.GetAssetPath(mt.shader).Contains(builtin))
                    {
                        Node n;
                        if (res.Keys.Contains(mt)) n = res[mt];
                        else
                        {
                            n = new Node(mt, material);
                            res.Add(mt, n);
                        }
                        n.Add(mt.shader, shader);
                    }
                    #endregion
                    #region 找到material里的 texutre
                    for (int i = 0; i < ShaderUtil.GetPropertyCount(mt.shader); i++)
                    {
                        if (ShaderUtil.GetPropertyType(mt.shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                        {
                            string propertyname = ShaderUtil.GetPropertyName(mt.shader, i);
                            Texture t = mt.GetTexture(propertyname);
                            if (t && AssetDatabase.GetAssetPath(t).Contains(builtin))
                            {
                                Node n;
                                if (res.Keys.Contains(mt)) n = res[mt];
                                else
                                {
                                    n = new Node(mt, material);
                                    res.Add(mt, n);
                                }
                                n.Add(t, sprite);
                            }
                        }
                    }
                    #endregion
                }
            }
            EditorUtility.DisplayDialog("", "就绪", "OK");
        }
        /// <summary>
        /// 将standard 替换成Mobile Diffuse
        /// </summary>
        private static void ReplaceStandardToDiffuse()
        {
            Shader sd = Shader.Find("Standard");
            Shader diffuse_sd = Shader.Find("Mobile/Diffuse");
            int count = 0;
            foreach (var item in res.Values)
            {
                TransforNode(item, (s) =>
                {
                    if (s.des == material)
                    {
                        Material mt = s.content as Material;
                        if (mt && mt.shader == sd)
                        {
                            mt.shader = diffuse_sd;
                            count++;
                        }
                    }
                });
            }
            EditorUtility.DisplayDialog("结果", "替换了" + count + "个Standard shader", "OK");
            if (count != 0)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                GetBuildinResource();
            }
        }
        /// <summary>
        /// 将buildin shader 替换成本地shader
        /// </summary>
        private void ReplaceBuildinToLocal()
        {
            int count = 0;
            foreach (var item in res.Values)
            {
                TransforNode(item, (s) =>
                {
                    if (s.des == material)
                    {
                        Material mt = s.content as Material;
                        if (mt)
                        {
                            mt.shader = Shader.Find(mt.shader.name);
                            count++;
                        }
                    }
                });
            }
            EditorUtility.DisplayDialog("结果", "替换了" + count + "个buildin shader", "OK");
            if (count != 0)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                GetBuildinResource();
            }
        }
        /// <summary>
        /// 替换默认材质
        /// </summary>
        private void ReplaceDefaultMaterial()
        {
            string materialName = "Default-Material";
            string[] x = Directory.GetFiles("Assets/Resources/", materialName + ".mat", SearchOption.AllDirectories);
            if (x.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "Resource/misc/下没有" + materialName + "!!!", "OK");
                return;
            }
            Material defaultMaterial = AssetDatabase.LoadAssetAtPath<Material>(x[0]);
            int count = 0;
            foreach (var item in res.Values)
            {
                TransforNode(item, (s) =>
                {
                    if (s.des == renderer)
                    {
                        Renderer render = s.content as Renderer;
                        if (render)
                        {
                            Material[] mats = render.sharedMaterials;
                            for (int i = 0; i < mats.Length; i++)
                            {
                                if (mats[i].name == materialName)
                                {
                                    Material mt = defaultMaterial as Material;
                                    if (mt)
                                    {
                                        mats[i] = mt;
                                        count++;
                                    }
                                }
                            }
                            render.sharedMaterials = mats;
                        }
                    }
                });
            }
            EditorUtility.DisplayDialog("提示", "替换了" + count + "个" + materialName, "OK");
            if (count != 0)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                GetBuildinResource();
            }
        }
        /// <summary>
        /// 移除使用默认材质的ParticleSystem组件
        /// </summary>
        private void RemoveParticleSystemWithDefaultParticle()
        {
            int count = 0;
            foreach (var item in res.Values)
            {
                TransforNode(item, (s) =>
                {
                    if (s.des == renderer)
                    {
                        Renderer render = s.content as Renderer;
                        if (render)
                        {
                            Material[] mats = render.sharedMaterials;
                            for (int i = 0; i < mats.Length; i++)
                            {
                                if (mats[i].name == "Default-Particle")
                                {
                                    ParticleSystem ps = render.GetComponent<ParticleSystem>();
                                    if (ps)
                                    {
                                        render.materials = new Material[] { };
                                        DestroyImmediate(ps, true);
                                        EditorUtility.SetDirty(render.gameObject);
                                        count++;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                });
            }
            EditorUtility.DisplayDialog("提示", "Remove" + count + "个ParticleSystem组件", "OK");
            if (count != 0)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                GetBuildinResource();
            }
        }
        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("将所有Standard shader 替换成MobileDiffuse  ", GUILayout.Width(400), GUILayout.Height(50))) ReplaceStandardToDiffuse();
            if (GUILayout.Button("将所有buildin shader 替换成本地shader  ", GUILayout.Width(400), GUILayout.Height(50))) ReplaceBuildinToLocal();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("替换所有Default Material", GUILayout.Width(400), GUILayout.Height(50))) ReplaceDefaultMaterial();
            if (GUILayout.Button("移除所有带Default Particle的ParticleSystem组件", GUILayout.Width(400), GUILayout.Height(50))) RemoveParticleSystemWithDefaultParticle();
            EditorGUILayout.EndHorizontal();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(850));
            EditorGUILayout.BeginVertical();
            foreach (var item in res.Keys)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item, item.GetType(), true, GUILayout.Width(200));
                TransforNode(res[item]);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();

        }


        /// <summary>
        /// 遍历显示
        /// </summary>
        /// <param name="n"></param>
        private static void TransforNode(Node n)
        {
            EditorGUILayout.BeginVertical();
            foreach (var item in n.next.Values)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item.content, item.content.GetType(), true, GUILayout.Width(200));
                TransforNode(item);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 遍历 操作
        /// </summary>
        /// <param name="n"></param>
        /// <param name="a"></param>
        private static void TransforNode(Node n, Action<Node> a)
        {
            a(n);
            foreach (var item in n.next.Values)
            {
                a(item);
                TransforNode(item, a);
            }
        }


    }
    public class Node
    {
        public UnityEngine.Object content;
        public string des;
        public Dictionary<UnityEngine.Object, Node> next;
        public Node Add(UnityEngine.Object obj, string type)
        {
            if (!next.Keys.Contains(obj))
            {
                Node no = new Node(obj, type);
                next.Add(obj, no);
                return no;
            }
            return next[obj];
        }
        public Node(UnityEngine.Object content, string des)
        {
            this.content = content;
            this.des = des;
            next = new Dictionary<UnityEngine.Object, Node>();
        }
        public void TransforNode(Action<Node> a)
        {
            a(this);
            foreach (var item in next.Values)
            {
                a(item);
                TransforNode(a);
            }
        }
    }
}