#pragma warning disable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class AnimationClipExtract : EditorWindow
{
    private const float AnimationPositionError = 0.2f;
    private const float AnimationRotationError = 0.1f;
    //ModelImporterAnimationCompression--动画压缩选项
    //Optimal--执行减少关键帧的操作，并在运行时选择最佳动画曲线表示以减少内存占用（默认）。
    private const ModelImporterAnimationCompression Compression = ModelImporterAnimationCompression.Optimal;
    private const int DecimalAccuracy = 10000;

    // [MenuItem("Tools/AnimationClip提取")]
    static void RightBtn()
    {
        ProcessFbx(true);
    }

    public static void ProcessFbx(bool reProcess = false)
    {
        mAnimationClipList.Clear();

        //获取工具路径
        List<string> assets = new List<string>()/*ToolUtility.GetAssetPath()*/;
        int index = 0;
        int count = assets.Count;

        foreach (var ass in assets)
        {
            //显示或更新含有 Cancel 按钮的进度条
            bool isCancel = EditorUtility.DisplayCancelableProgressBar("提取AnimationClip", string.Format("正在提取:{0}/{1}", ++index, count), (float)index / count);
            if (isCancel)
            {
                EditorUtility.ClearProgressBar();
                return;
            }
            //Path.GetExtension    返回指定路径字符串的扩展名（包括句点“.”）
            if (Path.GetExtension(ass).ToLower().Contains(".fbx"))
            {
                ExtractAnimationClip(ass, reProcess);
            }
        }
        //清理进度条
        EditorUtility.ClearProgressBar();
    }

    public static List<string> mAnimationClipList = new List<string>();
    //提取动画片段
    private static string ExtractAnimationClip(string fbxPath, bool reProcess = false)
    {
        //表示空字符串,此字段为只读
        string folderPath = string.Empty;

        //返回一个值，该值指示指定的字符是否出现在此字符串中。
        if (fbxPath.Contains("@"))
        {
            //AssetDatabase-可用于访问项目中包含的资源
            //LoadAllAssetsAtPath在加载的时候， 从脚本加载和访问资源
            Object[] objs = AssetDatabase.LoadAllAssetsAtPath(fbxPath);
            int index = 0;
            foreach (var obj in objs)
            {
                //AnimationClip-保存基于关键帧的动画
                if (obj is AnimationClip)
                {
                    //预览和第一帧就跳出
                    if (obj.name == "__preview__Take 001")
                        continue;
                    if (obj.name.Contains("__preview__"))
                        continue;
                    if (index++ == 0)
                    {
                        //GetDirectoryName--返回指定路径的目录信息
                        folderPath = Path.GetDirectoryName(fbxPath) + "/Clips";
                        //Directory.Exists-确定给定路径是否引用磁盘上的现有目录。
                        if (!Directory.Exists(folderPath))
                        {
                            //没有就创建
                            Directory.CreateDirectory(folderPath);
                        }
                    }
                    AnimationClip clip = (AnimationClip)obj;
                    string _filePath = folderPath + "/" + clip.name + ".anim";
                    if (reProcess)
                    {
                        if (File.Exists(_filePath))
                        {
                            mAnimationClipList.Add(_filePath);
                            continue;
                        }
                    }
                    if (!OptimizeAnimationCurveData(clip, folderPath))
                    {
                        Debug.LogError("动画文件异常,导出动画失败：" + fbxPath + "  " + clip.name);
                    }
                    mAnimationClipList.Add(_filePath);
                }
            }
        }
        return folderPath;
    }

    /// <summary>
    /// 优化动画片段
    /// 1.删除不需要的序列帧
    /// 2.降低帧信息的精度(缩短动画曲线数据产生的浮点数精度)
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="folderPath"></param>
    /// <returns></returns>
    public static bool OptimizeAnimationCurveData(AnimationClip clip, string folderPath)
    {
        if (clip == null)
        {
            return false;
        }

        //AnimationUtility.GetAllCurves从特定动画剪辑中检索所有曲线
        var curveDatas = AnimationUtility.GetAllCurves(clip, true);
        if (curveDatas == null || curveDatas.Length == 0)
        {
            return false;
        }

        AnimationClip newClip = new AnimationClip();
        //复制 Unity Object 的所有设置
        EditorUtility.CopySerialized(clip, newClip);


        newClip.name = clip.name;
        //ClearCurves 清除该剪辑中的所有曲线
        newClip.ClearCurves();

        foreach (var dt in curveDatas)
        {
            var nodeName = dt.path.ToLower().Split('/').Last();

            //缩短动画曲线数据产生的浮点数精度
            var keys = dt.curve.keys;
            for (var i = 0; i < keys.Length; i++)
            {
                //如果数字结尾是 .5，从而使它处于两个整数正中间（其中一个是偶数，另一个是奇数），则返回偶数
                keys[i].time = Mathf.Round(keys[i].time * DecimalAccuracy) / DecimalAccuracy;
                keys[i].value = Mathf.Round(keys[i].value * DecimalAccuracy) / DecimalAccuracy;
                //曲线正切值
                keys[i].outTangent = Mathf.Round(keys[i].outTangent * DecimalAccuracy) / DecimalAccuracy;
                keys[i].inTangent = Mathf.Round(keys[i].inTangent * DecimalAccuracy) / DecimalAccuracy;
            }

            //过滤位移值没有变化的帧动画
            //因为帧信息有初始位置，所有要保留头尾两帧，如果全部删除会出现初始位置为默认值的问题
            if (IsFilterApproximateKeyFrame(ref keys))
            {
                //创建关键帧
                var newKeys = new Keyframe[2];
                newKeys[0] = keys[0];
                newKeys[1] = keys[keys.Length - 1];
                keys = newKeys;
            }
            dt.curve.keys = keys;
            //设置新数据
            newClip.SetCurve(dt.path, dt.type, dt.propertyName, dt.curve);
        }

        //在此路径下创建一个新资源。
        AssetDatabase.CreateAsset(newClip, folderPath + @"/" + newClip.name + ".anim");
        //导入所有更改的资源。
        AssetDatabase.Refresh();


        return true;
    }

    /// <summary>
    /// 动画默认不导出Scale序列帧，除非该节点包含scale关键词(加scale关键词表示该节点需要进行scale变换)
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    private static bool IsFilterCurveData(AnimationClipCurveData dt, string nodeName)
    {
        if (dt.propertyName.ToLower().Contains("scale") && !nodeName.Contains("scale"))
            return true;
        return false;
    }

    /// <summary>
    /// 过滤值一样的序列帧
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    private static bool IsFilterApproximateKeyFrame(ref Keyframe[] keys)
    {
        for (var i = 0; i < keys.Length - 1; i++)
        {
            //Mathf.Abs 返回 f 的绝对值
            if (Mathf.Abs(keys[i].value - keys[i + 1].value) > 0 ||
                Mathf.Abs(keys[i].outTangent - keys[i + 1].outTangent) > 0
                || Mathf.Abs(keys[i].inTangent - keys[i + 1].inTangent) > 0)
            {
                return false;
            }
        }
        return true;
    }

    private static string GetNewClipDir(string clipPath)
    {
        int index = clipPath.LastIndexOf("/");
        if (index < 0)
        {
            index = clipPath.LastIndexOf("\\");
        }
        string ret = clipPath.Substring(0, index + 1);

        string newFolder = string.Format("{0}\\Clips", ret);
        if (!Directory.Exists(newFolder))
        {
            Directory.CreateDirectory(newFolder);
        }
        return newFolder;
    }

}
#pragma warning restore