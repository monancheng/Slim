using UnityEngine;

public class ColorTheme : MonoBehaviour {

	public static Color GetPlayerColor()
	{
		float ran = Random.value;
		if (ran < 1f/8f) return new Color(1.0f, 105f/255f, 182f/255f);
		if (ran < 2f/8f) return new Color(98f / 255.0f, 201f / 255f, 255f / 255f);
		if (ran < 3f/8f) return new Color(255f / 255.0f, 201f / 255f, 104f / 255f);
		if (ran < 4f/8f) return new Color(203f / 255.0f, 216f / 255f, 235f / 255f);
		if (ran < 5f/8f) return new Color(250f / 255.0f, 69f / 255f, 78f / 255f);
		if (ran < 6f/8f) return new Color(253f / 255.0f, 141f / 255f, 38f / 255f);
		if (ran < 7f/8f) return new Color(126f / 255.0f, 199f / 255f, 207f / 255f);
		return new Color(107f / 252.0f, 253f / 255f, 185f / 255f);
	}
	
	
	public static Color GetTubeColor()
	{
		float ran = Random.value;
		if (ran < 1f/8f) return new Color(1.0f, 105f/255f, 182f/255f);
		if (ran < 2f/8f) return new Color(98f / 255.0f, 201f / 255f, 255f / 255f);
		if (ran < 3f/8f) return new Color(255f / 255.0f, 201f / 255f, 104f / 255f);
		if (ran < 4f/8f) return new Color(203f / 255.0f, 216f / 255f, 235f / 255f);
		if (ran < 5f/8f) return new Color(250f / 255.0f, 69f / 255f, 78f / 255f);
		if (ran < 6f/8f) return new Color(253f / 255.0f, 141f / 255f, 38f / 255f);
		if (ran < 7f/8f) return new Color(126f / 255.0f, 199f / 255f, 207f / 255f);
		return new Color(107f / 252.0f, 253f / 255f, 185f / 255f);
	}
}
