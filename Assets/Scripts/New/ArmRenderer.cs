﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmRenderer : MonoBehaviour
{
    [SerializeField] BaseCharacter data;
    public Transform start;
    public Transform mid;
    public Transform end;
    public int segmentCount = 10;
    [SerializeField] LineRenderer line;
    private float t;

    private void Awake()
    {
        line.positionCount = segmentCount;
        line.useWorldSpace = true;
        line.numCapVertices = 5;
        data.skinHandler.onSetSkinAction += SetColor;
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < segmentCount; i++)
        {
            t = (float)i / ((float)segmentCount - 1f);
            Vector3 pos = BezierCurve.QuadraticBezier(start.position, mid.position, end.position, t);
            line.SetPosition(i, pos);
        }
    }

    void SetColor()
    {
        line.startColor = data.skinHandler.ColorForParts;
        line.endColor = data.skinHandler.ColorForParts;
    }
}