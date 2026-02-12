using UnityEngine;
using System.Collections;

public class UVAnimationController : MonoBehaviour
{
    [SerializeField]
    private float NowU_Value;
    [SerializeField]
    private float NowV_Value;

    private float prevUValue = 0f;
    private float prevVValue = 0f;

	private Renderer myRenderer;
	private MaterialPropertyBlock mpb;
	private readonly int mainTexID = Shader.PropertyToID("_MainTex_ST");
	
	// private Material myMaterial;

	void Awake()
	{
		myRenderer = GetComponent<Renderer>();
		
		if (null != myRenderer) 
		{
			myRenderer = GetComponent<Renderer>();
			mpb = new MaterialPropertyBlock();
			myRenderer.GetPropertyBlock(mpb);
			Vector4 v = myRenderer.sharedMaterial.GetVector(mainTexID);
			mpb.SetVector(mainTexID, v);
			// myMaterial = myRenderer.material;
			myRenderer.SetPropertyBlock(mpb);
		}
	}

    void Update()
    {
        if (NowU_Value != prevUValue || NowV_Value != prevVValue)
        {
            prevUValue = NowU_Value;
            prevVValue = NowV_Value;
            myRenderer.GetPropertyBlock(mpb);
            Vector4 v = mpb.GetVector(mainTexID);
            v.z = NowU_Value;
            v.w = NowV_Value;
            mpb.SetVector(mainTexID, v);
            myRenderer.SetPropertyBlock(mpb);
			// myMaterial.mainTextureOffset = new Vector2(NowU_Value, NowV_Value);
        }
    }
}
