using DoozyUI;
using UnityEngine;

public class ScreenSettings : MonoBehaviour
{
    private UIElement[] elements;

    private void Start()
    {
        elements = GetComponentsInChildren<UIElement>();
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

    public void Hide()
    {
        foreach (UIElement element in elements)
            UIManager.HideUiElement(element.elementName);
    }
}