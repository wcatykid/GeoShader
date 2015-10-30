using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Pebbler;

namespace GeometryTutorLib.Hypergraph
{
    //
    // This class implements a Fibonacci heap data structure based on the algorithms in CLRS. The amortized running time
    // of most of these methods is constant: O(1); ExtractMin() and delete() are O(log n) since they require consolidation.
    // DecreaseKey is amortized O(1). In our implementation, we do NOT require the union operation so it is not implemented.
    //
    public class FibonacciHeap<T>
    {
        // Constant that allows us to create an array of the appropriate size for consolidation
        private static double oneOverLogPhi = 1.0 / Math.Log((1.0 + Math.Sqrt(5.0)) / 2.0);

        // Pointer to the minimum node in the heap (facilitating O(1) peek and O(log n) amortized ExtractMin)
        private HeapNode<T> minNode;

        // Number of elements in the heap
        private int numNodes;

        //
        // Standard default constructor
        //
        public FibonacciHeap()
        {
            minNode = null;
            numNodes = 0;
        }

        //
        // Standard IsEmpty method
        //
        public bool IsEmpty() { return numNodes == 0; }

        //
        // A method to peek at the minimum element in the heap
        //
        public HeapNode<T> Min() { return minNode; }

        //
        // Clear the heap completely
        //
        public void Clear()
        {
            minNode = null;
            numNodes = 0;
        }

        //
        // Decreases the key value for a heap node, given the new value to take on.
        // The structure of the heap may be changed and will not be consolidated.
        //
        public void DecreaseKey(HeapNode<T> x, double k)
        {
            if (k > x.key) throw new ArgumentException("DecreaseKey() got larger key value");

            // set the new key
            x.key = k;

            HeapNode<T> y = x.parent;

            if (y != null && x.key < y.key)
            {
                Cut(x, y);
                CascadingCut(y);
            }

            // Update the min node (if needed)
            if (x.key < minNode.key)
            {
                minNode = x;
            }
        }

        //
        // Deletes a node from the heap given the reference to the node.
        //
        public void Delete(HeapNode<T> x)
        {
            // make x minimal and extract
            DecreaseKey(x, Double.NegativeInfinity);
            ExtractMin();
        }

        //
        // Insert an element lazily: O(1) amortized time by adding to the root list
        //
        public void Insert(HeapNode<T> node, double key)
        {
            // Set the key
            node.key = key;

            // We've added a node
            numNodes++;

            if (minNode == null)
            {
                minNode = node;
                return;
            }

            // attach node to the min node list
            node.left = minNode;
            node.right = minNode.right;
            minNode.right = node;
            node.right.left = node;

            // Update min pointer
            if (key < minNode.key)
            {
                minNode = node;
            }
        }

        //
        // Removes the min node from the heap, consolidates, and returns the minimum value
        // in amortized O(log n) time
        //
        public HeapNode<T> ExtractMin()
        {
            if (minNode == null) return null;

            HeapNode<T> oldMin = minNode;
            int numChildren = minNode.degree;
            HeapNode<T> child = minNode.child;

            //
            // for all the children of the min node pointer
            // cut the children and add to the root list
            //
            while (numChildren-- > 0) // Each iteration we have one fewer children
            {
                HeapNode<T> tempRight = child.right;

                // remove x from child list
                child.left.right = child.right;
                child.right.left = child.left;

                // Re-attach the child node to the root list
                child.left = minNode;
                child.right = minNode.right;
                minNode.right = child;
                child.right.left = child;

                // set the parent to null
                child.parent = null;

                // Move to the next child in the min-pointer children
                child = tempRight;
            }

            // remove the old min node from the root list
            oldMin.left.right = oldMin.right;
            oldMin.right.left = oldMin.left;

            // If this was the only node in the root list
            if (oldMin == oldMin.right)
            {
                minNode = null;
            }
            else
            {
                // Otherwise, reassign the min node arbitrarily and perform consolidation 
                minNode = oldMin.right;
                consolidate();
            }

            // We have successfully updated the heap, now decrement the size
            numNodes--;

            return oldMin;
        }

        //
        // Return the number of elements in the heap
        //
        public int Size()
        {
            return numNodes;
        }

