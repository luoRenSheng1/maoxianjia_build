using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class UVTextureAnimator : MonoBehaviour
{
    public Material[] AnimatedMaterialsNotInstance = null;
    public int Rows = 4;
    public int Columns = 4;
    public float Fps = 30; // 备注，当前游戏帧率锁定30帧，写死保证最终效果是OK的，By 码神
    public int OffsetMat = 0;
    public Vector2 SelfTiling = new Vector2();
    public bool IsLoop = true;
    public bool IsReverse = false;
    public bool IsRandomOffsetForInctance = false;


    private bool isInizialised;
    private int index;
    private int count, allCount;
    private float deltaFps;
    private bool isVisible;
    private bool isCorutineStarted;

    #region Non-public methods

    // private Material mMaterial;
    private Renderer myRenderer;
    private MaterialPropertyBlock mpb;
    private readonly int mainTexID = Shader.PropertyToID("_MainTex_ST");

    private void Awake()
    {
        InitDefaultVariables();
        isInizialised = true;
    }

    private void InitDefaultVariables()
    {
        allCount = 0;
        deltaFps = 1f / Fps;
        count = Rows * Columns;
        index = Columns - 1;
        var offset = new Vector2((float)index / Columns - (index / Columns),
          1 - (index / Columns) / (float)Rows);

        OffsetMat = !IsRandomOffsetForInctance
          ? OffsetMat - (OffsetMat / count) * count
          : Random.Range(0, count);

        var size = SelfTiling == Vector2.zero ? new Vector2(1f / Columns, 1f / Rows) : SelfTiling;

        // mMaterial = GetComponent<Renderer>().material;
        myRenderer = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
        myRenderer.GetPropertyBlock(mpb);

        if (AnimatedMaterialsNotInstance.Length > 0)
        {
            foreach (var mat in AnimatedMaterialsNotInstance)
            {
                mat.SetTextureScale("_MainTex", size);
                mat.SetTextureOffset("_MainTex", Vector2.zero);
            }
        }
        else
        {
            mpb.SetVector(mainTexID, new Vector4(size.x, size.y, offset.x, offset.y));
            myRenderer.SetPropertyBlock(mpb);
            // mMaterial.SetTextureScale("_MainTex", size);
            // mMaterial.SetTextureOffset("_MainTex", offset);
        }
    }

    #region CorutineCode

    private void OnEnable()
    {
        if (!isInizialised)
            InitDefaultVariables();

        isVisible = true;

        if (!isCorutineStarted)
            StartCoroutine(UpdateCorutine());
    }

    private void OnDisable()
    {
        isCorutineStarted = false;
        isVisible = false;
        allCount = 0;
        StopAllCoroutines();
    }

    private void OnBecameVisible()
    {
        isVisible = true;
        if (!isCorutineStarted)
            StartCoroutine(UpdateCorutine());
    }

    private void OnBecameInvisible()
    {
        isVisible = false;
    }

    private IEnumerator UpdateCorutine()
    {
        isCorutineStarted = true;
        while (isVisible && (IsLoop || allCount != count))
        {
            UpdateCorutineFrame();
            if (!IsLoop && allCount == count)
                break;
            yield return new WaitForSeconds(deltaFps);
        }
        isCorutineStarted = false;
    }

    #endregion CorutineCode

    private void UpdateCorutineFrame()
    {
        ++allCount;
        if (IsReverse)
            --index;
        else
            ++index;
        if (index >= count)
            index = 0;

        if (AnimatedMaterialsNotInstance.Length > 0)
            for (int i = 0; i < AnimatedMaterialsNotInstance.Length; i++)
            {
                var idx = i * OffsetMat + index;
                idx = idx - (idx / count) * count;
                var offset = new Vector2((float)idx / Columns - (idx / Columns),
                  1 - (idx / Columns) / (float)Rows);
                AnimatedMaterialsNotInstance[i].SetTextureOffset("_MainTex", offset);


            }
        else
        {
            Vector2 offset;
            if (IsRandomOffsetForInctance)
            {
                var idx = index + OffsetMat;
                offset = new Vector2((float)idx / Columns - (idx / Columns),
                  1 - (idx / Columns) / (float)Rows);
            }
            else
            {
                offset = new Vector2((float)index / Columns - (index / Columns),
                  1 - (index / Columns) / (float)Rows);
            }

            myRenderer.GetPropertyBlock(mpb);
            Vector4 v = mpb.GetVector(mainTexID);
            v.z = offset.x;
            v.w = offset.y;
            mpb.SetVector(mainTexID, v);
            myRenderer.SetPropertyBlock(mpb);
            // mMaterial.SetTextureOffset("_MainTex", offset);

        }
    }

    #endregion
}