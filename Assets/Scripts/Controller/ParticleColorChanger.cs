using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleColorChanger : MonoBehaviour
{
    [SerializeField] List<ParticleSystem> allParticles;
    [SerializeField] List<ParticleSystem.MinMaxGradient> allGrads;
    int activeBG = -1;
    int activeColor = -1;
    public bool isInit = false;

    public void Init()
    {
        GameUtils.Shuffle(allParticles);
        GameUtils.Shuffle(allGrads);
        isInit = true;
    }

    public void SetParticleAndColor()
    {
        activeBG++;
        activeColor++;
        if (activeBG >= allParticles.Count) activeBG = 0;
        if (activeColor >= allGrads.Count) activeColor = 0;
        foreach (ParticleSystem ps in allParticles) ps.gameObject.SetActive(false);
        var main = allParticles[activeBG].main;
        main.startColor = allGrads[activeColor];
        allParticles[activeBG].gameObject.SetActive(true);
    }
}
