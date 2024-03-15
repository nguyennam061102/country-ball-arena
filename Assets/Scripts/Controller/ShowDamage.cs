using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowDamage : MonoBehaviour
{
    [SerializeField] private UILabel damageTextPrefab, criticalDamageTextPrefab;
    [SerializeField] private Vector2 moveAmount;
    [SerializeField] private Vector2 spawnAmount;
    [SerializeField] private float duration;
    [SerializeField] private Camera gameMainCamera;

    private int manualWidth = 1920;
    private int manualHeight = 1080;

    public void OnTakeDamage(float damage, Vector3 worldPosition, bool criticalDamage)
    {
        if (SkygoBridge.instance.isForRecording()) return;
        Vector2 position = gameMainCamera.WorldToScreenPoint(worldPosition);
        position -= new Vector2(Screen.width / 2, Screen.height / 2);
        Show(Mathf.RoundToInt(damage), position, criticalDamage);
    }

    void Show(int damage, Vector2 wPos, bool criticalDamage)
    {
        var lb = Instantiate(criticalDamage ? criticalDamageTextPrefab : damageTextPrefab, transform);

        lb.text = damage.ToString();
        lb.gameObject.SetActive(true);

        Vector3 overlay = NGUIMath.WorldToLocalPoint(wPos, gameMainCamera, UICamera.mainCamera, lb.transform);
        overlay.z = 0f;
        lb.transform.localPosition = overlay;

        TweenPosition twPosition = lb.GetComponent<TweenPosition>();
        twPosition.from = wPos + new Vector2(Random.Range(spawnAmount.x, -spawnAmount.x), Random.Range(spawnAmount.y, -spawnAmount.y));
        twPosition.to = wPos + new Vector2(Random.Range(moveAmount.x, -moveAmount.x), moveAmount.y);
        //        twPosition.duration = duration * 0.75f;
        twPosition.duration = duration / 2f;
        twPosition.ResetToBeginning();
        twPosition.PlayForward();

        TweenAlpha twAlpha = lb.GetComponent<TweenAlpha>();
        twAlpha.duration = duration / 2f;
        twAlpha.ResetToBeginning();
        twAlpha.PlayForward();

        Destroy(lb.gameObject, duration);
    }
}
