﻿using UnityEngine;

public class PlayerVelocity : MonoBehaviour
{
    internal bool simulated = true;

    internal bool isKinematic;

    internal Vector2 velocity;

    internal float mass = 100f;

    internal float angularVelocity;

    private BaseCharacter data;

    public Vector2 position
    {
        get
        {
            return base.transform.position;
        }
        set
        {
            base.transform.position = value;
        }
    }

    internal void AddTorque(float v)
    {
    }

    private void Start()
    {
        data = GetComponent<BaseCharacter>();
        Rigidbody2D rb2d = GetComponent<Rigidbody2D>();
        if (rb2d != null) rb2d.mass = this.mass;
    }

    private void FixedUpdate()
    {
        if (data.isPlaying)
        {
            if (isKinematic)
            {
                velocity *= 0f;
            }
            if (simulated && !isKinematic)
            {
                velocity += Vector2.down * Time.fixedDeltaTime * TimeHandler.timeScale * 20f;
                base.transform.position += Time.fixedDeltaTime * TimeHandler.timeScale * (Vector3)velocity;
                base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, 0f);
            }
        }
    }

    internal void AddForce(Vector2 force, ForceMode2D forceMode)
    {
        force = ((forceMode != 0) ? (force * 1f) : (force * 0.02f));
        velocity += force / mass;
    }

    internal void AddForce(Vector3 force, ForceMode2D forceMode)
    {
        AddForce((Vector2)force, forceMode);
    }

    internal void AddForce(Vector2 force)
    {
        AddForce(force, ForceMode2D.Force);
    }

    internal void AddForce(Vector3 force)
    {
        AddForce((Vector2)force, ForceMode2D.Force);
    }
}