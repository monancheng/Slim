using UnityEngine;

public class DestroyByTime : MonoBehaviour
{
    public float LifeTime = 3f;

    private void Awake()
    {
        Destroy(gameObject, LifeTime);
    }

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }
}