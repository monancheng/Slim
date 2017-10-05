using System.Collections.Generic;
using DoozyUI;
using PrefsEditor;
using UnityEngine;

public class GiftRandomSkin : MonoBehaviour
{
	private bool _isSkinsAllGeneralOpened;
	private bool _isFirstGift;

	void Start()
	{
		_isFirstGift = SecurePlayerPrefs.GetBool("isFirstSkinGift", true);
	}
	
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
		int id = -1;
		// TEMP
		// Первым подарком дарим Спинер
		if (_isFirstGift)
		{
			_isFirstGift = false;
			SecurePlayerPrefs.SetBool("isFirstSkinGift", false);
			if (DefsGame.FaceAvailable[7] == 0) id = 7;
		}
		
		if (id == -1) id = GetRandomAvailableSkin();
			
		if (id != -1)
		{
			transform.localScale = Vector3.one;

			GlobalEvents<OnBuySkin>.Call(new OnBuySkin {Id = id});
		}

		GlobalEvents<OnHideGiftScreen>.Call(new OnHideGiftScreen());
	}

	private int GetRandomAvailableSkin()
	{
		Debug.Log("GetRandomAvailableSkin");
		
		if (_isSkinsAllGeneralOpened) return -1;
		
		// Создаем список со всеми доступными скинами
		List<int> availableSkins = new List<int>();
		for (int j = DefsGame.FacesGeneralMin; j < DefsGame.FacesGeneralMax; j++)
		{
			if (DefsGame.FaceAvailable[j] == 0)
			{
				availableSkins.Add(j);
			}
		}

		if (availableSkins.Count > 0)
		{
			int id = Random.Range(0, availableSkins.Count + 1);
			Debug.Log("GetRandomAvailableSkin RETURN id = " + availableSkins[id]);
			return availableSkins[id];
		}

		Debug.Log("GetRandomAvailableSkin RETURN id = " + -1);
		return -1;
	}
}
