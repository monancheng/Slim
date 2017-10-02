using System;
using DoozyUI;
using UnityEngine;
using UnityEngine.UI;

public class ScreenRateBtnStar : MonoBehaviour
{
	public static event Action<int> OnRateBtnClick;
	[SerializeField] private int _id;
	
	[SerializeField] private Sprite _checked;
	[SerializeField] private Sprite _unchecked;
	
	public void Click()
	{
		GameEvents.Send(OnRateBtnClick, _id);
		UIManager.ShowUiElement("ScreenRateBtnRate");
	}

	public void SetCheck(bool flag)
	{
		GetComponent<Image>().sprite = flag ? _checked : _unchecked;
	}
}
