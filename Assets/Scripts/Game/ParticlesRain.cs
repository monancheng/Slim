using UnityEngine;

public class ParticlesRain : MonoBehaviour
{
	private ParticleSystem _ps;
	private float _startSpeed;
	
	// Use this for initialization
	void Start ()
	{
		_ps = GetComponent<ParticleSystem>();
		_startSpeed = _ps.main.startSpeedMultiplier;
	}

	private void OnEnable()
	{
		TubeManager.OnTubesSpeedScale += OnTubesSpeedScale;
	}

	private void OnTubesSpeedScale(float scale)
	{
		var main = _ps.main;
		main.startSpeedMultiplier = _startSpeed*scale;
	}

}
