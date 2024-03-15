using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image hp;

    public Image white;

    private float drag = 25f;

    private float spring = 25f;

    private float hpCur;

    private float hpVel;

    private float hpTarg;

    private float whiteCur;

    private float whiteVel;

    private float whiteTarg;

    private float sinceDamage;

    private BaseCharacter data;

    private HealthHandler HealthHandler => GetComponentInParent<HealthHandler>();

    public Color32 color;

    private void Start()
    {
        data = GetComponentInParent<BaseCharacter>();
        //CharacterStatModifiers componentInParent = GetComponentInParent<CharacterStatModifiers>();
        //componentInParent.WasDealtDamageAction = (Action<Vector2, bool>)Delegate.Combine(componentInParent.WasDealtDamageAction, new Action<Vector2, bool>(TakeDamage));
        HealthHandler.onHealthChange += TakeDamage;
        
        
        color = !data.AI ? new Color32(149, 255, 0, 255) : new Color32(255, 17, 0, 255);
        hp.color = color;
    }

    private void Update()
    {
        hpTarg = data.health / data.maxHealth;
        sinceDamage += TimeHandler.deltaTime;
        hpVel = FRILerp.Lerp(hpVel, (hpTarg - hpCur) * spring, drag);
        whiteVel = FRILerp.Lerp(whiteVel, (whiteTarg - whiteCur) * spring, drag);
        hpCur += hpVel * TimeHandler.deltaTime;
        whiteCur += whiteVel * TimeHandler.deltaTime;
        hp.fillAmount = hpCur;
        white.fillAmount = whiteCur;
        if (sinceDamage > 0.5f)
        {
            whiteTarg = hpTarg;
        }
    }

    public void TakeDamage()
    {
        sinceDamage = 0f;
    }
}
