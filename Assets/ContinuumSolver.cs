using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContinuumSolver : MonoBehaviour {
//	// private static variables
//	private static int KNOWN = 1;
//	private static int UNKNOWN = 0;

	// Set up agents
	public GameObject original_agent;
	private Agent[] agents;
	public int quantity;
	private int maxNumAgents = 200;

	// Set up goal
	public GameObject goal;
//	private Vector2 goal_min;
//	private Vector2 goal_max;

	// Set up obstacles
//	public Obstacle[] obstacles;

	private Vector2 min = new Vector2(-50, -50); // Bottom left corner
	private Vector2 max = new Vector2(50, 50); // Top right corner
	private Vector2 resolution = new Vector2(20, 20);

	private MACGrid mGrid;

	// TODO: TINKER WITH THESE PARAMETERS
	// Weights for Cost Function
	// Unit Cost Field
	public float PATH_LENGTH_WEIGHT = 0.8f;
	public float TIME_WEIGHT = 0.8f;
	public float DISCOMFORT = 0.8f;
	public float DISCOMFORT_WEIGHT = 0.8f;


//	// FPS Counter
//	public float updateInterval = 0.5F;
//	private double lastInterval;
//	private int frames;
//	private float fps;

//	void OnGUI() {
//		GUILayout.Label("" + fps.ToString("f2"));
//	}

	// Use this for initialization
	void Start () {
//		lastInterval = Time.realtimeSinceStartup;
//		frames = 0;

		quantity = (int) Mathf.Clamp (quantity, 0, maxNumAgents);
//		obstacles = Object.FindObjectsOfType (typeof(Obstacle)) as Obstacle[];

		// Initalize the MAC Grid
		mGrid = new MACGrid(min, max, resolution, goal.GetComponent<BoxCollider>());

//		// Set Goals
//		goal_min = mGrid.getLocalPoint(goal.GetComponent<BoxCollider>().center - 0.5f * goal.GetComponent<BoxCollider>().size);
//		goal_max = mGrid.getLocalPoint(goal.GetComponent<BoxCollider>().center + 0.5f * goal.GetComponent<BoxCollider>().size);
	}

	// After everything has been initalized
	void Awake() {
		// Set FPS
		Application.targetFrameRate = -1; // As fast as it can

		// Populate Agents
		agents = new Agent[quantity];
		agents [0] = original_agent.GetComponent<Agent> ();
		for (int i = 1; i < quantity; i++) {
			GameObject temp = (GameObject) Instantiate (original_agent, new Vector3 (i * 2.0f, 0, 0), Quaternion.identity);
			agents [i] = temp.GetComponent<Agent> ();
		}

	}

	// Update is called once per frame
	void FixedUpdate () {
//		++frames;
//		float timeNow = Time.realtimeSinceStartup;
//		if (timeNow > lastInterval + updateInterval) {
//			fps = (float)(frames / timeNow - lastInterval);
//			frames = 0;
//			lastInterval = timeNow;
//		}


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
		mGrid.constructPotentialField();

		// Boundary Conditions

		// Update Colors
		GetComponent<PointCloud> ().updateMesh (mGrid);

		// Advect
		foreach (Agent a in agents) {
			Rigidbody rb = a.GetComponent<Rigidbody> ();
//			Vector3 whatamilookingat = rb.velocity;
//
//			Vector2 localpt = mGrid.getLocalPoint (a.getWorldPosition ());
//			int Uidx = mGrid.gridRose [0].getIdx (localpt);
////			float Uvelocity = mGrid.gridRose [0].get (Uidx);
//
//			int Vidx = mGrid.gridRose [1].getIdx (localpt);
////			float Vvelocity = mGrid.gridRose [1].get (Uidx);
//
//			float Upotential = mGrid.gradU.get (Uidx);
//			float Vpotential = mGrid.gradV.get (Vidx);
//
////			Vector2 flowspeed = new Vector2 (Uvelocity, Vvelocity);
//			float flowspeed = mGrid.getSpeed(localpt, a.getNormal());
//			Vector2 potential = new Vector2 (Upotential, Vpotential);
//
//			Vector2 result = -1 * flowspeed * potential.normalized;

//			rb.velocity = new Vector3(result[0], 0, result[1]);
//			rb.velocity = new Vector3(3, 0, 3);
		}

		mGrid.clear ();
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

//	public void constructPotentialField() {
//		int a = 4; // TODO: NOTHING
//
//
//		// Set goal potential
//		Vector2 what = mGrid.marker.getIdxVec2 (goal_min);
//
//		int goal_minx = (int) mGrid.marker.getIdxVec2 (goal_min)[0];
//		int goal_miny = (int) mGrid.marker.getIdxVec2 (goal_min)[1];
//		int goal_maxx = (int) mGrid.marker.getIdxVec2 (goal_max)[0];
//		int goal_maxy = (int) mGrid.marker.getIdxVec2 (goal_max)[1];
//
//		for (int i = 0; i < resi; i++) {
//			for (int j = 0; j < resj; j++) {
//				if (isWithinBounds (goal_minx, goal_miny, goal_maxx, goal_maxy, i, j)) {
//					mGrid.gridPotential.set (i, j, 0);
//				} else {
//					mGrid.gridPotential.set (i, j, Mathf.Infinity);
//				}
//			}
//		}
//
//		// Add Candidate Cells to the Stack
////		Stack<Vector2> cells = new Stack<Vector2>();
//		MyMinHeap cells = new MyMinHeap(mGrid);
//
//		// UNKNOWN Cells adjcent to KNOWN cells are included int he list of CANDIDATE CELLS
//		int border_minx = Mathf.Clamp(goal_minx - 1, 0, resi);
//		int border_miny = Mathf.Clamp(goal_miny - 1, 0, resj);
//		int border_maxx = Mathf.Clamp(goal_maxx + 1, 0, resi);
//		int border_maxy = Mathf.Clamp(goal_maxy + 1, 0, resj);
//
//		// minx to maxx
//		if (border_miny < goal_miny) {
//			for (int i = border_minx; i <= border_maxx; i++) {
//				cells.insert (new Vector2(i, border_miny));
//			}
//		}
//
//		if (border_maxy > goal_maxy) {
//			for (int i = border_minx; i <= border_maxx; i++) {
//				cells.insert (new Vector2(i, border_maxy));
//			}
//		}
//
//		if (border_minx < goal_minx) {
//			for (int j = border_miny; j <= border_maxy; j++) {
//				cells.insert (new Vector2 (border_minx, j));
//			}
//		}
//
//		if (border_maxx > goal_maxx) {
//			for (int j = border_miny; j <= border_maxy; j++) {
//				cells.insert (new Vector2(border_maxx, j));
//			}
//		}
//
//		// Main Loop
//		while (cells.count > 0) { // While not empty
//			// Pop Candidate with Minimal Potential
//			Vector2 idx = cells.removeMin();
////			float minPotential = mGrid.gridPotential.get(idx);
//			mGrid.marker.set(idx, KNOWN); // MARK
//
//			int i = (int) idx [0];
//			int j = (int) idx [1];
//
//			// Get Neighbors
//			Vector2[] neighbors = mGrid.gridPotential.getFaceNeighbors(i, j);
//			float mx, my;
//			int x, y;
//
//
//			if (mGrid.gridPotential.get (neighbors [0]) + mGrid.gridCost [0].get (neighbors [0]) <
//				mGrid.gridPotential.get (neighbors [2]) + mGrid.gridCost [2].get (neighbors [2])) {
//				mx = mGrid.gridPotential.get (neighbors [0]) + mGrid.gridCost [0].get (neighbors [0]);
//				x = 0;
//			} else {
//				mx = mGrid.gridPotential.get (neighbors [2]) + mGrid.gridCost [2].get (neighbors [2]);
//				x = 2;
//			}
//
//			if (mGrid.gridPotential.get (neighbors [1]) + mGrid.gridCost [1].get (neighbors [1]) <
//			    mGrid.gridPotential.get (neighbors [3]) + mGrid.gridCost [3].get (neighbors [3])) {
//				my = mGrid.gridPotential.get (neighbors [1]) + mGrid.gridCost [1].get (neighbors [1]);
//				y = 1;
//			} else {
//				my = mGrid.gridPotential.get (neighbors [3]) + mGrid.gridCost [3].get (neighbors [3]);
//				y = 3;
//			}
//				
//			// Wolfram Alpha Check
//			float b = mGrid.gridCost [x].get (neighbors [x]);
//			float d = mGrid.gridCost [y].get (neighbors [y]);
//			float M = 0;
//			if (mx >= Mathf.Infinity - 100 && my >= Mathf.Infinity) {
//				// Both are infinity, shouldn't happen.
//				print ("mx and my are both infinity");
//			} else if (mx >= Mathf.Infinity - 100) {
//				M = mx - d;
//			} else if (my >= Mathf.Infinity - 100) {
//				M = my - d;
//			} else {
//				M = Mathf.Sqrt (b * b * d * d * (-mx * mx + 2 * mx * my + b * b - my * my + d * d)) + mx * d * d + b * b * my;
//				M /= b * b + d * d;
//			}
//
//			mGrid.gridPotential.set (i, j, M); // Set potential at this point
//
//			// Add all neighbors to the queue if unknown
//			for (int n = 0; n < 4; n++) {
//				if (mGrid.marker.get (neighbors [n]).Equals (UNKNOWN)) {
//					cells.insert (neighbors [n]);
//				}
//			}
//		}
//	}

//	void constructGradientPotential() {
//		// grad U
//		for (int i = 0; i < resi - 1; i++) {
//			for (int j = 0; j < resj; j++) {
//				float curr = mGrid.gridPotential.get (i, j);
//				float next = mGrid.gridPotential.get (i + 1, j);
//				mGrid.gradU.set (i, j, next - curr);
//			}
//		}
//
//		// Boundary
//		for (int j = 0; j < resj; j++) {
////			float curr = mGrid.gridPotential.get (resi - 1);
//			mGrid.gradU.set (resi - 1, j, Mathf.Infinity);
//		}
//
//		for (int i = 0; i < resi; i++) {
//			mGrid.gradV.set (i, resj - 1, Mathf.Infinity);
//		}
//
//		// Grad v
//		for (int i = resi; i < resi; i++) {
//			for (int j = resj; j < resj; j++) {
//				float curr = mGrid.gridPotential.get (i, j);
//				float next = mGrid.gridPotential.get (i, j + 1);
//				mGrid.gradV.set (i, j, next - curr);
//			}
//		}
//	}
}
