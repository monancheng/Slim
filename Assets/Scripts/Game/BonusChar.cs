using System;
using UnityEngine;

public class BonusChar : ItemBonus
{
    public static event Action OnBonusGot;
    [SerializeField] private AudioClip SoundCollide;
	[SerializeField] private MeshRenderer _meshRenderer;
	[SerializeField] private MeshRenderer _meshRendererText;
	[SerializeField] private TextMesh _textMesh;

	private Material _material;

	private string _nextChar;

    private void Awake()
    {
		Init ();
		_material = _meshRenderer.material;
    }

    private void OnEnable()
    {
        MyTube.OnCanSpawnBonus += OnCanSpawn;
        TubeManager.OnCreateChar += OnCreate;
		Words.OnWordSetChar += OnWordSetChar;

        GlobalEvents<OnGameOver>.Happened += GameOver;
    }
	
	override protected void OnCanSpawn()
	{
		if (!IsVisible && _isTimeToCreate)
			_textMesh.text = _nextChar;
		
		base.OnCanSpawn();
	}

	void OnWordSetChar (string obj)
	{
		_nextChar = obj;
	}

    // Update is called once per frame
    private void Update()
    {
		MoveUpdate ();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
			Defs.PlaySound(SoundCollide);
            GameEvents.Send(OnBonusGot);
            Activate();
        }
    }

	override protected void HideAnimation() {
		base.HideAnimation ();
		_meshRendererText.material.SetColor ("_Color", new Color(_meshRendererText.material.color.r, _meshRendererText.material.color.g, _meshRendererText.material.color.b, _material.color.a));
	}


	override protected void ShowAnimation() {
		base.ShowAnimation ();
		_meshRendererText.material.SetColor ("_Color", new Color(_meshRendererText.material.color.r, _meshRendererText.material.color.g, _meshRendererText.material.color.b, _material.color.a));
	}
}
