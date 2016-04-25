using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Agent : MonoBehaviour {
	public Transform goal;

	public float radius = 0.5f;
	public float height = 2.0f;
	public float baseOffset = 0.0f;

	// TODO. Does nothing.
	public float MAX_SPEED = 3.5f; // Maximum movement speed when following a path
	public float MAX_ANGULAR_SPEED = 120.0f; // maximum turning speed in (deg/s) while following a path
	public float MAX_ACCELERATION = 8.0f; // max acceleration of an agent as it follows a path, given in units/sec^2
	public float STOPPING_DISTANCE = 0.0f; // Stop within this distance form the target position
	public bool autoBraking = true;

	private Vector3 destination;
//	private float densityExponent = 2.0f; // lamda, speed of density falloff


	public List<GameObject> collidingWalls;

	// Pointer to ContinuumSolver
	public ContinuumSolver cs;

	// Use this for initialization
	void Start () {
		destination = goal.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

	}

	public void setVelocity(Vector2 v) {
		Vector3 result = new Vector3 (v [0], GetComponent<Rigidbody> ().velocity.y, v [1]);
		GetComponent<Rigidbody> ().velocity = result;
	}

	public Vector2 getWorldPosition () {
		return new Vector2(GetComponent<Animator> ().bodyPosition.x, 
			GetComponent<Animator> ().bodyPosition.z);
	}

	public Vector2 getNormal() {
		Vector3 orientation = GetComponent<Animator> ().bodyRotation.eulerAngles;
		return new Vector2 (Mathf.Cos (orientation.y), Mathf.Sin (orientation.y));
	}
}
