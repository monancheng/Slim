using System.Collections.Generic;
using DarkTonic.MasterAudio;
using DoozyUI;
using PrefsEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScreenSkins : ScreenItem
{
    [SerializeField] private GameObject[] _skinBtns;
    [SerializeField] private GameObject _choosedSkin;
    private bool _isNewSkinAvailable;
    
    private void Start()
    {
        InitUi();
        AreThereSkins();
        AreThereSkinsGeneral();
//        CheckAvailableSkin();
        
        if (DefsGame.GameBestScore > 0) UIManager.ShowUiElement("LabelBestScore");
        if (DefsGame.CoinsCount > 0)
        {
            UIManager.ShowUiElement("LabelCoins");
            UIManager.ShowUiElement("ScreenMenuBtnPlus");
        }
    }

    private void OnEnable()
    {
        GlobalEvents<OnBuySkin>.Happened += OnBuySkin;
        GlobalEvents<OnBuySkinByIAP>.Happened += OnBuySkinByIAP;
//        GlobalEvents<OnCoinsAdded>.Happened += OnCoinsAdded;
    }

//    private void OnCoinsAdded(OnCoinsAdded obj)
//    {
//        CheckAvailableSkin();
//    }

    private void OnBuySkin(OnBuySkin obj)
    {
        GlobalEvents<OnCoinsAdd>.Call(new OnCoinsAdd {Count = -200});
        OpenSkin(obj.Id);
    }
    
    private void OnBuySkinByIAP(OnBuySkinByIAP obj)
    {
        switch (obj.Id)
        {
            case BillingManager.IAP_SKIN_1: OpenSkin(DefsGame.IAP_SKIN_1);
                break;
            case BillingManager.IAP_SKIN_2: OpenSkin(DefsGame.IAP_SKIN_2);
                break;
            case BillingManager.IAP_SKIN_3: OpenSkin(DefsGame.IAP_SKIN_3);
                break;
            case BillingManager.IAP_SKIN_4: OpenSkin(DefsGame.IAP_SKIN_4);
                break;
        }
        MasterAudio.PlaySoundAndForget("GUI_Grab");
        Hide();
        GlobalEvents<OnShowMenu>.Call(new OnShowMenu());
        GlobalEvents<OnGameOverScreenShowActiveItems>.Call(new OnGameOverScreenShowActiveItems());
    }

    public void SetSkin(int id)
    {
//        if (id == DefsGame.CurrentFaceId)
//        {
//            Hide();
//            GlobalEvents<OnShowMenu>.Call(new OnShowMenu());
//            GlobalEvents<OnGameOverScreenShowActiveItems>.Call(new OnGameOverScreenShowActiveItems());
//            return;
//        }

        bool isAvailable = false;
        if (DefsGame.FaceAvailable[id] == 1)
        {
            DefsGame.CurrentFaceId = id;
            GlobalEvents<OnChangeSkin>.Call(new OnChangeSkin{Id = id});
            SecurePlayerPrefs.SetInt("currentFaceID", DefsGame.CurrentFaceId);
            ChooseColorForButtons();
            isAvailable = true;
        } else
        // Social
        if (id == DefsGame.SKIN_FACEBOOK)
        {
            Application.OpenURL("http://facebook.com");
            OpenSkin(id);
            isAvailable = true;
        } else 
        if (id == DefsGame.SKIN_TWITTER)
        {
            Application.OpenURL("http://twitter.com");
            OpenSkin(id);
            isAvailable = true;
        } else 
        if (id == DefsGame.SKIN_INSTAGRAM)
        {
            Application.OpenURL("http://instagram.com");
            OpenSkin(id);
            isAvailable = true;
        } else 
        if (id == DefsGame.SKIN_SPONSOR)
        {
            Application.OpenURL("http://www.ketchappstudio.com");
            OpenSkin(id);
            isAvailable = true;
        } 
        else 
            if (DefsGame.CoinsCount >= 200 /*DefsGame.FacePrice[_id - 1]*/)
            {
                BuySkin(id);
                isAvailable = true;
            }

        if (isAvailable)
        {
            MasterAudio.PlaySoundAndForget("GUI_Grab");
            Hide();
            GlobalEvents<OnShowMenu>.Call(new OnShowMenu());
            GlobalEvents<OnGameOverScreenShowActiveItems>.Call(new OnGameOverScreenShowActiveItems());
        } else
        {
            GlobalEvents<OnShowScreenCoins>.Call(new OnShowScreenCoins());
        }
    }

    public void BuyPayableSkin(int id)
    {
        if (id == 1) DefsGame.IAPs.BuySkin(BillingManager.IAP_SKIN_1); else
        if (id == 2) DefsGame.IAPs.BuySkin(BillingManager.IAP_SKIN_2); else
        if (id == 3) DefsGame.IAPs.BuySkin(BillingManager.IAP_SKIN_3); else
        if (id == 4) DefsGame.IAPs.BuySkin(BillingManager.IAP_SKIN_4); 
    }

    private void BuySkin(int id)
    {
        GlobalEvents<OnBuySkin>.Call(new OnBuySkin{Id = id});

        //DefsGame.gameServices.ReportProgressWithGlobalID (DefsGame.gameServices.ACHIEVEMENT_NEW_SKIN, 1);
        //DefsGame.gameServices.ReportProgressWithGlobalID (DefsGame.gameServices.ACHIEVEMENT_COLLECTION, DefsGame.QUEST_CHARACTERS_Counter);
        GlobalEvents<OnGotNewCharacter>.Call(new OnGotNewCharacter());
    }

    private void OpenSkin(int id)
    {
        DefsGame.FaceAvailable[id] = 1;
        DefsGame.CurrentFaceId = id;
        SecurePlayerPrefs.SetInt("currentFaceID", id);
        SecurePlayerPrefs.SetInt("faceAvailable_" + id, 1);
        ++DefsGame.QUEST_CHARACTERS_Counter;
        SecurePlayerPrefs.SetInt("QUEST_CHARACTERS_Counter", DefsGame.QUEST_CHARACTERS_Counter);
        ChooseColorForButtons();
        AreThereSkins();
        AreThereSkinsGeneral();
        GlobalEvents<OnChangeSkin>.Call(new OnChangeSkin{Id = id});
    }
    
    public void Show()
    {
        DefsGame.CurrentScreen = DefsGame.SCREEN_SKINS;
        
        GlobalEvents<OnHideTubes>.Call(new OnHideTubes());
        
        foreach (UIElement element in elements)
            UIManager.ShowUiElement(element.elementName);
        
        // Other screens
        UIManager.ShowUiElement("LabelCoins");
        UIManager.ShowUiElement("ScreenMenuBtnPlus");
        
        Invoke("ChooseColorForButtons", 1f);
    }
    
    private void ChooseColorForButtons()
    {
        for (var i = 0; i < DefsGame.FaceAvailable.Length; i++)
            if (DefsGame.FaceAvailable[i] == 1)
            {
                _skinBtns[i].GetComponent<ScreenSkinsBtn>().SetLock(true);
            }
            else
            {
                _skinBtns[i].GetComponent<ScreenSkinsBtn>().SetLock(false);
            }
        
        _choosedSkin.transform.position = _skinBtns[DefsGame.CurrentFaceId].transform.position;
    }

//    private void CheckAvailableSkin()
//    {
//        for (var i = DefsGame.FacesGeneralMin; i < DefsGame.FacesGeneralMax; i++)
//            if (DefsGame.FaceAvailable[i] == 0 && DefsGame.CoinsCount >= 200)
//            {
//                _isNewSkinAvailable = true;
//                return;
//            }
//        _isNewSkinAvailable = false;
//    }
    
    private void AreThereSkins()
    {
        for (var i = 1; i < DefsGame.FaceAvailable.Length; i++)
            if (DefsGame.FaceAvailable[i] == 0)
            {
                return;
            }
        GlobalEvents<OnSkinAllOpened>.Call(new OnSkinAllOpened());
    }
    
    private void AreThereSkinsGeneral()
    {
        for (var i = DefsGame.FacesGeneralMin; i < DefsGame.FacesGeneralMax; i++)
            if (DefsGame.FaceAvailable[i] == 0)
            {
                return;
            }
        GlobalEvents<OnSkinAllGeneralOpened>.Call(new OnSkinAllGeneralOpened());
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
        SecurePlayerPrefs.SetInt("currentFaceID", DefsGame.CurrentFaceId);
        _choosedSkin.transform.position = _skinBtns[DefsGame.CurrentFaceId].transform.position;
        availableList.Clear();
        
        Hide();
        GlobalEvents<OnShowMenu>.Call(new OnShowMenu());
    }
}