using UnityEngine;
using UnityEngine.UI;

public class ScreenSkinBtn : MonoBehaviour {

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
			_coinImage.SetActive(false);
			_price.SetActive(false);
		}
		else
		{
			_button.GetComponent<Image>().sprite = spriteLock;
			_coinImage.SetActive(true);
			_price.SetActive(true);
		}
	}
}
