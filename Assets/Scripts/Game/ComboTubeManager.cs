using PrimitivesPro.GameObjects;
using UnityEngine;

public class ComboTubeManager : MonoBehaviour {
	private int _comboEffectCounter;
	private float _comboEffectRadius;
	private float _combatEffectSpeed;
	private float _comboEffectPositionY;
	private Vector3 _comboEffectPosition;
	private float _comboEffectTime;
	private Color _color;
	private const float ComboEffectDelay = 0.1f;

	void Start()
	{
		_color = new Color(50f / 255.0f, 50f / 255f, 50f / 255f);
	}

	private void OnEnable()
	{
		Player.OnCombo += OnCombo;
	}

	private void OnCombo(int _comboCounter,  float _radius, GameObject go)
	{
		D.Log("OnCombo", _comboCounter);
		_comboEffectCounter = _comboCounter;
		if (_comboEffectCounter > 3) _comboEffectCounter = 3;
		_comboEffectRadius = _radius;
		_combatEffectSpeed = go.GetComponent<MyTube>().Speed;
		_comboEffectTime = ComboEffectDelay;
		
		_comboEffectPosition = new Vector3(go.transform.position.x, go.transform.position.y - go.GetComponent<Tube>().height*0.5f + 0.1f, go.transform.position.z);
		_comboEffectPositionY = _comboEffectPosition.y;
	}

	void Update () {
		_comboEffectPositionY -= _combatEffectSpeed*Time.deltaTime;
		if (_comboEffectCounter > 0)
		{
			_comboEffectTime += Time.deltaTime;
			if (_comboEffectTime >= ComboEffectDelay)
			{
				_comboEffectTime = 0;
				--_comboEffectCounter;
				CreateGoodTube(_comboEffectRadius, _combatEffectSpeed);
				if (_comboEffectCounter <= 0)
				{
					ResetSettings();
				}
			}
		}
		
	}

	private void ResetSettings()
	{
		_color = new Color(50f / 255.0f, 50f / 255f, 50f / 255f);
	}

	private void CreateGoodTube(float radius, float speed)
	{
		BaseObject shapeObject = Tube.Create(radius, radius + 1.0f, 0.4f, 23, 1, 0.0f, false,
			PrimitivesPro.Primitives.NormalsType.Vertex,
			PrimitivesPro.Primitives.PivotPosition.Center);
		
		GameObject go = shapeObject.gameObject;
		go.GetComponent<Renderer>().material = new Material(GetTransparentDiffuseShader());
		go.GetComponent<Renderer>().material.SetColor("_Color", _color);
		go.transform.position = new Vector3(_comboEffectPosition.x, _comboEffectPositionY, _comboEffectPosition.z);
		PlayerTubeGood pt = go.AddComponent<PlayerTubeGood>();
		pt.Speed = speed;
		pt.GoodAnimation(_comboEffectCounter);
	}
	
	Shader GetTransparentDiffuseShader()
	{
		return Shader.Find("Transparent/Diffuse");
	}
}
