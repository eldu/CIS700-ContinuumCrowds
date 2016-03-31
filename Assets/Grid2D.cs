using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid2D {
	public float[] data;
	private int resx, resz;

	// -----------------------------------------------------------------------------
	// Constructor
	// ----------------------------------------------------------------------------- 
	public Grid2D(int resx, int resz) {
		this.resx = resx;
		this.resz = resz;

		data = new float[resx * resz];
	}


	// -----------------------------------------------------------------------------
	// Getters and Setters for Data
	// Override operators basically (TODO: Actually override operators)
	// -----------------------------------------------------------------------------
	public float getVal(int idx) {
		return  data[idx];
	}

	public float getVal(Vector2 index) {
		int idx = getIdxFromIdxVec2(index);
		return  data[idx];
	}

	public void setVal(int idx, float value) {
		data [idx] = value;
	}

	public void setVal(Vector2 idx, float value) {
		data [getIdxFromIdx (idx)] = value;
	}

	public void addVal(int idx, float value) {
		data [idx] += value;
	}

	public void addVal(Vector2 idx, float value) {
		data [getIdxFromIdx (idx)] += value;
	}


	// -----------------------------------------------------------------------------
	// Indexing Mania (Hash functions)
	// -----------------------------------------------------------------------------
	public Vector2 getIdxVec2FromIdx(int idx) {
		int i = idx / resx;
		int j = idx - (i * resx);

		return new Vector2 (i, j);
	}

	public int getIdxFromIdxVec2(Vector2 idx) {
		return getIdxFromIdx ((int) idx [0], (int) idx [1]);
	}

	public int getIdxFromIdx(Vector2 idx) {
		return getIdxFromIdx ((int) idx [0], (int) idx [1]);
	}

	public int getIdxFromIdx(int i, int j) {
		// Check if in the range, otherwise return -1
		if (i < 0 || j < 0 || i >= resx || j >= resz) {
			return -1;
		}

		return i * resx + j;
	}

	// Get the index of the grid from a point on the local grid
	public int getIdxFromPos(Vector2 p) {
		int i = (int) (p[0] / resx);
		int j = (int) (p[1] / resz);

		return getIdxFromIdx (i, j);
	}

	public Vector2 getIdxVector2FromPos(Vector2 p) {
		int i = (int) (p[0] / resx);
		int j = (int) (p[1] / resz);

		return new Vector2 (i, j);
	}

	// -----------------------------------------------------------------------------
	// Get Neighbors
	// -----------------------------------------------------------------------------


	// Get neighbors including itself
	public List<int> getNeighbors(Vector2 pos) {
		int idx = getIdxFromPos (pos);
		return getNeighbors (idx);
	}


	public List<int> getNeighbors(int idx) {
		List<int> result = new List<int>();
		Vector2 idxVec2 = getIdxVec2FromIdx(idx);

		int i = (int) idxVec2[0];
		int j = (int) idxVec2[1];


		int[] neighbors = new int[4];
		neighbors[0] = getIdxFromIdx (i, j);
		neighbors[1] = getIdxFromIdx (i + 1, j);
		neighbors[2] = getIdxFromIdx (i, j + 1);
		neighbors[4] = getIdxFromIdx (i + 1, j + 1);

		for (int k = 0; k < 4; k ++) {
			if (neighbors[k] >= 0) {
				result.Add(neighbors[k]);
			}
		}
		return result;

	}


	// -----------------------------------------------------------------------------
	// Get Cooridnate Positions
	// -----------------------------------------------------------------------------
	public Vector2 getCoordVector2(Vector2 idx) {
		return new Vector2 (idx [0] + 0.5f, idx [1] + 0.5f);
	}


	// -----------------------------------------------------------------------------
	// Get the closest bottom left data point
	// -----------------------------------------------------------------------------
	public Vector2 getA(Vector2 p) {
		Vector2 idx = getIdxVector2FromPos (p);
		Vector2 c1 = getCoordVector2(idx);

		if (p [0] >= c1 [0]) {
			if (p [1] >= c1 [1]) {
				// Cell A, bottom left
				return idx;
			} else {
				// Cell D, top left
				return new Vector2(idx[0] - 1, idx[1]);
			}
		} else {
			if (p [1] >= c1 [1]) {
				// Cell B, bottom right
				return new Vector2(idx[0], idx[1] - 1);
			} else {
				// Cell C, top right
				return new Vector2(idx[0] - 1, idx[1] - 1);
			}
		}
	}

	// -----------------------------------------------------------------------------
	// Bilinear Interpolation
	// Input: pos = position in local space
	// -----------------------------------------------------------------------------
	public float bilinear(Vector2 pos) {
		// A = 00, B = 01, C = 11, D = 10 
		Vector2 Aidx = getA (pos);
		Vector2 Bidx = new Vector2 (Aidx [0], Aidx [1] + 1);
		Vector2 Cidx = new Vector2 (Aidx [0] + 1, Aidx [1] + 1);
		Vector2 Didx = new Vector2 (Aidx [0] + 1, Aidx [1]);

		Vector2 Acoord = getCoordVector2(Aidx);

		float dx = pos [0] - Acoord [0];
		float dy = pos [1] - Acoord [1];

		float c00 = getVal(Aidx);
		float c01 = getVal(Bidx);
		float c10 = getVal(Didx);
		float c11 = getVal(Cidx);

		// Interpolate on x
		float a = c00 * (1 - dx) + c10 * dx;
		float b = c01 * (1 - dx) + c11 * dx;

		// Interpolate on y
		return a * (1 - dy) + b * dy;
	}
}
