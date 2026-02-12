using UnityEngine;

public class UVAnimationAdvance : MonoBehaviour
{
    public Vector2 uvDefaultTiling = Vector2.one;
    public Vector2 uvDefaultOffset = Vector2.zero;
    public Vector2 uvOffsetSpeed = Vector2.zero;
    public Color matDefaultColor = Color.white;

    private Renderer _renderer;
    private MaterialPropertyBlock mpb;
    private readonly int mainTexID = Shader.PropertyToID("_MainTex_ST");
    // private Material _mat;
    private Vector2 m_offset;
    private Vector2 m_tiling;

    // Use this for initialization
    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (null != _renderer)
        {
            mpb = new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(mpb);
            mpb.SetColor("_TintColor", matDefaultColor);
            _renderer.SetPropertyBlock(mpb);
        }
        // _renderer.material.SetColor("_TintColor", matDefaultColor);
    }

    void Start()
    {
        m_offset = uvDefaultOffset;
        m_tiling = uvDefaultTiling;
        if (null != _renderer)
        {
            _renderer.GetPropertyBlock(mpb);
            Vector4 v = new Vector4(m_tiling.x, m_tiling.y, m_offset.x, m_offset.y);
            mpb.SetVector(mainTexID, v);
            _renderer.SetPropertyBlock(mpb);
        }
        // _mat = _renderer.material;
        // m_offset = uvDefaultOffset;
        // m_tiling = uvDefaultTiling;
        // if (_mat != null)
        // {
        //     _mat.mainTextureOffset = m_offset;
        //     _mat.mainTextureScale = m_tiling;
        // }
    }

    // Update is called once per frame
    void Update()
    {
        if (uvOffsetSpeed.x != 0 || uvOffsetSpeed.y != 0)
        {
            m_offset.x += uvOffsetSpeed.x * Time.deltaTime;
            if (m_offset.x > 1)
            {
                m_offset.x -= 1;
            }
            else if (m_offset.x < -1)
            {
                m_offset.x += 1;
            }
            m_offset.y += uvOffsetSpeed.y * Time.deltaTime;
            if (m_offset.y > 1)
            {
                m_offset.y -= 1;
            }
            else if (m_offset.y < -1)
            {
                m_offset.y += 1;
            }
            if (null != _renderer)
            {
                _renderer.GetPropertyBlock(mpb);
                Vector4 v = new Vector4(m_tiling.x, m_tiling.y, m_offset.x, m_offset.y);
                mpb.SetVector(mainTexID, v);
                _renderer.SetPropertyBlock(mpb);
            }
            // _mat.mainTextureOffset = m_offset;
        }
    }
}
