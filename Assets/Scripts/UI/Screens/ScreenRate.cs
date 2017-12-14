using System;
using System.Collections.Generic;
using DoozyUI;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class ScreenRate : ScreenItem
{
	[SerializeField] private ScreenRateBtnStar[] starButtons;
	[SerializeField] private Image smile;
	[SerializeField] private Sprite[] smileSprites;
	private int _voteValue;
	
	private void Start()
	{
		InitUi();
		
		ScreenRateBtnStar.OnRateBtnClick += OnRateBtnClick;
		GlobalEvents<OnRateScreenShow>.Happened += OnRateScreenShow;
	}

	private void OnRateScreenShow(OnRateScreenShow obj)
	{
		Show();
		Analytics.CustomEvent("SettingsRateClick",
			new Dictionary<string, object> {{"sessions", PrefsManager.GameplayCounter}});
	}
	
	public void RateClick()
	{
#if UNITY_ANDROID
			Application.OpenURL("market://details?id=com.crazylabs.nopixels");
#elif UNITY_IOS
		if (VersionState() && PlayerPrefs.GetInt("RateNativeCounter") < 3)
		{
			PlayerPrefs.SetInt("RateNativeCounter", PlayerPrefs.GetInt("RateNativeCounter")+1);
				iOSReviewRequest.Request();
			}
			else
			{
				Application.OpenURL("itms-apps://itunes.apple.com/app/id1288514456");
			}
#endif
			PrefsManager.RateCounter = 1;
			PlayerPrefs.SetInt("RateCounter", 1);
			PlayerPrefs.SetInt("RateForVersion", PrefsManager.GameVersion);
		
		Analytics.CustomEvent("RateFeedbackClick",
			new Dictionary<string, object> {{"sessions", PrefsManager.GameplayCounter},{"value", _voteValue}});
	}

	private void OnRateBtnClick(int obj)
	{
		_voteValue = obj;
		for (int i = 0; i < obj; i++)
		{
			ScreenRateBtnStar item = starButtons[i];
			item.SetCheck(true);
		}
		for (int i = obj; i < starButtons.Length; i++)
		{
			ScreenRateBtnStar item = starButtons[i];
			item.SetCheck(false);
		}

		smile.sprite = smileSprites[_voteValue-1];
	}
	
	public override void Show()
	{
		GlobalEvents<OnGameInputEnable>.Call(new OnGameInputEnable{Flag = false});
//		UIManager.ShowUiElement("ScreenRateMe");
//		UIManager.ShowUiElement("ScreenRateBackground");
//		UIManager.ShowUiElement("ScreenRatePanel");
//		UIManager.ShowUiElement("ScreenRateBtnBack");
//		UIManager.HideUiElement("ScreenRateBtnRate");
		RateClick();
	}

	public override void Hide()
	{
		UIManager.HideUiElement("ScreenRateMe");
		UIManager.HideUiElement("ScreenRatePanelThanks");
		GlobalEvents<OnGameInputEnable>.Call(new OnGameInputEnable{Flag = true});
	}

	public void GoToRate()
	{
//		if (_voteValue > 3)
//		{
#if UNITY_ANDROID
			Application.OpenURL("market://details?id=com.crazylabs.nopixels");
#elif UNITY_IOS
			if (VersionState() && PlayerPrefs.GetInt("RateNativeCounter") < 3)
			{
				PlayerPrefs.SetInt("RateNativeCounter", PlayerPrefs.GetInt("RateNativeCounter")+1);
				iOSReviewRequest.Request();
			}
			else
			{
				Application.OpenURL("itms-apps://itunes.apple.com/app/id1288514456");
			}
#endif
			PrefsManager.RateCounter = 1;
			PlayerPrefs.SetInt("RateCounter", 1);
			PlayerPrefs.SetInt("RateForVersion", PrefsManager.GameVersion);
//		}
//		else
//		{
//			ShareMail();
//		}
		UIManager.HideUiElement("ScreenRateBtnRate");
		UIManager.HideUiElement("ScreenRatePanel");
		UIManager.HideUiElement("ScreenRateBtnBack");
		UIManager.ShowUiElement("ScreenRatePanelThanks");
		
		Analytics.CustomEvent("RateFeedbackClick",
			new Dictionary<string, object> {{"sessions", PrefsManager.GameplayCounter},{"value", _voteValue}});
	}
	
	public static bool VersionState()
	{
		var str = SystemInfo.operatingSystem;
		string tmp = String.Empty;
		bool point = false;
		for (int i = 0; i < str.Length; i++)
		{
			if (Char.IsDigit(str[i]))
			{
				tmp += str[i];
				continue;
			}
			if (str[i] == '.')
			{
				if (!point)
				{
					tmp += str[i];
					point = true;
				}
				else break;
			}
		}
		if (Convert.ToSingle(tmp) >= 10.3f) return true;
		return false;
	}
	
//	private void ShareMail()
//	{
//		if (NPBinding.Sharing.IsMailServiceAvailable())
//		{
//			MailShareComposer    _composer    = new MailShareComposer();
//			_composer.Subject                = "Make your game better!";
//			_composer.Body                    = "";
//
//// Set below to true if the body is HTML
//			_composer.IsHTMLBody            = false;
//
//// Send array of receivers if required
//			string[] recipients = {"po4offee@gmail.com"};
//			_composer.ToRecipients            = recipients;
//
//			// Show share view
//			NPBinding.Sharing.ShowView(_composer, FinishedSharing);
//
//			Debug.Log("Mail sharing");
//		}
//	}
//
//	void FinishedSharing (eShareResult _result){
//		Debug.Log("Mail Result = " + _result);
//	}
	
}
