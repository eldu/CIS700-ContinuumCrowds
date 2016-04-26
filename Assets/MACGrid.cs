using UnityEngine;
using System.Collections;


public class MACGrid {
	public Vector2 min;
	public Vector2 max;
	public Vector2 resolution;

	private int resx;
	private int resz;

	public float cellWidth;

	// Goal
	public BoxCollider box;
	Vector3 box_min;
	Vector3 box_max;

	// Initalize all of the grids
//	public Grid2D gridU; // X Velocity Grid
//	public Grid2D gridV; // Z Velocity Grid
//	public Grid2D gridW;
//	public Grid2D gridS;
	public Grid2D gridPotential;
	public Grid2D gridD; // Density Grid
//	public Grid2D gridCU; // Cost in x direction
//	public Grid2D gridCV; // Cost in z direction
	public Grid2D marker; // 0 = empty, 1 = obstacle
	public Grid2D[] gridRose = new Grid2D[2]; // Velocity
	public Grid2D[] gridCost = new Grid2D[2]; 
	public Grid2D gradU;
	public Grid2D gradV;
	public Vector2[] gridAverageVelocity;

	// Cardinal Directions
	Vector2[] n = { new Vector2 (1, 0), new Vector2 (0, 1)};
//		, new Vector2 (-1, 0), new Vector2 (0, -1) };


//	Vector3[] gridAveVelocity;

	// Tinkering Parameters
	public float lamda = 2.0f; // Density

	public float MAX_SPEED = 2.5f; // m/s
	public float MIN_SPEED = 0.5f; // m/s

	public float MIN_DENSITY = 0.1f;
//	private float density = (0.5f) ^ lamda;
	public float MAX_DENSITY = 0.8f;

	public MACGrid (Vector2 min, Vector2 max, Vector2 resolution, BoxCollider goal) {
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

		// Set up Goal
		this.box = goal;
		Vector3 box_min = getLocalPoint(box.center - 0.5f * box.size);
		Vector3 box_max = getLocalPoint(box.center + 0.5f * box.size);

		// Initialization of Grids
		//		gridU = new Grid2D(resx + 1, resz);
		//		gridV = new Grid2D(resx, resz + 1);
		//		gridW = new Grid2D (resx, resz);
		//		gridS = new Grid2D (resx, resz);
		gridD = new Grid2D(resx, resz);
		//		gridCU = new Grid2D(resx, resz);
		//		gridCV = new Grid2D(resx, resz);
		gridAverageVelocity = new Vector2[resx * resz];
		gridPotential = new Grid2D (resx, resz);

		gradU = new Grid2D (resx + 1, resz);
		gradV = new Grid2D (resx, resz + 1);


		// TODO: Change gridRose and gridCost
//		for (int i = 0; i < 2; i++) {
			gridRose [0] = new Grid2D (resx + 1, resz);
			gridCost [0] = new Grid2D (resx + 1, resz);
			gridRose [0] = new Grid2D (resx, resz + 1);
			gridCost [0] = new Grid2D (resx, resz + 1);
//		}
	}

	public Vector2 getLocalPoint(Vector3 world) {
		float x = (world.x - min.x) / cellWidth;
		float y = (world.z - min.y) / cellWidth;
		return new Vector2 (x, y);
	}

