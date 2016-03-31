// #define ACTIVE
	#if ACTIVE
using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {
	// Public Parameters
	public GameObject original_agent;
	private Agent[] agents;
	public int quantity;

	public Vector3 min = new Vector3(-5, 0, -5); // Bottom left corner
	public Vector3 max = new Vector3(5, 0, 5); // Top right corner
	public int resx = 10; // x by z
	public int resz = 10;

	private int columns;
	private int rows; 

	private float cellWidth;
	private float cellHeight;

	// Grids
//	private float[] density;
	private float[] ux;
	private float[] uz;

	private GridDensity densities;

	// Private Tickerable Variables
	private int maxNumAgents = 200;

	// Use this for initialization
	void Start () {
		quantity = Mathf.Clamp(quantity, 0, maxNumAgents);

		cellWidth = (max [0] - min [0]) / resx;
		cellHeight = (max [2] - min [2]) / resz;

		// Initalize Grids
//		density = new float[resx * resz];
		densities = new GridDensity (resx, resz, cellWidth, cellHeight);

	}

	// After everything has been initialized
	void Awake() {
		// Instantiate Agents
		agents = new Agent[quantity];
		agents [0] = original_agent.GetComponent<Agent>();
		//		agents [0] = original_agent.GetComponent<Agent>();
		for (int i = 1; i < quantity; i++) {
			GameObject temp = (GameObject)Instantiate (original_agent, new Vector3 (i * 2.0f, 0, 0), Quaternion.identity);
			agents [i] = temp.GetComponent<Agent> ();
		}
	}
		
	// Update is called once per frame
	void Update () {
		// Clear Grids
		densities = new GridDensity (resx, resz, cellWidth, cellHeight);

		// Update Density
//		splat();
		densities.clampDensity ();

		//
	
	}


	/**
	 * Density
	 * */
//	void updateDensity() {
//		foreach (Agent a in agents) {
//			Vector2 localpt = getLocalPoint (a.transform.position);
//			densities.addDensity (localpt);
//		}
//		densities.clampDensity ();
//	}

	// Shifts of the position to relate to the bottom left corner of the grid
	Vector3 calibrate(Vector3 p) {
		return p - min;
	}

	Vector2 getLocalPoint(Vector3 p) {
		// Calibrate such that the min is (0, 0)
		Vector2 result = new Vector2(p[0] - min[0], p[1] - min[2]);

		// Divide by cellWidth and cellHeight
		result[0] /= cellWidth;
		result[1] /= cellHeight;

		return result;
	}

	// TODO: Clean up scrap

	//	// Get the coordinate of the centroid of the density grid
	//	Vector2 getDensityCoord(int idx) {
	//		// Bottom left corner of grid cell
	//		int i = idx / resx;
	//		int j = idx - (i * resx);
	//
	//		return new Vector3 (cellWidth * (i + 0.5f), cellHeight * (j + 0.5f)); 
	//	}
	//
	//	// Get the index of the density grid from a point on the world grid
	//	int getDensityIdx(Vector2 p) {
	//		int i = (int) (p[0] / (cellWidth * resx));
	//		int j = (int) (p[1] / (cellWidth * resz));
	//
	//		return resx * i + j;
	//	}
	//
	//	Vector2 getDensityIdxV2(Vector2 p) {
	//		int i = (int) (p[0] / (cellWidth * resx));
	//		int j = (int) (p[1] / (cellWidth * resz));
	//
	//		return new Vector2 (i, j);
	//	}

}
#endif