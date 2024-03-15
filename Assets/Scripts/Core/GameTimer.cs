using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : SingletonMonoBehavior<GameTimer>
{
    private bool _pausing;
    private float _deltaTime;
    private float _deltaFrameCount;
	
    public float getDeltaTime { get { return pausing ? 0f : _deltaTime; } }
    public float getDeltaFrameCount { get { return pausing ? 0f : _deltaFrameCount; } }
    public bool pausing { get { return _pausing; } }

    protected override void Awake()
    {
        base.Awake();
        UpdateTimes();
    }

    private void Update()
    {
        UpdateTimes();
    }

    private void UpdateTimes()
    {
        _deltaTime = Time.deltaTime;
        _deltaFrameCount = _deltaTime / (1f / Application.targetFrameRate);
    }

    public void Pause()
    {
        _pausing = true;
    }

    public void Resume()
    {
        _pausing = false;
    }
}
