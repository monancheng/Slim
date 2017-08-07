using System.Collections.Generic;
using DoozyUI;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameOverNotifications : MonoBehaviour
{
    [SerializeField] private Text _nextCharacterText;
    [SerializeField] private Text _shareText;
    
    private float _centerPointY = 20f;
    private const float ItemHeightHalf = 50f;
    private const float HeightStep = 120f;
    private bool _isGiftAvailable;
    private bool _isRewardedAvailable;
    private readonly List<string> _activeNamesList  = new List<string>();

    private int _showCounter;
    private int _showCounterGlobal;
    
    private int shareRewardValue;

    private bool _isGotNewCharacter;
    private int _giftValue;

    private void Start()
    {
        _showCounterGlobal = PlayerPrefs.GetInt("showCounterGlobal", 0);
    }

    private void OnEnable()
    {
        GlobalEvents<OnStartGame>.Happened += HideNotifications;
        GlobalEvents<OnShowNotifications>.Happened += ShowNotifications;
        GlobalEvents<OnRewardedAvailable>.Happened += IsRewardedAvailable;
        GlobalEvents<OnGiftAvailable>.Happened += IsGiftAvailable;
        GlobalEvents<OnGotNewCharacter>.Happened += OnGotNewCharacter;
        GlobalEvents<OnBtnRateClick>.Happened += OnBtnRateClick;
    }

    private void OnDisable()
    {
        GlobalEvents<OnStartGame>.Happened -= HideNotifications;
        GlobalEvents<OnShowNotifications>.Happened -= ShowNotifications;
        GlobalEvents<OnRewardedAvailable>.Happened -= IsRewardedAvailable;
        GlobalEvents<OnGiftAvailable>.Happened -= IsGiftAvailable;
        GlobalEvents<OnGotNewCharacter>.Happened -= OnGotNewCharacter;
        GlobalEvents<OnBtnRateClick>.Happened -= OnBtnRateClick;
    }
    
    private void ShowNotifications(OnShowNotifications e)
    {
        UIManager.ShowUiElement("ScreenGameOver");
        ++_showCounter;
        PlayerPrefs.SetInt("showCounterGlobal", ++_showCounterGlobal);

        float ran = Random.value;
        
        // Важность - Высокая
        
        if (DefsGame.CoinsCount >= 200) 
            _activeNamesList.Add("NotifyNewCharacter");
        
        if (_isGiftAvailable
            /*||_activeNamesList.Count == 0 && Random.value < 0.5f
            ||_activeNamesList.Count == 1 && Random.value < 0.25f*/)
        {
            _activeNamesList.Add("NotifyGift");
            if (DefsGame.CoinsCount > 100 && DefsGame.CoinsCount < 155)
                _giftValue = 180-DefsGame.CoinsCount;
            else
            {
                if (ran < 0.5f) _giftValue = 45;
                else _giftValue = 50;
            }
        }
        
        // Важность - Средняя
        
        if (_activeNamesList.Count < 4 && _isGotNewCharacter && DefsGame.RateCounter == 0)
        {
            _activeNamesList.Add("NotifyRate");
            _isGotNewCharacter = false;
        }
        
        if (_activeNamesList.Count < 4 && (_showCounter == 3 || (_showCounter-3) % 5 == 0) && _isRewardedAvailable)
        {
            _activeNamesList.Add("NotifyRewarded");
        }

        if (_activeNamesList.Count < 4 && (DefsGame.CurrentPointsCount > DefsGame.GameBestScore * 0.5f
                                           || _isGotNewCharacter
                                           || _activeNamesList.Count == 0 && ran < 0.3f
                                           || _activeNamesList.Count == 1 && ran < 0.25f))
        {
            _activeNamesList.Add("NotifyShare");
            if (DefsGame.CoinsCount > 100 && DefsGame.CoinsCount < 155)
                shareRewardValue = 180-DefsGame.CoinsCount;
            else
            {
                if (ran < 0.5f) shareRewardValue = 45;
                else shareRewardValue = 50;
            }
            _shareText.text = shareRewardValue.ToString();
        }
        
        // Важность - Низкая
        
        if (_activeNamesList.Count < 4 && (_activeNamesList.Count == 0 && ran > 0.7f
                                           || _activeNamesList.Count == 1 && ran > 0.75f
                                           || _activeNamesList.Count == 2 && ran > 0.80f))
        {
            _activeNamesList.Add("NotifyGiftWaiting");
        }

        if (ran > 0.5f)
        {
            _activeNamesList.Add("NotifyNextCharacter");
            int coinsCount = DefsGame.CoinsCount;
            if (DefsGame.CoinsCount > 200) coinsCount = 200;
            int toNextSkin = 200 - coinsCount;
            _nextCharacterText.text = toNextSkin.ToString();
        }

        SetItemsPositions();
    }

    private void SetItemsPositions()
    {
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
    
    
    //----------------------------------------------------
    // Touches
    //----------------------------------------------------

    public void BtnRateClick()
    {
        GlobalEvents<OnBtnRateClick>.Call(new OnBtnRateClick());
        UIManager.HideUiElement("NotifyRate");
    }
    
    private void OnBtnRateClick(OnBtnRateClick obj)
    {
        DefsGame.RateCounter = 1;
        PlayerPrefs.SetInt("RateCounter", 1);
    }
    
    public void BtnShareClick()
    {
        GlobalEvents<OnBtnShareClick>.Call(new OnBtnShareClick());
        UIManager.HideUiElement("NotifyShare");
    }
    
    public void BtnNewSkinClick()
    {
        GlobalEvents<OnBtnGetRandomSkinClick>.Call(new OnBtnGetRandomSkinClick());
        UIManager.HideUiElement("NotifyNewCharacter");
    }
    
    public void BtnGiftClick()
    {
        GlobalEvents<OnBtnGiftClick>.Call(new OnBtnGiftClick{CoinsCount = _giftValue});
        UIManager.HideUiElement("NotifyGift");
    }
    
    public void BtnRewardedClick()
    {
        GlobalEvents<OnShowRewarded>.Call(new OnShowRewarded());
        UIManager.HideUiElement("NotifyRewarded");
    }
}