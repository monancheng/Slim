using System;
using UnityEngine;

public class MyAds : MonoBehaviour
{
    public static int NoAds;
    private static int _rewardedAdCounter;
    private bool _isRewardedVideoReadyToShow;
    private DateTime _rewardDate;
    private bool _isRewardedWaitTimer;
    private static bool _isRewardedAdCalcNext;

    private static int _videoAdCounter;
    private static bool _isVideoAdCalcNext;
    private bool _isVideoReadyToShow;
    private DateTime _videoDate;
    private bool _isVideoWaitTimer;
    private bool _isFirstTimeVideo;

    private void Start()
    {
        NoAds = PlayerPrefs.GetInt ("noAds", 0);
        _rewardDate = DateTime.UtcNow;
        _isRewardedWaitTimer = true;
        _isRewardedAdCalcNext = true;

        _videoDate = DateTime.UtcNow;
        _isVideoWaitTimer = true;
        _isVideoAdCalcNext = true;
        _isFirstTimeVideo = true;
    }

    void OnEnable()
    {
        GlobalEvents<OnRewardedTryShow>.Happened += OnRewardedTryShow;
        GlobalEvents<OnAdsVideoTryShow>.Happened += OnAdsVideoTryShow;
        GlobalEvents<OnRewardedAvailable>.Happened += OnRewardedAvailable;
        GlobalEvents<OnAdsVideoShowing>.Happened += OnAdsVideoShowing;
    }

    void OnDisable()
    {
        GlobalEvents<OnRewardedTryShow>.Happened -= OnRewardedTryShow;
        GlobalEvents<OnAdsVideoTryShow>.Happened -= OnAdsVideoTryShow;
        GlobalEvents<OnRewardedAvailable>.Happened -= OnRewardedAvailable;
        GlobalEvents<OnAdsVideoShowing>.Happened -= OnAdsVideoShowing;
    }

    private void OnRewardedTryShow(OnRewardedTryShow obj)
    {
        if (_rewardedAdCounter >= 4 )
        {
            if (_isRewardedVideoReadyToShow) GlobalEvents<OnShowRewarded>.Call(new OnShowRewarded());
            _isRewardedAdCalcNext = false;
        }
        else
        {
            if (_isRewardedAdCalcNext) ++_rewardedAdCounter;
        }
    }

    private void OnAdsVideoTryShow(OnAdsVideoTryShow obj)
    {
        if (_isFirstTimeVideo && _videoAdCounter == 3 ||
            _videoAdCounter >= 5)
        {
            if (_isFirstTimeVideo) _isFirstTimeVideo = false;

            if (_isVideoReadyToShow) GlobalEvents<OnShowVideoAds>.Call(new OnShowVideoAds());
            _isVideoAdCalcNext = false;
        }
        else
        {
            if (_isVideoAdCalcNext) ++_videoAdCounter;
        }
    }

    private void OnAdsVideoShowing(OnAdsVideoShowing obj)
    {
        // продолжаем считать геймлпеи, после которых можно показыавть Video рекламу
        _isVideoAdCalcNext = true;
        _videoAdCounter = 0;
    }

    private void OnRewardedAvailable(OnRewardedAvailable e)
    {
        if (e.IsAvailable)
        {
        }
        else
        {
            _rewardDate = DateTime.UtcNow;
            _rewardDate = _rewardDate.AddMinutes(2);
            _isRewardedVideoReadyToShow = false;
            _isRewardedWaitTimer = true;
            _rewardedAdCounter = 0;

            //Обнуляем Video таймер и коунтер
            _videoAdCounter = 0;
            _videoDate = DateTime.UtcNow;
            _videoDate = _videoDate.AddMinutes(2);
        }
        // продолжаем считать геймлпеи, после которых можно показыавть Rewarded рекламу
        _isRewardedAdCalcNext = true;
    }

    private void Update()
    {
        UpdateVideoTimerEnd();
    }

    private void UpdateVideoTimerEnd()
    {
        if (_isRewardedWaitTimer)
        {
            TimeSpan difference = _rewardDate.Subtract(DateTime.UtcNow);
            if (difference.TotalSeconds <= 0f)
            {
                _isRewardedWaitTimer = false;
            }
        }
        
        if (_isVideoWaitTimer)
        {
            TimeSpan difference = _videoDate.Subtract(DateTime.UtcNow);
            if (difference.TotalSeconds <= 0f)
            {
                _isVideoWaitTimer = false;
            }
        }

        if (!_isRewardedWaitTimer)
        {
            if (!_isRewardedVideoReadyToShow)
            {
                _isRewardedVideoReadyToShow = true;
                GlobalEvents<OnRewardedAvailable>.Call(
                    new OnRewardedAvailable {IsAvailable = true});
            }
        }
        
        if (!_isVideoWaitTimer)
        {
            if (!_isVideoReadyToShow)
                _isVideoReadyToShow = true;
        }
    }
}