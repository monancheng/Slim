using System;
using DoozyUI;
using UnityEngine;

public class ScreenCoins : MonoBehaviour
{
    private bool _isWaitReward;
    private bool isShowBtnViveoAds;

    public static event Action<int> OnAddCoinsVisual;

    private void OnEnable()
    {
        GlobalEvents<OnGiveReward>.Happened += GetReward;
        GlobalEvents<OnRewardedAvailable>.Happened += OnRewardedAvailable;
        GlobalEvents<OnShowScreenCoins>.Happened += OnShowScreenCoins;
    }

    private void OnDisable()
    {
        GlobalEvents<OnGiveReward>.Happened -= GetReward;
        GlobalEvents<OnRewardedAvailable>.Happened -= OnRewardedAvailable;
        GlobalEvents<OnShowScreenCoins>.Happened -= OnShowScreenCoins;
    }

    private void OnShowScreenCoins(OnShowScreenCoins obj)
    {
        Show();
    }

    public void ShowButtons()
    {
        UIManager.ShowUiElement("ScreenCoinsBackground");
        UIManager.ShowUiElement("ScreenCoins");
        UIManager.ShowUiElement("ScreenCoinsBtnBack");
        UIManager.ShowUiElement("BtnTier1");
        UIManager.ShowUiElement("BtnTier2");
        if (isShowBtnViveoAds)
        {
            UIManager.ShowUiElement("ScreenCoinsBtnVideo");
        }
        
        // TEMP
        UIManager.ShowUiElement("ScreenCoinsBtnVideo");
        
        
//        if (MyAds.noAds < 1) UIManager.ShowUiElement("ScreenCoinsBtnNoAds");
//#if UNITY_IPHONE
//        UIManager.ShowUiElement("ScreenCoinsBtnRestore");
//#endif
        
        UIManager.HideUiElement("ScreenMenuBtnPlus");
    }

    public void HideButtons()
    {
        UIManager.HideUiElement("ScreenCoins");
        UIManager.HideUiElement("ScreenCoinsBtnBack");
        UIManager.HideUiElement("BtnTier1");
        UIManager.HideUiElement("BtnTier2");
        UIManager.HideUiElement("ScreenCoinsBtnVideo");
        UIManager.HideUiElement("ScreenCoinsBackground");
//        UIManager.HideUiElement("ScreenCoinsBtnNoAds");
//        UIManager.HideUiElement("ScreenCoinsBtnRestore");
        
        UIManager.ShowUiElement("ScreenMenuBtnPlus");
    }

    private void OnRewardedAvailable(OnRewardedAvailable e)
    {
        isShowBtnViveoAds = e.IsAvailable;
        if (isShowBtnViveoAds)
        {
            if (DefsGame.CurrentScreen == DefsGame.SCREEN_IAPS)
            {
                UIManager.ShowUiElement("ScreenCoinsBtnVideo");
            }
        }
        else
        {
            UIManager.HideUiElement("ScreenCoinsBtnVideo");
        }
    }

    public void BtnTier3()
    {
        GlobalEvents<OnRewardedTryShow>.Call(new OnRewardedTryShow());
        _isWaitReward = true;
    }

    private void GetReward(OnGiveReward e)
    {
        if (_isWaitReward)
        {
            _isWaitReward = false;
            if (e.IsAvailable)
                GameEvents.Send(OnAddCoinsVisual, 25);
        }
    }

    public void Show()
    {
        DefsGame.CurrentScreen = DefsGame.SCREEN_IAPS;
        DefsGame.IsCanPlay = false;
        ShowButtons();
    }

    public void Hide()
    {
        DefsGame.CurrentScreen = DefsGame.SCREEN_MENU;
        DefsGame.IsCanPlay = true;
        HideButtons();
    }
}