using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.Pebbler
{
    //
    // Implements a node for the Minimum container (can use MinHeap or Fibonacci Heap).
    // It holds the information necessary for maintaining the structure of the Fibonacci heap (which is the most demanding).
    //
    public class HeapNode<T>
    {
        public T data;
        public HeapNode<T> child;
        public HeapNode<T> left;
        public HeapNode<T> parent;
        public HeapNode<T> right;
        public bool mark; // Marking for the Fibonacci heap: if this node has had a child removed
        public double key;
        public int degree; // number of children in Fibonacci Heap; index in the min-heap

        public HeapNode(T data)
        {
            right = this;
            left = this;
            this.data = data;
        }

        public override string ToString() { return System.Convert.ToString(key); }
    }
}
