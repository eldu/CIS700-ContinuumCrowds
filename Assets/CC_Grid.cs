using UnityEngine;
using System.Collections;


// TODO: Allow it so that grid can be negative space
public class CC_Grid : MonoBehaviour {

	public Transform origin;
	private Vector3 size;

	// Resolution of Grid
	private int res = 10;
	private float sidelength;

	// Toggle between grids
	// false (0) = 
	private bool toggle = false;

	private CC_Cell cell;
	private CC_Cell[] gridA;
	private CC_Cell[] gridB;

	private object[] agents;

	// Variables
	private float dt;

	// TODO: Ticker with these variables
	// DENSITY
	private float expp = 2.0f; // Speed of density falloff
	private float minp = 0.05f; // for own cell contribution
	private float maxp = 0.70f; // for neighboring cell contribution

	public GameObject prefab;



	/**
	 * Unity Functions
	 **/
	void Awake () {
//		dt = Time.deltaTime; // 0.04 25fps
//
//		// Grid Components
//		size = origin.localScale;
//		sidelength = size.x / res;
//		cell = GetComponent<CC_Cell> ();
//
//		// Declare grids
//		gridA = new CC_Cell[res * res];
//		gridB = new CC_Cell[res * res];

		// Get all agents
		agents = GameObject.FindObjectsOfType(typeof (NavMeshAgent));

		foreach (NavMeshAgent a in agents) {
			print("HI THERE");
		}

		for (int i = 0; i < 10; i++) {
			Instantiate(prefab, new Vector3(i * 2.0f, 0, 0), Quaternion.identity);	
		}

		// Initialize gridA
		// get all of the densities


	}
//
//	// Update is called once per frame
//	void Update () {
//		
//
//
//	}
//
//
//	/**
//	 * Helper Functions
//	 **/
//
//	void updateDensity(CC_Cell[] grid) {
//		foreach (NavMeshAgent a in agents) {
//			Vector3 pos = a.nextPosition;
//			float speed = a.desiredVelocity.magnitude;
//
//
//			float r = a.radius;
//		}
//	}
//
//
//
//
//
//
//	/**
//	 *  Get and set functions below
//	 * */
//
//
//	// Get the active grid in the cell
//	CC_Cell[] getActiveGrid() {
//		// false = 0 = gridA
//		// true = gridB
//		if (toggle) {
//			return gridB;
//		} else {
//			return gridA;
//		}
//	}
//
//	CC_Cell[] getInactiveGrid() {
//		if (!toggle) {
//			return gridB;
//		} else {
//			return gridA;
//		}
//	}
//
//	// Overload
//	int getCellidx(Vector2 pos) {
//		return getCellidx (pos.x, pos.y);
//	}
//
//	// Get cell index from location
//	// x and y range from [0, 1]
//	int getCellidx(float x, float y) {
//		int i = (int)(x / sidelength);
//		int j = (int)(y / sidelength);
//
//		int idx = res * i + j;
//
//		// Edge case is (1, 1)
//		if (idx > res * res - 1) {
//			idx = res * res - 1;
//		}
//
//		return idx;
//	}
//
//	// bilerp
//	// Return the 
//	float bilerp(float q, float x1, float y1, float x2, float y2) {
//		return 1.0f;
//	}
//}
}