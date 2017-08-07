using UnityEngine;

public class ScreenGift : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	private void OnEnable()
	{
		GlobalEvents<OnBtnGiftClick>.Happened += OnBtnGiftClick;
	}

	private void OnDisable()
	{
		GlobalEvents<OnBtnGiftClick>.Happened -= OnBtnGiftClick;
	}

	private void OnBtnGiftClick(OnBtnGiftClick obj)
	{
		
	}
}
