using DoozyUI;

public class ScreenSettings : ScreenItem
{

    private void Start()
    {
        InitUi();
    }

    public void Show()
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
        
        GlobalEvents<OnGameInputEnable>.Call(new OnGameInputEnable{Flag = false});
    }

    public override void Hide()
    {
        base.Hide();
        GlobalEvents<OnGameInputEnable>.Call(new OnGameInputEnable{Flag = true});
    }
}