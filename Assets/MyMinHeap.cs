using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

public class MyMinHeap {
	private static int CAPACITY = 400; // TODO: Make this useful

	private int size;
	private Node[] heap;

	public MyMinHeap () {
		size = 0;
		heap = new Node[CAPACITY];
	}

	public MyMinHeap (Node[] arr) {
		size = arr.Length;
		heap = new Node[arr.Length];
	}

	// Rearranges array in ascending order 
	public void heapify() {
		int N = size;
		for (int k = N / 2; k > 1; k--) {
			sink (k, N);
		} 
	}


	public void sink(int k, int N) {
		while (2 * k <= N) {
			int j = 2 * k;
			// Get which child is smaller
			if (j < N && heap[j].compare(heap[j + 1]) > 0)
				j++;

			float kk = heap [k].value;
			float jj = heap [j].value;

			// If Parent k < j, then break, otherwise swap with smaller
			if (heap[k].compare(heap[j]) < 0)
				break;
			swap (k, j);
			k = j;
		}
	}

	public void insert(Node a) {
		if (size == heap.Length - 1)
			doubleSize ();

		heap[++size] = a;
		swim (size);
	}

	public void doubleSize() {
		Node[] old = heap;
		heap = new Node[heap.Length * 2];
		old.CopyTo (heap, 0);
	}

	public Node peek() {
		if (size == 0) {
			throw new System.InvalidOperationException ("Empty Heap");
		}
		return heap [1];
	}

	public bool isEmpty() {
		return size == 0;
	}

	public Node removeMin() {
		if (size == 0) {
			throw new System.InvalidOperationException ("Empty Heap");
		}

		Node min = heap [1]; // Save for return
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
			if (heap [parent].compare(heap [child]) > 0) {
				return false;
			}
				
			if (child < N) {
				if (heap [parent].compare (heap [child + 1]) > 0) {
					return false;
				}
			}
		}

		// Made it through! This is in fact a min heap! :)
	 	return true;
	}

	// Reheapify if the heap order is violated if parent > child
	public void swim(int k) {
		while (k > 1 && heap [k / 2].compare (heap[k]) > 0) {
			swap (k, k / 2);
			k = k / 2;
		}
	}
	
	// Switches elements at different indicies
	public void swap(int a, int b) {
		Node temp = heap [a];
		heap [a] = heap [b];
		heap [b] = temp;
	}


	// Compare based on potentials
//	public float compare(Vector2 a, Vector2 b) {
//		float pa = g.gridPotential.get (a);
//		float pb = g.gridPotential.get (b);
//
//		if (fequal (pa, pb)) {
//			return 0;
//		}
//
//		return pa - pb;
	//	}

	// TODO: Add out of range as well
	public Node get(int i) {
		return heap [i];
	}

	public int getSize() {
		return size;
	}
}

public class Node {
	public int i;
	public int j;
	public float value;

	public Node(int x, int z, float v) {
		i = x;
		j = z;
		value = v;
	}

	public float compare(Node b) {
		if (fequal (this.value, b.value)) {
			return 0;
		}

		return this.value - b.value;
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
}