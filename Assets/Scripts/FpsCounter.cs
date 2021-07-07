using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class FpsCounter : MonoBehaviour
{
    private int _frameCounter = 0;
    private float _timeCounter = 0;
    private float _refreshTime = 0.5f;
    private float _maxFrameRate = 0f;
    private float _minFrameRate = 1000f;
    [SerializeField] private Text _debugFPS;
    [SerializeField] private Text _debugMinFPS;
    [SerializeField] private Text _debugMaxFPS;

    private IEnumerator ResetMinFrameRate()
    {
        yield return new WaitForSeconds(1f);
        _minFrameRate = 1000f;
    }
    private void Start()
    {
        StartCoroutine(ResetMinFrameRate());
    }
    void Update()
    {
        if (_timeCounter < _refreshTime)
        {
            _timeCounter += Time.unscaledDeltaTime;
            _frameCounter++;
        }
        else
        {
            float lastFrameRate = _frameCounter / _timeCounter;
            if (_minFrameRate > lastFrameRate) _minFrameRate = lastFrameRate;
            if (_maxFrameRate < lastFrameRate) _maxFrameRate = lastFrameRate;
            _frameCounter = 0;
            _timeCounter = 0.0f;
            _debugFPS.text = Convert.ToString($"FPS: {Mathf.Floor(lastFrameRate)}");
            _debugMinFPS.text = Convert.ToString($"minFPS: {Mathf.Floor(_minFrameRate)}");
            _debugMaxFPS.text = Convert.ToString($"maxFPS: {Mathf.Floor(_maxFrameRate)}");
        }
    }
}
