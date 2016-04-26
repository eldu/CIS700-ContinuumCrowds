using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class MyMinHeap {
	private MACGrid g;
	private List<Vector2> items = new List<Vector2>();

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
		while (2 * k < items.Count) {
			int j = 2 * k;
			if (j < items.Count - 1 && compare (items [j], items [j + 1]) < 0) {
				j++;
			}

			if (compare (items[k], items[j]) >= 0) {
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
		return g.gridPotential.get (a)  - g.gridPotential.get(b);
	}
}