using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlatformMovingPro : MonoBehaviour
{
    [SerializeField] GameObject[] platformtomove;
    //28f
    int side = 1;
    float xMaxPos = 28f;
    [SerializeField] float speed;

    // Start is called before the first frame update
    void Start()
    {
        side = (int)Random.Range(0, 2) == 0 ? -1 : 1;
        GameObject lastplatform = platformtomove[platformtomove.Length - 1];
        lastplatform.transform.position = new Vector3(-side * xMaxPos, lastplatform.transform.position.y, 0);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject obj in platformtomove)
        {
            obj.transform.position += Vector3.right * side * speed * Time.deltaTime;
            if (Mathf.Abs(obj.transform.position.x) >= xMaxPos)
            {
                obj.transform.position = new Vector3(-xMaxPos * side, obj.transform.position.y, 0);
            }
        }
    }
}
