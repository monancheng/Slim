using UnityEngine;
using Random =  UnityEngine.Random;

public class Star : MonoBehaviour
{
	private SpriteRenderer spr;
	private float _movementSpeed = 1000f;

	// Use this for initialization
	void Start ()
	{
		spr = GetComponent<SpriteRenderer>();
		_movementSpeed = Random.Range(0.05f, 0.15f);
		Destroy(gameObject, 5f);
	}
	
	// Update is called once per frame
	void Update () {
		spr.color = new Color(spr.color.r,spr.color.g,spr.color.b, Mathf.PingPong(Time.time / 2.5f, 1.0f));
		
		// Move
//		transform.position += transform.up * Time.deltaTime * _movementSpeed;
		
	}
}
