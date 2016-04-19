using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContinuumSolver : MonoBehaviour {

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

		// Splat
		mGrid.splat (agents);

		// Calculate Speed Fields and Update Average Velocity
		mGrid.UpdateVelocityAndCostFields ();

		// Dynamic Potentional Field Construction
		// Flood Field

	}

	// The farther away the agent is, the more discomforted he may be
	float getDiscomfort(Agent a, Vector2 position) {
		return Vector3.Distance (position, a.goal.position);
	}


	void flood(Vector2 start) {
		// Assign Cells with goal
		Vector2 goal_minidx = mGrid.marker.getIdxVector2FromPos (goal_min);
		Vector2 goal_maxidx = mGrid.marker.getIdxVector2FromPos (goal_max);

		for (int i = goal_minidx [0]; i <= goal_maxidx [1]; i++) {
			for (int j = goal_minidx [1]; i <= goal_maxidx [1]; i++) {
				mGrid.marker.setVal (i, j, 1f); // Mark Known

				mGrid.gridCost
			}
		}


		int startBound = mGrid.marker.getIdxFromIdx (start);

		if (startBound < 0) {
			print ("ERROR: Start flood elsewhere. Idx does not exist");
			return;
		}

		// Stack
		Stack<Vector2> cells = new Stack<Vector2>();

		// First
		cells.Push (start);


		while (cells.Count > 0) {
			// Pop
			Vector2 idx = cells.Pop ();

			// Unknown
			if (mGrid.marker.getVal (idx) == 0) {
				// Do things
				float mx;
				float my;
				
				// || gradient * potential(x) || = C




				// Push Neighbors
				cells.Push (new Vector2 (idx [0] + 1, idx [1]));
				cells.Push (new Vector2 (idx [0], idx [1] + 1));
				cells.Push (new Vector2 (idx [0] - 1, idx [1]));
				cells.Push (new Vector2 (idx [0], idx [1] - 1));
			} // Otherwise known cell, move on.
		}

	}


//	void flood(Vector2 idx) {
//		int inBound = mGrid.marker.getIdxFromIdx (idx);
//
//		// Boundary Condition
//		if (inBound < 0) {
//			// Out of bounds
//			return;
//		}
//
//		// Marked?
//		if (mGrid.marker.getVal (inBound)) {
//			// Marked
//			return;
//		}
//
//		// Do things
//
//
//
//
//		// Recursive Calls
//		flood (idx [0] + 1, idx [1]);
//		flood (idx [0], idx [1] + 1);
//		flood (idx [0] - 1, idx [1]);
//		flood (idx [0], idx [1] - 1);
//	}
}
