using System;
using PrefsEditor;
using UnityEngine;
using UnityEngine.UI;

public class WordTimer : MonoBehaviour {
	[SerializeField] private  Text timeText;
	private DateTime _nextDate;
	private bool _isWaitTime;
	
	// Use this for initialization
	void Start () {
		//Grab the old time from the player prefs as a long
//		SecurePlayerPrefs.SetInt("isNeedToWaitWord", 1);
//		SecurePlayerPrefs.SetString("WaitNewWordDate", "");
		int isNeedToWait = SecurePlayerPrefs.GetInt("isNeedToWaitWord");
		if (isNeedToWait == 1)
		{
			string strTime = SecurePlayerPrefs.GetString("WaitNewWordDate");

			if (strTime == "")
			{
				_nextDate = DateTime.UtcNow;
			}
			else
			{
				var _timeOld = Convert.ToInt64(strTime);
				//Convert the old time from binary to a DataTime variable
				_nextDate = DateTime.FromBinary(_timeOld);
			}

			var _currentDate = DateTime.UtcNow;
			var _difference = _nextDate.Subtract(_currentDate);
			if (_difference.TotalSeconds <= 0f)
			{
				_isWaitTime = false;
			}
			else
			{
				_isWaitTime = true;
				timeText.text = _difference.Hours + ":" + _difference.Minutes;
			}
		}
		GlobalEvents<OnWordNeedToWait>.Call(new OnWordNeedToWait{IsWait = _isWaitTime});
	}
	
	private void OnEnable()
	{
		GlobalEvents<OnWordStartTimer>.Happened += StartTimer;
	}

	private void OnDisable()
	{
		GlobalEvents<OnWordStartTimer>.Happened -= StartTimer;
	}
	
	private void StartTimer(OnWordStartTimer obj)
	{
		StartTimer();
	}

	private void UpdateTimer()
	{
		if (_isWaitTime)
		{
			var _currentDate = DateTime.UtcNow;
			var _difference = _nextDate.Subtract(_currentDate);
			if (_difference.TotalSeconds <= 0f)
			{
				_isWaitTime = false;
				
				SecurePlayerPrefs.SetInt("isNeedToWaitWord", 0);
				SecurePlayerPrefs.SetString("WaitNewWordDate", "");
		
				GlobalEvents<OnWordNeedToWait>.Call(new OnWordNeedToWait{IsWait = _isWaitTime});
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
		UpdateTimer();
	}
	
	private void StartTimer()
	{
		_isWaitTime = true;
		_nextDate = DateTime.UtcNow;
		_nextDate = _nextDate.AddMinutes(1);
		
		SecurePlayerPrefs.SetInt("isNeedToWaitWord", 1);
		SecurePlayerPrefs.SetString("WaitNewWordDate", _nextDate.ToBinary().ToString());
		
		GlobalEvents<OnWordNeedToWait>.Call(new OnWordNeedToWait{IsWait = _isWaitTime});
	}
}
