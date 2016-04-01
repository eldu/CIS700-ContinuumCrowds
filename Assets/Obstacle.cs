using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour {
	private Vector3 min;
	private Vector3 max;

	void start() {
		
	}

	public Vector3 getMin() {
		return min;
	}

	public Vector3 getMax() {
		return max;
	}
}
