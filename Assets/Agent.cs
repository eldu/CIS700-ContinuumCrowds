using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Agent : MonoBehaviour {
	public Transform goal;
	public GameObject goalObject;

	public string goalTag = "DEBUG";
	private bool reachedGoal = false;

	public float radius = 0.5f;
	public float height = 2.0f;
	public float baseOffset = 0.0f;

	// TODO
	public float MAX_SPEED = 2.0f; // Maximum movement speed when following a path
	public float MAX_ANGULAR_SPEED = 120.0f; // maximum turning speed in (deg/s) while following a path
	public float MAX_ACCELERATION = 8.0f; // max acceleration of an agent as it follows a path, given in units/sec^2
	public float STOPPING_DISTANCE = 0.0f; // Stop within this distance form the target position
	public bool autoBraking = true;

//	private Vector3 destination;
//	private float densityExponent = 2.0f; // lamda, speed of density falloff


	public List<GameObject> collidingWalls;

	// Pointer to ContinuumSolver
	public ContinuumSolver cs;

	// Use this for initialization
	void Awake () {
//		destination = goal.position;
		Console.WriteLine(goalTag);
//		goalObject.tag = goalTag;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

	}

	public void setVelocity(Vector2 velocity, float dt) {
		Vector2 v = Vector2.ClampMagnitude (velocity, MAX_SPEED);
//		Vector2 direction = velocity.normalized;

		Vector3 result = new Vector3 (v [0], GetComponent<Rigidbody> ().velocity.y, v [1]);
		if (float.IsNaN (result.x)) {
			result.x = 0.0f;
		}

		if (float.IsNaN (result.z)) {
			result.z = 0.0f;
		}

		GetComponent<Rigidbody> ().velocity = result;
	}
		
	public Vector2 getWorldPosition () {
		Vector2 result = new Vector2(GetComponent<Transform> ().position.x, GetComponent<Transform> ().position.z);
		return result;
	}

	public Vector2 getNormal() {
		Vector3 orientation = GetComponent<Animator> ().bodyRotation.eulerAngles;
		return new Vector2 (Mathf.Cos (orientation.y), Mathf.Sin (orientation.y));
	}

	public void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == goalTag) {
			reachedGoal = true;
		}
	}

	public bool atGoal() {
		return reachedGoal;
	}
}
