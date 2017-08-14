using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BonusChar : ItemBonus
{
//    public static event Action OnBonusGrow;
    [SerializeField] private AudioClip SoundCollide;

    private void Awake()
    {
		Init ();
    }

    private void OnEnable()
    {
        MyTube.OnCanSpawnBonus += OnCanSpawnBonus;
        TubeManager.OnCreateChar += OnCreate;
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
			Defs.PlaySound(SoundCollide);
//            GameEvents.Send(OnBonusGrow);
            Activate();
        }
    }
}
