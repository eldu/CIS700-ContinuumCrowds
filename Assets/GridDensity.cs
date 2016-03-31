//#define ACTIVE
#if ACTIVE

using UnityEngine;
using System.Collections;

public class GridDensity {
	public float[] densities;
	public float[] averageVelocities;
	public float[] ux;
	public float[] uz;
	private int resx;
	private int resz;
	private float lamda = 2.0f; // density exponent

	public GridDensity(int resx, int resz, float cellWidth, float cellHeight) {
		this.resx = resx;
		this.resz = resz;
		densities = new float[resx * resz];
//		speeds = new float[resx * resz];
		ux = new float[(resx + 1) * resz];
		uz = new float[resx * (resz + 1)];
	}
		

	void updateDensity(Agent[] agents) {
		foreach (Agent a in agents) {
			Vector2 localpt = getLocalPoint (a.transform.position);
			addDensity (localpt);
		}
		clampDensity ();
	}


	void updateGrids(Agent[] agents) {
		foreach (Agent a in agents) {
			Vector2 localpt = getLocalPoint (a.transform.position);

			// the density field must be continuous with respect to the location of the people
			// each person should contribute no less than p to any neighboring cell//
			// and a minimum of p in their own cell to insure that each individual is
			// not affected by their own density
			// lamda is the density falloff
			// p = (0.5) ^ lambda

			Vector2 Aidx = getAVertex2(localpt);
			Vector2 Bidx = new Vector2 (Aidx [0], Aidx [1] + 1);
			Vector2 Cidx = new Vector2 (Aidx [0] + 1, Aidx [1] + 1);
			Vector2 Didx = new Vector2 (Aidx [0] + 1, Aidx [1]);

			Vector2 Acoord = getDensityCoordVector2(Aidx);

			float dx = localpt [0] - Acoord [0];
			float dy = localpt [1] - Acoord [1];

			int A = getIdxFromIJ (Aidx);
			int B = getIdxFromIJ (Bidx);
			int C = getIdxFromIJ (Cidx);
			int D = getIdxFromIJ (Didx);

			if (A > 0) {
				float p = Mathf.Pow (Mathf.Min (1 - dx, 1 - dy), lamda);
				densities [A] += p;
				averageVelocities [A] += p * a.GetComponent<Animator> ().velocity;

			}
			if (B > 0) {
				float p = Mathf.Pow (Mathf.Min (dx, 1 - dy), lamda);
				densities [B] += p;
				averageVelocities [B] += p * a.GetComponent<Animator> ().velocity;
			}
			if (C > 0) {
				float p = Mathf.Pow (Mathf.Min (dx, dy), lamda);
				densities [C] += p;
				averageVelocities [C] += p * a.GetComponent<Animator> ().velocity;
			}
			if (D > 0) {
				float p = Mathf.Pow (Mathf.Min (1 - dx, dy), lamda);
				densities [D] += p;
				averageVelocities [D] += p * a.GetComponent<Animator> ().velocity;
			}
		}

		for (int i = 0; i < resx * resz; i++) {
			averageVelocities [i] /= densities [i];
		}
	}

	// Get the coordinate of the centroid of the density grid
	Vector2 getDensityCoord(int idx) {
		// Bottom left corner of grid cell
		int i = idx / resx;
		int j = idx - (i * resx);

		return new Vector3 (i + 0.5f, j + 0.5f); 
	}

	// Get the index of the density grid from a point on the local grid
	int getDensityIdx(Vector2 p) {
		int i = (int) (p[0] / resx);
		int j = (int) (p[1] / resz);

		if (i < 0 || j < 0) {
			return -1;
		}

		return resx * i + j;
	}

	Vector2 getDensityIdxVector2(Vector2 p) {
		int i = (int) (p[0] / resx);
		int j = (int) (p[1] / resz);

		return new Vector2 (i, j);
	}

	Vector2 getAVertex2(Vector2 p) {
		Vector2 idx = getDensityIdxVector2 (p);
		Vector2 c1 = getDensityCoordVector2(idx);
		if (p [0] >= c1 [0]) {
			if (p [1] >= c1 [1]) {
				// Cell A, bottom left
				return idx;
			} else {
				// Cell D, top left
				return new Vector2(idx[0] - 1, idx[1]);
			}
		} else {
			if (p [1] >= c1 [1]) {
				// Cell B, bottom right
				return new Vector2(idx[0], idx[1] - 1);
			} else {
				// Cell C, top right
				return new Vector2(idx[0] - 1, idx[1] - 1);
			}
		}
	}

	Vector2 getDensityCoordVector2(Vector2 idx) {
		return new Vector3 (idx[0] + 0.5f, idx[1] + 0.5f); 
	}

	int getIdxFromIJ(Vector2 idx) {
		return resx * (int) idx [0] + (int) idx [1];
	}

	public void addDensity(Vector2 localpt) {
		// the density field must be continuous with respect to the location of the people
		// each person should contribute no less than p to any neighboring cell//
		// and a minimum of p in their own cell to insure that each individual is
		// not affected by their own density
		// lamda is the density falloff
		// p = (0.5) ^ lambda

		Vector2 Aidx = getAVertex2(localpt);
		Vector2 Bidx = new Vector2 (Aidx [0], Aidx [1] + 1);
		Vector2 Cidx = new Vector2 (Aidx [0] + 1, Aidx [1] + 1);
		Vector2 Didx = new Vector2 (Aidx [0] + 1, Aidx [1]);

		Vector2 Acoord = getDensityCoordVector2(Aidx);

		float dx = localpt [0] - Acoord [0];
		float dy = localpt [1] - Acoord [1];

		int A = getIdxFromIJ (Aidx);
		int B = getIdxFromIJ (Bidx);
		int C = getIdxFromIJ (Cidx);
		int D = getIdxFromIJ (Didx);

		if (A > 0) {
			float p = Mathf.Pow (Mathf.Min (1 - dx, 1 - dy), lamda);
			densities [A] += p;

		}
		if (B > 0) {
			float p = Mathf.Pow (Mathf.Min (dx, 1 - dy), lamda);
			densities [B] += p;
		}
		if (C > 0) {
			float p = Mathf.Pow (Mathf.Min (dx, dy), lamda);
			densities [C] += p;
		}
		if (D > 0) {
			float p = Mathf.Pow (Mathf.Min (1 - dx, dy), lamda);
			densities [D] += p;
		}
	}

	// Clamp the values of the density grid
	public void clampDensity() {
		for (int i = 0; i < densities.Length; i++) {
			densities[i] = Mathf.Clamp01 (densities[i]);
		}
	}


	Vector2 getLocalPoint(Vector3 p) {
		// Calibrate such that the min is (0, 0)
		Vector2 result = new Vector2(p[0] - min[0], p[1] - min[2]);

		// Divide by cellWidth and cellHeight
		result[0] /= cellWidth;
		result[1] /= cellHeight;

		return result;
	}
}

// TODO: Clean up scraps
// TODO: Fix these edge cases
//int getA(Vector2 p) {
//	int idx = getDensityIdx (p);
//	Vector2 c1 = getDensityCoord(idx);
//	if (p [0] >= c1 [0]) {
//		if (p [1] >= c1 [1]) {
//			// Cell A, bottom left
//			return idx;
//		} else {
//			// Cell D, top left
//			return idx - resx;
//		}
//	} else {
//		if (p [1] >= c1 [1]) {
//			// Cell B, bottom right
//			return idx - 1;
//		} else {
//			// Cell C, top right
//			return idx - resx - 1;
//		}
//	}
//}

#endif