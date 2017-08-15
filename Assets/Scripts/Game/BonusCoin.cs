using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BonusCoin : ItemBonus
{
	public static event Action<int> OnAddCoinsVisual;

    [SerializeField] private AudioClip _sndTakeCoin;
	[SerializeField] private AudioClip[] SoundsGrow;

	private void Awake()
	{
		Init ();
	}

    private void OnEnable()
    {
        MyTube.OnCanSpawnBonus += OnCanSpawn;
        TubeManager.OnCreateCoin += OnCreate;
        GlobalEvents<OnGameOver>.Happened += GameOver;
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
            GameEvents.Send(OnAddCoinsVisual, 1);
            Defs.PlaySound(_sndTakeCoin);
            Activate();
        }
    }
}