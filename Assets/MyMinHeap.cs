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

	// TODO: Add out of range as well
	public Node get(int i) {
		return heap [i];
	}

	public int getSize() {
		return size;
	}


	// Debugging/Testing Functions
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

	public void TestMyMinHeap() {
		Node a = new Node (0, 0, 0);
		Node b = new Node (0, 1, 1);
		Node c = new Node (0, 2, Mathf.Infinity);
		Node d = new Node (0, 4, 20);
		Node e = new Node (0, 5, 17);
		Node f = new Node (0, 6, 19);
		Node g = new Node (0, 7, 30);
		Node h = new Node (0, 8, 38);
		Node i = new Node (0, 9, 0.5f);
		Node j = new Node (0, 10, 4.5f);


		// Testing Insertion
		Console.WriteLine ("Inserting a: 0");
		insert (a);
		if (!isMinHeap()) {
			Console.WriteLine ("Failed");
		}

		Console.WriteLine ("Inserting b: 1");
		insert (b);
		if (!isMinHeap()) {
			Console.WriteLine ("Failed");
		}

		Console.WriteLine ("Inserting c: Mathf.Infinity");
		insert (c);
		if (!isMinHeap()) {
			Console.WriteLine ("Failed");
		}

		Console.WriteLine ("Inserting d: 20");
		insert (d);
		if (!isMinHeap()) {
			Console.WriteLine ("Failed");
		}

		Console.WriteLine ("Inserting e: 17");
		insert (e);
		if (!isMinHeap()) {
			Console.WriteLine ("Failed");
		}

		Console.WriteLine ("Inserting f: 19");
		insert (f);
		if (!isMinHeap()) {
			Console.WriteLine ("Failed");
		}

		Console.WriteLine ("Inserting g: 30");
		insert (g);
		if (!isMinHeap()) {
			Console.WriteLine ("Failed");
		}

		Console.WriteLine ("Inserting h: 38");
		insert (h);
		if (!isMinHeap()) {
			Console.WriteLine ("Failed");
		}
			
		Console.WriteLine ("Inserting h: 0.5f");
		insert (i);
		if (!isMinHeap()) {
			Console.WriteLine ("Failed");
		}

		Console.WriteLine ("Inserting j: 4.5f");
		insert (j);
		if (!isMinHeap()) {
			Console.WriteLine ("Failed");
		}

		// Testing popping
		Node min0 = removeMin();
		Console.WriteLine ("Removed: " + min0.value);
		if (!isMinHeap()) {
			Console.WriteLine ("Failed");
		}

		Node min1 = removeMin ();
		Console.WriteLine ("Removed: " + min1.value);
		if (!isMinHeap()) {
			Console.WriteLine ("Failed");
		}

		Node min2 = removeMin ();
		Console.WriteLine ("Removed: " + min2.value);
		if (!isMinHeap()) {
			Console.WriteLine ("Failed");
		}

		Node min3 = removeMin ();
		Console.WriteLine ("Removed: " + min3.value);
		if (!isMinHeap()) {
			Console.WriteLine ("Failed");
		}

		Node min4 = removeMin ();
		Console.WriteLine ("Removed: " + min4.value);
		if (!isMinHeap()) {
			Console.WriteLine ("Failed");
		}

		Node min5 = removeMin ();
		Console.WriteLine ("Removed: " + min5.value);
		if (!isMinHeap()) {
			Console.WriteLine ("Failed");
		}

		Node min6 = removeMin ();
		Console.WriteLine ("Removed: " + min6.value);
		if (!isMinHeap()) {
			Console.WriteLine ("Failed");
		}

		Node min7 = removeMin ();
		Console.WriteLine ("Removed: " + min7.value);
		if (!isMinHeap()) {
			Console.WriteLine ("Failed");
		}

		Node min8 = removeMin ();
		Console.WriteLine ("Removed: " + min8.value);
		if (!isMinHeap()) {
			Console.WriteLine ("Failed");
		}

		Node min9 = removeMin ();
		Console.WriteLine ("Removed: " + min9.value);
		if (!isMinHeap()) {
			Console.WriteLine ("Failed");
		}

//		Node min10 = removeMin ();
//		Console.WriteLine ("Removed: " + min10.value);
//		if (!isMinHeap()) {
//			Console.WriteLine ("Failed");
//		}
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