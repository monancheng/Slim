using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BonusIncrease : ItemBonus
{
    public static event Action OnBonusGrow;
    [SerializeField] private AudioClip[] SoundsGrow;

    private void Awake()
    {
		Init ();
    }

    private void OnEnable()
    {
        MyTube.OnCanSpawnBonus += OnCanSpawnBonus;
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
            Defs.PlaySound(GetRandomGrowSound());
            GameEvents.Send(OnBonusGrow);
            Activate();
        }
    }

    private AudioClip GetRandomGrowSound()
    {
        return SoundsGrow[(int) Mathf.Round(Random.value * (SoundsGrow.Length - 1))];
    }
}