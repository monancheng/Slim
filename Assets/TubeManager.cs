using PrimitivesPro.GameObjects;
using UnityEngine;

public class TubeManager : MonoBehaviour {

	private float Radius = 3.5f;
	private float _height = 3.5f;
	private int _sides = 30;
	private float OuterRadiusMax = 10;
	private BaseObject _shapeObject;

	void Start ()
	{
		CreateTube();
	}

	private void CreateTube()
	{
		float outerRadius = Random.Range(Radius + 2f, 12f);
		if (outerRadius > OuterRadiusMax) outerRadius = OuterRadiusMax;
		_shapeObject = Tube.Create(Radius, outerRadius, _height, _sides, 1, 0.0f, false,
			PrimitivesPro.Primitives.NormalsType.Vertex,
			PrimitivesPro.Primitives.PivotPosition.Botttom);
		
		_shapeObject.gameObject.GetComponent<Renderer>().material = new Material(GetSpecularShader());
		_shapeObject.gameObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(1.0f, 255.0f/255f, 0f/255f));
		_shapeObject.gameObject.GetComponent<Renderer>().material.SetColor("_SpecColor", Color.white);
		_shapeObject.gameObject.transform.position = Vector3.zero;
	}
	
	Shader GetSpecularShader()
	{
		return Shader.Find("Specular");
	}
}
