using System;
using UnityEngine;

public class CollisionChecker : MonoBehaviour
{
    private BaseCharacter data;

    //public Action<Collision2D> collisionAction;

    private void Awake()
    {
        data = GetComponent<BaseCharacter>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collide(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Collide(collision);
    }

    private void Collide(Collision2D collision)
    {
        // if (collisionAction != null)
        // {
        //     collisionAction(collision);
        // }
        if (Vector3.Angle(Vector3.up, collision.contacts[0].normal) > 70f)
        {
            Vector3.Angle(Vector3.up, collision.contacts[0].normal);
        }
        else
        {
            data.TouchGround(collision.contacts[0].point, collision.contacts[0].normal, collision.otherRigidbody, collision.transform);
        }
    }
}