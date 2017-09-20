using System.Collections.Generic;
using DoozyUI;
using PrefsEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScreenSkins : MonoBehaviour
{
    [SerializeField] private GameObject[] _skinBtns;
    [SerializeField] private GameObject _choosedSkin;
    private bool _isNewSkinAvailable;
    private UIElement[] elements;

    private void Start()
    {
        elements = GetComponentsInChildren<UIElement>();
        AreThereSkins();
        AreThereSkinsGeneral();
//        CheckAvailableSkin();
    }

    private void OnEnable()
    {
        GlobalEvents<OnBuySkin>.Happened += OnBuySkin;
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
            Application.OpenURL("http://squaredino.com");
            OpenSkin(id);
            isAvailable = true;
        } 
        else if (DefsGame.CoinsCount >= 200/*DefsGame.FacePrice[_id - 1]*/)
        {
            BuySkin(id);
        }

        if (isAvailable)
        {
            Hide();
            GlobalEvents<OnShowMenu>.Call(new OnShowMenu());
            GlobalEvents<OnGameOverScreenShowActiveItems>.Call(new OnGameOverScreenShowActiveItems());
        } else
        {
            GlobalEvents<OnShowScreenCoins>.Call(new OnShowScreenCoins());
        }
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

    public void Hide()
    {
        foreach (UIElement element in elements)
            UIManager.HideUiElement(element.elementName);
        // Other screens
        UIManager.HideUiElement("LabelCoins");
        UIManager.HideUiElement("ScreenMenuBtnPlus");
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