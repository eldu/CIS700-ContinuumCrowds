using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContinuumSolver : MonoBehaviour {
	// private static variables
	private static int KNOWN = 1;
	private static int UNKNOWN = 0;

	// Set up agents
	public GameObject original_agent;
	private Agent[] agents;
	public int quantity;
	private int maxNumAgents = 200;

	// Set up goal
	public GameObject goal;
	Vector3 goal_min;
	Vector3 goal_max;

	// Set up obstacles
	public Obstacle[] obstacles;

	public Vector3 min = new Vector2(-5, -5); // Bottom left corner
	public Vector3 max = new Vector2(5, 5); // Top right corner
	public Vector2 resolution = new Vector2(5, 5);
	public int resi = 5;
	public int resj = 5;


	private MACGrid mGrid;

	// TODO: TINKER WITH THESE PARAMETERS
	// Weights for Cost Function
	// Unit Cost Field
	public float PATH_LENGTH_WEIGHT = 0.8f;
	public float TIME_WEIGHT = 0.8f;
	public float DISCOMFORT = 0.8f;
	public float DISCOMFORT_WEIGHT = 0.8f;


	// Use this for initialization
	void Start () {
		quantity = (int) Mathf.Clamp (quantity, 0, maxNumAgents);
		obstacles = Object.FindObjectsOfType (typeof(Obstacle)) as Obstacle[];

		// Initalize the MAC Grid
		mGrid = new MACGrid(min, max, resolution, goal.GetComponent<BoxCollider>());

		// Set Goals
		goal_min = mGrid.getLocalPoint(goal.GetComponent<BoxCollider>().center - 0.5f * goal.GetComponent<BoxCollider>().size);
		goal_max = mGrid.getLocalPoint(goal.GetComponent<BoxCollider>().center + 0.5f * goal.GetComponent<BoxCollider>().size);
	}

	// After everything has been initalized
	void Awake() {
		// Populate Agents
		agents = new Agent[quantity];
		agents [0] = original_agent.GetComponent<Agent> ();
		for (int i = 1; i < quantity; i++) {
			GameObject temp = (GameObject)Instantiate (original_agent, new Vector3 (i * 2.0f, 0, 0), Quaternion.identity);
			agents [i] = temp.GetComponent<Agent> ();
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		// Clear Grids
		mGrid.clear ();

		// Center of grid cell
		// Splat Density
		// Average velocity
		mGrid.splat (agents);

		// For each group
		// Construct Unit Cost Field
		// Calculate Speed Fields and Update Average Velocity
		mGrid.UpdateVelocityAndCostFields ();
		// Dynamic Potentional Field Construction


		// Advect
	}


	/*
	 *  NON-STANDARD UNITY FUNCTIONS BELOW
	 */ 

	// The farther away the agent is, the more discomforted he may be
	float getDiscomfort(Agent a, Vector2 position) {
		return Vector3.Distance (position, a.goal.position);
	}



	bool isWithinBounds(int bound_minx, int bound_miny, int bound_maxx, int bound_maxy, int i, int j) {
		return i >= bound_minx && i <= bound_maxx && j >= bound_miny && j <= bound_maxy;
	}


	void floodfill() {

		// Set goal potential
		int goal_minx = (int) mGrid.marker.getIdxVector2FromPos (goal_min).x;
		int goal_miny = (int) mGrid.marker.getIdxVector2FromPos (goal_min).y;
		int goal_maxx = (int) mGrid.marker.getIdxVector2FromPos (goal_max).x;
		int goal_maxy = (int) mGrid.marker.getIdxVector2FromPos (goal_max).y;

		for (int i = 0; i < resi; i++) {
			for (int j = 0; j < resj; j++) {
				if (isWithinBounds (goal_minx, goal_miny, goal_maxx, goal_maxy, i, j)) {
					mGrid.gridPotential.setVal (i, j, 0);
				} else {
					mGrid.gridPotential.setVal (i, j, Mathf.Infinity);
				}
			}
		}

		// Add Candidate Cells to the Stack
		Stack<Vector2> cells = new Stack<Vector2>();

		// UNKNOWN Cells adjcent to KNOWN cells are included int he list of CANDIDATE CELLS
		int border_minx = Mathf.Clamp(goal_minx - 1, 0, resi);
		int border_miny = Mathf.Clamp(goal_miny - 1, 0, resj);
		int border_maxx = Mathf.Clamp(goal_maxx + 1, 0, resi);
		int border_maxy = Mathf.Clamp(goal_maxy + 1, 0, resj);

		// minx to maxx
		if (border_miny < goal_miny) {
			for (int i = border_minx; i <= border_maxx; i++) {
				cells.Push(new Vector2(i, border_miny));
			}
		}

		if (border_maxy > goal_maxy) {
			for (int i = border_minx; i <= border_maxx; i++) {
				cells.Push(new Vector2(i, border_maxy));
			}
		}

		if (border_minx < goal_minx) {
			for (int j = border_miny; j <= border_maxy; j++) {
				cells.Push (new Vector2 (border_minx, j));
			}
		}

		if (border_maxx > goal_maxx) {
			for (int j = border_miny; j <= border_maxy; j++) {
				cells.Push(new Vector2(border_maxx, j));
			}
		}

		// Main Loop
		while (cells.Count > 0) { // While not empty
			// Pop
			Vector2 idx = cells.Pop();
			int i = (int) idx [0];
			int j = (int) idx [1];

			// Minimum
			float minCost = Mathf.Infinity;
			Vector2 minNeigh;

			// Get Neighbors
			List<int> neighbors = getNeighbors(i, j);




		}




	}

	float getCost(int from, int to) {
		
		return 0;
	}

	List<int> getNeighbors(int i, int j) {
		List<int> result = new List<int> ();;
		int[] neighbors = new int[4];

		neighbors[0] = mGrid.gridPotential.getIdxFromIdx (i + 1, j);
		neighbors[1] = mGrid.gridPotential.getIdxFromIdx (i - 1, j);
		neighbors[2] = mGrid.gridPotential.getIdxFromIdx (i, j + 1);
		neighbors[3] = mGrid.gridPotential.getIdxFromIdx (i, j - 1);

		for (int n = 0; n < 4; n++) {
			if (neighbors [n] >= 0 && neighbors [n] < mGrid.gridPotential.data.Length) { // In bounds
				result.Add(neighbors[n]);
			}
		}

		return result;
	}


//	void flood(Vector2 start) {
//
//		// TODO: Assign goals more efficiently
//		// Assign Cells with goal
//		Vector2 goal_minidx = mGrid.marker.getIdxVector2FromPos (goal_min);
//		Vector2 goal_maxidx = mGrid.marker.getIdxVector2FromPos (goal_max);
//
//		for (int i = goal_minidx [0]; i <= goal_maxidx [1]; i++) {
//			for (int j = goal_minidx [1]; i <= goal_maxidx [1]; i++) {
//				mGrid.marker.setVal (i, j, 1f); // Mark Known
//				mGrid.gridPotential.setVal (i, j, 0.f); // Potential at goal is 0.
//			}
//		}
//
//		// Create the stack
//
//		// Stack
//		Stack<Vector2> cells = new Stack<Vector2>();
//
//		// UNKNOWN Cells adjcent to KNOWN cells are included int he list of CANDIDATE CELLS
//		int border_minx = Mathf.Clamp(goal_minidx.x - 1, 0, resolution.x);
//		int border_miny = Mathf.Clamp(goal_minidx.y - 1, 0, resolution.y);
//		int border_maxx = Mathf.Clamp(goal_maxidx.x + 1, 0, resolution.x);
//		int border_maxy = Mathf.Clamp(goal_maxidx.y + 1, 0, resolution.y);
//
//		// minx to maxx
//		if (border_miny < goal_minidx [1]) {
//			for (int i = border_minx; i <= border_maxx; i++) {
//				cells.Push(new Vector2(i, border_miny));
//			}
//		}
//
//		if (border_maxy > goal_maxidx [1]) {
//			for (int i = border_minx; i <= border_maxx; i++) {
//				cells.Push(new Vector2(i, border_maxy));
//			}
//		}
//
//		if (border_minx < goal_minidx [0]) {
//			for (int j = border_miny; j <= border_maxy; j++) {
//				cells.Push (new Vector2 (border_minx, j));
//			}
//		}
//
//		if (border_maxy > goal_maxidx [0]) {
//			for (int j = border_miny; j <= border_maxy; j++) {
//				cells.Push(new Vector2(border_maxx, j));
//			}
//		}
//
//		while (cells.Count > 0) {
//			// Pop
//			Vector2 idx = cells.Pop ();
//
//			// Unknown
//			if (mGrid.marker.getVal (idx) == UNKNOWN) {
//				// Do things
//				float mx;
//				float my;
//				
//				// || gradient * potential(x) || = C
//
//
//
//
//				// Push Neighbors
//				cells.Push (new Vector2 (idx [0] + 1, idx [1]));
//				cells.Push (new Vector2 (idx [0], idx [1] + 1));
//				cells.Push (new Vector2 (idx [0] - 1, idx [1]));
//				cells.Push (new Vector2 (idx [0], idx [1] - 1));
//			} // Otherwise known cell, move on.
//		}
//
//	}
}
