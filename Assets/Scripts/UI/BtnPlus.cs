using UnityEngine;
using UnityEngine.UI;

public class BtnPlus : MonoBehaviour
{
	[SerializeField] private GameObject _textItem;
	
	// Use this for initialization
	void Start ()
	{
		FitText();
	}

	private void FitText()
	{
		TextGenerator textGen = new TextGenerator();
		Text text = _textItem.GetComponent<Text>();
		TextGenerationSettings generationSettings = text.GetGenerationSettings(text.rectTransform.rect.size); 
		float width = textGen.GetPreferredWidth(text.text, generationSettings);
//		float height = textGen.GetPreferredHeight(text.text, generationSettings);
		transform.position = new Vector3(_textItem.transform.position.x + 55 - width, transform.position.y, transform.position.z);
	}

	// Update is called once per frame
	void Update () {
		FitText();
	}
}
