using DoozyUI;
using UnityEngine;

public class ScreenMenu : MonoBehaviour
{
    [SerializeField] private AudioClip sndBtnClick;

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
        GlobalEvents<OnStartGame>.Happened += ScreenGame_OnHideMenu;
        GlobalEvents<OnShowMenu>.Happened += OnShowMenu;
        GlobalEvents<OnHideMenu>.Happened += OnHideMenu;
        GlobalEvents<OnRewardedAvailable>.Happened += IsRewardedVideoAvailable;
    }

    private void OnDisable()
    {
        GlobalEvents<OnShowMenu>.Happened -= OnShowMenu;
        GlobalEvents<OnHideMenu>.Happened -= OnHideMenu;
        GlobalEvents<OnStartGame>.Happened -= ScreenGame_OnHideMenu;
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
    
    private void ScreenGame_OnHideMenu(OnStartGame obj)
    {
        UIManager.HideUiElement("GameName");
        UIManager.ShowUiElement("LabelPoints");
        HideButtons();
    }

    private void IsRewardedVideoAvailable(OnRewardedAvailable e)
    {
        _isShowBtnViveoAds = e.IsAvailable;
        if (_isShowBtnViveoAds)
        {
            if (DefsGame.CurrentScreen == DefsGame.SCREEN_MENU)
            {
                UIManager.ShowUiElement("BtnVideoAds");
                FlurryEventsManager.SendEvent("RV_strawberries_impression", "start_screen");
            }
        }
        else
        {
            UIManager.HideUiElement("BtnVideoAds");
        }
    }

    public void ShowButtons()
    {
        _isButtonHiden = false;

        FlurryEventsManager.SendStartEvent("start_screen_length");

        //UIManager.ShowUiElement ("MainMenu");
        UIManager.ShowUiElement("LabelBestScore");
        UIManager.ShowUiElement("elementBestScore");
        UIManager.ShowUiElement("elementCoins");
        
        UIManager.ShowUiElement("BtnSkins");
        if (DefsGame.GameplayCounter != 0) 
        UIManager.ShowUiElement("ScreenMainBtnRepeate");
        UIManager.ShowUiElement("BtnLeaderboard");
        UIManager.ShowUiElement("BtnAchievements");
        UIManager.ShowUiElement("ScreenMainBtnSettings");
#if UNITY_ANDROID || UNITY_EDITOR
        UIManager.ShowUiElement("BtnGameServices");
#endif
        
        if (DefsGame.ScreenSkins)
            if (DefsGame.ScreenSkins.CheckAvailableSkinBool()) UIManager.ShowUiElement("BtnHaveNewSkin");
            else
                UIManager.HideUiElement("BtnHaveNewSkin");
        
        FlurryEventsManager.SendEvent("candy_shop_impression");
        if (_isShowBtnViveoAds)
        {
            UIManager.ShowUiElement("BtnVideoAds");
            FlurryEventsManager.SendEvent("RV_strawberries_impression", "start_screen");
        }
        FlurryEventsManager.SendEvent("rate_us_impression", "start_screen");
        FlurryEventsManager.SendEvent("iap_shop_impression");

        

        _isBtnSettingsClicked = false;
    }

    public void HideButtons()
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
        FlurryEventsManager.SendEvent("more_games");
    }

    public void OnVideoAdsClicked()
    {
        FlurryEventsManager.SendEvent("RV_strawberries", "start_screen");
        MyAds.ShowRewardedAds();
        _isWaitReward = true;
    }

    public void RateUs()
    {
//		FlurryEventsManager.SendEvent ("rate_us_impression", "start_screen");
//		Defs.Rate.RateUs ();
        Defs.PlaySound(sndBtnClick);
    }

    public void Share()
    {
        GlobalEvents<OnBtnShareClick>.Call(new OnBtnShareClick());
        Defs.PlaySound(sndBtnClick);
    }

    public void BtnPlusClick()
    {
        HideButtons();
        DefsGame.ScreenCoins.Show("start_screen");
        Defs.PlaySound(sndBtnClick);
    }

    public void BtnSkinsClick()
    {
        FlurryEventsManager.SendEvent("candy_shop");
        HideButtons();
        Defs.PlaySound(sndBtnClick);
    }
}