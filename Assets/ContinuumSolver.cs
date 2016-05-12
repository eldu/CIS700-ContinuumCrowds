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

	// Set up obstacles
//	public Obstacle[] obstacles;

	private Vector2 min = new Vector2(-50, -50); // Bottom left corner
	private Vector2 max = new Vector2(50, 50); // Top right corner
	private Vector2 resolution = new Vector2(20, 20);

	// MACGRID
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
//		obstacles = Object.FindObjectsOfType (typeof(Obstacle)) as Obstacle[];

		// Initalize the MAC Grid
		mGrid = new MACGrid(min, max, resolution, goal.GetComponent<BoxCollider>());
	}

	// After everything has been initalized
	void Awake() {
		// Populate Agents
		agents = new Agent[quantity];
		agents[0] = original_agent.GetComponent<Agent> ();
		original_agent.GetComponent<Rigidbody> ().velocity = new Vector3 (0, 0, 3.5f);
		int count = 1;
		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < 4; j++) {
				if (count < quantity) {
					GameObject temp = (GameObject)Instantiate (original_agent, new Vector3 (i * 5 - 2.0f, 0, j * 5 - 2.0f), Quaternion.identity);
					agents [count] = temp.GetComponent<Agent> ();
					Rigidbody rb = agents [count].GetComponent<Rigidbody> ();
					rb.velocity = new Vector3 (0, 0, 3.5f);
					count++;
				} else {
					break;
				}
			}
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
		mGrid.UpdateSpeedAndCostFields ();

		// Dynamic Potentional Field Construction
		mGrid.constructPotentialField();
		mGrid.computePotentialGradient ();
		mGrid.computeVelocitiesFromPotentials ();

//		// TODO Boundary Conditions
//
//		// Update Colors
		GetComponent<PointCloud> ().updateMesh (mGrid);
//
//		// Advect
		foreach (Agent a in agents) {
			Vector2 localpt = mGrid.getLocalPoint (a.getWorldPosition ());
			Vector2 velocity = mGrid.interpolateVelocity (localpt);
			a.setVelocity(velocity, Time.deltaTime);

			// Collision with goal
//			if (a.atGoal()) {
//				agents.Remove (a);
//			}

//			// Boundary Conditions
//			if ((a.getWorldPosition ().x < min.x && a.getWorldPosition ().y < min.y) ||
//				a.getWorldPosition().y > max.y && a.getWorldPosition().x > max.x) {
//				a.setVelocity(new Vector2 (0.0f, 0.0f), Time.deltaTime);
//			}

			// TODO Forced minimum distance
//			foreach (Agent b in agents) {
//				if (a != b) {
//					Vector2 blocal = mGrid.getLocalPoint (b.getWorldPosition ());
//					if (Vector2.Distance (a, b) < 0.2f) {
//						// Shove b
//
//					}
//				}
//			}
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
}