	// Clears all of the grids
	public void clear() {
//		gridU = new Grid2D(resx + 1, resz);
//		gridV = new Grid2D(resx, resz + 1);
//		gridW = new Grid2D (resx, resz);
//		gridS = new Grid2D (resx, resz);
		gridD.clear();
//		gridCU = new Grid2D(resx, resz);
//		gridCV = new Grid2D(resx, resz);
		gridAverageVelocity = new Vector2[resx * resz];
		for (int i = 0; i < resx * resz; i++) {
			gridAverageVelocity [i] = new Vector2 (0, 0); // Sets to 0.
		}

		gridPotential = new Grid2D (resx, resz);

		for (int i = 0; i < 2; i++) {
			gridRose [0] = new Grid2D (resx + 1, resz);
			gridCost [0] = new Grid2D (resx + 1, resz);
			gridRose [1] = new Grid2D (resx, resz + 1);
			gridCost [1] = new Grid2D (resx, resz + 1);
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

			Vector3 bodyPosition = a.getWorldPosition();
			Vector2 localpt = getLocalPoint (new Vector2 (bodyPosition [0], bodyPosition [2]));

			Vector2 Aidx = gridD.getA(localpt);
			Vector2 Bidx = new Vector2 (Aidx [0], Aidx [1] + 1);
			Vector2 Cidx = new Vector2 (Aidx [0] + 1, Aidx [1] + 1);
			Vector2 Didx = new Vector2 (Aidx [0] + 1, Aidx [1]);

			Vector2 Acoord = gridD.getCoordVector2(Aidx);

			float dx = localpt [0] - Acoord [0];
			float dy = localpt [1] - Acoord [1];

			int A = gridD.convertIdx (Aidx);
			int B = gridD.convertIdx (Bidx);
			int C = gridD.convertIdx (Cidx);
			int D = gridD.convertIdx (Didx);

			if (A > 0) {
				float p = Mathf.Pow (Mathf.Min (1 - dx, 1 - dy), lamda);
				gridD.add(A, p);

				gridAverageVelocity[A] += new Vector2 (a.GetComponent<CharacterController> ().velocity.x / p, a.GetComponent<CharacterController> ().velocity.y / p);

			}
			if (B > 0) {
				float p = Mathf.Pow (Mathf.Min (dx, 1 - dy), lamda);
				gridD.add(B, p);
				gridAverageVelocity[B] += new Vector2 (a.GetComponent<CharacterController> ().velocity.x / p, a.GetComponent<CharacterController> ().velocity.y / p);
			}
			if (C > 0) {
				float p = Mathf.Pow (Mathf.Min (dx, dy), lamda);
				gridD.add(C, p);
				gridAverageVelocity[C] += new Vector2 (a.GetComponent<CharacterController> ().velocity.x / p, a.GetComponent<CharacterController> ().velocity.y / p);
			}
			if (D > 0) {
				float p = Mathf.Pow (Mathf.Min (1 - dx, dy), lamda);
				gridD.add(D, p);
				gridAverageVelocity[D] += new Vector2 (a.GetComponent<CharacterController> ().velocity.x / p, a.GetComponent<CharacterController> ().velocity.y / p);
			}

			// Divide the speed by the density
			for (int i = 0; i < resx * resz; i++) {
				gridAverageVelocity[i] *= 1/gridD.get(i);
			}
		}
	}

	public float getDensity(Vector2 localpt) {
		int idx = gridD.getIdx (localpt);

		// Boundary Conditions
		if (idx < 0) {
			return MAX_DENSITY + 1;
		} else {
			return gridD.get (idx);
		}
	}

	public Vector2 getAverageVelocity(Vector2 localpt) {
		int idx = gridD.getIdx (localpt);
		if (idx < 0) {
			return new Vector2(0f, 0f); 
		} else {
			return gridAverageVelocity[idx];
		}
	}

	// Helper function for UpdateVelocityFields
	public float getSpeed (Vector2 localpt, Vector2 n) {
//		Vector2 localpt = getLocalPoint (a.getWorldPosition ());
//		int idx = gridD.getIdx (localpt);
//		float r = a.radius / cellWidth;
		float r = 0.5f;
//		Vector2 n = a.getNormal ();
		float p = getDensity(localpt);

		Vector2 localstep = localpt + r * n; // 
		float density = getDensity (localstep);

		// TODO: Check boundary conditions
		// TODO: Incorporate terrain heightfield

		float fx;
		float ft = MAX_SPEED; // Topological Speed, Ignore Terrain
		float fv = Vector2.Dot(getAverageVelocity(localstep), n); // flow speed

		if (p < MIN_DENSITY) {
			// Low density
			fx = ft;
		} else if (p < MAX_DENSITY) {
			// Middle density
			fx = ft + (density - MIN_DENSITY) / (MAX_DENSITY - MIN_DENSITY) * (fv - ft);
		} else {
			// High Density
			fx = fv;
		}

		return fx;
	}

