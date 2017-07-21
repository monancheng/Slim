using System.Collections.Generic;
using DoozyUI;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameOverNotifications : MonoBehaviour
{
    private float _centerPointY = 20f;
    private const float ItemHeightHalf = 50f;
    private const float HeightStep = 120f;
    private bool _isGiftAvailable;
    private bool _isRewardedAvailable;
    private readonly List<string> _activeNamesList  = new List<string>();

    private int _showCounter;
    private int _showCounterGlobal;
    private int _shareCounter;
    private bool _isShowShare;
    [SerializeField] private int ShareFirstShow = 3;
    [SerializeField] private int ShareSecondShow = 5;
    [SerializeField] private int ShareThirdShow = 10;
    
    private int _isAvailableRate;
    private bool _isShowRate;

    private void Start()
    {
        _showCounterGlobal = PlayerPrefs.GetInt("showCounterGlobal", 0);
        _shareCounter = PlayerPrefs.GetInt("shareCounter", 0);
        _isAvailableRate = PlayerPrefs.GetInt("_isAvailableRate", 0);
    }

    private void OnEnable()
    {
        GlobalEvents<OnStartGame>.Happened += HideNotifications;
        GlobalEvents<OnShowNotifications>.Happened += ShowNotifications;
        GlobalEvents<OnRewardedAvailable>.Happened += IsRewardedAvailable;
        GlobalEvents<OnGiftAvailable>.Happened += IsGiftAvailable;
    }

    private void OnDisable()
    {
        GlobalEvents<OnStartGame>.Happened -= HideNotifications;
        GlobalEvents<OnShowNotifications>.Happened -= ShowNotifications;
        GlobalEvents<OnRewardedAvailable>.Happened -= IsRewardedAvailable;
        GlobalEvents<OnGiftAvailable>.Happened -= IsGiftAvailable;
    }

    private void ShowNotifications(OnShowNotifications e)
    {
        ++_showCounter;
        PlayerPrefs.SetInt("showCounterGlobal", ++_showCounterGlobal);
        PlayerPrefs.GetInt("shareCounter", ++_shareCounter);

        CheckParams();
        
        if (_isShowShare) _activeNamesList.Add("NotifyShare");
        if (_isShowRate) _activeNamesList.Add("NotifyRate");

        if (_showCounter == 3 || (_showCounter-3) % 5 == 0 && _isRewardedAvailable)
        {
            _activeNamesList.Add("NotifyRewarded");
        }
        
        if (_activeNamesList.Count < 4 && _isGiftAvailable
            ||_activeNamesList.Count == 0 && Random.value < 0.5f
            ||_activeNamesList.Count == 1 && Random.value < 0.25f)
        {
            _activeNamesList.Add("NotifyGift");
        }

        float startPos = CalcStartPosition(_activeNamesList.Count);

        for (int i = 0; i < _activeNamesList.Count; i++)
        {
            var element = GetUIElement(_activeNamesList[i]);
            if (element)
            {
                element.customStartAnchoredPosition = new Vector3(0f, startPos + i*HeightStep, 0f);
                element.useCustomStartAnchoredPosition = true;
                UIManager.ShowUiElement(_activeNamesList[i]);
            }
        }
    }

    private void CheckParams()
    {
        if (_shareCounter == ShareFirstShow) _isShowShare = true; else
        if (_shareCounter == ShareSecondShow) _isShowShare = true; else
        if (_shareCounter == ShareThirdShow) _isShowShare = true;

        if (_isAvailableRate == 0) _isShowRate = true;
    }

    private float CalcStartPosition(int notificationCounter)
    {
        return _centerPointY - notificationCounter * HeightStep * 0.5f + ItemHeightHalf;
    }

    private UIElement GetUIElement(string elementName)
    {
        List<UIElement> list = UIManager.GetUiElements(elementName);
        if (list.Count > 0)
        {
            return list[0];
        }
        return null;
    }

    private void HideNotifications(OnStartGame e)
    {
        foreach (string t in _activeNamesList)
        {
            var element = GetUIElement(t);
            if (element)
            {
                UIManager.HideUiElement(t);
            }
        }
        _activeNamesList.Clear();
    }

    private void IsRewardedAvailable(OnRewardedAvailable e)
    {
        _isRewardedAvailable = e.IsAvailable;
    }

    private void IsGiftAvailable(OnGiftAvailable e)
    {
        _isGiftAvailable = e.IsAvailable;
    }
}