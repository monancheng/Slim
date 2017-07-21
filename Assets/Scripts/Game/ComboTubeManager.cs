using PrimitivesPro.GameObjects;
using PrimitivesPro.Primitives;
using UnityEngine;

public class ComboTubeManager : MonoBehaviour
{
    private const float ComboEffectDelay = 0.1f;
    private Color _color;
    private int _comboEffectCounter;
    private Vector3 _comboEffectPosition;
    private float _comboEffectPositionY;
    private float _comboEffectRadius;
    private float _comboEffectTime;

    private void Start()
    {
        _color = new Color(50f / 255.0f, 50f / 255f, 50f / 255f);
    }

    private void OnEnable()
    {
        Player.OnCombo += OnCombo;
    }

    private void OnCombo(int comboCounter, float radius, GameObject go)
    {
        D.Log("OnCombo", comboCounter);
        _comboEffectCounter = comboCounter;
        if (_comboEffectCounter > 3) _comboEffectCounter = 3;
        _comboEffectRadius = radius;
        _comboEffectTime = ComboEffectDelay;

        _comboEffectPosition = new Vector3(go.transform.position.x,
            go.transform.position.y - go.GetComponent<Tube>().height * 0.5f + 0.1f, go.transform.position.z);
        _comboEffectPositionY = _comboEffectPosition.y;
    }

    private void Update()
    {
        _comboEffectPositionY -= TubeManager.CurrentSpeed * Time.deltaTime;
        if (_comboEffectCounter > 0)
        {
            _comboEffectTime += Time.deltaTime;
            if (_comboEffectTime >= ComboEffectDelay)
            {
                _comboEffectTime = 0;
                --_comboEffectCounter;
                CreateGoodTube(_comboEffectRadius);
                if (_comboEffectCounter <= 0)
                    ResetSettings();
            }
        }
    }

    private void ResetSettings()
    {
        _color = new Color(50f / 255.0f, 50f / 255f, 50f / 255f);
    }

    private void CreateGoodTube(float radius)
    {
        BaseObject shapeObject = Tube.Create(radius, radius + 1.0f, 0.4f, 23, 1, 0.0f, false,
            NormalsType.Vertex,
            PivotPosition.Center);

        var go = shapeObject.gameObject;
        go.GetComponent<Renderer>().material = new Material(GetTransparentDiffuseShader());
        go.GetComponent<Renderer>().material.SetColor("_Color", _color);
        go.transform.position = new Vector3(_comboEffectPosition.x, _comboEffectPositionY, _comboEffectPosition.z);
        var pt = go.AddComponent<PlayerTubeGood>();
        pt.GoodAnimation(_comboEffectCounter);
    }

    private Shader GetTransparentDiffuseShader()
    {
        return Shader.Find("Transparent/Diffuse");
    }
}