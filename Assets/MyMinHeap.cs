using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

public class MyMinHeap {
	private static int CAPACITY = 400; // TODO: Make this useful

	private int size;
	private Vector2[] heap;


	private MACGrid g;
//	public List<Vector2> items = new List<Vector2>();

	public MyMinHeap (MACGrid grid) {
		g = grid;
		size = 0;
		heap = new Vector2[CAPACITY];
	}

	public MyMinHeap (Vector2[] arr) {
		size = arr.Length;
		heap = new Vector2[arr.Length];
	}

	// Rearranges array in ascending order 
	public void heapify() {
		int N = size;
		for (int k = N / 2; k > 1; k--) {
			sink (k, N);
		} 
//		while (N > 1) { 
//			swap (1, N--);
//			sink (1, N);
//		}
	}


	public void sink(int k, int N) {
		while (2 * k <= N) {
			int j = 2 * k;
			// Get which child is smaller
			if (j < N && compare (heap[j], heap[j + 1]) > 0)
				j++;

			float kk = g.gridPotential.get (heap [k]);
			float jj = g.gridPotential.get (heap [j]);

			// If Parent k < j, then break, otherwise swap with smaller
			if (compare (heap[k], heap[j]) < 0)
				break;
			swap (k, j);
			k = j;
		}
	}

	public void insert(Vector2 a) {
		if (size == heap.Length - 1)
			doubleSize ();

		heap[++size] = a;
		swim (size);
	}

	public void doubleSize() {
		Vector2[] old = heap;
		heap = new Vector2[heap.Length * 2];
		old.CopyTo (heap, 0);
	}

	public Vector2 peek() {
		if (size == 0) {
			throw new System.InvalidOperationException ("Empty Heap");
		}
		return heap [1];
	}

	public bool isEmpty() {
		return size == 0;
	}

	public Vector2 removeMin() {
		if (size == 0) {
			throw new System.InvalidOperationException ("Empty Heap");
		}

		Vector2 min = heap [1]; // Save for return
		heap [1] = heap [size--];

		if (size > 1) {
			sink (1, size);
		}

		return min;
	}


	public bool isMinHeap() {
		// if small size then already sorted
		if (size <= 1) {
			return true;
		}

		int N = size;
		for (int parent = 1; parent <= N / 2; parent++) {
			int child = parent * 2;

			// If the parent is greater than the child, then false
			if (compare (heap [parent], heap [child]) > 0) {
				return false;
			}
				
			if (child < N) {
				if (compare (heap [parent], heap [child + 1]) > 0) {
					return false;
				}
			}
		}

		// Made it through! This is in fact a min heap! :)
	 	return true;
	}

	// Reheapify if the heap order is violated if parent > child
	public void swim(int k) {
		while (k > 1 && compare (heap [k / 2], heap[k]) > 0) {
			swap (k, k / 2);
			k = k / 2;
		}
	}
	
	// Switches elements at different indicies
	public void swap(int a, int b) {
		Vector2 temp = heap [a];
		heap [a] = heap [b];
		heap [b] = temp;
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

	// TODO: Add out of range as well
	public Vector2 get(int i) {
		return heap [i];
	}

	public int getSize() {
		return size;
	}
}