	// Unit Cost Field
	public float PATH_LENGTH_WEIGHT = 0.8f;
	public float TIME_WEIGHT = 0.8f;
	public float DISCOMFORT = 0.8f;
	public float DISCOMFORT_WEIGHT = 0.8f;

	public void UpdateVelocityAndCostFields() {
		for (int i = 0; i < resx; i++) {
			for (int j = 0; j < resz; j++) {
				Vector2 ij = new Vector2 (i, j);
				Vector2[] directions = getDirections (ij);

				for (int k = 0; k < directions.Length; k++) {
					// fm --> i
					float fu = getSpeed (directions [k], n [k]); 
					gridRose [k].set (ij, fu); // 

					// cm --> i
					float cu = (PATH_LENGTH_WEIGHT * fu + TIME_WEIGHT + DISCOMFORT_WEIGHT * distance (directions[k])) / fu;
					gridCost [k].set (ij, cu);
				}

//				Vector2 eij = new Vector2 (i + 0.5f, j);
//				Vector2 nij = new Vector2 (i, j + 0.5f);
//				Vector2 wij = new Vector2 (i - 0.5f, j);
//				Vector2 sij = new Vector2 (i - 0.5f, j - 0.5f);
//
//				float fu = getVelocity (eij, new Vector2 (1f, 0f));
//				gridU.setVal(ij, fu);
//				float cu = (PATH_LENGTH_WEIGHT * fu + TIME_WEIGHT + DISCOMFORT_WEIGHT * distance (eij)) / fu;
//				gridCU.setVal (ij, cu);
//
//				float fv = getVelocity (nij, new Vector2 (0f, 1f));
//				gridV.setVal(ij, fv);
//				float cv = (PATH_LENGTH_WEIGHT * fu + TIME_WEIGHT + DISCOMFORT_WEIGHT * distance (nij)) / fv;
//				gridCV.setVal (ij, cv);
//
//				float fw = getVelocity (wij, new Vector2 (-1f, 0f));
//				gridW.setVal (ij, fw);
//				float cw = (PATH_LENGTH_WEIGHT * fu + TIME_WEIGHT + DISCOMFORT_WEIGHT * distance (wij)) / fw;
//				gridCV.setVal (ij, cw);
//
//				float fs = getVelocity (sij, new Vector2 (0f, -1f));
//				gridS.setVal(ij, fs);
//				float cs = (PATH_LENGTH_WEIGHT * fu + TIME_WEIGHT + DISCOMFORT_WEIGHT * distance (sij)) / fs;
//				gridCV.setVal (ij, cs);
			}
		}
	}

	Vector2[] getDirections(Vector2 ij) {
		Vector2[] result = new Vector2[2];
		// Counterclockwise
		result [0] = new Vector2 (ij [0] + 0.5f, ij [1]       ); // East
		result [1] = new Vector2 (ij [0]       , ij [1] + 0.5f); // North
//		result [2] = new Vector2 (ij [0] - 0.5f, ij [1]       ); // West
//		result [3] = new Vector2 (ij [0]       , ij [1] - 0.5f); // South
		return result;
	}

	// Discomfort From Goal From Any Point
	// Local point
	float distance(Vector2 pos) {
		float dx = Mathf.Max(box_min.x - pos[0], 0, pos[0] - box_max.x);
		float dy = Mathf.Max(box_min.z - pos[1], 0, pos[1] - box_max.z);
		return Mathf.Sqrt(dx*dx + dy*dy);
	}


	bool isknown(int i, int j) {
		return Mathf.Equals(marker.get (i, j), 1); // Known
	}




}
