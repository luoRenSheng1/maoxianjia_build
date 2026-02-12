using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShakeCamera : MonoBehaviour
{
    [Header("震动幅度")]
    public float shakeLevel = 3f;
    [Header("震动幅度")]
    public float SetShakeTime = 0.5f;
    [Header("震动幅度")]
    public float ShakeFps = 45f;

    private bool isShakeCamera = false;
    private float fps;
    private float shakeTime = 0;
    private float frameTime = 0;
    private float shakeDelta = 0.005f;
    private Camera selfCamera;

    private void OnEnable()
    {
        isShakeCamera = true;
        selfCamera = gameObject.GetComponent<Camera>();
        shakeTime = SetShakeTime;
        fps = ShakeFps;
        frameTime = 0.03f;
        shakeDelta = 0.005f;
    }

    private void Update()
    {
        if (isShakeCamera)
        {
            if (shakeTime > 0)
            {
                shakeTime -= Time.deltaTime;
                if (shakeTime <= 0)
                {
                    enabled = false;
                }
                else
                {
                    frameTime += Time.deltaTime;
                    if (frameTime > 1.0 / fps)
                    {
                        frameTime = 0;
                        selfCamera.rect = new Rect(shakeDelta*(-1.0f+shakeLevel*Random.value), shakeDelta*(-1.0f+shakeLevel*Random.value),1.0f,1.0f);
                    }
                }
            }
        }
    }
}