using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingBuffer<T> {
	int start, end;
	int count;
	T[] items;

	public RingBuffer(int size) {
		start = end = count = 0;
		items = new T[size];
	}

	public bool empty() {
		return start == end;
	}

	public void clear() {
		start = end;
		count = 0;
	}

	public int getCount() {
		return count;
	}

	public bool push(T item) {
		if (count < items.Length) {
			items [end] = item;
			end = (end + 1) % items.Length;
			count++;
			return true;
		}
		return false;
	}

	public T pop() {
		if (start != end) {
			T res = items [start];
			start = (start + 1) % items.Length;
			count--;
			return res;
		} else {
			return default(T);
		}
	}
}
