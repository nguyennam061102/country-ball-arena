using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameUtils
{
    public static IEnumerator WaitForSeconds(float waitTime)
    {
        float elapsedTime = 0f;
        while (elapsedTime < waitTime)
        {
            elapsedTime += GameTimer.Instance.getDeltaTime;
            yield return 0;
        }
    }

    public static float GetShiftedAngle(int wayIndex, float baseAngle, float betweenAngle)
    {
        var angle = wayIndex % 2 == 0
            ? baseAngle - (betweenAngle * (float)wayIndex / 2f)
            : baseAngle + (betweenAngle * Mathf.Ceil((float)wayIndex / 2f));
        return angle;
    }

    public static Vector2 screenSize;

    public static void CalculateScreenSizeInUnits()
    {
        screenSize = Vector2.zero;
        screenSize.x = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)), Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0))) * 0.5f;
        screenSize.y = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)), Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height))) * 0.5f;
    }

    public static float GetBackgroundScaleSize()
    {
        return screenSize.x / 17.5f;
    }

    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static float GetEXPTargetToLevel(int level)
    {
        return 60 * Mathf.Pow(level, 2.8f) - 60;
    }

    public static float GetLevelFromTotalEXP(float exp)
    {
        return Mathf.Pow((exp + 60) / 60, 1 / 2.8f);
    }
}
