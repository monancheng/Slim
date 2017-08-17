using System;
using DarkTonic.MasterAudio;
using UnityEngine;
using Random = UnityEngine.Random;

public class BonusCoin : ItemBonus
{
	public static event Action<int> OnAddCoinsVisual;

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
            MasterAudio.PlaySoundAndForget("CoinTake");
            Activate();
        }
    }
}