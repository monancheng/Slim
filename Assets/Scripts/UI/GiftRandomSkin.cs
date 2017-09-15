using System.Collections.Generic;
using DoozyUI;
using UnityEngine;

public class GiftRandomSkin : MonoBehaviour
{
	private bool _isSkinsAllGeneralOpened;

	private void OnEnable()
	{
		GlobalEvents<OnGiftShowRandomSkinAnimation>.Happened += OnGiftShowRandomSkinAnimation;
		GlobalEvents<OnSkinAllGeneralOpened>.Happened += OnSkinAllGeneralOpened;
	}

	private void OnSkinAllGeneralOpened(OnSkinAllGeneralOpened obj)
	{
		_isSkinsAllGeneralOpened = true;
	}

	private void OnGiftShowRandomSkinAnimation(OnGiftShowRandomSkinAnimation obj)
	{
		Debug.Log("OnGiftShowRandomSkinAnimation(OnGiftShowRandomSkinAnimation obj)");
		int id = GetRandomAvailableSkin();
		if (id != -1)
		{
			transform.localScale = Vector3.one;	

			GlobalEvents<OnBuySkin>.Call(new OnBuySkin {Id = id});
		}
		
		Invoke("ShowBtnClose", 1.5f);
	}

	private int GetRandomAvailableSkin()
	{
		Debug.Log("GetRandomAvailableSkin");
		
		if (_isSkinsAllGeneralOpened) return -1;
		
		// Создаем список со всеми доступными скинами
		List<int> availableSkins = new List<int>();
		for (int j = 0; j < DefsGame.FaceAvailable.Length; j++)
		{
			if (DefsGame.FaceAvailable[j] == 0)
			{
				availableSkins.Add(j);
			}
		}

		if (availableSkins.Count > 0)
		{
			int id = Random.Range(0, availableSkins.Count + 1);
			Debug.Log("GetRandomAvailableSkin RETURN id = " + id);
			return id;
		}

		Debug.Log("GetRandomAvailableSkin RETURN id = " + -1);
		return -1;
	}

	public void BtnClose()
	{
		UIManager.HideUiElement("ScreenGiftBtnPlay");
		
		GlobalEvents<OnHideGiftScreen>.Call(new OnHideGiftScreen());
		GlobalEvents<OnHideTubes>.Call(new OnHideTubes());
	}

	private void ShowBtnClose()
	{
		UIManager.ShowUiElement("ScreenGiftBtnPlay");
	}
}
