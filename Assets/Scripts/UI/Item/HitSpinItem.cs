using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSpinItem : MonoBehaviour
{
    [SerializeField] TweenRotation tweenrot;
    [SerializeField] AudioSource hitfx;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("SpinPoint"))
        {
            if (GameData.Sound) hitfx.Play();
            tweenrot.PlayForward();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("SpinPoint"))
        {
            tweenrot.PlayReverse();
        }
    }
}
