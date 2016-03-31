using UnityEngine;
using System.Collections;


public class MACGrid : MonoBehaviour {
	public Vector2 min;
	public Vector2 max;
	public Vector2 resolution;

	int resx;
	int resz;

	float cellWidth;

	// Initalize all of the grids
	Grid2D gridU; // X Velocity Grid
	Grid2D gridV; // Z Velocity Grid
	Grid2D gridD; // Density Grid
	Grid2D gridCostLR; // Cost from going left to right
	Grid2D gridCostRL; // Cost from going right to left
	Vector3[] gridAveVelocity;

	// Tinkering Parameters
	public float lamda = 2.0f; // Density

	// Weights for Cost Function
	// Unit Cost Field
	public float PATH_LENGTH_COEFF;
	public float TIME_COEFF;
	public float DISCOMFORT_COEFF;


	public MACGrid (Vector2 min, Vector2 max, Vector2 resolution) {
		this.min = min;
		this.max = max;
		Vector2 dimensions = max - min;

		resx = (int) resolution [0];
		resz = (int) resolution [1];

		Vector2 cellsize = new Vector2 (dimensions [0] / resx, dimensions [1] / resz);

		if (Mathf.Approximately(cellsize[0], cellsize[1])) {
			// You gave me great parameters
			cellWidth = cellsize[0];
		} else {
			// You messed up so I need to fix your damned mess
			// Readjust maximum accordingly
			cellWidth = Mathf.Max(cellsize[0], cellsize[1]);
			max = min + cellWidth * resolution;
		}

		clear ();
	}

	public Vector2 getLocalPoint(Vector2 world) {
		return (world - min) / cellWidth;
	}

	// Clears all of the grids
	public void clear() {
		gridU = new Grid2D(resx + 1, resz);
		gridV = new Grid2D(resx, resz + 1);
		gridD = new Grid2D(resx, resz);
		gridCostLR = new Grid2D(resx, resz);
		gridCostRL = new Grid2D(resx, resz);
		gridAveVelocity = new Vector3[resx * resz];
	}

	public void splat(Agent[] agents) {
		foreach (Agent a in agents) {
			// the density field must be continuous with respect to the location of the people
			// each person should contribute no less than p to any neighboring cell//
			// and a minimum of p in their own cell to insure that each individual is
			// not affected by their own density
			// lamda is the density falloff
			// p = (0.5) ^ lambda
			Vector3 bodyPosition = a.GetComponent<Animator>().bodyPosition;
			Vector2 localpt = getLocalPoint (new Vector2 (bodyPosition [0], bodyPosition [2]));

			Vector2 Aidx = gridD.getA(localpt);
			Vector2 Bidx = new Vector2 (Aidx [0], Aidx [1] + 1);
			Vector2 Cidx = new Vector2 (Aidx [0] + 1, Aidx [1] + 1);
			Vector2 Didx = new Vector2 (Aidx [0] + 1, Aidx [1]);

			Vector2 Acoord = gridD.getCoordVector2(Aidx);

			float dx = localpt [0] - Acoord [0];
			float dy = localpt [1] - Acoord [1];

			int A = gridD.getIdxFromIdx (Aidx);
			int B = gridD.getIdxFromIdx (Bidx);
			int C = gridD.getIdxFromIdx (Cidx);
			int D = gridD.getIdxFromIdx (Didx);

			if (A > 0) {
				float p = Mathf.Pow (Mathf.Min (1 - dx, 1 - dy), lamda);
				gridD.addVal(A, p);
				gridAveVelocity[A] += a.GetComponent<Animator> ().velocity / p;

			}
			if (B > 0) {
				float p = Mathf.Pow (Mathf.Min (dx, 1 - dy), lamda);
				gridD.addVal(B, p);
				gridAveVelocity[B] += a.GetComponent<Animator> ().velocity / p;
			}
			if (C > 0) {
				float p = Mathf.Pow (Mathf.Min (dx, dy), lamda);
				gridD.addVal(C, p);
				gridAveVelocity[C] += a.GetComponent<Animator> ().velocity / p;
			}
			if (D > 0) {
				float p = Mathf.Pow (Mathf.Min (1 - dx, dy), lamda);
				gridD.addVal(D, p);
				gridAveVelocity[D] += a.GetComponent<Animator> ().velocity / p;
			}
		}
	}

//	// Clamp the values of the density grid
//	public void clampDensity() {
//		for (int i = 0; i < densities.Length; i++) {
//			densities[i] = Mathf.Clamp01 (densities[i]);
//		}
//	}

}
