using System;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public static void StartTimer(String id)
    {
        PrefsManager.SaveTimerEnabled(id, true);
        PrefsManager.SaveTimestamp(id, UnbiasedTime.Instance.Now());
    }

    public static void StopTimer(String id)
    {
        PrefsManager.SaveTimerEnabled(id, false);
    }

    public static bool IsTimerPlaying(String id)
    {
        return PrefsManager.LoadTimerEnabled(id);
    }

    public static bool IsTimerExpired(String id, float timerSeconds)
    {
        if (IsTimerPlaying(id))
        {
            TimeSpan timeSpan = UnbiasedTime.Instance.Now() - PrefsManager.LoadTimestamp(id);
            return timeSpan.TotalSeconds >= timerSeconds;
        }
       
        return false;
    }

    public static string GetTimerRemainingString(String id, float timerSeconds)
    {
        if (IsTimerPlaying(id))
        {
            TimeSpan timeSpan = UnbiasedTime.Instance.Now() - PrefsManager.LoadTimestamp(id);
            if (timeSpan.TotalSeconds < 0) return "--:--";
            
            TimeSpan remainingTimeSpan = TimeSpan.FromSeconds(timerSeconds) - timeSpan;
            if (remainingTimeSpan.TotalSeconds <= 0)
            {
                return "00:00";
            }

            if (remainingTimeSpan.TotalHours < 1f)
            {
                return string.Format("{0:D2}:{1:D2}", remainingTimeSpan.Minutes, remainingTimeSpan.Seconds);
            }

            return "--:--"; 
        }

        return "--:--";
    }
}
