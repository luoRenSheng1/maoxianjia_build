using BigMap;
using Common;
using Engine;
using FairyGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class UICoinEffect : MonoBehaviour
{
    [Tooltip("硬币资源类型")]
    public int coinType = 0;
    
    [Tooltip("爆炸中心点")]
    public Vector2 explosionPoint;
    
    [Tooltip("目标角色")]
    public GObject targetCharacter;
    
    [Tooltip("一次爆炸的金币数量")]
    public int coinCount = 13;
    
    [Tooltip("金币飞行速度")]
    public float flySpeed = 550f;
    
    [Tooltip("爆炸范围")]
    public float explosionRadius = 150f;


    private GComponent mainUI;
    private List<GObject> activeCoins = new List<GObject>();
    private List<Coroutine> coroutineList = new List<Coroutine>();

    private int count;
    /// <summary>
    /// 目标点的偏移锚点位置
    /// </summary>
    public Vector2 mp = Vector2.zero;
    /// <summary>
    /// 金币绑定的父节点
    /// </summary>
    public GObject root;
    /// <summary>
    /// 飞入到身上第一次的回调
    /// </summary>
    public Action callback = null;
    /// <summary>
    /// 完成后回调
    /// </summary>
    public Action<UICoinEffect> overCallback = null;

    void Start()
    {
        mainUI = GetComponent<UIPanel>().ui;
        root = GRoot.inst;
    }

    // 触发金币爆炸效果
    public void ExplodeCoins(int type, Vector2 begPos, GObject endTarget)
    {
        if(endTarget == null || endTarget.isDisposed) { return; }
        coinType = type;
        explosionPoint = begPos;
        targetCharacter = endTarget;
        count = 0;

        for (int i = 0; i < coinCount; i++)
        {
            // 创建金币
            GObject coin = CreateCoin();
            // 添加到舞台
            root.asCom.AddChild(coin);
            //coin.sortingOrder = 1000;

            // 设置随机爆炸位置
            //Vector2 randomPos = GetRandomExplosionPosition();
            Vector2 randomPos = GetRandomPointInEllipse(begPos, explosionRadius, explosionRadius * 0.5f, Random.Range(0, 1) * Mathf.Deg2Rad);
            randomPos.x += Random.Range(-10, 11);
            randomPos.y += Random.Range(-10, 11);
            Drop(coin, coin.position, randomPos, 10f, 0.3f, (GTweener t) =>
            {
                ++count;
                if (count >= coinCount)
                {
                    //开始所有金币飞行动画
                    foreach (var c in activeCoins)
                    {
                        if(c != null)
                        {
                            coroutineList.Add(GameManager.Instance.StartCoroutine(FlyToTarget(c)));
                        }
                    }
                }
            });
        }
    }

    private GObject SpawnFrag(Vector2 pos)
    {
        GObject coin = null;
        switch (coinType)
        {
            case 0:
                coin = UI_Gold.CreateInstance();
            break;
            case 1:
                UI_DiamondChip c = UI_DiamondChip.CreateInstance();
                //随机一个碎片纹理
                if(Random.Range(0, 10) < 9)
                {
                    c.icon1.visible = true;
                    c.icon2.visible = false;
                }
                else
                {
                    c.icon1.visible = false;
                    c.icon2.visible = true;
                }
                coin = c;
            break;
        }
        if (coin != null)
        {
            GRoot.inst.AddChild(coin);
            coin.xy = pos;
        }

        return coin;
    }

    // 创建单个金币
    private GObject CreateCoin()
    {
        GObject coin = SpawnFrag(explosionPoint);
        coin.touchable = false;
        coin.SetPivot(0.5f, 0.5f, true);
        activeCoins.Add(coin);
        return coin;
    }

    // 获取随机爆炸位置
    private Vector2 GetRandomExplosionPosition()
    {
        // 基于爆炸中心点生成随机位置
        float angle = Random.Range(0, 360);
        float distance = Random.Range(0, explosionRadius);

        float x = explosionPoint.x + Mathf.Cos(angle * Mathf.Deg2Rad) * distance;
        float y = explosionPoint.y + Mathf.Sin(angle * Mathf.Deg2Rad) * distance;
        
        return new Vector2(x, y);
    }

    // 金币飞向目标的动画
    private IEnumerator FlyToTarget(GObject coin)
    {
        var view = UIManager.Instance.FindByName("ChapterMap") as ChapterMapView;
        if(view == null || !view.IsShow() || !view.IsOnStage() || coin == null || coin.isDisposed || coin.parent == null || targetCharacter == null)
        {
            this.Destroy();
            yield break;
        }
        // 等待一小段时间，让爆炸效果更自然
        yield return new WaitForSeconds(Random.Range(0.5f, 0.8f));

        Vector2 pos1 = coin.LocalToGlobal(Vector2.zero);
        pos1.x += coin.width * 0.5f;
        pos1.y += coin.height * 0.5f;
        Vector2 pos2 = targetCharacter.LocalToGlobal(Vector2.zero);
        pos2.x += targetCharacter.width * mp.x;
        pos2.y += targetCharacter.height * mp.y;

        float dis0 = Vector2.Distance(pos1, pos2);
        float rotation = 0;

        // 飞行动画
        while (Vector2.Distance(pos1, pos2) > 50f)
        {
            // 计算方向
            Vector2 direction = (pos2 - pos1).normalized;
            float dis = Vector2.Distance(pos1, pos2);

            float sp = (dis0 / dis) * flySpeed;
            sp = Math.Max(300, sp);
            // (直线)移动金币
            coin.xy += direction * sp * Time.deltaTime;

            // 旋转对象
            rotation += Time.deltaTime * 360;
            if (rotation >= 360){rotation = 0;}
            coin.rotation = rotation;
            //Debug.Log(coin.rotation);

            // 缩放金币，使其逐渐变大
            float scale = Mathf.Min(1f, dis / explosionRadius);
            scale = Mathf.Max(0.1f, scale);
            coin.scale = new Vector2(scale, scale);

            pos1 = coin.LocalToGlobal(Vector2.zero);
            pos1.x += coin.width * 0.5f;
            pos1.y += coin.height * 0.5f;
            pos2 = targetCharacter.LocalToGlobal(Vector2.zero);
            pos2.x += targetCharacter.width * mp.x;
            pos2.y += targetCharacter.height * mp.y;
            yield return null;
        }
        
        // 到达目标后消失
        OnCoinArrived(coin);
    }

    // 金币到达目标后的处理
    private void OnCoinArrived(GObject coin)
    {
        // 从列表中移除
        activeCoins.Remove(coin);
        
        // 销毁金币
        coin.Dispose();

        if (callback != null)
        {// 回调只触发一次
            callback?.Invoke();
            callback = null;
        }

        if (activeCoins.Count <= 0)
        {
            this.Destroy();
            if (overCallback != null)
            {// 回调只触发一次
                overCallback?.Invoke(this);
                overCallback = null;
            }
        }
    }

    /// <summary>
    /// 让FGUI对象沿抛物线掉落
    /// </summary>
    /// <param name="target">目标对象</param>
    /// <param name="startPos">起始位置</param>
    /// <param name="endPos">终点位置（地面）</param>
    /// <param name="height">抛物线高度</param>
    /// <param name="duration">动画时长</param>
    /// <param name="completeCallback">完成回调</param>
    public void Drop(
        GObject target,
        Vector2 startPos,
        Vector2 endPos,
        float height,
        float duration,
        Action<GTweener> completeCallback = null)
    {
        var view = UIManager.Instance.FindByName("ChapterMap") as ChapterMapView;
        if (view == null || !view.IsShow() || !view.IsOnStage())
        {
            return;
        }
        if (target == null || target.isDisposed || target.parent == null) { return; }
        target.SetScale(0.1f, 0.1f);
        float rotation = Random.Range(0, 360);
        target.rotation = rotation;
        // 记录起始时间
        float startTime = Time.time;
        float time = 1f;
        //计算目标点与中心点的Y轴比例
        float ySacle = (endPos.y - (startPos.y - explosionRadius * 0.60f)) / explosionRadius;
        //Debug.Log("碎片倍率：" + ySacle + " y = " + endPos.y);
        // 创建自定义Tween
        GTween.To(0, time, duration)
            .SetEase(EaseType.QuadIn) // 下落加速效果
            .OnUpdate((GTweener t) =>
            {
                if(target == null || target.isDisposed) { return; }
                // 计算当前动画进度
                float progress = (float)t.value.d;

                // 计算抛物线位置
                Vector2 position = CalculateParabolaPosition(startPos, endPos, height, progress);
                //Debug.Log(position);
                // 更新对象位置
                target.position = position;

                ySacle = Math.Min(1f, ySacle);
                ySacle = Math.Max(0.5f, ySacle);
                float s = progress / time * ySacle;
                s = Math.Min(s, 1f);
                s = Math.Max(s, 0.5f);
                target.SetScale(s, s);

                // 旋转对象（可选效果）
                target.rotation = rotation + progress * 360;
            })
            .OnComplete((GTweener t) =>
            {
                // 动画完成回调
                completeCallback?.Invoke(t);
            });
    }

    /// <summary>
    /// 计算抛物线路径上的点
    /// </summary>
    private Vector2 CalculateParabolaPosition(Vector2 start, Vector2 end, float height, float progress)
    {
        // 水平方向线性插值
        float x = Mathf.Lerp(start.x, end.x, progress);

        // 垂直方向抛物线
        float parabolicHeight = 4 * height * (progress - progress * progress);
        float y = Mathf.Lerp(start.y, end.y, progress) - parabolicHeight;
        
        return new Vector2(x, y);
    }

    /// <summary>
    /// 在椭圆内生成随机点
    /// </summary>
    private Vector2 GetRandomPointInEllipse(Vector2 center, float majorAxis, float minorAxis, float rotationRadians)
    {
        // 生成单位圆内的随机点
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float radius = Mathf.Sqrt(Random.Range(0f, 1f));

        // 转换到椭圆坐标系
        float x = radius * Mathf.Cos(angle) * majorAxis / 2f;
        float y = radius * Mathf.Sin(angle) * minorAxis / 2f;

        // 应用旋转
        float rotatedX = x * Mathf.Cos(rotationRadians) - y * Mathf.Sin(rotationRadians);
        float rotatedY = x * Mathf.Sin(rotationRadians) + y * Mathf.Cos(rotationRadians);

        // 转换到世界坐标系
        return new Vector2(rotatedX + center.x, rotatedY + center.y);
    }


    /// <summary>
    /// 销毁
    /// </summary>
    public void Destroy()
    {
        //Debug.Log("开始销毁");
        foreach (var c in coroutineList)
        {
            GameManager.Instance.StopCoroutine(c);
        }
        coroutineList.Clear();
        foreach (var item in activeCoins)
        {
            GTween.Kill(item);
            item.Dispose();
        }
        activeCoins.Clear();
    }
}
