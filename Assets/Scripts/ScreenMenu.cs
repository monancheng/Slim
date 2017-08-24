using DoozyUI;
using UnityEngine;

public class ScreenMenu : MonoBehaviour
{
    private bool _isBtnSettingsClicked;
    private bool _isButtonHiden;
    private bool _isShowBtnViveoAds;
    private bool _isWaitReward;

    private void Start()
    {
        ShowButtons();
    }

    private void OnEnable()
    {
        GlobalEvents<OnStartGame>.Happened += OnStartGame;
        GlobalEvents<OnShowMenu>.Happened += OnShowMenu;
        GlobalEvents<OnHideMenu>.Happened += OnHideMenu;
        GlobalEvents<OnShowMenuButtons>.Happened += OnShowMenuButtons;
        GlobalEvents<OnHideMenuButtons>.Happened += OnHideMenuButtons;
        GlobalEvents<OnRewardedAvailable>.Happened += IsRewardedVideoAvailable;
    }

    private void OnDisable()
    {
        GlobalEvents<OnShowMenu>.Happened -= OnShowMenu;
        GlobalEvents<OnHideMenu>.Happened -= OnHideMenu;
        GlobalEvents<OnStartGame>.Happened -= OnStartGame;
        GlobalEvents<OnRewardedAvailable>.Happened -= IsRewardedVideoAvailable;
    }

    private void OnShowMenu(OnShowMenu obj)
    {
        UIManager.ShowUiElement("GameName");
        UIManager.HideUiElement("LabelPoints");
        ShowButtons();
    }
    
    private void OnHideMenu(OnHideMenu obj)
    {
        UIManager.HideUiElement("GameName");
        UIManager.ShowUiElement("LabelPoints");
        HideButtons();
    }
    
    private void OnStartGame(OnStartGame obj)
    {
        Hide();
    }
    
    private void OnHideMenuButtons(OnHideMenuButtons obj)
    {
        if (_isButtonHiden)
            return;

        _isButtonHiden = true;
        FlurryEventsManager.SendEndEvent("start_screen_length");
        //UIManager.HideUiElement ("MainMenu");
        UIManager.HideUiElement("LabelBestScore");
        UIManager.HideUiElement("elementBestScore");
        //UIManager.HideUiElement ("elementCoins");
        UIManager.HideUiElement("BtnSkins");
        UIManager.HideUiElement("ScreenMainBtnPlay");
        UIManager.HideUiElement("ScreenMainBtnRepeate");
        UIManager.HideUiElement("BtnAchievements");
        UIManager.HideUiElement("BtnLeaderboard");
        UIManager.HideUiElement("BtnGameServices");
        UIManager.HideUiElement("ScreenMainBtnSettings");
        UIManager.HideUiElement("BtnHaveNewSkin");
        UIManager.HideUiElement("ScreenMenuBtnPlus");
    }

    private void OnShowMenuButtons(OnShowMenuButtons obj)
    {
        _isButtonHiden = false;

        //UIManager.ShowUiElement ("MainMenu");
        UIManager.ShowUiElement("LabelBestScore");
        UIManager.ShowUiElement("elementBestScore");
        UIManager.ShowUiElement("LabelCoins");
        
        UIManager.ShowUiElement("BtnSkins");
//        if (DefsGame.GameplayCounter != 0) 
//        UIManager.ShowUiElement("ScreenMainBtnRepeate");
        UIManager.ShowUiElement("ScreenMainBtnPlay");
        UIManager.ShowUiElement("BtnLeaderboard");
        UIManager.ShowUiElement("BtnAchievements");
        UIManager.ShowUiElement("ScreenMainBtnSettings");
        UIManager.ShowUiElement("ScreenMenuBtnPlus");
#if UNITY_ANDROID || UNITY_EDITOR
        UIManager.ShowUiElement("BtnGameServices");
#endif
        
        if (_isShowBtnViveoAds)
        {
            UIManager.ShowUiElement("BtnVideoAds");
        }
        
        _isBtnSettingsClicked = false;
    }

    private void IsRewardedVideoAvailable(OnRewardedAvailable e)
    {
        _isShowBtnViveoAds = e.IsAvailable;
        if (_isShowBtnViveoAds)
        {
            if (DefsGame.CurrentScreen == DefsGame.SCREEN_MENU)
            {
                UIManager.ShowUiElement("BtnVideoAds");
            }
        }
        else
        {
            UIManager.HideUiElement("BtnVideoAds");
        }
    }

    public void ShowButtons()
    {
        GlobalEvents<OnShowMenuButtons>.Call(new OnShowMenuButtons());
    }

    public void HideButtons()
    {
        GlobalEvents<OnHideMenuButtons>.Call(new OnHideMenuButtons());
    }

    public void BtnSettingsClick()
    {
        _isBtnSettingsClicked = !_isBtnSettingsClicked;

        if (_isBtnSettingsClicked)
        {
            UIManager.ShowUiElement("BtnSound");
            UIManager.ShowUiElement("BtnInaps");
            UIManager.ShowUiElement("BtnGameServices");
        }
        else
        {
            UIManager.HideUiElement("BtnSound");
            UIManager.HideUiElement("BtnInaps");
            UIManager.HideUiElement("BtnGameServices");
        }
    }

    public void OnMoreAppsClicked()
    {
        //PublishingService.Instance.ShowAppShelf();
    }

    public void OnVideoAdsClicked()
    {
        MyAds.ShowRewardedAds();
        _isWaitReward = true;
    }

    public void RateUs()
    {
//		FlurryEventsManager.SendEvent ("rate_us_impression", "start_screen");
//		Defs.Rate.RateUs ();
    }

    public void Share()
    {
        GlobalEvents<OnBtnShareClick>.Call(new OnBtnShareClick());
    }

    public void BtnSkinsClick()
    {
        HideButtons();
    }

    public void Show()
    {
        GlobalEvents<OnShowMenu>.Call(new OnShowMenu());
    }
    
    public void Hide()
    {
        GlobalEvents<OnHideMenu>.Call(new OnHideMenu());
    }
}