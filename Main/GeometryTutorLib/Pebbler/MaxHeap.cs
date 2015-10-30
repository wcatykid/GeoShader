using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.Pebbler
{
    //
    // A classic Max-heap implementation done so with an array 
    // ExtractMax and decreaseKey are O(log n)
    //
    public class MaxHeap
    {
        private HeapNode<int>[] heap;
        public int Count { get; private set; }

        //
        // Creates the Max-heap array and places the smallest value possible in array position 0
        //
        public MaxHeap(int sz)
        {
            heap = new HeapNode<int>[sz + 5]; // +5 for safety
            Count = 0;

            //
            // Make index 0 a sentinel value
            //
            HeapNode<int> node = new HeapNode<int>(-1);
            node.key = Double.PositiveInfinity;
            heap[0] = node;
        }

        private int LeftChildIndex(int n) { return 2 * n; }
        private int ParentIndex(int n) { return n / 2; }
        private bool IsLeaf(int n) { return n > Count / 2 && n <= Count; }
        public bool IsEmpty() { return Count == 0; }

        //
        // Swaps two nodes in the array
        // Necessary since we need to update the indices of the nodes in the array (for decreaseKey)
        //
        private void Swap(int index1, int index2)
        {
            // Swap the nodes
            HeapNode<int> temp = heap[index1];
            heap[index1] = heap[index2];
            heap[index2] = temp;

            // Update the array indices
            heap[index1].degree = index1;
            heap[index2].degree = index2;
        }

        //
        // Makes the given node priority the newKey value and updates the heap
        // O(log n)
        //
        public void DecreaseKey(HeapNode<int> node, int newKey)
        {
            int index = node.degree;
            node.key = newKey;

            // Moves the value up the heap until a suitable point is determined
            while (index > 0 && heap[ParentIndex(index)].key <= heap[index].key)
            {
                Swap(index, ParentIndex(index));
                index = ParentIndex(index);
            }
        }

        //
        // Inserts an element (similar to Insertion; update the heap)
        // O(log n)
        //
        public void Insert(HeapNode<int> node, int key)
        {
            // Add the node to the last open position
            node.key = key;
            node.degree = ++Count;
            heap[Count] = node;

            // Moves the value up the heap until a suitable point is determined
            int current = Count;
            while (heap[current].key >= heap[ParentIndex(current)].key)
            {
                Swap(current, ParentIndex(current));
                current = ParentIndex(current);
            }
        }

        //
        // Removes the node at the first position: O(log n) due to Heapify
        //
        public HeapNode<int> ExtractMax()
        {
            // Switch the first (Max) with the last and re-Heapify
            Swap(1, Count);
            if (--Count != 0) Heapify(1);

            // Return the minimal value
            return heap[Count + 1];
        }

        //
        // Heapify based on the given index
        //
        private void Heapify(int index)
        {
            int maxChild;

            while (!IsLeaf(index))
            {
                maxChild = LeftChildIndex(index);

                // If we're within the bounds of the valid data
                if (maxChild < Count)
                {
                    // left child is greater than the right child
                    if (heap[maxChild].key <= heap[maxChild + 1].key)
                    {
                        maxChild++;
                    }
                }

                if (heap[index].key > heap[maxChild].key) return;

                Swap(index, maxChild);

                index = maxChild;
            }
        }

        //
        // For debugging purposes: traverse the list and dump (key, data) pairs
        //
        public override String ToString()
        {
            String retS = "";

            // Traverse the array and dump the (key, data) pairs
            for (int i = 1; i <= Count; i++)
            {
                retS += "(" + heap[i].key + ", " + heap[i].data + ") ";
            }

            return retS + "\n";
        }

    }
}
