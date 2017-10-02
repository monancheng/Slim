using DoozyUI;
using UnityEngine;
using VoxelBusters.NativePlugins;

public class ScreenRate : ScreenItem
{
	[SerializeField] private ScreenRateBtnStar[] buttons;
	[SerializeField] private UIButton buttonClose;
	private int VoteValue;
	
	private void Start()
	{
		InitUi();
	}

	void OnEnable ()
	{
		ScreenRateBtnStar.OnRateBtnClick += OnRateBtnClick;
	}

	private void OnRateBtnClick(int obj)
	{
		VoteValue = obj;
		for (int i = 0; i < obj; i++)
		{
			ScreenRateBtnStar item = buttons[i];
			item.SetCheck(true);
		}
		for (int i = obj; i < buttons.Length; i++)
		{
			ScreenRateBtnStar item = buttons[i];
			item.SetCheck(false);
		}
	}

	public void GoToRate()
	{
		if (VoteValue > 3)
		{
#if UNITY_ANDROID
			Application.OpenURL("http://squaredino.com");
#elif UNITY_IPHONE
			Application.OpenURL("http://squaredino.com");
#endif
		}
		else
		{
			ShareMail();
		}
		buttonClose.ExecuteClick();
	}
	
	private void ShareMail()
	{
		if (NPBinding.Sharing.IsMailServiceAvailable())
		{
			MailShareComposer    _composer    = new MailShareComposer();
			_composer.Subject                = "Make your game better!";
			_composer.Body                    = "";

// Set below to true if the body is HTML
			_composer.IsHTMLBody            = false;

// Send array of receivers if required
			string[] recipients = {"po4offee@gmail.com"};
			_composer.ToRecipients            = recipients;

			// Show share view
			NPBinding.Sharing.ShowView(_composer, FinishedSharing);

			Debug.Log("Mail sharing");
		}
	}

	void FinishedSharing (eShareResult _result){
		Debug.Log("Mail Result = " + _result);
	}
}
