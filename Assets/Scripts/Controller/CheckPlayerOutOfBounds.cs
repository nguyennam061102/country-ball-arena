using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlayerOutOfBounds : MonoBehaviour
{
    public int countToDie = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckPlayers()
    {
        foreach(Player p in PlayerManager.Instance.players)
        {
            if (Mathf.Abs(p.transform.position.x) > GameUtils.screenSize.x || Mathf.Abs(p.transform.position.y) > GameUtils.screenSize.y) p.outOfBoundCount++;
            if(p.outOfBoundCount >= countToDie)
            {
                p.healthHandler.TakeDamage(Vector2.zero, 9999999, null, false);
            }
        }
    }
}
