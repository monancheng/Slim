using PrimitivesPro.GameObjects;
using UnityEngine;

public class Hero : MonoBehaviour
{
	public float Radius = 3.5f;
	private float _height = 6.0f;
	private int _sides = 30;
	
	private BaseObject _shapeObject;


	void Start () {
		CreateCylinder();
	}

	private void CreateCylinder()
	{
		_shapeObject = Cylinder.Create(Radius, _height, _sides, 1, 
			PrimitivesPro.Primitives.NormalsType.Vertex,
			PrimitivesPro.Primitives.PivotPosition.Botttom);
		
		_shapeObject.gameObject.GetComponent<Renderer>().material = new Material(GetSpecularShader());
		_shapeObject.gameObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(1.0f, 0.0f/255f, 255f/255f));
		_shapeObject.gameObject.GetComponent<Renderer>().material.SetColor("_SpecColor", Color.white);
		_shapeObject.gameObject.transform.position = Vector3.zero;
	}
	
	Shader GetSpecularShader()
	{
		return Shader.Find("Specular");
	}
}