        //
        // Performs cascading cut operation: cuts y from its parent and adds y to the root list.
        // Then, if the parent is marked, we continue cutting (and so forth up the tree)
        //
        private void CascadingCut(HeapNode<T> y)
        {
            if (y.parent == null) return;

            HeapNode<T> z = y.parent;


            // if y is unmarked, mark it
            if (!y.mark)
            {
                y.mark = true;
            }
            else // We have a marked node: cut it from parent
            {
                // cut y from its parent (z)
                Cut(y, z);

                // cut z, y's parent as well
                CascadingCut(z);
            }
        }

        //
        // Performs consolidation of all the root nodes with the same degree.
        //
        private void consolidate()
        {
            if (minNode == null) return;

            int arraySize = ((int)Math.Floor(Math.Log(numNodes) * oneOverLogPhi)) + 1;

            // Use an array to perform consolidation
            // If two elements in the root list have the same degree, we combine accordingly
            // Java should initialize the array to be all null elements
            HeapNode<T>[] array = (HeapNode<T>[])new HeapNode<T>[arraySize];

            //
            // Traverse the root list to determine the number of root nodes
            //
            // We know the minNode contains at least one node (since minNode is non-null at this point)
            int numRoots = 1;
            HeapNode<T> x = minNode.right;

            while (x != minNode)
            {
                numRoots++;
                x = x.right;
            }

            //
            // For each element in the root list, perform consolidation
            // (if we have two nodes with the same degree)
            // each time we reduce the number of root nodes (hence, --)
            //
            while (numRoots-- > 0)
            {
                int degree = x.degree;
                HeapNode<T> next = x.right;

                // Search for another node with the same degree
                for (; ; )
                {
                    HeapNode<T> y = array[degree];

                    // if there are no other elements of the same degree, break
                    if (y == null) break;

                    // We have two nodes of the same degree: make one a child of the other.
                    // Swap to ensure x is always the parent
                    if (x.key > y.key)
                    {
                        HeapNode<T> temp = y;
                        y = x;
                        x = temp;
                    }

                    // Make 
                    Link(y, x);

                    // We've handled this degree, go to next one.
                    array[degree] = null;
                    degree++;
                }

                // We have a node of greater degree now.
                array[degree] = x;

                // Walk to the next element in the root list.
                x = next;
            }

            //
            // Rebuild the root list from the consolidation array
            //
            minNode = null;

            // Find the first non-null element and make it the min-node
            int i;
            for (i = 0; i < array.Length && array[i] == null; i++) ;
            minNode = array[i];

            // For all remaining elements in the array list
            for (; i < array.Length; i++)
            {
                // If there is an element of degree i
                if (array[i] != null)
                {
                    // Use y for simplicity
                    HeapNode<T> y = array[i];

                    // Remove y from root list.
                    y.left.right = y.right;
                    y.right.left = y.left;

                    // Re-attach y to the root list
                    y.left = minNode;
                    y.right = minNode.right;
                    minNode.right = y;
                    y.right.left = y;

                    // Update the min pointer
                    if (y.key < minNode.key)
                    {
                        minNode = y;
                    }
                }
            }
        }

        //
        // Removes x from the children of y
        //
        private void Cut(HeapNode<T> x, HeapNode<T> y)
        {
            // remove x from the children of y
            x.left.right = x.right;
            x.right.left = x.left;

            // y lost a child, and indicate it
            y.degree--;

            // if other children than x, set y's pointer to the next child
            if (y.child == x) y.child = x.right;

            // indicate no children, if needed
            if (y.degree == 0) y.child = null;

            // add x to root list of heap
            x.left = minNode;
            x.right = minNode.right;
            minNode.right = x;
            x.right.left = x;

            // indicate x has no parent
            x.parent = null;

            // indicate x is unmarked
            x.mark = false;
        }

        //
        // Make node y a child of x.
        //
        private void Link(HeapNode<T> y, HeapNode<T> x)
        {
            // remove y from root list of heap
            y.left.right = y.right;
            y.right.left = y.left;

            // make y a child of x
            y.parent = x;

            // If x has no children, set y to be the child of x as well as
            // y pointing to itself (as it is the only child of x)
            if (x.child == null)
            {
                x.child = y;
                y.right = y;
                y.left = y;
            }
            else
            {
                // Attach y to the list of x's children
                y.left = x.child;
                y.right = x.child.right;
                x.child.right = y;
                y.right.left = y;
            }

            // increase x's degree since we added another child
            x.degree++;

            // unmark y
            y.mark = false;
        }
    }
}
