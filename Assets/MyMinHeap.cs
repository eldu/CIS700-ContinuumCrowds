using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class MyMinHeap {
	private MACGrid g;
	public List<Vector2> items = new List<Vector2>();

	public MyMinHeap (MACGrid grid) {
		g = grid;
	}

	public int count {
		get { return items.Count; }
	}

	public void clear() {
		items.Clear ();
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

	public Vector2 removeMin() {
		if (items.Count == 0) {
			throw new System.InvalidOperationException ("Empty Heap");
		}

		Vector2 min = items [0];

		// Swap with Last Item
		Vector2 temp = items[items.Count - 1];
		items [0] = temp;
		items.RemoveAt (items.Count - 1);
		sink (0);


		return min;
	}
		
	public void sink(int k) {
		// TODO: YOU HAVE AN INFINITE LOOP
		while (2 * k < items.Count) {
			int j = 2 * k;

			float what = compare (items [j], items [j + 1]);

			if (j < items.Count - 1 && what >= 0) {
				j++;
			}

			if (compare (items[k], items[j]) <= 0) {
				break;
			}

			swap (k, j);
			k = j;
		}
	}

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

	public bool fequal(float a, float b) {
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
}