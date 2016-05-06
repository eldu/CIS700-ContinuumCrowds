using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

public class MyMinHeap {
	public MACGrid g;
	public List<Vector2> items = new List<Vector2>();

	public MyMinHeap (MACGrid grid) {
		g = grid;
		items.Add (new Vector2(0, 0));
	}

	public int count {
		get { return items.Count; }
	}

	public void clear() {
		items.Clear ();
	}


	public void heapify() {
		int N = items.Count - 1;
		for (int k = N / 2; k >= 1; k--) {
			sink (k, N);
		} 
		while (N > 1) {
			swap (1, N--);
			sink (1, N);
		}
	}

	public void sink(int k, int N) {
		while (2 * k <= N) {
			int j = 2 * k;
			if (j < N && compare (items[j], items[j + 1]) < 0)
				j++;
			if (compare (items[k], items[j]) > 0)
				break;
			swap (k, j);
			k = j;
		}
	}

	public void insert(Vector2 a) {
		items.Add (a);
		swim (items.Count - 1);
	}

	public Vector2 peek() {
		if (items.Count == 0) {
			throw new System.InvalidOperationException ("Empty Heap");
		}
		return items [0];
	}

	public bool isEmpty() {
		return items.Count <= 1;
	}

	public Vector2 removeMin() {
		if (items.Count <= 1) {
			throw new System.InvalidOperationException ("Empty Heap");
		}

		Vector2 min = items [1];

		// Swap with Last Item
		Vector2 temp = items[items.Count - 1];
		items [1] = temp;
		items.RemoveAt (items.Count - 1);

		if (items.Count > 1) {
			sink (1, items.Count - 1);
		}


		return min;
	}
		
//	public void sink(int k, int N) {
//		while (2 * k <= N) {
//			int j = 2 * k;
//
//			if (j < N && compare (items [j], items [j + 1]) >= 0) {
//				j++;
//			}
//
//			if (compare (items[k], items[j]) <= 0) {
//				break;
//			}
//
//			swap (k, j);
//			k = j;
//		}
//	}
//
	public void swim(int k) {
		while (k > 1 && compare (items [k / 2], items[k]) < 0) {
			swap (k, k / 2);
			k = k / 2;
		}
	}

	public void swap(int a, int b) {
		Vector2 temp = items [a];
		items [a] = items [b];
		items [b] = temp;
	}


	// Compare based on potentials
	public float compare(Vector2 a, Vector2 b) {
		float pa = g.gridPotential.get (a);
		float pb = g.gridPotential.get(b);

		if (fequal (pa, pb)) {
			return 0;
		}

		return pa - pb;
	}

	public static bool fequal(float a, float b) {
		float epsilon = 0.0001F;

		if (a == b) {
			return true;
		}

		float diff = Mathf.Abs(a - b);
		if (a * b == 0) {
			return diff < (epsilon * epsilon);
		}

		return diff / (Mathf.Abs (a) + Mathf.Abs (b)) < epsilon;
	}

//	public void heapify() {
//		// TODO: Actually check heapify...
//		int N = items.Count - 1;
//		for (int k = N / 2; k >= 0; k--) {
//			sink (k, N);
//		}
//		while (N > 1) {
//			swap(0, N--);
//			sink(0, N);
//		}
//	}

	public void printHeap() {
		float[] p = new float[items.Count - 1];

		for (int k = 1; k < items.Count; k++) {
			p[k - 1] = g.gridPotential.get (items [k]);
		}
		Console.WriteLine ("********************");
	}
}