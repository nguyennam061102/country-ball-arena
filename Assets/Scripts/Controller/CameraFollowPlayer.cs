using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] Camera gameCamera, renderCamera;
    Transform target;
    Transform midScreenTarget;
    float camOthorSize = 10f;

    float baseSize = 10f;
    float zoomSize = 3f;

    float followSpeed = 10f;
    Vector3 targetPosition = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        midScreenTarget = new GameObject("camfollowmain").transform;
        midScreenTarget.transform.position = Vector3.zero;
        target = midScreenTarget;
        camOthorSize = baseSize;
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            targetPosition = target.transform.position + new Vector3(0, 0, -10);
            gameCamera.transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
            gameCamera.orthographicSize = Mathf.Lerp(gameCamera.orthographicSize, camOthorSize, followSpeed * Time.deltaTime);
            renderCamera.orthographicSize = gameCamera.orthographicSize;
        }
    }

    public void StartFollow(Transform t)
    {
        target = t;
        camOthorSize = zoomSize;
    }

    public void StopFollow()
    {
        target = midScreenTarget;
        camOthorSize = baseSize;
    }
}
