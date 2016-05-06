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
//		Application.targetFrameRate = -1; // As fast as it can

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
//
//		// Boundary Conditions
//
//		// Update Colors
//		GetComponent<PointCloud> ().updateMesh (mGrid);
//
//		// Advect
//		foreach (Agent a in agents) {
//			Rigidbody rb = a.GetComponent<Rigidbody> ();
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
//
//			rb.velocity = new Vector3(result[0], 0, result[1]);
////			rb.velocity = new Vector3(3, 0, 3);
//		}

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
}
