using System;
using UnityEngine;
using UnityEngine.UI;

public class GiftTimer : MonoBehaviour
{
    [SerializeField] private  Text timeText;
    private DateTime _giftNextDate;
    private bool _isWaitGiftTime;

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
            timeText.text = _difference.Hours + ":" + _difference.Minutes;
        }
    }

    private void OnEnable()
    {
        GlobalEvents<OnGameOver>.Happened += OnGameOver;
        GlobalEvents<OnGiftResetTimer>.Happened += OnGiftResetTimer;
    }

    private void OnDisable()
    {
        GlobalEvents<OnGameOver>.Happened -= OnGameOver;
        GlobalEvents<OnGiftResetTimer>.Happened -= OnGiftResetTimer;
    }

    private void OnGameOver(OnGameOver e)
    {
        UpdateTmer();
    }

    private void UpdateTmer()
    {
        if (_isWaitGiftTime)
        {
            var _currentDate = DateTime.UtcNow;
            var _difference = _giftNextDate.Subtract(_currentDate);
            if (_difference.TotalSeconds <= 0f)
            {
                _isWaitGiftTime = false;
                GlobalEvents<OnGiftAvailable>.Call(new OnGiftAvailable {IsAvailable = true});
            }
            else {
                string _minutes = _difference.Minutes.ToString ();
                if (_difference.Minutes < 10) {
                    _minutes = "0" + _minutes;
                }
                string _seconds = _difference.Seconds.ToString ();
                if (_difference.Seconds < 10) {
                    _seconds = "0" + _seconds;
                }
                timeText.text = _minutes + ":" + _seconds;
            }
        }
    }

    private void Update()
    {
        UpdateTmer();
    }

    private void OnGiftResetTimer(OnGiftResetTimer obj)
    {
        if (obj.IsResetTimer)
        {
            ResetTimer();
        }
    }

    private void ResetTimer()
    {
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
        _isWaitGiftTime = true;

        GlobalEvents<OnGiftAvailable>.Call(new OnGiftAvailable {IsAvailable = false});
    }
}