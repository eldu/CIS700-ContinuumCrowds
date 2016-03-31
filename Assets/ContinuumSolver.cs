using UnityEngine;
using System.Collections;

public class ContinuumSolver : MonoBehaviour {
	public GameObject original_agent;
	private Agent[] agents;
	public int quantity;
	private int maxNumAgents = 200;

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

		// Initalize the MAC Grid
		mGrid = new MACGrid(min, max, resolution);
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
		mGrid.clear();

		// Splat
		mGrid.splat(agents);

		// Calculate Speed Fields
		mGrid.UpdateVelocityFields();

		// Compute the unit cost field
		foreach (Agent a in agents) {
			
		}
		// Update Cost
//		float cost = (PATH_LENGTH_WEIGHT * fx + TIME_WEIGHT + DISCOMFORT_WEIGHT * DISCOMFORT) / fx;
//		gridCU.setVal (uij, PATH_LENGTH_COEFF);
	}

	// The farther away the agent is, the more discomforted he may be
	float getDiscomfort(Agent a) {
		return Vector3.Distance (a.GetComponent<Animator> ().bodyPosition, a.goal.position);
	}

	// TODO: YOU MESSED UP WHAT ARE YOU DOING
	#if WRONG
	void UpdateVelocityFields() {
		foreach (Agent a in agents) {
			Vector3 bodyPosition = a.GetComponent<Animator>().bodyPosition;
			Vector3 orientation = a.GetComponent<Animator> ().bodyRotation.eulerAngles;
			Vector2 orientationVec2 = new Vector2 (orientation [0], orientation [2]);

			Vector2 worldpt = new Vector2 (bodyPosition [0], bodyPosition [2]);

			float p = mGrid.getDensity(worldpt);

			Vector3 worldstep = bodyPosition + a.radius * orientation;
			Vector2 worldstepVec2 = new Vector2(worldstep[0], worldstep[1]);

			Vector2 v_xrn = mGrid.getAverageVelocity (worldstepVec2);
			float p_xrn = mGrid.getDensity (worldstepVec2);

			float fx; // Speed
			// TODO: Incorporate terrain heightfield
			float ft = MAX_SPEED; // Topological Speed, Ignore Terrain
			float fv = Vector2.Dot(v_xrn,  orientationVec2); // flow speed


			if (p < MIN_DENSITY) {
				// Low density
				fx = ft;
			} else if (p < MAX_DENSITY) {
				// Middle density
				fx = ft + (p_xrn - MIN_DENSITY) / (MAX_DENSITY - MIN_DENSITY) * (fv - ft);
			} else {
				// High Density
				fx = fv
			}
		}
	}
	#endif
}
