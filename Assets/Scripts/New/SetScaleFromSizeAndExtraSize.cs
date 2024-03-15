using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetScaleFromSizeAndExtraSize : MonoBehaviour
{
    //public float scalePerSize = 1;
    public float sizeMultiplier = 0.1f;
    public float destroyBulletAfterMultiplier;

    private void Update()
    {
        if (destroyBulletAfterMultiplier != 1)
        {
            transform.localScale += new Vector3(sizeMultiplier, sizeMultiplier, sizeMultiplier) * Time.deltaTime;
        }
    }
}
