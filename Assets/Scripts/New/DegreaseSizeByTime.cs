using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DegreaseSizeByTime : MonoBehaviour
{
    private float sizeMultiplier = 1f;
    void Update()
    {
        transform.localScale -= new Vector3(sizeMultiplier, sizeMultiplier, sizeMultiplier) * Time.deltaTime;
    }
}
