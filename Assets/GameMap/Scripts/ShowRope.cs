using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowRope : MonoBehaviour
{
    LineRenderer lineToShow;
    HingeJoint2D hinge;

    // Start is called before the first frame update
    void Start()
    {
        hinge = GetComponent<HingeJoint2D>();
        lineToShow = GetComponent<LineRenderer>();
        lineToShow.useWorldSpace = true;
        lineToShow.positionCount = 2;
        lineToShow.startWidth = 0.2f;
        lineToShow.endWidth = 0.2f;

    }

    // Update is called once per frame
    void Update()
    {
        lineToShow.SetPosition(0, hinge.connectedAnchor);
        lineToShow.SetPosition(1, transform.position);
    }
}
