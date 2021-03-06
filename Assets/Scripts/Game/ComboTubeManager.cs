﻿using PrimitivesPro.GameObjects;
using PrimitivesPro.Primitives;
using UnityEngine;
using UnityEngine.Rendering;

public class ComboTubeManager : MonoBehaviour
{
    private const float ComboEffectDelay = 0.1f;
    private int _comboEffectCounter;
    private Vector3 _comboEffectPosition;
    private float _comboEffectPositionY;
    private float _comboEffectRadius;
    private float _comboEffectTime;

    private void OnEnable()
    {
        MyPlayer.OnCombo += OnCombo;
        MyPlayer.OnCrash += OnCrash;
        MyPlayer.OnSlim += OnSlim;
    }

    private void OnCombo(int comboCounter, float radius, Vector3 pos, float height)
    {
        _comboEffectCounter = comboCounter;
        if (_comboEffectCounter > 3) _comboEffectCounter = 3;
        _comboEffectRadius = radius;
        _comboEffectTime = ComboEffectDelay;
        _comboEffectPosition = new Vector3(pos.x, pos.y - height * 0.5f + 0.1f, pos.z);
        _comboEffectPositionY = _comboEffectPosition.y;
    }
    
    private void OnCrash(float radius, Vector3 pos, float height)
    {
        _comboEffectCounter = 2;
        _comboEffectRadius = radius;
        _comboEffectPosition = new Vector3(pos.x, pos.y - height * 0.5f + 0.1f, pos.z);
        _comboEffectPositionY = _comboEffectPosition.y;
        CreateCrashTube(_comboEffectRadius);
        _comboEffectCounter = 0;
    }
    
    private void OnSlim(float radius, Vector3 pos, float height)
    {
        _comboEffectCounter = 2;
        _comboEffectRadius = radius;
        _comboEffectPosition = new Vector3(pos.x, pos.y - height * 0.5f + 0.1f, pos.z);
        _comboEffectPositionY = _comboEffectPosition.y;
        CreateSlimTube(_comboEffectRadius);
        _comboEffectCounter = 0;
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
            }
        }
    }


    private void CreateGoodTube(float radius)
    {
        BaseObject shapeObject = Tube.Create(radius, radius * 1.25f, 0.4f, 23, 1, 0.0f, false,
            NormalsType.Vertex,
            PivotPosition.Center);

        var go = shapeObject.gameObject;
        go.GetComponent<Renderer>().material = new Material(GetTransparentDiffuseShader());
        go.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
        go.GetComponent<Renderer>().receiveShadows = false;
        go.GetComponent<Renderer>().material.SetColor("_Color", new Color(255f / 255.0f, 255f / 255f, 255f / 255f));
        go.transform.position = new Vector3(_comboEffectPosition.x, _comboEffectPositionY, _comboEffectPosition.z);
        var pt = go.AddComponent<PlayerTubeGood>();
        pt.GoodAnimation(_comboEffectCounter);
    }
    
    private void CreateSlimTube(float radius)
    {
        BaseObject shapeObject = Tube.Create(radius, radius * 1.25f, 0.4f, 23, 1, 0.0f, false,
            NormalsType.Vertex,
            PivotPosition.Center);

        var go = shapeObject.gameObject;
        go.GetComponent<Renderer>().material = new Material(GetTransparentDiffuseShader());
        go.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
        go.GetComponent<Renderer>().receiveShadows = false;
        go.GetComponent<Renderer>().material.SetColor("_Color", new Color(255f / 255.0f, 201f / 255f, 104f / 255f, 0.8f));
        go.transform.position = new Vector3(_comboEffectPosition.x, _comboEffectPositionY, _comboEffectPosition.z);
        var pt = go.AddComponent<PlayerTubeGood>();
        pt.GoodAnimation(_comboEffectCounter);
    }
    
    private void CreateCrashTube(float radius)
    {
        BaseObject shapeObject = Tube.Create(radius, radius * 1.3f, 0.4f, 23, 1, 0.0f, false,
            NormalsType.Vertex,
            PivotPosition.Center);

        var go = shapeObject.gameObject;
        go.GetComponent<Renderer>().material = new Material(GetTransparentDiffuseShader());
        go.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
        go.GetComponent<Renderer>().receiveShadows = false;
        go.GetComponent<Renderer>().material.SetColor("_Color", new Color(255f / 255.0f, 0f / 255f, 0f / 255f));
        go.transform.position = new Vector3(_comboEffectPosition.x, _comboEffectPositionY, _comboEffectPosition.z);
        var pt = go.AddComponent<PlayerTubeGood>();
        pt.GoodAnimation(_comboEffectCounter);
    }

    private Shader GetTransparentDiffuseShader()
    {
        return Shader.Find("Transparent/Diffuse");
    }
}