
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : SingletonMonoBehavior<CameraShake>
{
    private TweenPosition Tp => GetComponent<TweenPosition>();

    public float time = 0.2f;
    public float mag = 0.01f;

    public void ImShakingBroooooo(Vector2 shakeDirection)
    {
        shakeDirection.Normalize();
        Tp.@from = new Vector3(0, 0, -10);
        Tp.to = new Vector3(shakeDirection.x * mag *GameEventTrackerProVCL.Instance.CameraShakeMultiplier, shakeDirection.y * mag * GameEventTrackerProVCL.Instance.CameraShakeMultiplier, -10);
        Tp.duration = time;
        Tp.ResetToBeginning();
        Tp.PlayForward();
       // StartCoroutine(ShakeCoro());
    }

    IEnumerator ShakeCoro()
    {
        Tp.PlayForward();
        yield return new WaitForSeconds(time);
        Tp.PlayReverse();
    }
}
