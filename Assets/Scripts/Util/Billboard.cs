using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {

    private Transform mainCamTrans;
    private Transform trans;

    public enum BILLBOARD_TYPE
    {
        BILLBOARD_VERTICAL,
        BILLBOARD_HORIZONTAL,
        BILLBOARD_NORM,
    }
    public BILLBOARD_TYPE type;
    // Use this for initialization
    void Start () {
        mainCamTrans = Camera.main.transform;
        trans = gameObject.transform;
    }
	
	// Update is called once per frame
	void Update () {
	    if(mainCamTrans != null && (trans.position - mainCamTrans.position).sqrMagnitude > 0.001f)
        {
            Vector3 euler = Quaternion.LookRotation(mainCamTrans.position - trans.position).eulerAngles;
            switch(type)
            {
                case BILLBOARD_TYPE.BILLBOARD_VERTICAL:
                    euler.x = euler.z = 0;
                    break;
                case BILLBOARD_TYPE.BILLBOARD_HORIZONTAL:
                    euler.z = 0;
                    euler.x = -90;
                    break;
            }
            gameObject.transform.rotation = Quaternion.Euler(euler);
        }
	}
}
