using System.Collections;
using UnityEngine;

public class SpawnStars : MonoBehaviour
{
	public GameObject Star;
	
	// Use this for initialization
	void Start ()
	{
		StartCoroutine(Spawn());
	}

	private IEnumerator Spawn()
	{
		while (true)
		{
			Vector3 pos = new Vector3(transform.position.x + Random.Range(0, 20f),
				transform.position.y + Random.Range(2f, 7f), transform.position.z);
			Instantiate(Star, pos, Quaternion.Euler(0, 0, Random.Range(0, 360)));
			yield return new WaitForSeconds(2.01f);
		}
	}
}
