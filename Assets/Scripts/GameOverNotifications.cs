using System.Collections.Generic;
using DoozyUI;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameOverNotifications : MonoBehaviour
{
    [SerializeField] private Text _nextCharacterText;
    [SerializeField] private Text _shareText;
    [SerializeField] private Text _wordsProgressText;
    [SerializeField] private Text _wordsText;
    
    private float _centerPointY = 20f;
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
    private int _wordRewardValue;
    private bool _isWaitNewWord;
    private bool _isWordActive;

    private void Start()
    {
        _showCounterGlobal = PlayerPrefs.GetInt("showCounterGlobal", 0);
    }

    private void OnEnable()
    {
        // Глобальные
        GlobalEvents<OnStartGame>.Happened += OnHideNotifications;
        GlobalEvents<OnShowNotifications>.Happened += OnShowNotifications;
        GlobalEvents<OnRewardedAvailable>.Happened += IsRewardedAvailable;
        GlobalEvents<OnGiftAvailable>.Happened += IsGiftAvailable;
        GlobalEvents<OnCoinsAdded>.Happened += OnCoinsAdded;
        GlobalEvents<OnWordCollected>.Happened += OnWordCollected;
        GlobalEvents<OnWordUpdateProgress>.Happened += OnWordGotChar;
        GlobalEvents<OnWordNeedToWait>.Happened += OnWordNeedToWait;
        GlobalEvents<OnWordsAvailable>.Happened += OnWordsAvailable;
        
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
        GlobalEvents<OnShowNotifications>.Happened -= OnShowNotifications;
        GlobalEvents<OnRewardedAvailable>.Happened -= IsRewardedAvailable;
        GlobalEvents<OnGiftAvailable>.Happened -= IsGiftAvailable;
        GlobalEvents<OnCoinsAdded>.Happened -= OnCoinsAdded;
        GlobalEvents<OnWordCollected>.Happened -= OnWordCollected;
        GlobalEvents<OnWordUpdateProgress>.Happened -= OnWordGotChar;
        GlobalEvents<OnWordNeedToWait>.Happened -= OnWordNeedToWait;
        GlobalEvents<OnWordsAvailable>.Happened -= OnWordsAvailable;
        GlobalEvents<OnGotNewCharacter>.Happened -= OnGotNewCharacter;
        GlobalEvents<OnBtnRateClick>.Happened -= OnBtnRateClick;
        GlobalEvents<OnGiftCollected>.Happened -= OnGiftCollected;
    }

    private void OnShowNotifications(OnShowNotifications e)
    {

        DefsGame.CurrentScreen = DefsGame.SCREEN_NOTIFICATIONS;
            
        ++_showCounter;
        PlayerPrefs.SetInt("showCounterGlobal", ++_showCounterGlobal);

        float ran = Random.value;
        
        // Важность - Высокая

        if (DefsGame.CoinsCount >= 200 && DefsGame.QUEST_CHARACTERS_Counter < DefsGame.FaceAvailable.Length - 1)
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

        if (_activeNamesList.Count < 4 && (_activeNamesList.Count == 0 && ran > 0.7f
                                              || _activeNamesList.Count == 1 && ran > 0.75f
                                              || _activeNamesList.Count == 2 && ran > 0.80f)
        && ran > 0.5f)
        {
            AddNotifyNextSkin();
        }

        // Перемешиваем элементы списка, чтобы они располагались рандомно по оси У
        ShuffleItems();
        SetItemsPositions();
        ShowNotifications();
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
        if (DefsGame.CoinsCount > 100 && DefsGame.CoinsCount < 155)
            _wordRewardValue = 190-DefsGame.CoinsCount;
        else
        {
            if (Random.value < 0.5f) _wordRewardValue = 40;
            else _wordRewardValue = 45;
        }
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
        
        for (int i = 0; i < _activeNamesList.Count; i++)
        {
            var element = GetUIElement(_activeNamesList[i]);
            if (element)
            {
                element.customStartAnchoredPosition = new Vector3(0f, startPos + i*HeightStep, 0f);
                element.useCustomStartAnchoredPosition = true;
            }
        }
    }

    public void ShowNotifications()
    {
        if (_activeNamesList.Count == 0)
        {
            GlobalEvents<OnNoGameOverButtons>.Call(new OnNoGameOverButtons());
            return;
        } 
        
        UIManager.ShowUiElement("ScreenGameOver");
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

    private void HideAndRemoveNotifications()
    {
        HideNotifications();
        _activeNamesList.Clear();

        UIManager.HideUiElement("ScreenGameOver");
    }

    public void HideNotifications()
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
        HideAndRemoveNotifications();
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
        ShowNotifications();
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
        }
        _activeNamesList.RemoveAt(idNotifyOld);
        
        UIManager.ShowUiElement("NotifyWordProgress");
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

			HideNotifications ();

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
        HideNotifications();
        int id = _activeNamesList.IndexOf("NotifyNewCharacter"); 
        if (id != -1) _activeNamesList.RemoveAt(id);

        AddNotifyNextSkin(200);
        
        GlobalEvents<OnBtnGetRandomSkinClick>.Call(new OnBtnGetRandomSkinClick());
        GlobalEvents<OnHideMenu>.Call(new OnHideMenu());
    }
    
    
    public void BtnWordClick()
    {
        HideNotifications();

        _isGotWord = false;
//        AddNotifyNextSkin(200);
        
        GlobalEvents<OnBtnWordClick>.Call(new OnBtnWordClick{CoinsCount = _wordRewardValue, IsResetTimer = false});
        _wordRewardValue = 0;
        GlobalEvents<OnHideMenu>.Call(new OnHideMenu());
    }
    
    public void BtnGiftClick()
    {
        HideNotifications();
        int id = _activeNamesList.IndexOf("NotifyGift"); 
        if (id != -1) _activeNamesList.RemoveAt(id);
        
        _activeNamesList.Add("NotifyGiftWaiting");
            
        GlobalEvents<OnBtnGiftClick>.Call(new OnBtnGiftClick{CoinsCount = _giftValue, IsResetTimer = true});
        _giftValue = 0;
        GlobalEvents<OnHideMenu>.Call(new OnHideMenu());
    }
    
    public void BtnRewardedClick()
    {
        GlobalEvents<OnShowRewarded>.Call(new OnShowRewarded());
        UIManager.HideUiElement("NotifyRewarded");
    }
}