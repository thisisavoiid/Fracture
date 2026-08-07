using System;
using System.Collections.Generic;
using System.Linq;
using ToolkitByJonathan;
using UnityEngine;

public class FPSTracker : MonoBehaviour
{
    [SerializeField]
    private int _updatesPerSecond = 1;

    [SerializeField]
    private bool _showDebugGui = false;

    [SerializeField]
    private List<BetterKeyValuePair<int, Color>> _performanceIndicators = new();

    private float _frameTime = 0.0f;
    private float _deltaTime = 0.0f;
    private int _deltaFps = 0;
    private int _fps = 0;

    public int FPS => _fps;
    public float FrameTime => _frameTime;

    [SerializeField]
    private int _maxRecordItems = 20;

    private List<float> _frameTimeRecordItems = new();
    private List<float> _fpsRecordItems = new();

    private float _frameTimeAverage;
    private float _fpsAverage;

    private void Awake()
    {
        if (_performanceIndicators != null || _performanceIndicators.Count != 0)
        {
            List<BetterKeyValuePair<int, Color>> sortedList = _performanceIndicators.OrderByDescending(i => i.Key).ToList();
            _performanceIndicators = sortedList;
        }
    }
    private void Update()
    {
        _deltaTime += Time.deltaTime;
        _deltaFps++;

        if (_deltaTime >= (1f / _updatesPerSecond))
        {
            _frameTime = Time.deltaTime * 1000f;
            _fps = _deltaFps * _updatesPerSecond;
            _deltaTime = 0.0f;
            _deltaFps = 0;

            if (_fpsRecordItems.Count >= _maxRecordItems)
            {
                _fpsAverage = _fpsRecordItems.Sum() / _fpsRecordItems.Count;
                _fpsRecordItems.Clear();
            }

            if (_frameTimeRecordItems.Count >= _maxRecordItems)
            {
                _frameTimeAverage = _frameTimeRecordItems.Sum() / _frameTimeRecordItems.Count;
                _frameTimeRecordItems.Clear();
            }

            _fpsRecordItems.Add(_fps);
            _frameTimeRecordItems.Add(_frameTime);
        }
    }

    private void OnGUI()
    {
        if (!_showDebugGui)
            return;

        Color displayColor = Color.white;
        int fpsCache = FPS;

        for (int i = _performanceIndicators.Count - 1; i >= 0; i--)
        {
            if (fpsCache >= _performanceIndicators[i].Key)
                displayColor = _performanceIndicators[i].Value;
        }

        GUI.contentColor = displayColor;

        GUI.Box(
            new Rect(0, 0, 400, 70), 
            $"FPS: {FPS}\nFrame time: {Math.Round(FrameTime, 3)}ms\n" +
            $"Avg frame time: {Math.Round(_frameTimeAverage, 3)}\n" +
            $"Avg fps: {Math.Round(_fpsAverage, 3)}"
        );
    }
}