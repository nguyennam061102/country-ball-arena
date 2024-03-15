using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideWhileTutorial : MonoBehaviour
{
    public GameObject[] objectsToHide;

    public void HideObjectForTutorial()
    {
        foreach (GameObject go in objectsToHide) go.SetActive(false);
    }
}
