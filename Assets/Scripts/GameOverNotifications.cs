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
    
    private int _isAvailableRate;
    private bool _isGotNewCharacter;

    private void Start()
    {
        _showCounterGlobal = PlayerPrefs.GetInt("showCounterGlobal", 0);
        _isAvailableRate = PlayerPrefs.GetInt("_isAvailableRate", 0);
    }

    private void OnEnable()
    {
        GlobalEvents<OnStartGame>.Happened += HideNotifications;
        GlobalEvents<OnShowNotifications>.Happened += ShowNotifications;
        GlobalEvents<OnRewardedAvailable>.Happened += IsRewardedAvailable;
        GlobalEvents<OnGiftAvailable>.Happened += IsGiftAvailable;
        GlobalEvents<OnGotNewCharacter>.Happened += OnGotNewCharacter;
    }
    
    private void OnDisable()
    {
        GlobalEvents<OnStartGame>.Happened -= HideNotifications;
        GlobalEvents<OnShowNotifications>.Happened -= ShowNotifications;
        GlobalEvents<OnRewardedAvailable>.Happened -= IsRewardedAvailable;
        GlobalEvents<OnGiftAvailable>.Happened -= IsGiftAvailable;
        GlobalEvents<OnGotNewCharacter>.Happened -= OnGotNewCharacter;
    }

    private void ShowNotifications(OnShowNotifications e)
    {
        UIManager.ShowUiElement("ScreenGameOver");
        ++_showCounter;
        PlayerPrefs.SetInt("showCounterGlobal", ++_showCounterGlobal);

        float ran = Random.value;
        
        if (DefsGame.CoinsCount >= 200) 
            _activeNamesList.Add("NotifyNewCharacter");
        
        if (DefsGame.CoinsCount >= 100 && ran > 0.5f) 
            _activeNamesList.Add("NotifyNextCharacter");
        
        if (_isGotNewCharacter&&_isAvailableRate == 0)
        {
            _activeNamesList.Add("NotifyRate");
            _isGotNewCharacter = false;
        }
        
        if (_activeNamesList.Count < 4 && _isGiftAvailable
            /*||_activeNamesList.Count == 0 && Random.value < 0.5f
            ||_activeNamesList.Count == 1 && Random.value < 0.25f*/)
        {
            _activeNamesList.Add("NotifyGift");
        }
        
        if ((_showCounter == 3 || (_showCounter-3) % 5 == 0) && _isRewardedAvailable)
        {
            _activeNamesList.Add("NotifyRewarded");
        }

        if (_activeNamesList.Count < 4 && (DefsGame.CurrentPointsCount > DefsGame.GameBestScore * 0.5f
                                           || _isGotNewCharacter
                                           || _activeNamesList.Count == 0 && ran < 0.3f
                                           || _activeNamesList.Count == 1 && ran < 0.25f))
        {
            _activeNamesList.Add("NotifyShare");
        }
        
        if (_activeNamesList.Count < 4 && (_activeNamesList.Count == 0 && ran > 0.7f
                                           || _activeNamesList.Count == 1 && ran > 0.75f
                                           || _activeNamesList.Count == 2 && ran > 0.80f))
        {
            _activeNamesList.Add("NotifyGiftWaiting");
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
        UIManager.HideUiElement("ScreenGameOver");
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
    
    private void OnGotNewCharacter(OnGotNewCharacter obj)
    {
        _isGotNewCharacter = true;
    }
}