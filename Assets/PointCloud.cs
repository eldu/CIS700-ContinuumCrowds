using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PointCloud : MonoBehaviour {

	private Mesh mesh;
//	int numPoints = 60000;

	// Use this for initialization
	void Start () {
		mesh = new Mesh();

		GetComponent<MeshFilter>().mesh = mesh;
//		CreateMesh();
//	}
//
//	void CreateMesh() {
		int side = 100;
		float scale = 0.06f;
		int numIndicies = (side - 1) * (side - 1) * 6;
		int numVertices = side * side;

		Vector3[] points = new Vector3[numVertices];
		int[] indicies = new int[numIndicies];
		Color[] colors = new Color[numVertices];

		for (int i = 0; i < side; i++) {
			for (int j = 0; j < side; j++) {
				int idx = i * side + j;
				points [idx] = new Vector3 ((i - side / 2) * scale, (j - side / 2) * scale, 0);
				colors [idx] = new Color(Random.Range(0.0f,1.0f), Random.Range (0.0f,1.0f), Random.Range(0.0f,1.0f),1.0f);
			
			}
		}

//		for (int k = 0; k < side * side * 2; k += 2) {
//			indicies [k] = k / 2;
//			indicies [k + 1] = k / 2 + 1;
//		}
//
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

//		for (int k = 0; k < numIndicies; k += 6) {
//			int a = k / (side + 1);
//			int b = k - k * (side + 1); 
//
//			if (a < side + 1 && b < side + 1) {
//				indicies [k] = a * (side + 1) + b;
//				indicies [k + 1] = indicies [k] + 1;
//				indicies [k + 2] = indicies [k] + side + 1;
//				indicies [k + 3] = indicies [k] + 1;
//				indicies [k + 4] = indicies [k] + 2 + side;
//				indicies [k + 5] = indicies [k] + 1 + side;
//			}
//		}

		mesh.vertices = points;
		mesh.colors = colors;
//		mesh.triangles = indicies;
		mesh.SetIndices(indicies, MeshTopology.Triangles, 0);

		#if HELLO
		Vector3[] points = new Vector3[numPoints];
		int[] indicies = new int[numPoints];
		Color[] colors = new Color[numPoints];
		for(int i = 0; i < points.Length; i++) {
			points[i] = new Vector3(Random.Range(-10,10), Random.Range(-10,10), 0);
			indicies[i] = i;
			colors [i] = new Color (1.0f, 0.0f, 0.0f);
//			colors[i] = new Color(Random.Range(0.0f,1.0f),Random.Range (0.0f,1.0f),Random.Range(0.0f,1.0f),1.0f);
		}

		mesh.vertices = points;
		mesh.colors = colors;
		mesh.triangles = indicies;
//		mesh.SetIndices(indicies, MeshTopology.Points, 0);
//		mesh.RecalculateBounds();
		#endif
	}

//	int getpt(int i, int j) {
//		return i * side + j;
//	}

	void updateMesh(Grid2D g) {
		
	}
}