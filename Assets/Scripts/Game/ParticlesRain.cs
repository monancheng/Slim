using UnityEngine;

public class ParticlesRain : MonoBehaviour
{
	private ParticleSystem _ps;
	
	// Use this for initialization
	void Start ()
	{
		_ps = GetComponent<ParticleSystem>();
	}

	private void OnEnable()
	{
		TubeManager.OnTubesSpeedScale += OnTubesSpeedScale;
	}

	private void OnTubesSpeedScale(float scale)
	{
		var main = _ps.main;
		main.startSpeedMultiplier = scale;
	}

}
