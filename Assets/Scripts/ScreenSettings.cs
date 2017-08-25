﻿using DoozyUI;
using UnityEngine;

public class ScreenSettings : MonoBehaviour
{

    [HideInInspector] public string PrevScreenName;

    public void ShowButtons()
    {
        UIManager.ShowUiElement("ScreenSettingsBtnBack");
        UIManager.ShowUiElement("BtnSound");
        UIManager.ShowUiElement("BtnPlus");
        UIManager.ShowUiElement("BtnRate");
        UIManager.ShowUiElement("BtnShare");
  
        if (MyAds.NoAds < 1) UIManager.ShowUiElement("ScreenSettingsBtnNoAds");
#if UNITY_IPHONE
        UIManager.ShowUiElement("ScreenSettingsBtnRestore");
#endif
    }

    public void HideButtons()
    {
        UIManager.HideUiElement("ScreenSettingsBtnBack");
        UIManager.HideUiElement("ScreenSettingsBtnNoAds");
        UIManager.HideUiElement("ScreenSettingsBtnRestore");
        UIManager.HideUiElement("BtnSound");
        UIManager.HideUiElement("BtnPlus");
        UIManager.HideUiElement("BtnRate");
        UIManager.HideUiElement("BtnShare");
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