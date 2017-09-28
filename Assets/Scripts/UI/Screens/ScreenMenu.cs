using DoozyUI;
using UnityEngine;

public class ScreenMenu : MonoBehaviour
{
    private bool _isButtonHiden;
    private UIButton[] buttons;

    private void Start()
    {
        buttons = GetComponentsInChildren<UIButton>();
        ShowButtons();
    }

    private void OnEnable()
    {
        GlobalEvents<OnStartGame>.Happened += OnStartGame;
        GlobalEvents<OnShowMenu>.Happened += OnShowMenu;
        GlobalEvents<OnHideMenu>.Happened += OnHideMenu;
        GlobalEvents<OnShowMenuButtons>.Happened += OnShowMenuButtons;
        GlobalEvents<OnHideMenuButtons>.Happened += OnHideMenuButtons;
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
        //UIManager.HideUiElement ("MainMenu");
        UIManager.HideUiElement("LabelBestScore");
        UIManager.HideUiElement("BtnSkins");
        UIManager.HideUiElement("ScreenMainBtnPlay");
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
        if (DefsGame.GameBestScore > 0)
        {
            UIManager.ShowUiElement("LabelBestScore");
        }

        if (DefsGame.CoinsCount > 0)
        {
            UIManager.ShowUiElement("LabelCoins");
            UIManager.ShowUiElement("ScreenMenuBtnPlus");
        }
        else
        {
            UIManager.HideUiElement("LabelCoins");
            UIManager.HideUiElement("ScreenMenuBtnPlus");
        }
        
        UIManager.ShowUiElement("BtnSkins");
        UIManager.ShowUiElement("ScreenMainBtnPlay");
        UIManager.ShowUiElement("BtnLeaderboard");
        UIManager.ShowUiElement("BtnAchievements");
        UIManager.ShowUiElement("ScreenMainBtnSettings");
        
#if UNITY_ANDROID || UNITY_EDITOR
        UIManager.ShowUiElement("BtnGameServices");
#endif
    }

    public void ShowButtons()
    {
        GlobalEvents<OnShowMenuButtons>.Call(new OnShowMenuButtons());
        DefsGame.CurrentScreen = DefsGame.SCREEN_MENU;
    }

    public void HideButtons()
    {
        GlobalEvents<OnHideMenuButtons>.Call(new OnHideMenuButtons());
    }

    public void Show()
    {
        GlobalEvents<OnShowMenu>.Call(new OnShowMenu());
    }
    
    public void Hide()
    {
        GlobalEvents<OnHideMenu>.Call(new OnHideMenu());
    }

    // ENABLE / DISABLE BUTTONS ***************************************************************************************

    public void EnableButtons()
    {
        foreach (UIButton button in buttons)
        button.EnableButton();
    }

    public void DisableButtons()
    {
        foreach (UIButton button in buttons)
        button.DisableButton();
    }
}