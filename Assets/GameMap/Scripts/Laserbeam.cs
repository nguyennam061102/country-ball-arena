using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laserbeam : MonoBehaviour
{
    public LineRenderer laserbeam;
    public Transform startRaycastPoint, endLaser;
    RaycastHit2D hit;
    public float maxLaserDistance;
    public bool operating = false;
    float lastTimeDamage = 0f;
    float damageTick = 0.2f;
    Player lastPlayerHit;

    private void Start()
    {
        laserbeam.useWorldSpace = true;
        TurnOff();
    }

    public void TurnOff()
    {
        operating = false;
        laserbeam.gameObject.SetActive(false);
        endLaser.gameObject.SetActive(false);
    }

    public void TurnOn()
    {
        operating = true;
        laserbeam.gameObject.SetActive(true);
        endLaser.gameObject.SetActive(true);
        foreach (ParticleSystem ps in endLaser.GetComponentsInChildren<ParticleSystem>())
        {
            var main = ps.main;
            main.scalingMode = ParticleSystemScalingMode.Local;
            ps.transform.localScale = Vector3.one * 1.5f;
        }
        endLaser.GetComponentInChildren<ParticleSystem>().Play();
    }
    bool canDamage = false;

    private void Update()
    {
        if (!operating) return;
        lastTimeDamage += TimeHandler.deltaTime;
        hit = Physics2D.Raycast(startRaycastPoint.position, transform.up, maxLaserDistance);
        endLaser.transform.position = hit.point;
        laserbeam.SetPosition(0, laserbeam.transform.position);
        laserbeam.SetPosition(1, endLaser.position);

        if (hit.collider != null)
        {
            Player pl = hit.collider.gameObject.GetComponent<Player>();
            if (pl != null)
            {
                if (pl != lastPlayerHit) canDamage = true;
                else if (lastTimeDamage >= damageTick) canDamage = true;
                if (canDamage)
                {
                    canDamage = false;
                    lastTimeDamage = 0;
                    lastPlayerHit = pl;
                    float damage = pl.maxHealth * 0.08f;
                    pl.healthHandler.TakeDamage(Vector2.zero, damage, null, false);
                }
            }
        }
    }
}
