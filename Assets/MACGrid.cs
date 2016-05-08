using UnityEngine;
using System.Collections;
using System;


public class MACGrid {
	// private static variables
	private static int KNOWN = 1;
	private static int UNKNOWN = 0;

	public Vector2 min;
	public Vector2 max;
	public Vector2 resolution;

	private int resx;
	private int resz;

	public float cellWidth;

	// Goal
	public BoxCollider box;
	public Vector2 box_min;
	public Vector2 box_max;
	int goal_minx;
	int goal_maxx;
	int goal_miny;	// Initalize all of the grids
	int goal_maxy;

	public Grid2D<float> gridPotential;
	public Grid2D<float> gridD; // Density Grid
	public Grid2D<int> marker; // 0 = empty, 1 = obstacle
	public Grid2D<float>[] gridRose = new Grid2D<float>[4]; // Velocity
	public Grid2D<float>[] gridCost = new Grid2D<float>[4]; 
	public Grid2D<float> gradU;
	public Grid2D<float> gradV;
	public Grid2D<Vector2> gridAverageVelocity;

	// Cardinal Directions
	Vector2[] n = { new Vector2 (1, 0), new Vector2 (0, 1), new Vector2 (-1, 0), new Vector2 (0, -1) };

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
		box_min = getLocalPoint(box.GetComponent<Transform>().position - 0.5f * box.GetComponent<Transform>().localScale);
		box_max = getLocalPoint(box.GetComponent<Transform>().position + 0.5f * box.GetComponent<Transform>().localScale);
		goal_minx = marker.getIdxVec2 (box_min) [0];
		goal_miny = marker.getIdxVec2 (box_min) [1];
		goal_maxx = marker.getIdxVec2 (box_max) [0];
		goal_maxy = marker.getIdxVec2 (box_max) [1];

		// Initialization of Grids
		gridD = new Grid2D<float>(resx, resz);
		gridAverageVelocity = new Grid2D<Vector2>(resx, resz);
		gridPotential = new Grid2D<float> (resx, resz);

		gradU = new Grid2D<float> (resx + 1, resz);
		gradV = new Grid2D<float> (resx, resz + 1);
		marker = new Grid2D<int> (resx, resz);

