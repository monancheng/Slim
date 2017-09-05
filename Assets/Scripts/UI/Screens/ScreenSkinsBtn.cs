using UnityEngine;
using UnityEngine.UI;

public class ScreenSkinsBtn : MonoBehaviour {

	[SerializeField] private GameObject _button;
	[SerializeField] private GameObject _coinImage;
	[SerializeField] private GameObject _price;
	[SerializeField] private  Sprite spriteLock;
	[SerializeField] private  Sprite sprite;
	
	public void SetLock(bool isUnlocked)
	{
		if (isUnlocked)
		{
			_button.GetComponent<Image>().sprite = sprite;
			if (_coinImage!= null) _coinImage.SetActive(false);
			if (_price!= null) _price.SetActive(false);
		}
		else
		{
			_button.GetComponent<Image>().sprite = spriteLock;
			if (_coinImage!= null) _coinImage.SetActive(true);
			if (_price!= null) _price.SetActive(true);
		}
	}
}
