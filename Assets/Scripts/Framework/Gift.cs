using System;
using UnityEngine;

public class Gift : MonoBehaviour
{
    private DateTime _giftNextDate;

    private bool _isWaitGiftTime;

    //public Text timeText;
    public GameObject coin;

    private void Start()
    {
        //Grab the old time from the player prefs as a long
        string strTime = PlayerPrefs.GetString("dateGiftClicked");

        DefsGame.BTN_GIFT_HIDE_DELAY_COUNTER = 0;
        if (strTime == "")
        {
            _giftNextDate = DateTime.UtcNow;
            DefsGame.BTN_GIFT_HIDE_DELAY = DefsGame.BTN_GIFT_HIDE_DELAY_ARR[DefsGame.BTN_GIFT_HIDE_DELAY_COUNTER];
            _giftNextDate = _giftNextDate.AddMinutes(DefsGame.BTN_GIFT_HIDE_DELAY);
            if (DefsGame.BTN_GIFT_HIDE_DELAY_COUNTER < DefsGame.BTN_GIFT_HIDE_DELAY_ARR.Length - 1)
            {
                ++DefsGame.BTN_GIFT_HIDE_DELAY_COUNTER;
                PlayerPrefs.SetInt("BTN_GIFT_HIDE_DELAY_COUNTER", DefsGame.BTN_GIFT_HIDE_DELAY_COUNTER);
            }
        }
        else
        {
            var _timeOld = Convert.ToInt64(strTime);
            //Convert the old time from binary to a DataTime variable
            _giftNextDate = DateTime.FromBinary(_timeOld);
        }

        var _currentDate = DateTime.UtcNow;
        var _difference = _giftNextDate.Subtract(_currentDate);
        if (_difference.TotalSeconds <= 0f)
        {
            //timeText.enabled = false;
            _isWaitGiftTime = false;
            GlobalEvents<OnGiftAvailable>.Call(new OnGiftAvailable {IsAvailable = true});
        }
        else
        {
            //timeText.enabled = true;
            _isWaitGiftTime = true;
            //timeText.text = _difference.Hours.ToString () + ":" + _difference.Minutes.ToString ();
        }
    }

    private void OnEnable()
    {
        GlobalEvents<OnGameOver>.Happened += IsGiftAvailable;
    }

    private void OnDisable()
    {
        GlobalEvents<OnGameOver>.Happened -= IsGiftAvailable;
    }

    private void IsGiftAvailable(OnGameOver e)
    {
        if (_isWaitGiftTime)
        {
            var _currentDate = DateTime.UtcNow;
            var _difference = _giftNextDate.Subtract(_currentDate);
            if (_difference.TotalSeconds <= 0f)
            {
                _isWaitGiftTime = false;
                GlobalEvents<OnGiftAvailable>.Call(new OnGiftAvailable {IsAvailable = true});
                FlurryEventsManager.SendEvent("collect_prize_impression");
            }
//			else {
//				string _minutes = _difference.Minutes.ToString ();
//				if (_difference.Minutes < 10) {
//					_minutes = "0" + _minutes;
//				}
//				string _seconds = _difference.Seconds.ToString ();
//				if (_difference.Seconds < 10) {
//					_seconds = "0" + _seconds;
//				}
//				timeText.text = _minutes + ":" + _seconds;
//			}
        }
    }

    public void add10Coins()
    {
        FlurryEventsManager.SendEvent("collect_prize");

        for (var i = 0; i < 10; i++)
        {
            var _coin = Instantiate(coin, Camera.main.ScreenToWorldPoint(Vector3.zero), Quaternion.identity);
            var coinScript = _coin.GetComponent<Coin>();
            coinScript.MoveToEnd();
        }
        //Save the current system time as a string in the player prefs class
        _giftNextDate = DateTime.UtcNow;
        DefsGame.BTN_GIFT_HIDE_DELAY = DefsGame.BTN_GIFT_HIDE_DELAY_ARR[DefsGame.BTN_GIFT_HIDE_DELAY_COUNTER];
        if (DefsGame.BTN_GIFT_HIDE_DELAY_COUNTER < DefsGame.BTN_GIFT_HIDE_DELAY_ARR.Length - 1)
        {
            ++DefsGame.BTN_GIFT_HIDE_DELAY_COUNTER;
            PlayerPrefs.SetInt("BTN_GIFT_HIDE_DELAY_COUNTER", DefsGame.BTN_GIFT_HIDE_DELAY_COUNTER);
        }
        _giftNextDate = _giftNextDate.AddMinutes(DefsGame.BTN_GIFT_HIDE_DELAY);
        PlayerPrefs.SetString("dateGiftClicked", _giftNextDate.ToBinary().ToString());
        //timeText.enabled = true;
        //giftButton.enabled = false;
        _isWaitGiftTime = true;
        //giftButton.DisableButtonClicks();

        GlobalEvents<OnGiftAvailable>.Call(new OnGiftAvailable {IsAvailable = false});

        D.Log("Disable Button Clicks");
    }
}