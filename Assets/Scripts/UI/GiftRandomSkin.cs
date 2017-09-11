using DoozyUI;
using UnityEngine;

public class GiftRandomSkin : MonoBehaviour
{
	private void OnEnable()
	{
		GlobalEvents<OnGiftShowRandomSkinAnimation>.Happened += OnGiftShowRandomSkinAnimation;
	}

	private void OnGiftShowRandomSkinAnimation(OnGiftShowRandomSkinAnimation obj)
	{
		int id = GetRandomAvailableSkin();
		if (id != -1)
		{
			transform.localScale = Vector3.one;
			
			Invoke("ShowBtnClose", 1.5f);

			GlobalEvents<OnBuySkin>.Call(new OnBuySkin {Id = id});
		}
	}

	private int GetRandomAvailableSkin()
	{
		if (DefsGame.QUEST_CHARACTERS_Counter == DefsGame.FaceAvailable.Length - 1) return -1;
		int tryCount = Random.Range(DefsGame.FacesGeneralMin, DefsGame.FacesGeneralMax + 1);
		int i = DefsGame.FacesGeneralMin-1;
		while (i < tryCount)
		{
			for (int id = DefsGame.FacesGeneralMin+1; id < DefsGame.FacesGeneralMax; id++)
			{
				if (DefsGame.FaceAvailable[id] == 0)
				{
					++i;
					if (i == tryCount)
					{
						return id;
					}
				}
			}
		}
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
