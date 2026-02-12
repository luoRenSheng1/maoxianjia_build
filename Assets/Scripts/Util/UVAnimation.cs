using UnityEngine;

public class UVAnimation : MonoBehaviour
{
    private Vector2 v2;

    public float xspeed = 0;
    public float yspeed = 0;
    //public int materialIndex = 0;

    // private Material mat = null;
    private Renderer myRenderer;
    private MaterialPropertyBlock mpb;
    private readonly int mainTexID = Shader.PropertyToID("_MainTex_ST");

    void Awake()
    {
        // mat = GetComponent<Renderer>().material;
        myRenderer = GetComponent<Renderer>();
        if (null != myRenderer)
        {
            mpb = new MaterialPropertyBlock();
            myRenderer.GetPropertyBlock(mpb);    
            Vector4 v = myRenderer.sharedMaterial.GetVector(mainTexID);
            mpb.SetVector(mainTexID, v);
            myRenderer.SetPropertyBlock(mpb);
        }
    }

    void Start()
    {
        v2 = Vector2.zero;
    }

    public void SetTiling(Vector2 newTiling)
    {
        if (null != myRenderer)
        {
            myRenderer.GetPropertyBlock(mpb);    
            Vector4 v = mpb.GetVector(mainTexID);
            v.x = newTiling.x;
            v.y = newTiling.y;
            mpb.SetVector(mainTexID, v);
            myRenderer.SetPropertyBlock(mpb);
        }
        // if(mat != null)
        // {
        //     mat.mainTextureScale = newTiling;
        // }
    }

    void Update()
    {
        v2.x += Time.deltaTime * xspeed;
        v2.y += Time.deltaTime * yspeed;

        if (null != myRenderer)
        {
            Vector4 v = mpb.GetVector(mainTexID);
            v.z = v2.x;
            v.w = v2.y;
            mpb.SetVector(mainTexID, v);
            myRenderer.SetPropertyBlock(mpb);
        }
        // mat.mainTextureOffset = v2;
    }
}
