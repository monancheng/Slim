using System;
using DarkTonic.MasterAudio;
using UnityEngine;

public class BonusIncrease : ItemBonus
{
    public static event Action OnBonusGrow;

    private void Awake()
    {
		Init ();
    }

    private void OnEnable()
    {
        MyTube.OnCanSpawnBonus += OnCanSpawn;
        TubeManager.OnCreateBonusIncrease += OnCreate;
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
            MasterAudio.PlaySoundAndForget("BonusIncrease");
            GameEvents.Send(OnBonusGrow);
            Activate();
        }
    }
}