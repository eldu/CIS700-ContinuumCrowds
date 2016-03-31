using UnityEngine;
using System.Collections;

public class MeshColorer : MonoBehaviour {
	void Start() {
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;
		Color[] colors = new Color[vertices.Length];

		int i = 0;
		while (i < vertices.Length) {
			colors[i] = Color.Lerp(Color.red, Color.green, vertices[i].y);
			i++;
		}
		mesh.colors = colors;
	}
}