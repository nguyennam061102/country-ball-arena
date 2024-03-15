using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathMatchCacheBox : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if(player!= null && player.playerID == 0)
            {
                player.holding.GetRandomPlayerGunAndSpawn();
                //heal player
                this.gameObject.SetActive(false);
            }
        }
    }
}
