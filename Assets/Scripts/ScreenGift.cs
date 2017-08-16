using System.Collections.Generic;
using DoozyUI;
using UnityEngine;

public class ScreenGift : MonoBehaviour
{
	private int _coinsCount;
	private bool _isResetTimer;
	private int _giftType;
	private bool isFirstTime;
	
	private float _centerPointY = 20f;
	[SerializeField] private Object _gift;

	private void OnEnable()
	{
		GlobalEvents<OnBtnGiftClick>.Happened += OnBtnGiftClick;
		GlobalEvents<OnBtnWordClick>.Happened += OnBtnWordClick;
		GlobalEvents<OnBtnGetRandomSkinClick>.Happened += OnBtnGetRandomSkinClick;
		GlobalEvents<OnBtnShareGifClick>.Happened += OnBtnShareGifClick;
		GlobalEvents<OnGiftAnimationDone>.Happened += OnGiftAnimationDone;
		GlobalEvents<OnHideGiftScreen>.Happened += OnHideGiftScreen;
	}

	private void OnDisable()
	{
		GlobalEvents<OnBtnGiftClick>.Happened -= OnBtnGiftClick;
		GlobalEvents<OnBtnWordClick>.Happened -= OnBtnWordClick;
		GlobalEvents<OnBtnGetRandomSkinClick>.Happened -= OnBtnGetRandomSkinClick;
		GlobalEvents<OnGiftAnimationDone>.Happened -= OnGiftAnimationDone;
		GlobalEvents<OnHideGiftScreen>.Happened -= OnHideGiftScreen;
	}

	private void OnHideGiftScreen(OnHideGiftScreen obj)
	{
		// Предлагаем Еще один подарок
		if (isFirstTime)
		{
			isFirstTime = false;
			
			UIManager.ShowUiElement("ScreenGameOverBtnBack");
			UIElement element = null;
			if (_giftType == 1)
			{
				UIManager.ShowUiElement("NotifyGiftExtra");
				element = GetUIElement("NotifyGiftExtra");
			}
			else
			if (_giftType == 2)
			{
				UIManager.ShowUiElement("NotifySkinExtra");
				element = GetUIElement("NotifySkinExtra");
			}
			else
			if (_giftType == 3)
			{
				UIManager.ShowUiElement("NotifyWordExtra");
				element = GetUIElement("NotifyWordExtra");
			}
			
			if (element)
			{
				element.customStartAnchoredPosition = new Vector3(0f, _centerPointY, 0f);
				element.useCustomStartAnchoredPosition = true;
			}
			
			return;
		} 
		
		UIManager.HideUiElement("ScreenGift");
		GlobalEvents<OnGiftCollected>.Call(new OnGiftCollected());
		GlobalEvents<OnShowMenu>.Call(new OnShowMenu());
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

	private void OnBtnGetRandomSkinClick(OnBtnGetRandomSkinClick obj)
	{
		isFirstTime = true;
		_giftType = 2;

		CreateGiftAnimation();
	}

	private void OnBtnGiftClick(OnBtnGiftClick obj)
	{
		isFirstTime = true;
		_coinsCount = obj.CoinsCount;
		_isResetTimer = obj.IsResetTimer;
		_giftType = 1;

		CreateGiftAnimation();
	}
	
	private void OnBtnShareGifClick(OnBtnShareGifClick obj)
	{
		isFirstTime = false;
		_coinsCount = obj.CoinsCount;
		_giftType = 4;

		CreateGiftAnimation();
	}
	
	
	private void OnBtnWordClick(OnBtnWordClick obj)
	{
		isFirstTime = true;
		_coinsCount = obj.CoinsCount;
		_isResetTimer = obj.IsResetTimer;
		_giftType = 3;

		CreateGiftAnimation();
	}
	
	private void CreateGiftAnimation()
	{
		Instantiate(_gift);
	}
	
	private void OnGiftAnimationDone(OnGiftAnimationDone obj)
	{
		UIManager.ShowUiElement("ScreenGift");
		if (_giftType == 1) MakeAGift();
		else if (_giftType == 2) MakeAGiftRandomSkin();
		else if (_giftType == 3) MakeAGiftWord();
		else if (_giftType == 4) MakeAShareGift();
	}

	private void MakeAGift()
	{
		GlobalEvents<OnGiftResetTimer>.Call(new OnGiftResetTimer{IsResetTimer = _isResetTimer});
		GlobalEvents<OnCoinsAddToScreen>.Call(new OnCoinsAddToScreen{CoinsCount = _coinsCount});
	}
	
	private void MakeAGiftRandomSkin()
	{
		GlobalEvents<OnGiftShowRandomSkinAnimation>.Call(new OnGiftShowRandomSkinAnimation());
	}
	
	private void MakeAGiftWord()
	{
		GlobalEvents<OnWordStartTimer>.Call(new OnWordStartTimer());
		GlobalEvents<OnCoinsAddToScreen>.Call(new OnCoinsAddToScreen{CoinsCount = _coinsCount});
	}
	
	private void MakeAShareGift()
	{
		GlobalEvents<OnCoinsAddToScreen>.Call(new OnCoinsAddToScreen{CoinsCount = _coinsCount});
	}

	public void BtnClose()
	{
		UIManager.HideUiElement("ScreenGameOverBtnBack");
		UIManager.HideUiElement("NotifyGiftExtra");
		UIManager.HideUiElement("NotifySkinExtra");
		UIManager.HideUiElement("NotifyWordExtra");
		GlobalEvents<OnHideGiftScreen>.Call(new OnHideGiftScreen());
	}
}
