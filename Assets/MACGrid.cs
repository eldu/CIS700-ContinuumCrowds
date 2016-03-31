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
	Grid2D gridCU; // Cost in x direction
	Grid2D gridCV; // Cost in z direction
	Grid2D[] gridRose = new Grid2D[4];
	Vector2[] n = { new Vector2 (1, 0), new Vector2 (0, 1), new Vector2 (-1, 0), new Vector2 (0, -1) };
	Vector3[] gridAveVelocity;

	// Tinkering Parameters
	private float lamda = 2.0f; // Density

	public float MAX_SPEED = 2.5f; // m/s
	public float MIN_SPEED = 0.5f; // m/s

	public float MIN_DENSITY = 0.1f;
//	private float density = (0.5f) ^ lamda;
	public float MAX_DENSITY = 0.8f;

	public MACGrid (Vector2 min, Vector2 max, Vector2 resolution) {
//		TODO: Grab components form the transform component to set min and max
//		Vector3 origin = GetComponent<Transform> ().Translate;
//		Vector3 scale = GetComponent<Transform> ().localScale; // Because the quad is rotated, the original xy plane now lies on the world xz plane

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
		gridCU = new Grid2D(resx, resz);
		gridCV = new Grid2D(resx, resz);
		gridAveVelocity = new Vector3[resx * resz];

		for (int i = 0; i < 4; i++) {
			gridRose [i] = new Grid2D (resx, resz);
		}
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
				gridAveVelocity[A] += a.GetComponent<CharacterController> ().velocity / p;

			}
			if (B > 0) {
				float p = Mathf.Pow (Mathf.Min (dx, 1 - dy), lamda);
				gridD.addVal(B, p);
				gridAveVelocity[B] += a.GetComponent<CharacterController> ().velocity / p;
			}
			if (C > 0) {
				float p = Mathf.Pow (Mathf.Min (dx, dy), lamda);
				gridD.addVal(C, p);
				gridAveVelocity[C] += a.GetComponent<CharacterController> ().velocity / p;
			}
			if (D > 0) {
				float p = Mathf.Pow (Mathf.Min (1 - dx, dy), lamda);
				gridD.addVal(D, p);
				gridAveVelocity[D] += a.GetComponent<CharacterController> ().velocity / p;
			}

			// Divide the speed by the density
			for (int i = 0; i < resx * resz; i++) {
				gridAveVelocity [i] /= gridD.getVal(i);
			}
		}
	}

	public float getDensity(Vector2 localpt) {
//		Vector2 localpt = getLocalPoint (worldpt);
		return gridD.getVal (gridD.getIdxFromPos (localpt));
	}

	public Vector2 getAverageVelocity(Vector2 localpt) {
//		Vector2 localpt = getLocalPoint (worldpt);
		return gridAveVelocity[gridD.getIdxFromPos (localpt)];
	}

	// Helper function for UpdateVelocityFields
	public float getVelocity (Vector2 uij, Vector2 un) {
		// TODO: world radius = 0.5;
		float r = 0.5f / resx; // TODO: Currently only support square macgrids

		float p = getDensity (uij);
		Vector2 localstep = uij + r * un;
		Vector2 v_xrn = getAverageVelocity (localstep);
		float p_xrn = getDensity (localstep);

		// TODO: Incorporate terrain heightfield
		float fx;
		float ft = MAX_SPEED; // Topological Speed, Ignore Terrain
		float fv = Vector2.Dot(v_xrn,  un); // flow speed

		if (p < MIN_DENSITY) {
			// Low density
			fx = ft;
		} else if (p < MAX_DENSITY) {
			// Middle density
			fx = ft + (p_xrn - MIN_DENSITY) / (MAX_DENSITY - MIN_DENSITY) * (fv - ft);
		} else {
			// High Density
			fx = fv;
		}

		return fx;
	}

	public void UpdateVelocityFields() {
		for (int i = 0; i < resx; i++) {
			for (int j = 0; j < resz; j++) {
				for (int d = 0; d < 4; d++) {
					Vector2 ij = new Vector2 (i, j);
					Vector2 eij = new Vector2 (i + 0.5f, j);
					Vector2 nij = new Vector2 (i, j + 0.5f);
					Vector2 wij = new Vector2 (i - 0.5f, j);
					Vector2 sij = new Vector2 (i - 0.5f, j - 0.5f);

					gridRose[0].setVal(ij, getVelocity (eij, new Vector2 (1f, 0f)));
					gridRose[1].setVal(ij, getVelocity (nij, new Vector2 (0f, 1f)));
					gridRose[2].setVal(ij, getVelocity (wij, new Vector2 (-1f, 0f)));
					gridRose[3].setVal(ij, getVelocity (sij, new Vector2 (0f, -1f)));
				}
			}
		}
	}

//	public void UpdateVelocityFields() {
//		for (int i = 0; i < resx; i++) {
//			for (int j = 0; j < resz; j++) {
//				// TODO: world radius = 0.5;
//				float r = 0.5f / resx; // TODO: Currently only support square macgrids
//
//				// Location of u
//				Vector2 uij = new Vector2 (i + 0.5f, j);
//				Vector2 un = new Vector2 (1, 0);
//
//				float p = getDensity (uij);
//				Vector2 localstep = uij + r * un;
//				Vector2 v_xrn = getAverageVelocity (localstep);
//				float p_xrn = getDensity (localstep);
//
//				// TODO: Incorporate terrain heightfield
//				float fx;
//				float ft = MAX_SPEED; // Topological Speed, Ignore Terrain
//				float fv = Vector2.Dot(v_xrn,  un); // flow speed
//
//				if (p < MIN_DENSITY) {
//					// Low density
//					fx = ft;
//				} else if (p < MAX_DENSITY) {
//					// Middle density
//					fx = ft + (p_xrn - MIN_DENSITY) / (MAX_DENSITY - MIN_DENSITY) * (fv - ft);
//				} else {
//					// High Density
//					fx = fv;
//				}
//
//				gridU.setVal (uij, fx);
//					
//				// Location of v
//				Vector2 vij = new Vector2 (i, j + 0.5f);
//				Vector2 vn = new Vector2 (0f, 1f);
//
//				p = getDensity (vij);
//				localstep = vij + r * vn;
//				v_xrn = getAverageVelocity (localstep);
//				p_xrn = getDensity (localstep);
//
//				fv = Vector2.Dot(v_xrn,  vn); // flow speed
//
//				if (p < MIN_DENSITY) {
//					// Low density
//					gridV.setVal(uij, ft);
//				} else if (p < MAX_DENSITY) {
//					// Middle density
//					gridV.setVal(uij, ft + (p_xrn - MIN_DENSITY) / (MAX_DENSITY - MIN_DENSITY) * (fv - ft));
//				} else {
//					// High Density
//					gridV.setVal(uij, fv);
//				}
//			}
//		}
//	}

//	public void updateVelocityFields() {
//		
//	}

//	// Clamp the values of the density grid
//	public void clampDensity() {
//		for (int i = 0; i < densities.Length; i++) {
//			densities[i] = Mathf.Clamp01 (densities[i]);
//		}
//	}

}
