using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;

public class DelayActiveController : MonoBehaviour
{
    public float time;
    public GameObject[] arrGo;

    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(DelayActive());
    }

    IEnumerator DelayActive()
    {
        if (arrGo != null)
        {
            foreach (var go in arrGo)
            {
                if (go != null)
                {
                    go.SetActive(false);
                }
            }
        }
        
        if (time > 0)
        {
            yield return new WaitForSeconds(time);
        }
        
        if (arrGo != null)
        {
            foreach (var go in arrGo)
            {
                if (go != null)
                {
                    go.SetActive(true);
                }
            }
        }
    }
}
