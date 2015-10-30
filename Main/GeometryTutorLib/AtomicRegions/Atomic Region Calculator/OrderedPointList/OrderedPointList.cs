using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.Area_Based_Analyses.Atomizer
{
    //
    // A lexicographic ordering of a list of points.
    //
    public class OrderedPointList
    {
        List<Point> ordered;

        //
        // Creates the Min-heap array and places the smallest value possible in array position 0
        //
        public OrderedPointList()
        {
            ordered = new List<Point>();
        }

        public bool IsEmpty() { return ordered.Count == 0; }

        //
        // Inserts an element
        //
        private void Insert(Point thatNode)
        {
            // Empty list: add to beginning.
            if (!ordered.Any())
            {
                ordered.Add(thatNode);
                return;
            }

            // General insertion
            int n;
            for (n = 0; n < ordered.Count; n++)
            {
                if (Point.LexicographicOrdering(thatNode, ordered[n]) <= 0)
                {
                    break;
                }
            }

            ordered.Insert(n, thatNode);
        }

        public void Add(Point pt)
        {
            Insert(pt);
        }

        //
        // Removes the node at the first position: O(log n) due to Heapify
        //
        public Point ExtractMin()
        {
            if (!ordered.Any()) return null;

            Point min = ordered[0];
            ordered.RemoveAt(0);
            return min;
        }

        public Point PeekMin()
        {
            return ordered[0];
        }

        public void Remove(Point pt)
        {
            ordered.Remove(pt);
        }

        //
        // For debugging purposes: traverse the list and dump (key, data) pairs
        //
        public override String ToString()
        {
            String retS = "";

            // Traverse the array and dump the (key, data) pairs
            for (int n = 0; n < ordered.Count; n++)
            {
                retS += "(" + n + ": " + ordered[n] + ") ";
            }

            return retS + "\n";
        }

    }
}
