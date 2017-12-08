﻿using System.Collections.Generic;
using DoozyUI;
using PrefsEditor;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class Points : MonoBehaviour
{
    private const float Delay = 0.0f;
    private bool _isPointAdded;
    private float _startScale;
    private float _time;
    [SerializeField] private Text textField;
    private bool _isVisual;

    private void Start()
    {
        textField.text = "0";
        _startScale = textField.transform.localScale.x;
    }

    private void OnEnable()
    {
//        GlobalEvents<OnPointsShow>.Happened += OnPointsShow;
        GlobalEvents<OnPointsAdd>.Happened += OnPointsAdd;
        GlobalEvents<OnPointsReset>.Happened += OnPointsReset;
    }

    private void OnPointsReset(OnPointsReset obj)
    {
        PrefsManager.CurrentPointsCount = 0;
        textField.text = "0";
        _isVisual = false;
    }

//    private void OnPointsShow(OnPointsShow obj)
//    {
//        _isShowAnimation = true;
//    }

    private void OnPointsAdd(OnPointsAdd e)
    {
        AddPoint(e.PointsCount);
        if (!_isVisual)
        {
            _isVisual = true;
            UIManager.ShowUiElement("LabelPoints");
        }
    }

    private void Update()
    {
        if (_isPointAdded)
        {
            _time += Time.deltaTime;
            if (_time > Delay)
            {
                _time = 0f;
                _isPointAdded = false;
                AddPointVisual();
            }
        }

        if (textField.transform.localScale.x > _startScale)
            textField.transform.localScale = new Vector3(textField.transform.localScale.x - 2.5f * Time.deltaTime,
                textField.transform.localScale.y - 2.5f * Time.deltaTime, 1f);
    }

    private void AddPoint(int count)
    {
        PrefsManager.CurrentPointsCount += count;
        if (PrefsManager.GameBestScore < PrefsManager.CurrentPointsCount)
        {
            PrefsManager.GameBestScore = PrefsManager.CurrentPointsCount;
            SecurePlayerPrefs.SetInt("BestScore",PrefsManager.GameBestScore);
            Analytics.CustomEvent("HighScore",
            new Dictionary<string, object> {{"sessions", PrefsManager.GameplayCounter},{"score", PrefsManager.GameBestScore}});
        }
        _isPointAdded = true;
    }

    private void AddPointVisual()
    {
        textField.text = PrefsManager.CurrentPointsCount.ToString();
        textField.transform.localScale = new Vector3(_startScale * 1.3f, _startScale * 1.3f, 1f);
    }
}