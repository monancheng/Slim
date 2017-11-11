using System;
using System.Collections;
using System.Collections.Generic;
using DoozyUI;
using UnityEngine;
using UnityEngine.UI;

public class ReviveButton : MonoBehaviour
{
	public static event Action OnClickRevive, ReviveTimeEnded;
	private float timer = 0;
	[SerializeField] private float _reviveTime;
	[SerializeField] private GameObject _timerLavel;
	[SerializeField] private GameObject _backgroundImage;
	
	private void Start()
	{
		GetComponent<UIElement>().Show(true);
		timer = _reviveTime;
	}

	public void OnClickReviveButton()
	{
		GameEvents.Send(OnClickRevive);
		Destroy(gameObject);
	}
	
	void Update ()
	{
		if (timer > 0.0)
		{
			timer -= Time.deltaTime;
			_timerLavel.GetComponent<Text>().text = Convert.ToInt32(Math.Ceiling(timer)).ToString();
			_backgroundImage.GetComponent<Image>().fillAmount = timer / _reviveTime;

			if (timer <= 0.0)
			{
				GameEvents.Send(ReviveTimeEnded);
				Destroy(gameObject);
			}
		}
	}
}
