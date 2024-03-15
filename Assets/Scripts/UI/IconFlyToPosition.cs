using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconFlyToPosition : MonoBehaviour
{
    private IEnumerator activeJumpCoroutine = null;
    public float JumpProgress { get; private set; }

    public void Fly(Vector3 startPos, Vector3 destination, float maxHeight, float maxWidth, float time, float delay, Action onComplete)
    {
        if (activeJumpCoroutine != null)
        {
            StopCoroutine(activeJumpCoroutine);
            activeJumpCoroutine = null;
            JumpProgress = 0.0f;
        }
        activeJumpCoroutine = FlyCoroutineDelay(startPos, destination, maxHeight, maxWidth, time, delay, onComplete);
        StartCoroutine(activeJumpCoroutine);
    }

    private IEnumerator FlyCoroutineDelay(Vector3 startPos, Vector3 destination, float maxHeight, float maxWidth, float time, float delay, Action onComplete)
    {
        yield return new WaitForSeconds(delay);
        while (JumpProgress <= 1.0)
        {
            JumpProgress += Time.deltaTime / time;
            var height = Mathf.Sin(Mathf.PI * JumpProgress) * maxHeight;
            if (height < 0f)
            {
                height = 0f;
            }
            var width = Mathf.Sin(Mathf.PI * JumpProgress) * maxWidth;
            if (width < 0f)
            {
                width = 0f;
            }
            transform.position = Vector3.Lerp(startPos, destination, JumpProgress) + Vector3.right * width + Vector3.up * height;
            yield return null;
        }
        if (onComplete != null)
        {
            onComplete.Invoke();
        }
        transform.position = destination;
    }

    public void StopJump()
    {
        if (activeJumpCoroutine != null) StopCoroutine(activeJumpCoroutine);
    }
}
