using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpecialRotator : MonoBehaviour
{
    [SerializeField] TweenRotation rotator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartRotate()
    {
        rotator.ResetToBeginning();
        rotator.PlayForward();
    }

    public void StopRotate()
    {
        rotator.enabled = false;
    }
}
