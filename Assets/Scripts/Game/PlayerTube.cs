using UnityEngine;

public class PlayerTube : MonoBehaviour
{
	[HideInInspector] public float Speed;
	
	private void Update()
	{
		transform.position = new Vector3(transform.position.x, transform.position.y - Speed, transform.position.z);
		if (transform.position.y < - 21f) Destroy(gameObject);
	}
}
