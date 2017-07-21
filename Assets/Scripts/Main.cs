using UnityEngine;

public class Main : MonoBehaviour
{
    private void Awake()
    {
        DefsGame.LoadVariables();
        Defs.AudioSource = GetComponent<AudioSource>();
    }
}