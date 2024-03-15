using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DropPlatform : MonoBehaviour
{
    float dropCountDown = 2f;
    bool dropping = false;
    float rungRadius = 0.1f;
    Vector3 basePos;
    // Update is called once per frame
    void Update()
    {
        if (dropping && dropCountDown > 0)
        {
            dropCountDown -= Time.deltaTime;
            Rung();
            if (dropCountDown <= 0)
            {
                DropIt();
            }
        }
    }

    void DropIt()
    {
        GetComponent<Rigidbody2D>().isKinematic = false;
        GetComponent<Rigidbody2D>().AddTorque(Random.Range(-100f, 100f));
        GetComponent<Rigidbody2D>().AddForce(Vector2.down * Random.Range(-20, 20f), ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player p = collision.gameObject.GetComponent<Player>();
        if (p != null)
        {
            dropping = true;
            basePos = transform.position;
        }
    }

    void Rung()
    {
        transform.position = basePos + new Vector3(Random.Range(-rungRadius, rungRadius), Random.Range(-rungRadius, rungRadius), 0);
    }
}
