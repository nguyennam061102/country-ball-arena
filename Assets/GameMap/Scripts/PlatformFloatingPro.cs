using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformFloatingPro : MonoBehaviour
{
    [SerializeField] GameObject[] objecttofloat;
  
    public void StartFloat()
    {
        foreach(GameObject g in objecttofloat)
        {
            if (g == null) continue;
            TweenPosition tw = g.GetComponent<TweenPosition>();
            if (tw != null)
            {
                //tw.from = new Vector3(g.transform.position.x * Random.Range(0.9f, 1.1f), g.transform.position.y * Random.Range(0.9f, 1.1f), 0);
                tw.from = g.transform.position;
                tw.to = new Vector3(g.transform.position.x * Random.Range(0.9f, 1.1f), g.transform.position.y * Random.Range(0.9f, 1.1f), 0);
                tw.duration = Random.Range(3f, 6f);
                tw.style = UITweener.Style.PingPong;
                tw.ResetToBeginning();
                tw.PlayForward();
            }
        }
    }
}
