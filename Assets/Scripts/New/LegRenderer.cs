using System.Collections.Generic;
using UnityEngine;

public class LegRenderer : MonoBehaviour
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

        line.startWidth = 0.12f;
        line.endWidth = 0.12f;

        data.skinHandler.onSetSkinAction += SetColor;
        line.enabled = false;
    }

    private void FixedUpdate()
    {
        return;
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

    public void SetSize(float mul)
    {
        line.startWidth = 0.12f * mul;
        line.endWidth = 0.12f * mul;
    }
}
