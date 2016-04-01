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

		// Calculate Speed Fields
		mGrid.UpdateVelocityAndCostFields ();
	}

	// The farther away the agent is, the more discomforted he may be
	float getDiscomfort(Agent a, Vector2 position) {
		return Vector3.Distance (position, a.goal.position);
	}
}
