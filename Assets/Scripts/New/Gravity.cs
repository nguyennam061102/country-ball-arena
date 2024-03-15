using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    public float gravityForce;
    public float exponent = 1f;
    private PlayerVelocity rig;
    public float num = 0;
    private BaseCharacter data;

    private void Start()
    {
        data = GetComponent<BaseCharacter>();
        rig = GetComponent<PlayerVelocity>();
        //OutForceField();
    }

    private void OnEnable()
    {
        //OutForceField();
    }

    private void FixedUpdate()
    {
        num = data.sinceGrounded;
        if (data.sinceWallGrab < num)
        {
            num = data.sinceWallGrab;
        }
        if (num > 0f)
        {
            rig.AddForce(Vector3.down * TimeHandler.timeScale * Mathf.Pow(num, exponent) * gravityForce * rig.mass, ForceMode2D.Force);
        }
        else
        {
            rig.AddForce(Vector3.down * TimeHandler.timeScale * num * gravityForce * rig.mass, ForceMode2D.Force);
        }
    }

    //public void InForceField()
    //{
        
    //}

    //public void OutForceField()
    //{
        
    //}
}
