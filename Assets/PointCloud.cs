
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
		float scale = 5.0f;
		int numIndicies = (side - 1) * (side - 1) * 6;
		int numVertices = side * side;

		points = new Vector3[numVertices];
		colors = new Color[numVertices];
		indicies = new int[numIndicies];

		for (int i = 0; i < side; i++) {
			for (int j = 0; j < side; j++) {
				int idx = i * side + j;
//				points[idx] = new Vector3 (i * scale - 50.0f, 0, j * scale - 50.0f);
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

			Vector2 world = new Vector2 (mesh.vertices [i].x - 0.5f, mesh.vertices [i].y - 0.5f); // TODO: Minus 0.5f not necessary
			Vector2 lp = mGrid.getLocalPoint(world);

			float value = mGrid.getDensity (lp);
//			float value = mGrid.getAverageVelocityZ(lp) / 4;
//			float value = mGrid.getAverageVelocityX (lp);
//			float value = mGrid.getCost (lp) / 9.0f;
//			float value = mGrid.getPotential(lp) / 17.0f;

			if (value == Mathf.Infinity) {
				value = 1;
				colors [i] = new Color (0, 0, value, 1);
			} else {
				colors [i] = new Color (value, value, value, 1);
			}

//			if (mGrid.getDensity (lp) > 2.0) {
//				colors [i] = new Color (0, 1, 0, 1);
//			}
		}

//		for (int i = 0; i < colors.Length; i++) {
//			colors [i] = new Color (1, 1, 1);
//		}

		// Orange, finding relevant point.
//		colors [92] = new Color (1, 1, 0);
//		colors [152] = new Color (1, 1, 0);

		// Green, testing outside of goal boundaries
//		colors [168] = new Color (0, 1, 0);
//		colors [231] = new Color (0, 1, 0);
//	
		// RED, Initallaly have density in them.
//		colors [209] = new Color (1, 0, 0, 1);
//		colors [210] = new Color (1, 0, 0, 1);
//		colors [229] = new Color (1, 0, 0, 1);
//		colors [230] = new Color (1, 0, 0, 1);

		mesh.colors = colors;
	}
}