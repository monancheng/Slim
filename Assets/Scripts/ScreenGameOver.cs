using System;
using System.Collections.Generic;
using DoozyUI;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ScreenGameOver : MonoBehaviour
{
    [SerializeField] private Text _nextCharacterText;
    [SerializeField] private Text _shareText;
    [SerializeField] private Text _wordsProgressText;
    [SerializeField] private Text _wordsText;
    
    private float _centerPointY = 60f;
    private const float ItemHeightHalf = 50f;
    private const float HeightStep = 120f;
    private bool _isGiftAvailable;
    private bool _isRewardedAvailable;
    private readonly List<string> _activeNamesList  = new List<string>();

    private int _showCounter;
    private int _showCounterGlobal;
    
    private int _shareRewardValue;

    private bool _isGotNewCharacter;
    private int _giftValue;
    private bool isVisual;
    private bool _isGotWord;
    private bool _isWaitNewWord;
    private bool _isWordActive;
    private bool _isAllSkinsOpened;

    private void Start()
    {
        _showCounterGlobal = PlayerPrefs.GetInt("showCounterGlobal", 0);
    }

    private void OnEnable()
    {
        // Глобальные
        GlobalEvents<OnStartGame>.Happened += OnHideNotifications;
        GlobalEvents<OnShowGameOverScreen>.Happened += OnShowGameOverScreen;
        GlobalEvents<OnRewardedAvailable>.Happened += IsRewardedAvailable;
        GlobalEvents<OnGiftAvailable>.Happened += IsGiftAvailable;
        GlobalEvents<OnCoinsAdded>.Happened += OnCoinsAdded;
        GlobalEvents<OnWordCollected>.Happened += OnWordCollected;
        GlobalEvents<OnWordUpdateProgress>.Happened += OnWordGotChar;
        GlobalEvents<OnWordNeedToWait>.Happened += OnWordNeedToWait;
        GlobalEvents<OnWordsAvailable>.Happened += OnWordsAvailable;
        GlobalEvents<OnSkinAllOpened>.Happened += OnSkinAllOpened;
        
        // Внутренние
        GlobalEvents<OnGotNewCharacter>.Happened += OnGotNewCharacter;
        GlobalEvents<OnBtnRateClick>.Happened += OnBtnRateClick;
        GlobalEvents<OnGiftCollected>.Happened += OnGiftCollected;
		GlobalEvents<OnGifShared>.Happened += OnGifShared;
//		Record.OnShareGIFEvent += OnShareGIFEvent;
    }

    private void OnDisable()
    {
        GlobalEvents<OnStartGame>.Happened -= OnHideNotifications;
        GlobalEvents<OnShowGameOverScreen>.Happened -= OnShowGameOverScreen;
        GlobalEvents<OnRewardedAvailable>.Happened -= IsRewardedAvailable;
        GlobalEvents<OnGiftAvailable>.Happened -= IsGiftAvailable;
        GlobalEvents<OnCoinsAdded>.Happened -= OnCoinsAdded;
        GlobalEvents<OnWordCollected>.Happened -= OnWordCollected;
        GlobalEvents<OnWordUpdateProgress>.Happened -= OnWordGotChar;
        GlobalEvents<OnWordNeedToWait>.Happened -= OnWordNeedToWait;
        GlobalEvents<OnWordsAvailable>.Happened -= OnWordsAvailable;
        GlobalEvents<OnSkinAllOpened>.Happened -= OnSkinAllOpened;
        GlobalEvents<OnGotNewCharacter>.Happened -= OnGotNewCharacter;
        GlobalEvents<OnBtnRateClick>.Happened -= OnBtnRateClick;
        GlobalEvents<OnGiftCollected>.Happened -= OnGiftCollected;
    }

    private void OnShowGameOverScreen(OnShowGameOverScreen e)
    {

        DefsGame.CurrentScreen = DefsGame.SCREEN_NOTIFICATIONS;
            
        ++_showCounter;
        PlayerPrefs.SetInt("showCounterGlobal", ++_showCounterGlobal);

        float ran = Random.value;
        
        // Важность - Высокая

        if (!_isAllSkinsOpened && DefsGame.CoinsCount >= 200 && DefsGame.QUEST_CHARACTERS_Counter < DefsGame.FaceAvailable.Length - 1)
        {
            AddNotifyNewGift();
        }

        if (_isGiftAvailable)
        {
            AddNotifyGift();
        }
        
        if (_isGotWord)
        {
            AddNotifyWord();
        }
        
        // Важность - Средняя
        
        if (_activeNamesList.Count < 4 && _isGotNewCharacter && DefsGame.RateCounter == 0)
        {
            _activeNamesList.Add("NotifyRate");
            _isGotNewCharacter = false;
        }
        
        if (_activeNamesList.Count < 4 && (DefsGame.GameplayCounter == 3 || (DefsGame.GameplayCounter-3) % 5 == 0)/* && _isRewardedAvailable*/)
        {
            _activeNamesList.Add("NotifyRewarded");
        }
        
        if (Random.value > 0.5f) AddWordTimerOrProgress();

        if (_activeNamesList.Count < 4 && (DefsGame.CurrentPointsCount > DefsGame.GameBestScore * 0.5f
                                           || _isGotNewCharacter
                                           || _activeNamesList.Count == 0 && ran < 0.3f
                                           || _activeNamesList.Count == 1 && ran < 0.25f))
        {
            _activeNamesList.Add("NotifyShare");
			UIManager.ShowUiElement ("ScreenGameOverImageShareGif");
            if (DefsGame.CoinsCount > 100 && DefsGame.CoinsCount < 155)
				_shareRewardValue = 180-DefsGame.CoinsCount;
            else
            {
				if (ran < 0.5f) _shareRewardValue = 25;
				else _shareRewardValue = 30;
            }
			_shareText.text = _shareRewardValue.ToString();
        }
        
        // Важность - Низкая
        ran = Random.value;
        if (_giftValue == 0 && _activeNamesList.Count < 4 && (_activeNamesList.Count == 0 && ran > 0.7f
                                           || _activeNamesList.Count == 1 && ran > 0.75f
                                           || _activeNamesList.Count == 2 && ran > 0.80f))
        {
            _activeNamesList.Add("NotifyGiftWaiting");
        }

        if (!_isAllSkinsOpened && _activeNamesList.Count < 4 && (_activeNamesList.Count == 0 && ran > 0.7f
                                              || _activeNamesList.Count == 1 && ran > 0.75f
                                              || _activeNamesList.Count == 2 && ran > 0.80f)
        && ran > 0.4f)
        {
            AddNotifyNextSkin();
        } 
        
        if (MyAds.NoAds == 0 && _activeNamesList.Count < 4 && Random.value > 0.5f)
        {
            _activeNamesList.Add("NotifyNoAds");
        }
        
        if (_activeNamesList.Count < 4 && Random.value > 0.5f)
        {
            _activeNamesList.Add("NotifyTier1");
        }
        
        if (_activeNamesList.Count < 4 && Random.value > 0.5f)
        {
            _activeNamesList.Add("NotifyTier2");
        }

        // Перемешиваем элементы списка, чтобы они располагались рандомно по оси У
        ShuffleItems();
        SetItemsPositions();
        ShowActiveItems();
    }

    private void AddWordTimerOrProgress()
    {
        if (_activeNamesList.Count < 4 && _isWordActive && !_isGotWord)
        {
            if (_isWaitNewWord)
            {
                _activeNamesList.Add("NotifyWordTimer");
            }
            else
            {
                _activeNamesList.Add("NotifyWordProgress");
            }
        }
    }

    private void AddNotifyNewGift()
    {
        _activeNamesList.Add("NotifyNewCharacter");
    }

    private void ShuffleItems()
    {
        // Перемешиваем 10 раз
        for (int n = 0; n < 10; n++)
        for (int i = 0; i < _activeNamesList.Count; i++)
        {
            int j = Random.Range(0, _activeNamesList.Count-1);
            if (j != i)
            {
                string str = _activeNamesList[i];
                _activeNamesList[i] = _activeNamesList[j];
                _activeNamesList[j] = str;
            }
        }
    }

    private void AddNotifyGift()
    {
        _activeNamesList.Add("NotifyGift");
        if (DefsGame.CoinsCount > 100 && DefsGame.CoinsCount < 155)
            _giftValue = 190-DefsGame.CoinsCount;
        else
        {
            if (Random.value < 0.5f) _giftValue = 40;
            else _giftValue = 45;
        }
    }
    
    private void AddNotifyWord()
    {
        _activeNamesList.Add("NotifyWord");
    }
    
    private void AddNotifyNextSkin(int spendMoneyCount = 0)
    {
        if (DefsGame.CoinsCount - spendMoneyCount < 200 && DefsGame.QUEST_CHARACTERS_Counter < DefsGame.FaceAvailable.Length - 1)
        {
            _activeNamesList.Add("NotifyNextCharacter");
        }
    }

    private void SetItemsPositions()
    {
        float startPos = CalcStartPosition(_activeNamesList.Count);
        bool isLeft = true;
        for (int i = 0; i < _activeNamesList.Count; i++)
        {
            var element = GetUIElement(_activeNamesList[i]);
            if (element)
            {
                if (isLeft)
                {
                    element.customStartAnchoredPosition = new Vector3(0, startPos + i * HeightStep, 0f);
                    element.inAnimations.move.moveDirection = Move.MoveDirection.Left;
                    element.outAnimations.move.moveDirection = Move.MoveDirection.Left;
                }
                else
                {
                    element.customStartAnchoredPosition = new Vector3(0, startPos + i * HeightStep, 0f);
                    element.inAnimations.move.moveDirection = Move.MoveDirection.Right;
                    element.outAnimations.move.moveDirection = Move.MoveDirection.Right;
                }
                isLeft = !isLeft;
                element.inAnimations.move.startDelay = i * 0.1f;
                element.outAnimations.move.startDelay = i * 0.1f;
                element.useCustomStartAnchoredPosition = true;
            }
        }
    }

    public void ShowActiveItems()
    {
        if (_activeNamesList.Count == 0)
        {
            GlobalEvents<OnNoGameOverButtons>.Call(new OnNoGameOverButtons());
            return;
        } 
        
//        UIManager.ShowUiElement("ScreenGameOver");
        for (int i = 0; i < _activeNamesList.Count; i++)
        {
            var element = GetUIElement(_activeNamesList[i]);
            if (element)
            {
                UIManager.ShowUiElement(_activeNamesList[i]);
            }
        }
        
        isVisual = true;
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

    public void Hide()
    {
        HideActiveItems();
        _activeNamesList.Clear();

//        UIManager.HideUiElement("ScreenGameOver");
    }

    public void HideActiveItems()
    {
        foreach (string t in _activeNamesList)
        {
            var element = GetUIElement(t);
            if (element)
            {
                UIManager.HideUiElement(t);
            }
        }
        isVisual = false;

		// TEMP
		UIManager.HideUiElement ("ScreenGameOverImageShareGif");
    }

    private void OnHideNotifications(OnStartGame e)
    {
        Hide();
    }

    private void IsRewardedAvailable(OnRewardedAvailable e)
    {
        _isRewardedAvailable = e.IsAvailable;
    }

    private void IsGiftAvailable(OnGiftAvailable e)
    {
        _isGiftAvailable = e.IsAvailable;
        
        int idNotifyOld = _activeNamesList.IndexOf("NotifyGiftWaiting");
        if (!_isGiftAvailable || !isVisual || idNotifyOld == -1) return;

        AddNotifyGift();
   
            
        var element = GetUIElement(_activeNamesList[idNotifyOld]);
        UIManager.HideUiElement(_activeNamesList[idNotifyOld]);
        var element2 = GetUIElement("NotifyGift");
        if (element)
        {
            element2.customStartAnchoredPosition = element.customStartAnchoredPosition;
            element2.useCustomStartAnchoredPosition = true;
            element2.inAnimations.move.moveDirection = element.inAnimations.move.moveDirection;
            element2.outAnimations.move.moveDirection = element.outAnimations.move.moveDirection;
            element2.inAnimations.move.startDelay = element.inAnimations.move.startDelay;
            element2.outAnimations.move.startDelay = element.outAnimations.move.startDelay;
        }
        _activeNamesList.RemoveAt(idNotifyOld);
        
        UIManager.ShowUiElement("NotifyGift");
    }
    
    private void OnCoinsAdded(OnCoinsAdded obj)
    {
        int idNotifyOld = _activeNamesList.IndexOf("NotifyNextCharacter");
        
        if (!isVisual || idNotifyOld == -1) return;

        if (obj.Total >= 200)
        {
            AddNotifyNewGift();
            var element = GetUIElement(_activeNamesList[idNotifyOld]);
            UIManager.HideUiElement(_activeNamesList[idNotifyOld]);
            var element2 = GetUIElement("NotifyNewCharacter");
            if (element)
            {
                element2.customStartAnchoredPosition = element.customStartAnchoredPosition;
                element2.inAnimations.move.moveDirection = element.inAnimations.move.moveDirection;
                element2.outAnimations.move.moveDirection = element.outAnimations.move.moveDirection;
                element2.inAnimations.move.startDelay = element.inAnimations.move.startDelay;
                element2.outAnimations.move.startDelay = element.outAnimations.move.startDelay;
                element2.useCustomStartAnchoredPosition = true;
            }
            _activeNamesList.RemoveAt(idNotifyOld);
            UIManager.ShowUiElement("NotifyNewCharacter");
        } else 
        {
            int toNextSkin = 200 - obj.Total;
            _nextCharacterText.text = toNextSkin.ToString();
        }
    }

    private void OnGotNewCharacter(OnGotNewCharacter obj)
    {
        _isGotNewCharacter = true;
    }
    
    private void OnGiftCollected(OnGiftCollected obj)
    {
        int idNotifyOld = _activeNamesList.IndexOf("NotifyWord");
        if (idNotifyOld != -1)
        { 
            _activeNamesList.RemoveAt(idNotifyOld);
            AddWordTimerOrProgress();
        }

        SetItemsPositions();
        ShowActiveItems();
        DefsGame.CurrentScreen = DefsGame.SCREEN_MENU;
    }
    
    private void OnWordsAvailable(OnWordsAvailable obj)
    {
        _isWordActive = obj.IsAvailable;
    }
    
    private void OnWordNeedToWait(OnWordNeedToWait obj)
    {
        _isWaitNewWord = obj.IsWait;
        
        int idNotifyOld = _activeNamesList.IndexOf("NotifyWordTimer");
        
        if (!isVisual || idNotifyOld == -1) return;

        _activeNamesList.Add("NotifyWordProgress");
        
        var element = GetUIElement(_activeNamesList[idNotifyOld]);
        UIManager.HideUiElement(_activeNamesList[idNotifyOld]);
        var element2 = GetUIElement("NotifyWordProgress");
        if (element)
        {
            element2.customStartAnchoredPosition = element.customStartAnchoredPosition;
            element2.useCustomStartAnchoredPosition = true;
            element2.inAnimations.move.moveDirection = element.inAnimations.move.moveDirection;
            element2.outAnimations.move.moveDirection = element.outAnimations.move.moveDirection;
            element2.outAnimations.move.startDelay = element.outAnimations.move.startDelay;
        }
        _activeNamesList.RemoveAt(idNotifyOld);
        
        UIManager.ShowUiElement("NotifyWordProgress");
    }
    
    private void OnSkinAllOpened(OnSkinAllOpened obj)
    {
        _isAllSkinsOpened = true;
    }

//	private void OnShareGIFEvent ()
	private void OnGifShared(OnGifShared obj)
	{
		int id = _activeNamesList.IndexOf("NotifyShare"); 
		if (id != -1) {
			_activeNamesList.RemoveAt (id);

//			Record.DOReset ();

			GlobalEvents<OnBtnShareGifClick>.Call (new OnBtnShareGifClick{CoinsCount = _shareRewardValue});
			_shareRewardValue = 0;

			HideActiveItems ();

			UIManager.HideUiElement ("NotifyShare");
			GlobalEvents<OnHideMenu>.Call(new OnHideMenu());
		}
	}
    
    private void OnWordGotChar(OnWordUpdateProgress obj)
    {
        _wordsProgressText.text = obj.Text;
    }
    
    private void OnWordCollected(OnWordCollected obj)
    {
        _wordsText.text = obj.Text;
        _isGotWord = true;
    }
    
    //----------------------------------------------------
    // Touches
    //----------------------------------------------------

    public void BtnShareClick()
    {
        GlobalEvents<OnBtnShareClick>.Call(new OnBtnShareClick());
        UIManager.HideUiElement("NotifyShare");
    }
    
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
    
    public void BtnNewSkinClick()
    {
        HideActiveItems();
        int id = _activeNamesList.IndexOf("NotifyNewCharacter"); 
        if (id != -1) _activeNamesList.RemoveAt(id);

        AddNotifyNextSkin(200);
        
        GlobalEvents<OnBtnGetRandomSkinClick>.Call(new OnBtnGetRandomSkinClick());
        GlobalEvents<OnHideMenu>.Call(new OnHideMenu());
        GlobalEvents<OnHideTubes>.Call(new OnHideTubes());
    }
    
    
    public void BtnWordClick()
    {
        HideActiveItems();

        _isGotWord = false;
//        AddNotifyNextSkin(200);
        
        GlobalEvents<OnBtnWordClick>.Call(new OnBtnWordClick{CoinsCount = 100, IsResetTimer = false});
        GlobalEvents<OnHideMenu>.Call(new OnHideMenu());
        GlobalEvents<OnHideTubes>.Call(new OnHideTubes());
    }
    
    public void BtnGiftClick()
    {
        HideActiveItems();
        int id = _activeNamesList.IndexOf("NotifyGift"); 
        if (id != -1) _activeNamesList.RemoveAt(id);
        
        _activeNamesList.Add("NotifyGiftWaiting");
            
        GlobalEvents<OnBtnGiftClick>.Call(new OnBtnGiftClick{CoinsCount = _giftValue, IsResetTimer = true});
        _giftValue = 0;
        GlobalEvents<OnHideMenu>.Call(new OnHideMenu());
        GlobalEvents<OnHideTubes>.Call(new OnHideTubes());
    }
    
    public void BtnRewardedClick()
    {
        GlobalEvents<OnShowRewarded>.Call(new OnShowRewarded());
        UIManager.HideUiElement("NotifyRewarded");
    }
    
    public void BtnNoAds()
    {
        UIManager.HideUiElement("NotifyNoAds");
    }
    
    public void BtnTier1()
    {
        UIManager.HideUiElement("NotifyTier1");
    }
    
    public void BtnTier2()
    {
        UIManager.HideUiElement("NotifyTier2");
    }
}