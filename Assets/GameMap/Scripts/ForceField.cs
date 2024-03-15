using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    public float pushForce;
    [SerializeField] List<Player> playersToPush;
    [SerializeField] ParticleSystem effect;

    // Start is called before the first frame update
    void Start()
    {
        pushForce = 450000f;
        playersToPush = new List<Player>();
        GetComponent<BoxCollider2D>().enabled = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void PlayForceField()
    {
        GetComponent<BoxCollider2D>().enabled = true;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
        effect.Play();
    }

    private void FixedUpdate()
    {
        foreach (Player p in playersToPush)
        {
            p.playerVel.AddForce(transform.up * pushForce * TimeHandler.deltaTime, ForceMode2D.Force);
            //p.GetComponent<Gravity>().num = 0f;
            p.sinceGrounded = 0.06f;//for jump
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player p = collision.gameObject.GetComponent<Player>();
        if (p != null)
        {
            if (!playersToPush.Contains(p))
            {
                playersToPush.Add(p);
                //p.GetComponent<Gravity>().InForceField();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player p = collision.gameObject.GetComponent<Player>();
        if (p != null)
        {
            playersToPush.Remove(p);
            //p.GetComponent<Gravity>().OutForceField();
        }
    }

}
