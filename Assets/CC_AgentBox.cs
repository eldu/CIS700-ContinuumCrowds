using UnityEngine;
using System.Collections;

public class CC_AgentBox : MonoBehaviour {
	public GameObject agent;
	public int Quantity;

	private GameObject[] clones;

	// Use this for initialization
	void Start () {
		clones = new GameObject[Quantity];

//		for (int i = 0; i < 10; i++) {
//			clones[i] = Instantiate(prefab, new Vector3(i * 2.0f, 0, 0), Quaternion.identity);	
//		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
