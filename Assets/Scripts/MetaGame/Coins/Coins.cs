using DarkTonic.MasterAudio;
using DoozyUI;
using PrefsEditor;
using UnityEngine;
using UnityEngine.UI;

public class Coins : MonoBehaviour
{
    [SerializeField] private Image img;
    [SerializeField] private Text textField;
    private float _startScale;

    private void Start()
    {
        textField.text = PrefsManager.CoinsCount.ToString();
        _startScale = img.transform.localScale.x;
    }

    private void OnEnable()
    {
        GlobalEvents<OnCoinsAdd>.Happened += OnCoinsAdd;
    }

    private void OnCoinsAdd(OnCoinsAdd obj)
    {
        MasterAudio.PlaySoundAndForget("GUI_CoinTake");
        PrefsManager.CoinsCount += new SecureInt(obj.Count);
        SecurePlayerPrefs.SetInt("coinsCount", PrefsManager.CoinsCount.GetValue());
        GlobalEvents<OnCoinsAdded>.Call(new OnCoinsAdded{Total = PrefsManager.CoinsCount.GetValue()});
        textField.text = PrefsManager.CoinsCount.ToString();
        img.transform.localScale = new Vector3(_startScale * 1.4f, _startScale * 1.4f, 1f);
        UIManager.ShowUiElement("LabelCoins");
    }

    // Update is called once per frame
    private void Update()
    {
        if (img.transform.localScale.x > _startScale)
            img.transform.localScale = new Vector3(img.transform.localScale.x - 2.0f * Time.deltaTime,
                img.transform.localScale.y - 2.0f * Time.deltaTime, 1f);
    }
}