using UnityEngine;
using System.Collections;

public class MeshColorer : MonoBehaviour {
	public Vector3[] vertices;
	public Color[] colors;
	public Mesh mesh;

	void Start() {
		mesh = GetComponent<MeshFilter>().mesh;
		vertices = mesh.vertices;
		colors = new Color[vertices.Length];

//		int i = 0;
//		while (i < vertices.Length) {
//			colors[i] = Color.Lerp(Color.red, Color.green, vertices[i].y);
//			i++;
//		}
//		mesh.colors = colors;
	}

	public void updateColor(Grid2D grid) {
		
	}
}