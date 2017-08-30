using System;
using System.Collections.Generic;
using DoozyUI;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScreenSkins : MonoBehaviour
{
    [SerializeField] private GameObject[] _skinBtns;
    [SerializeField] private GameObject _choosedSkin;
    private bool _isNewSkinAvailable;
    public static event Action<int> OnAddCoinsVisual;

    private void Start()
    {
        AreThereSkins();
        CheckAvailableSkin();
    }

    private void OnEnable()
    {
        GlobalEvents<OnBuySkin>.Happened += OnBuySkin;
        GlobalEvents<OnCoinsAdded>.Happened += OnCoinsAdded;
        GlobalEvents<OnShowSkins>.Happened += OnShowSkins;
        GlobalEvents<OnHideSkins>.Happened += OnHideSkins;
    }
    
    private void OnShowSkins(OnShowSkins obj)
    {
        DefsGame.CurrentScreen = DefsGame.SCREEN_SKINS;
        DefsGame.IsCanPlay = false;
        
        ShowButtons();
        Invoke("ChooseColorForButtons", 1f);
    }
    
    private void OnHideSkins(OnHideSkins obj)
    {
        DefsGame.CurrentScreen = DefsGame.SCREEN_MENU;
        DefsGame.IsCanPlay = true;
        HideButtons();
    }

    private void OnCoinsAdded(OnCoinsAdded obj)
    {
        CheckAvailableSkin();
    }

    private void OnBuySkin(OnBuySkin obj)
    {
        GameEvents.Send(OnAddCoinsVisual, -200);
        OpenSkin(obj.Id);
        AreThereSkins();
        GlobalEvents<OnChangeSkin>.Call(new OnChangeSkin{Id = obj.Id});
    }

    private void SetSkin(int id)
    {
        if (id == DefsGame.CurrentFaceId)
            return;

        if (DefsGame.FaceAvailable[id] == 1)
        {
            DefsGame.CurrentFaceId = id;
            GlobalEvents<OnChangeSkin>.Call(new OnChangeSkin{Id = id});
            PlayerPrefs.SetInt("currentFaceID", DefsGame.CurrentFaceId);
            ChooseColorForButtons();
        }
        else if (DefsGame.CoinsCount >= 200/*DefsGame.FacePrice[_id - 1]*/)
        {
            GlobalEvents<OnBuySkin>.Call(new OnBuySkin{Id = id});

            //DefsGame.gameServices.ReportProgressWithGlobalID (DefsGame.gameServices.ACHIEVEMENT_NEW_SKIN, 1);
            //DefsGame.gameServices.ReportProgressWithGlobalID (DefsGame.gameServices.ACHIEVEMENT_COLLECTION, DefsGame.QUEST_CHARACTERS_Counter);
            GlobalEvents<OnGotNewCharacter>.Call(new OnGotNewCharacter());
        }
        else
        {
            GlobalEvents<OnShowScreenCoins>.Call(new OnShowScreenCoins());
        }
        
        Hide();
        GlobalEvents<OnShowMenu>.Call(new OnShowMenu());
    }

    private void OpenSkin(int id)
    {
        DefsGame.FaceAvailable[id] = 1;
        DefsGame.CurrentFaceId = id;
        PlayerPrefs.SetInt("currentFaceID", id);
        PlayerPrefs.SetInt("faceAvailable_" + id, 1);
        ++DefsGame.QUEST_CHARACTERS_Counter;
        PlayerPrefs.SetInt("QUEST_CHARACTERS_Counter", DefsGame.QUEST_CHARACTERS_Counter);
        ChooseColorForButtons();
    }

    private void ShowButtons()
    {
        UIManager.ShowUiElement("ScreenSkins");
        UIManager.ShowUiElement("ScreenSkinsTopBar");
//        UIManager.ShowUiElement("ScreenSkinsBackground");
        UIManager.ShowUiElement("ScreenSkinsBtnBack");
        UIManager.ShowUiElement("PreviewSkin");
        UIManager.ShowUiElement("ScreenSkinsContainerBackground");
        
        
        UIManager.ShowUiElement("BtnSkinRandom");
        UIManager.ShowUiElement("BtnSkin1");
        UIManager.ShowUiElement("BtnSkin2");
        UIManager.ShowUiElement("BtnSkin3");
        UIManager.ShowUiElement("BtnSkin4");
        UIManager.ShowUiElement("BtnSkin5");
        UIManager.ShowUiElement("BtnSkin6");
        UIManager.ShowUiElement("BtnSkin7");
        UIManager.ShowUiElement("BtnSkin8");
        UIManager.ShowUiElement("BtnSkin9");
        UIManager.ShowUiElement("BtnSkin10");
        UIManager.ShowUiElement("BtnSkin11");
        
        UIManager.ShowUiElement("ChoosedSkin");
       
        if (_isNewSkinAvailable)
        {
            UIManager.ShowUiElement("LabelNewSkin");
        }
        
        // Other screens
        UIManager.ShowUiElement("LabelCoins");
        UIManager.ShowUiElement("ScreenMenuBtnPlus");
    }

    private void HideButtons()
    {
        UIManager.HideUiElement("ScreenSkins");
        UIManager.HideUiElement("ScreenSkinsTopBar");
//        UIManager.ShowUiElement("ScreenSkinsBackground");
        UIManager.HideUiElement("ScreenSkinsBtnBack");
        UIManager.HideUiElement("PreviewSkin");
        UIManager.HideUiElement("ScreenSkinsContainerBackground");
        
        UIManager.HideUiElement("ChoosedSkin");
        UIManager.HideUiElement("BtnSkinRandom");
        UIManager.HideUiElement("BtnSkin1");
        UIManager.HideUiElement("BtnSkin2");
        UIManager.HideUiElement("BtnSkin3");
        UIManager.HideUiElement("BtnSkin4");
        UIManager.HideUiElement("BtnSkin5");
        UIManager.HideUiElement("BtnSkin6");
        UIManager.HideUiElement("BtnSkin7");
        UIManager.HideUiElement("BtnSkin8");
        UIManager.HideUiElement("BtnSkin9");
        UIManager.HideUiElement("BtnSkin10");
        UIManager.HideUiElement("BtnSkin11");
        UIManager.HideUiElement("LabelNewSkin");
        // Other screens
        UIManager.HideUiElement("LabelCoins");
        UIManager.HideUiElement("ScreenMenuBtnPlus");
    }

    public void SetSkin1()
    {
        SetSkin(0);
    }

    public void SetSkin2()
    {
        SetSkin(1);
    }

    public void SetSkin3()
    {
        SetSkin(2);
    }

    public void SetSkin4()
    {
        SetSkin(3);
    }

    public void SetSkin5()
    {
        SetSkin(4);
    }

    public void SetSkin6()
    {
        SetSkin(5);
    }

    public void SetSkin7()
    {
        SetSkin(6);
    }

    public void SetSkin8()
    {
        SetSkin(7);
    }

    public void SetSkin9()
    {
        SetSkin(8);
    }

    public void SetSkin10()
    {
        SetSkin(9);
    }

    public void SetSkin11()
    {
        SetSkin(10);
    }

    public void SetSkin12()
    {
        SetSkin(11);
    }

    public void Show()
    {
        GlobalEvents<OnShowSkins>.Call(new OnShowSkins());
    }

    public void Hide()
    {
        GlobalEvents<OnHideSkins>.Call(new OnHideSkins());
    }

    private void ChooseColorForButtons()
    {
        for (var i = 0; i < DefsGame.FaceAvailable.Length; i++)
            if (DefsGame.FaceAvailable[i] == 1)
            {
                _skinBtns[i].GetComponent<ScreenSkinBtn>().SetLock(true);
            }
            else
            {
                _skinBtns[i].GetComponent<ScreenSkinBtn>().SetLock(false);
            }
        
        _choosedSkin.transform.position = _skinBtns[DefsGame.CurrentFaceId].transform.position;
    }

    private void CheckAvailableSkin()
    {
        for (var i = 1; i < DefsGame.FaceAvailable.Length; i++)
            if (DefsGame.FaceAvailable[i] == 0 && DefsGame.CoinsCount >= 200)
            {
                _isNewSkinAvailable = true;
                UIManager.ShowUiElement("LabelNewSkin");
                return;
            }
        _isNewSkinAvailable = false;
        UIManager.HideUiElement("LabelNewSkin");
    }
    
    public void AreThereSkins()
    {
        for (var i = 1; i < DefsGame.FaceAvailable.Length; i++)
            if (DefsGame.FaceAvailable[i] == 0)
            {
                return;
            }
        GlobalEvents<OnSkinAllOpened>.Call(new OnSkinAllOpened());
    }

    public void SetRandomSkin()
    {
        List<int> availableList = new List<int>();
        for (int j = 0; j < DefsGame.FaceAvailable.Length; j++)
        {
            if (DefsGame.FaceAvailable[j] == 1)
            {
                availableList.Add(j);
            }
        }
        
        int id = Random.Range(0, availableList.Count);

        DefsGame.CurrentFaceId = availableList[id];
        _choosedSkin.transform.position = _skinBtns[DefsGame.CurrentFaceId].transform.position;
        availableList.Clear();
        
        Hide();
        GlobalEvents<OnShowMenu>.Call(new OnShowMenu());
    }
}