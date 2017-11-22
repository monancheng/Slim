using System;
using PrefsEditor;
using UnityEngine;
using UnityEngine.UI;

public class GiftTimer : MonoBehaviour
{
    private int _hideDelayCounter;
    private readonly int[] _hideDelayArr = {5, 5, 5, 60, 60, 120, 120, 180};
    [SerializeField] private  Text _timeText;
    private DateTime _startDateTime;
    private bool _isWaitGiftTime;

    private const string TimerId = "GiftTimer";

    private void Start()
    {
        _hideDelayCounter = SecurePlayerPrefs.GetInt("BTN_GIFT_HIDE_DELAY_COUNTER", -1);
        if (_hideDelayCounter == -1)
        {
            TimerManager.StartTimer(TimerId);
            PrefsManager.SaveTimestamp(TimerId, UnbiasedTime.Instance.Now());
            _hideDelayCounter = 0;
            SecurePlayerPrefs.SetInt("BTN_GIFT_HIDE_DELAY_COUNTER", 0);
        }
        _startDateTime = PrefsManager.LoadTimestamp(TimerId);
        if (TimerManager.IsTimerExpired(TimerId, _hideDelayArr[_hideDelayCounter]))
        {
            _isWaitGiftTime = false;
            GlobalEvents<OnGiftAvailable>.Call(new OnGiftAvailable {IsAvailable = true});
            TimerManager.StopTimer(TimerId);
        }
        else
        {
            _isWaitGiftTime = true;
            _timeText.text = TimerManager.GetTimerRemainingString(TimerId, _hideDelayArr[_hideDelayCounter]);
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
            var currentDate = UnbiasedTime.Instance.Now();
            var difference = currentDate.Subtract(_startDateTime);

            if (difference.TotalSeconds < 0)
            {
                _timeText.text = "--:--";
                return;
            }

            TimeSpan remainingTimeSpan = TimeSpan.FromSeconds(_hideDelayArr[_hideDelayCounter]) - difference;
            if (remainingTimeSpan.TotalSeconds <= 0)
            {
                _isWaitGiftTime = false;
                GlobalEvents<OnGiftAvailable>.Call(new OnGiftAvailable {IsAvailable = true});
                TimerManager.StopTimer(TimerId);
                _timeText.text = "00:00";
                return;
            }    
            _timeText.text = String.Format("{0:d2}:{1:d2}",remainingTimeSpan.Minutes,remainingTimeSpan.Seconds);
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
        if (_hideDelayCounter < _hideDelayArr.Length - 1)
        {
            ++_hideDelayCounter;
            SecurePlayerPrefs.SetInt("BTN_GIFT_HIDE_DELAY_COUNTER", _hideDelayCounter);
        }
        _isWaitGiftTime = true;
        TimerManager.StartTimer(TimerId);
        _startDateTime = UnbiasedTime.Instance.Now();
        PrefsManager.SaveTimestamp(TimerId, _startDateTime);
        
        
        GlobalEvents<OnGiftAvailable>.Call(new OnGiftAvailable {IsAvailable = false});
    }
}