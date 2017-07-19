using UnityEngine;

public class PlayerTubeBad : MonoBehaviour
{
	[HideInInspector] public float Speed;
	
	private void Update()
	{
		Move();
		if (transform.position.y < - 24f) Destroy(gameObject);
		Color color = gameObject.GetComponent<MeshRenderer>().material.color;
		if (color.a > 0f)
		{
			color.a -= 0.05f;
			gameObject.GetComponent<MeshRenderer>().material.color = color;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	protected void Move()
	{
		transform.position = new Vector3(transform.position.x, transform.position.y - Speed * Time.deltaTime, transform.position.z);
	}
}