		gridRose [0] = new Grid2D<float> (resx + 1, resz);
		gridCost [0] = new Grid2D<float> (resx + 1, resz);
		gridRose [1] = new Grid2D<float> (resx, resz + 1);
		gridCost [1] = new Grid2D<float> (resx, resz + 1);
		gridRose [2] = new Grid2D<float> (resx + 1, resz);
		gridCost [2] = new Grid2D<float> (resx + 1, resz);
		gridRose [3] = new Grid2D<float> (resx, resz + 1);
		gridCost [3] = new Grid2D<float> (resx, resz + 1);
	}

	public Vector2 getLocalPoint(Vector3 world) {
		float x = (world.x - min.x) / cellWidth;
		float y = (world.z - min.y) / cellWidth;
		return new Vector2 (x, y);
	}

	public Vector2 getLocalPoint(Vector2 world) {
		float x = (world.x - min.x) / cellWidth;
		float y = (world.y - min.y) / cellWidth;
		return new Vector2 (x, y);
	}

	// Clears all of the grids
	public void clear() {
		gridD.clear();
		marker.clear ();
		gridAverageVelocity.clear();
		gridPotential.clear ();
		gridRose [0] = new Grid2D<float> (resx + 1, resz);
		gridCost [0] = new Grid2D<float> (resx + 1, resz);
		gridRose [1] = new Grid2D<float> (resx, resz + 1);
		gridCost [1] = new Grid2D<float> (resx, resz + 1);
		gridRose [2] = new Grid2D<float> (resx + 1, resz);
		gridCost [2] = new Grid2D<float> (resx + 1, resz);
		gridRose [3] = new Grid2D<float> (resx, resz + 1);
		gridCost [3] = new Grid2D<float> (resx, resz + 1);
	}

	public void splat(Agent[] agents) {
		foreach (Agent a in agents) {
			// the density field must be continuous with respect to the location of the people
			// each person should contribute no less than p to any neighboring cell//
			// and a minimum of p in their own cell to insure that each individual is
			// not affected by their own density
			// lamda is the density falloff
			// p = (0.5) ^ lambda
			Rigidbody rb = a.GetComponent<Rigidbody> ();

			Vector2 bodyPosition = a.getWorldPosition();
			Vector2 localpt = getLocalPoint (bodyPosition);

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
				gridD.set(A, gridD.get(A) + p);
				gridAverageVelocity.set(A, gridAverageVelocity.get(A) + new Vector2 (rb.velocity.x / p, rb.velocity.y / p));

			}
			if (B > 0) {
				float p = Mathf.Pow (Mathf.Min (dx, 1 - dy), lamda);
				gridD.set(B, gridD.get(B) + p);
				gridAverageVelocity.set(B, gridAverageVelocity.get(B) + new Vector2 (rb.velocity.x / p, rb.velocity.y / p));
			}
			if (C > 0) {
				float p = Mathf.Pow (Mathf.Min (dx, dy), lamda);
				gridD.set(C, gridD.get(C) + p);
				gridAverageVelocity.set(C, gridAverageVelocity.get(C) + new Vector2 (rb.velocity.x / p, rb.velocity.y / p));
			}
			if (D > 0) {
				float p = Mathf.Pow (Mathf.Min (1 - dx, dy), lamda);
				gridD.set(D, gridD.get(D) + p);
				gridAverageVelocity.set(D, gridAverageVelocity.get(D) + new Vector2 (rb.velocity.x / p, rb.velocity.y / p));
			}

			// Divide the speed by the density
			for (int i = 0; i < resx * resz; i++) {
				if (gridD.get (i) > 0) {
					gridAverageVelocity.set(i, gridAverageVelocity.get(i) * 1 / gridD.get (i));
				}
			}
		}
	}


	// TODO: THESE GETS ARE SO FRIVOLOUS. SERIOUSLY REFACTOR TO ONE FUNCTION. WOULD DO, BUT 3AM THOUGH.
	public float getCost(Vector2 localpt) {
		int idx = gridCost[0].getIdx (localpt);

		// Boundary Conditions
		if (idx < 0) {
			return MAX_DENSITY + 1; // LOL
		} else {
			return gridCost[0].get (idx);
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

	public float getPotential(Vector2 localpt) {
		int idx = gridPotential.getIdx (localpt);

		// Boundary Conditions
		if (idx < 0) {
			return MAX_DENSITY + 1;
		} else {
			return gridPotential.get (idx);
		}
	}

	public Vector2 getAverageVelocity(Vector2 localpt) {
		int idx = gridD.getIdx (localpt);
		if (idx < 0) {
			return new Vector2(0f, 0f); 
		} else {
			return gridAverageVelocity.get(idx);
		}
	}

	// Helper function for UpdateVelocityFields
	public float getSpeed (Vector2 localpt, Vector2 n) {
		float r = 0.5f;
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
			}
		}
	}


	public void constructPotentialField() {

		for (int i = 0; i < resx; i++) {
			for (int j = 0; j < resz; j++) {
				if (isWithinBounds (goal_minx, goal_miny, goal_maxx, goal_maxy, i, j)) {
					// Set goal as known and potential as 0
					//					i >= bound_minx && i <= bound_maxx && j >= bound_miny && j <= bound_maxy;
					gridPotential.set (i, j, 0);
					marker.set (i, j, KNOWN);
				} else {
					// Set all other squares as infinity for now.
					gridPotential.set (i, j, Mathf.Infinity);
				}
			}
		}

		// Add Candidate Cells to the Stack
		//		Stack<Vector2> cells = new Stack<Vector2>();
		MyMinHeap cells = new MyMinHeap (this);

		// UNKNOWN Cells adjcent to KNOWN cells are included in the list of CANDIDATE CELLS
		// DO not include corners, only face borders 
		int border_minx = Mathf.Clamp (goal_minx - 1, 0, resx - 1);
		int border_miny = Mathf.Clamp (goal_miny - 1, 0, resz - 1);
		int border_maxx = Mathf.Clamp (goal_maxx + 1, 0, resx - 1);
		int border_maxy = Mathf.Clamp (goal_maxy + 1, 0, resz - 1);

		// minx to maxx
		if (border_miny < goal_miny) {
			for (int i = border_minx + 1; i < border_maxx; i++) {
				cells.insert (new Vector2 (i, border_miny));
			}
		}

		if (border_maxy > goal_maxy) {
			for (int i = border_minx + 1; i < border_maxx; i++) {
				cells.insert (new Vector2 (i, border_maxy));
			}
		}

		if (border_minx < goal_minx) {
			for (int j = border_miny + 1; j < border_maxy; j++) {
				cells.insert (new Vector2 (border_minx, j));
			}
		}

		if (border_maxx > goal_maxx) {
			for (int j = border_miny + 1; j < border_maxy; j++) {
				cells.insert (new Vector2 (border_maxx, j));
			}
		}

		// Main Loop
		while (!cells.isEmpty()) { // While not empty
			for (int w = 1; w <= cells.getSize(); w++) {
				Vector2 c = cells.get(w);
					
				int i = (int)c [0];
				int j = (int)c [1];

				// Get Neighbors
				Vector2[] neighbors = gridPotential.getFaceNeighbors (i, j);
				float mx, my;
				int x, y;


				if (gridPotential.get (neighbors [0]) + gridCost [0].get (neighbors [0]) <
					gridPotential.get (neighbors [2]) + gridCost [2].get (neighbors [2])) {
					mx = gridPotential.get (neighbors [0]) + gridCost [0].get (neighbors [0]);
					x = 0;
				} else {
					mx = gridPotential.get (neighbors [2]) + gridCost [2].get (neighbors [2]);
					x = 2;
				}

				if (gridPotential.get (neighbors [1]) + gridCost [1].get (neighbors [1]) <
					gridPotential.get (neighbors [3]) + gridCost [3].get (neighbors [3])) {
					my = gridPotential.get (neighbors [1]) + gridCost [1].get (neighbors [1]);
					y = 1;
				} else {
					my = gridPotential.get (neighbors [3]) + gridCost [3].get (neighbors [3]);
					y = 3;
				}

				// Wolfram Alpha Check
				float b = gridCost [x].get (neighbors [x]);
				float d = gridCost [y].get (neighbors [y]);
				float M = 0;
				if (mx >= Mathf.Infinity - 100 && my >= Mathf.Infinity - 100) {
					// Both are infinity, shouldn't happen.
					Console.WriteLine("mx and my are both infinity");
				} else if (mx >= Mathf.Infinity - 100) {
					//				M = mx - d; // TODO: ORIGINALY HAD THIS, BUT WHY?
					M = my - d;
				} else if (my >= Mathf.Infinity - 100) {
					//				M = my - d; // TODO: ORIGINALY HAD THIS, BUT WHY?
					M = mx - b;
				} else {
					M = Mathf.Sqrt (b * b * d * d * (-mx * mx + 2 * mx * my + b * b - my * my + d * d)) + mx * d * d + b * b * my;

					float denominator = b * b + d * d;
					if (MyMinHeap.fequal (denominator, 0)) {
						M = Mathf.Infinity;
					} else {
						M /= denominator;
					}
				}

				if (M < gridPotential.get (i, j)) {
					gridPotential.set (i, j, M); // Set potential at this point if less than it was previously
				}
			}

			// HEAPIFY
			cells.heapify();

			// Pop Candidate with Minimal Potential
			Vector2 idx = cells.removeMin ();

			marker.set (idx, KNOWN); // MARK

			Vector2[] idxneighbors = gridPotential.getFaceNeighbors ((int) idx[0], (int) idx[1]);

			// Add all neighbors to the queue if unknown
			for (int n = 0; n < 4; n++) {
				int ndx = marker.convertIdx(idxneighbors[n]);

				if (ndx >= 0 && ndx < marker.data.Length && marker.get (ndx).Equals (UNKNOWN)) {
					cells.insert (idxneighbors [n]);
				}
			}


		}
	}

	public Vector2[] getDirections(Vector2 ij) {
		Vector2[] result = new Vector2[4];
		// Counterclockwise
		result [0] = new Vector2 (ij [0] + 0.5f, ij [1]       ); // East
		result [1] = new Vector2 (ij [0]       , ij [1] + 0.5f); // North
		result [2] = new Vector2 (ij [0] - 0.5f, ij [1]       ); // West
		result [3] = new Vector2 (ij [0]       , ij [1] - 0.5f); // South
		return result;
	}

	// Discomfort From Goal From Any Point
	// Local point
	public float distance(Vector2 pos) {
		float dx = Mathf.Max(box_min.x - pos[0], 0, pos[0] - box_max.x);
		float dy = Mathf.Max(box_min.y - pos[1], 0, pos[1] - box_max.y);
		return Mathf.Sqrt(dx*dx + dy*dy);
	}
		
	public bool isknown(int i, int j) {
		return Mathf.Equals(marker.get (i, j), 1); // Known
	}
		
	bool isWithinBounds(int bound_minx, int bound_miny, int bound_maxx, int bound_maxy, int i, int j) {
		return i >= bound_minx && i <= bound_maxx && j >= bound_miny && j <= bound_maxy;
	}

}
