using DoozyUI;
using UnityEngine;

public class ScreenSettings : ScreenItem
{

    private void Start()
    {
        InitUi();
    }

    public void Show()
    {
        
        DefsGame.CurrentScreen = DefsGame.SCREEN_SETTINGS;
        
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
}