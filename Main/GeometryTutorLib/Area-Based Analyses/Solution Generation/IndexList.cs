using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.Area_Based_Analyses
{
    // A class representing a list of integers;
    // Implements set-based functionality and a nice hash function.
    public class IndexList
    {
        public List<int> orderedIndices { get; private set; }

        public IndexList() { orderedIndices = new List<int>(); }

        public int Count
        {
            get { return orderedIndices.Count; }
        }

        public IndexList(int index)
        {
            orderedIndices = new List<int>();
            orderedIndices.Add(index);
        }

        public IndexList(List<int> indices)
        {
            orderedIndices = indices;
        }

        public bool IsEmpty() { return !orderedIndices.Any(); }

        public void Add(int newIndex)
        {
            if (orderedIndices.Contains(newIndex)) return;

            orderedIndices.Add(newIndex);
            orderedIndices.Sort();
        }

        public void Add(List<int> newIndices) { newIndices.ForEach(i => Add(i)); }

        public bool EqualSets(IndexList that) { return Utilities.EqualOrderedSets(this.orderedIndices, that.orderedIndices); }

        public override bool Equals(object obj)
        {
            IndexList that = obj as IndexList;
            if (that == null) return false;

            return EqualSets(that);
        }

        // Create a unique hash code for the list
        // Ex. 1 2 3 4 10 would result in a hash code of 104321
        // If more than 8 indices, overlay the values: 0 1 2 3 4 5 6 7  8 9 10  becomes 76543210 + 1098 =  
        public override int GetHashCode()
        {
            // const int NUM_VALUES_BEFORE_CUT = 8;
            int runningHashCode = 0;

            string hashcode = "";
            for (int i = 0; i < orderedIndices.Count; i++)
            {
                // A reset
                if (hashcode.Length > 7) // i > 0 && i % NUM_VALUES_BEFORE_CUT == 0)
                {
                    runningHashCode += Int32.Parse(hashcode);
                    hashcode = "";
                }
                // Normal concatenation.
                else
                {
                    hashcode = orderedIndices[i].ToString() + hashcode;
                }
            }

            if (hashcode != "")
            {
                runningHashCode += Int32.Parse(hashcode);
            }

            return runningHashCode;
        }

        public static IndexList UnionIndices(IndexList left, IndexList right)
        {
            List<int> orderedUnion = Utilities.UnionIfDisjointOrderedSets(left.orderedIndices, right.orderedIndices);

            return orderedUnion == null ? null : new IndexList(orderedUnion);
        }

        public static IndexList DifferenceIndices(IndexList left, IndexList right)
        {
            List<int> difference = Utilities.DifferenceIfSubsetOrderedSets(left.orderedIndices, right.orderedIndices);

            return difference == null ? null : new IndexList(difference);
        }

        public static IndexList AcquireAtomicRegionIndices(List<Atomizer.AtomicRegion> complete, List<Atomizer.AtomicRegion> selections)
        {
            IndexList indices = new IndexList();

            foreach (Atomizer.AtomicRegion atom in selections)
            {
                int index = complete.IndexOf(atom);
                if (index == -1) throw new Exception("Atomic region not found during solution generations: " + atom.ToString());
                indices.Add(index);
            }

            return indices;
        }

        public override string ToString()
        {
            string retString = "";
            foreach (int index in orderedIndices)
            {
                retString += index + " ";
            }
            return retString;
        }
    }
}
