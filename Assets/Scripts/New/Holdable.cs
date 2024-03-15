using System;
using UnityEngine;

public class Holdable : MonoBehaviour
{
    public Rigidbody2D rig;

    public BaseCharacter holder;

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
    }

    // private void Update()
    // {
    //     if (holder == null || !holder.gameObject.activeInHierarchy) Destroy(gameObject);
    // }

    // public void SetTeamColors(PlayerSkin teamColor, Player player)
    // {
    //     SetTeamColor.TeamColorThis(base.gameObject, teamColor);
    // }
}