using UnityEngine;
using UnityEngine.UI;

public class Coins : MonoBehaviour
{
    private bool _isShowAnimation = true;
    private int _pointsCount;
    private AudioClip _sndCoin;
    private float _startScale;
    public Image img;

    [SerializeField] private Text textField;

    // Use this for initialization
    private void Start()
    {
        DefsGame.Coins = this;
        _sndCoin = Resources.Load<AudioClip>("snd/GUI/bonus");
        textField.text = DefsGame.CoinsCount.ToString();
        _pointsCount = DefsGame.CoinsCount;
        var color = textField.color;
        color.a = 0f;
        textField.color = color;
        img.color = new Color(img.color.r, img.color.g, img.color.b, color.a);
        _startScale = img.transform.localScale.x;
    }

    private void OnEnable()
    {
        CoinSensor.OnAddCoinsVisual += Coin_OnAddCoinsVisual;
        ScreenSkins.OnAddCoinsVisual += Coin_OnAddCoinsVisual;
        ScreenCoins.OnAddCoinsVisual += Coin_OnAddCoinsVisual;
        Coin.OnAddCoinsVisual += Coin_OnAddCoinsVisual;
        //BillingManager.OnAddCoinsVisual += Coin_OnAddCoinsVisual;
        //ScreenMenu.OnAddCoins += Bubble_OnAddCoins;
    }

    private void OnDisable()
    {
        CoinSensor.OnAddCoinsVisual -= Coin_OnAddCoinsVisual;
        ScreenSkins.OnAddCoinsVisual -= Coin_OnAddCoinsVisual;
        ScreenCoins.OnAddCoinsVisual -= Coin_OnAddCoinsVisual;
        Coin.OnAddCoinsVisual -= Coin_OnAddCoinsVisual;
        //BillingManager.OnAddCoinsVisual -= Coin_OnAddCoinsVisual;
        //ScreenMenu.OnAddCoins -= Bubble_OnAddCoins;
    }

    private void Coin_OnAddCoinsVisual(int value)
    {
        AddPointVisual(value);
        Defs.PlaySound(_sndCoin);
    }

    public void ResetCounter()
    {
        _pointsCount = 0;
        textField.text = "0";
    }

    // Update is called once per frame
    private void Update()
    {
        if (_isShowAnimation)
        {
            var color = textField.color;
            if (textField.color.a < 1f)
            {
                color.a += 0.1f;
            }
            else
            {
                _isShowAnimation = false;
                color.a = 1f;
            }
            textField.color = color;
            img.color = new Color(img.color.r, img.color.g, img.color.b, color.a);
        }

        if (img.transform.localScale.x > _startScale)
            img.transform.localScale = new Vector3(img.transform.localScale.x - 2.0f * Time.deltaTime,
                img.transform.localScale.y - 2.0f * Time.deltaTime, 1f);
    }

    public void AddPoint(int count)
    {
        _pointsCount += count;
        DefsGame.CoinsCount += count;
        PlayerPrefs.SetInt("coinsCount", DefsGame.CoinsCount);
        GlobalEvents<OnCoinsAdded>.Call(new OnCoinsAdded{Total = DefsGame.CoinsCount});
    }

    private void AddPointVisual(int value)
    {
        AddPoint(value);
        textField.text = _pointsCount.ToString();
        img.transform.localScale = new Vector3(_startScale * 1.4f, _startScale * 1.4f, 1f);
    }

    public void UpdateVisual()
    {
        textField.text = _pointsCount.ToString();
    }
}