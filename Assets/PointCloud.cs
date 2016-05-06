using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PointCloud : MonoBehaviour {
	Mesh mesh;

	Vector3[] points;
	Color[] colors;
	int[] indicies;

	// Use this for initialization
	void Start () {
		mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;

		// Create Mesh
		int side = 20;
		float scale = 0.05f;
		int numIndicies = (side - 1) * (side - 1) * 6;
		int numVertices = side * side;

		points = new Vector3[numVertices];
		colors = new Color[numVertices];
		indicies = new int[numIndicies];

		for (int i = 0; i < side; i++) {
			for (int j = 0; j < side; j++) {
				int idx = i * side + j;
				points [idx] = new Vector3 ((i - side / 2) * scale, (j - side / 2) * scale, 0);
				colors [idx] = new Color(Random.Range(0.0f,1.0f), Random.Range (0.0f,1.0f), Random.Range(0.0f,1.0f),1.0f);
//				colors [idx] = new Color(1, 0, 0, 1);
			}
		}

		for (int k = 0; k < numIndicies; k += 6) {
			int i = k / (6 * (side - 1));
			int j = k / 6 - i * (side - 1);

			if (i < side && j < side) {
				indicies [k] = i * side + j;
				indicies [k + 1] = indicies [k] + 1;
				indicies [k + 2] = indicies [k] + side;
				indicies [k + 3] = indicies [k] + 1;
				indicies [k + 4] = indicies [k] + 1 + side;
				indicies [k + 5] = indicies [k] + side;
			}
		}


		mesh.vertices = points;
		mesh.colors = colors;
		mesh.SetIndices(indicies, MeshTopology.Triangles, 0);
	}

	public void updateMesh(MACGrid mGrid) {
		for (int i = 0; i < colors.Length; i++) {
			// local point
			Vector2 lp = mGrid.getLocalPoint(mesh.vertices[i]);
			float density = mGrid.getDensity (lp);

			colors [i] = new Color (0, 1, density, 1);
		}

		mesh.colors = colors;
	}
}