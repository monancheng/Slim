using UnityEngine;

public class SkyBoxColors : MonoBehaviour
{
	public Light Light;
	public Color[] ColorsTop;
	public Color[] ColorsDown;
	
	private float _time;
	private const float Delay = 12f;
	private Color _currColorTop;
	private Color _currColorDown;
	private int _nextColorId;
	private int _currColorID;

	private float startTime;
	private const float Speed = 0.05f;

	private void Start()
	{
		_nextColorId = Mathf.RoundToInt(Random.Range(0, ColorsTop.Length - 1));
		ChooseNextColor();
//		_nextColorId = 19;
		_currColorTop = ColorsTop[_nextColorId];
		GetComponent<Skybox>().material.SetColor("_Color2", _currColorTop);
		_currColorDown = ColorsDown[_nextColorId];
		GetComponent<Skybox>().material.SetColor("_Color1", _currColorDown);
	}

//	private void OnEnable()
//	{
//		GlobalEvents<OnStartGame>.Happened += StartGame;
//	}
//	
//	private void OnDisable()
//	{
//		GlobalEvents<OnStartGame>.Happened -= StartGame;
//	}
//
//	private void StartGame(OnStartGame obj)
//	{
//		ChooseNextColor();
//	}

	private void ChooseNextColor()
	{
		_currColorID = _nextColorId;
		++_nextColorId;
		if (_nextColorId >= ColorsTop.Length) _nextColorId = 0;
		startTime = Time.time;
	}

	private void Update()
	{
		_time += Time.deltaTime;
		if (_time >= Delay)
		{
			_time = 0f;
			ChooseNextColor();
		}

		float distCovered = (Time.time - startTime) * Speed;
		float fracJourney = distCovered / 1f;

		if (_currColorID != _nextColorId)
		{
			_currColorTop = Color.Lerp(_currColorTop, ColorsTop[_nextColorId], fracJourney);
			if (_currColorTop != ColorsTop[_nextColorId])
				GetComponent<Skybox>().material.SetColor("_Color2", _currColorTop);
			else
				_currColorID = _nextColorId;
			_currColorDown = Color.Lerp(_currColorDown, ColorsDown[_nextColorId], fracJourney);
			if (_currColorDown != ColorsDown[_nextColorId])
				GetComponent<Skybox>().material.SetColor("_Color1", _currColorDown);

//			float r = _currColorTop.r * 2.4f + 0.15f;
//			if (r > 1) r = 1f; else if (r < 0.5f) r = 0.7f;
//			float g = _currColorTop.g * 2.4f + 0.15f;
//			if (g > 1) g = 1f; else if (g < 0.5f) g = 0.7f;
//			float b = _currColorTop.b * 2.4f + 0.15f;
//			if (b > 1) b = 1f; else if (b < 0.5f) b = 0.7f;
//			Light.color = new Color(r, g, b, 1f);
		}
	}
}
