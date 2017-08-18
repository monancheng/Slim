using System;
using DoozyUI;
using UnityEngine;
using UnityEngine.UI;

public class ScreenSkins : MonoBehaviour
{
    [SerializeField] private GameObject haveNewSkin;
    [SerializeField] private GameObject[] _skinBtns;
    private bool _isNewSkinAvailable;
    public static event Action<int> OnAddCoinsVisual;

    private void Awake()
    {
        DefsGame.ScreenSkins = this;
    }

    private void Start()
    {
        CheckClosedSkin();
        CheckAvailableSkin();
    }

    private void OnEnable()
    {
        GlobalEvents<OnBuySkin>.Happened += OnBuySkin;
        GlobalEvents<OnGiftSkin>.Happened += OnGiftSkin;
        GlobalEvents<OnCoinsAdded>.Happened += OnCoinsAdded;
    }
    
    private void OnCoinsAdded(OnCoinsAdded obj)
    {
        CheckAvailableSkin();
    }
    
    private void OnGiftSkin(OnGiftSkin obj)
    {
        OpenSkin(obj.Id);
        CheckClosedSkin();
        GlobalEvents<OnChangeSkin>.Call(new OnChangeSkin{Id = obj.Id});
    }

    private void OnBuySkin(OnBuySkin obj)
    {
        GameEvents.Send(OnAddCoinsVisual, -200);
        OpenSkin(obj.Id);
        CheckClosedSkin();
        GlobalEvents<OnChangeSkin>.Call(new OnChangeSkin{Id = obj.Id});
    }

    private void SetSkin(int id)
    {
        FlurryEventsManager.SendEvent("candy_purchase_<" + id + ">");

        if (id == DefsGame.CurrentFaceId)
            return;

        if (DefsGame.FaceAvailable[id] == 1)
        {
            DefsGame.CurrentFaceId = id;
            GlobalEvents<OnChangeSkin>.Call(new OnChangeSkin{Id = id});
            PlayerPrefs.SetInt("currentFaceID", DefsGame.CurrentFaceId);
//            DefsGame.CarSimulator.Car.SetNewSkin(_id);
        }
        else if (DefsGame.CoinsCount >= 200/*DefsGame.FacePrice[_id - 1]*/)
        {
            GlobalEvents<OnBuySkin>.Call(new OnBuySkin{Id = id});

            //DefsGame.gameServices.ReportProgressWithGlobalID (DefsGame.gameServices.ACHIEVEMENT_NEW_SKIN, 1);

            //DefsGame.gameServices.ReportProgressWithGlobalID (DefsGame.gameServices.ACHIEVEMENT_COLLECTION, DefsGame.QUEST_CHARACTERS_Counter);
            ChooseColorForButtons();
            GlobalEvents<OnGotNewCharacter>.Call(new OnGotNewCharacter());

            FlurryEventsManager.SendEvent("candy_purchase_completed_<" + id + ">");
        }
        else
        {
            HideButtons();
            FlurryEventsManager.SendEndEvent("candy_shop_length");

            DefsGame.ScreenCoins.Show("candy_shop");
        }
    }

    private void OpenSkin(int id)
    {
        DefsGame.FaceAvailable[id] = 1;
        DefsGame.CurrentFaceId = id;
        PlayerPrefs.SetInt("currentFaceID", DefsGame.CurrentFaceId);
        PlayerPrefs.SetInt("faceAvailable_" + id, 1);
//      DefsGame.CarSimulator.Car.SetNewSkin(_id);

        ++DefsGame.QUEST_CHARACTERS_Counter;
        PlayerPrefs.SetInt("QUEST_CHARACTERS_Counter", DefsGame.QUEST_CHARACTERS_Counter);
    }

    private void ShowButtons()
    {
        UIManager.ShowUiElement("ScreenSkins");
        UIManager.ShowUiElement("ScreenSkinsBtnBack");
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
        UIManager.ShowUiElement("BtnSkin12");
        if (_isNewSkinAvailable)
        {
            UIManager.ShowUiElement("LabelNewSkin");
        }
    }

    private void HideButtons()
    {
        UIManager.HideUiElement("ScreenSkins");
        UIManager.HideUiElement("ScreenSkinsBtnBack");
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
        UIManager.HideUiElement("BtnSkin12");
        UIManager.HideUiElement("LabelNewSkin");
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
        DefsGame.CurrentScreen = DefsGame.SCREEN_SKINS;
        DefsGame.IsCanPlay = false;
        ChooseColorForButtons();
        ShowButtons();
    }

    public void Hide()
    {
        DefsGame.CurrentScreen = DefsGame.SCREEN_MENU;
        DefsGame.IsCanPlay = true;
        ChooseColorForButtons();
        HideButtons();
    }

    private void ChooseColorForButtons()
    {
        for (var i = 1; i < DefsGame.FaceAvailable.Length; i++)
            if (DefsGame.FaceAvailable[i] == 1)
            {
                _skinBtns[i-1].GetComponentInChildren<UIButton>().GetComponent<Image>().color = Color.white;
                _skinBtns[i-1].GetComponentInChildren<Text>().text = "";
            }
            else
            {
                _skinBtns[i-1].GetComponentInChildren<UIButton>().GetComponent<Image>().color = Color.black;
                _skinBtns[i-1].GetComponentInChildren<Text>().text = "200";
            }
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
    
    public void CheckClosedSkin()
    {
        for (var i = 1; i < DefsGame.FaceAvailable.Length; i++)
            if (DefsGame.FaceAvailable[i] == 0)
            {
                return;
            }
        GlobalEvents<OnSkinAllOpened>.Call(new OnSkinAllOpened());
    }
}