using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGround : MonoBehaviour
{
    private GameController gameController => GameController.Instance;

    public enum Direction
    {
        Left,
        Right,
        Top,
        Bottom
    }

    public Direction direction;
    private float x, y;

    private void Start()
    {
        x = gameController.screenSize.x;
        y = gameController.screenSize.y;

        switch (direction)
        {
            case Direction.Left:
                transform.position = new Vector3(-x, 0, 0);
                break;
            case Direction.Right:
                transform.position = new Vector3(x, 0, 0);
                break;
            case Direction.Top:
                transform.position = new Vector3(0, y, 0);
                break;
            case Direction.Bottom:
                transform.position = new Vector3(0, -y, 0);
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player p = collision.gameObject.GetComponent<Player>();
        if (p != null)
        {
            float side = 0;
            if (direction.Equals(Direction.Left)) side = 1;
            else if (direction.Equals(Direction.Right)) side = -1;
            if (side != 0)
            {
                p.playerVel.velocity *= 0f;
                p.healthHandler.CallTakeForce(Vector3.right * side * 25f * p.playerVel.mass, ForceMode2D.Impulse, forceIgnoreMass: false, ignoreBlock: true);
            }
            else
            {
                if (direction.Equals(Direction.Bottom))
                {
                    p.healthHandler.CallTakeDamage(Vector2.up * 50f, p.maxHealth * 0.2f, collision.contacts[0].point);
                }
                else if (direction.Equals(Direction.Top))
                {
                    p.playerVel.velocity *= 0f;
                    p.healthHandler.CallTakeForce(Vector3.down * 25f * p.playerVel.mass, ForceMode2D.Impulse, forceIgnoreMass: false, ignoreBlock: true);
                }
            }

        }
    }
}
