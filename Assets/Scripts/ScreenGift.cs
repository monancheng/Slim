using UnityEngine;

public class ScreenGift : MonoBehaviour
{
	private int _coinsCount;
	private bool _isResetTimer;
	private int _giftType;

	private void OnEnable()
	{
		GlobalEvents<OnBtnGiftClick>.Happened += OnBtnGiftClick;
		GlobalEvents<OnBtnGetRandomSkinClick>.Happened += OnBtnGetRandomSkinClick;
	}

	private void OnDisable()
	{
		GlobalEvents<OnBtnGiftClick>.Happened -= OnBtnGiftClick;
		GlobalEvents<OnBtnGetRandomSkinClick>.Happened -= OnBtnGetRandomSkinClick;
	}

	private void OnBtnGetRandomSkinClick(OnBtnGetRandomSkinClick obj)
	{
		_giftType = 2;
		ShowGiftAnimation();
	}

	private void OnBtnGiftClick(OnBtnGiftClick obj)
	{
		_coinsCount = obj.CoinsCount;
		_isResetTimer = obj.IsResetTimer;
		_giftType = 1;
		ShowGiftAnimation();
	}

	private void ShowGiftAnimation()
	{
		EndShowing();
	}

	private void EndShowing()
	{
		if (_giftType == 1) MakeAGift();
		else if (_giftType == 2) MakeAGiftRandomSkin();
	}

	private void MakeAGift()
	{
		GlobalEvents<OnGiftTake>.Call(new OnGiftTake{CoinsCount = _coinsCount, IsResetTimer = _isResetTimer});
		Invoke("HideGiftScreen", 2f);	
	}
	
	
	private void MakeAGiftRandomSkin()
	{
		GlobalEvents<OnGiftTake>.Call(new OnGiftTake{CoinsCount = _coinsCount, IsResetTimer = _isResetTimer});
		Invoke("HideGiftScreen", 2f);	
	}
	
	private void HideGiftScreen()
	{
		GlobalEvents<OnGiftCollected>.Call(new OnGiftCollected());
	}
}
