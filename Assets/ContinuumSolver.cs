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
	void Update () {
		// Clear Grids
		mGrid.clear();

		// Splat
		mGrid.splat(agents);

		// 
	}
}
