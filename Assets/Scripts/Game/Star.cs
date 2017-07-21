using UnityEngine;

public class Star : MonoBehaviour
{
    private float _movementSpeed = 1000f;
    private SpriteRenderer spr;

    // Use this for initialization
    private void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        _movementSpeed = Random.Range(0.05f, 0.15f);
        Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    private void Update()
    {
        spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, Mathf.PingPong(Time.time / 2.5f, 1.0f));

        // Move
//		transform.position += transform.up * Time.deltaTime * _movementSpeed;
    }
}