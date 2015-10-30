using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.Area_Based_Analyses
{
    public class SolutionDatabase
    {
        private Dictionary<IndexList, SolutionAgg> solutions;

        public SolutionDatabase(int size)
        {
            solutions = new Dictionary<IndexList,SolutionAgg>(size);
        }

        //
        // If the solution exists in the root solutions as incomputable, seek a solution from the extended
        //
        public bool TryGetValue(IndexList indices, out SolutionAgg solutionAgg)
        {
            return solutions.TryGetValue(indices, out solutionAgg);
        }

        public bool Contains(SolutionAgg solution)
        {
            return Contains(solution.atomIndices);
        }

        public bool Contains(IndexList indices)
        {
            return solutions.ContainsKey(indices);
        }

        public List<SolutionAgg> GetComputableSolutions()
        {
            List<SolutionAgg> computable = new List<SolutionAgg>();

            foreach (KeyValuePair<IndexList, SolutionAgg> pair in solutions)
            {
                if (pair.Value.solType == SolutionAgg.SolutionType.COMPUTABLE) computable.Add(pair.Value);
            }

            return computable;
        }

        public List<SolutionAgg> GetIncomputableSolutions()
        {
            List<SolutionAgg> incomputable = new List<SolutionAgg>();

            foreach (KeyValuePair<IndexList, SolutionAgg> pair in solutions)
            {
                if (pair.Value.solType == SolutionAgg.SolutionType.INCOMPUTABLE) incomputable.Add(pair.Value);
            }

            return incomputable;
        }

        public List<SolutionAgg> GetSolutions()
        {
            return new List<SolutionAgg>(solutions.Values);
        }

        public bool AddSolution(SolutionAgg that)
        {
            return AddSolution(solutions, that);
        }

        public int GetNumComputable()
        {
            int computable = 0;
            foreach (KeyValuePair<IndexList, SolutionAgg> pair in solutions)
            {
                if (pair.Value.solType == SolutionAgg.SolutionType.COMPUTABLE) computable++;
            }
            return computable;
        }

        public int GetNumAtomicComputable()
        {
            int computable = 0;
            foreach (KeyValuePair<IndexList, SolutionAgg> pair in solutions)
            {
                if (pair.Value.solType == SolutionAgg.SolutionType.COMPUTABLE)
                {
                    if (pair.Value.atomIndices.Count == 1) computable++;
                }
            }
            return computable;
        }

        public int GetNumIncomputable()
        {
            int incomputable = 0;

            foreach (KeyValuePair<IndexList, SolutionAgg> pair in solutions)
            {
                if (pair.Value.solType == SolutionAgg.SolutionType.INCOMPUTABLE) incomputable++;
            }

            return incomputable;
        }
        
        //
        // Acquire a single solution equation and area value.
        //
        public KeyValuePair<ComplexRegionEquation, double> GetSolution(List<Atomizer.AtomicRegion> figureAtoms, List<Atomizer.AtomicRegion> desiredRegions)
        {
            IndexList indices = IndexList.AcquireAtomicRegionIndices(figureAtoms, desiredRegions);

            SolutionAgg solutionAgg = null;
            if (!solutions.TryGetValue(indices, out solutionAgg))
            {
                throw new ArgumentException("Could not find a solution in the database.");
            }

            return new KeyValuePair<ComplexRegionEquation, double>(solutionAgg.solEq, solutionAgg.solArea);
        }

        //
        // Acquire a single solution equation and area value.
        //
        public SolutionAgg GetSolutionAgg(List<Atomizer.AtomicRegion> figureAtoms, List<Atomizer.AtomicRegion> desiredRegions)
        {
            IndexList indices = IndexList.AcquireAtomicRegionIndices(figureAtoms, desiredRegions);

            SolutionAgg solutionAgg = null;
            if (!solutions.TryGetValue(indices, out solutionAgg))
            {
                throw new ArgumentException("Could not find a solution in the database.");
            }

            return solutionAgg;
        }

        //
        // Adds an equation, if it does not exist.
        // If an equation for the region already exists, take the shortest one or the one that is computable.
        //
        private bool AddSolution(Dictionary<IndexList, SolutionAgg> solDictionary, SolutionAgg that)
        {
            //
            // Does this equation NOT exist in the database?
            //
            SolutionAgg existentAgg = null;
            if (!solDictionary.TryGetValue(that.atomIndices, out existentAgg))
            {
                // Add this solution to the database.
                solDictionary.Add(that.atomIndices, that);

                return true;
            }

            //
            // The equation already exists in the database.
            //
            return UpdateSolution(solDictionary, existentAgg, that);
        }

        //
        // The equation already exists in the database.
        //
        private bool UpdateSolution(Dictionary<IndexList, SolutionAgg> solDictionary, SolutionAgg existentAgg, SolutionAgg that)
        {
            // Favor a straight-forward calculation of the area (no manipulations to acquire the value).
            if (existentAgg.IsDirectArea()) return false;

            // Favor a coomputable equation over incomputable.
            if (existentAgg.solType == SolutionAgg.SolutionType.INCOMPUTABLE && that.solType == SolutionAgg.SolutionType.COMPUTABLE)
            {
                solDictionary[that.atomIndices] = that;
                return true;
            }
            // Again, favor a computable solution over not.
            else if (existentAgg.solType == SolutionAgg.SolutionType.COMPUTABLE && that.solType == SolutionAgg.SolutionType.INCOMPUTABLE)
            {
                // NO-OP
            }
            // The computability is the same for both equations.
            else if (existentAgg.solType == that.solType || existentAgg.solType == SolutionAgg.SolutionType.UNKNOWN)
            {
                if (!Utilities.CompareValues(existentAgg.solArea, that.solArea))
                {
                    throw new Exception("Area for region " + existentAgg.atomIndices.ToString() +
                                        " was calculated now as (" + existentAgg.solArea + ") AND before (" + that.solArea + ")");
                }

                if (PreferNewComputableSolution(existentAgg, that))
                {
                    solDictionary[that.atomIndices] = that;
                    return true;
                }
            }

            return false;
        }

        //
        // Given that both the old and new solutions are computable, how to do we prefer one solution over another?
        //
        private bool PreferNewComputableSolution(SolutionAgg existent, SolutionAgg newSolution)
        {
            bool exisDefShapes = existent.solEq.DefinedByShapes();
            bool newDefShapes = newSolution.solEq.DefinedByShapes();

            //
            // Favor a solution consisting of all shapes over the alternative.
            //
            if (exisDefShapes && !newDefShapes) return false;
            if (!exisDefShapes && newDefShapes) return true;

            //
            // Favor a shorter solution.
            //
            return existent.solEq.Length > newSolution.solEq.Length;
        }
    }
}