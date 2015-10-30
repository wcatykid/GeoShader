using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;
using GeometryTutorLib.GenericInstantiator;
using GeometryTutorLib.Pebbler;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.Area_Based_Analyses
{
    public class AreaSolutionGenerator
    {
        private SolutionDatabase solutions;
        private List<Figure> topShapeForest;
        private Dictionary<IndexList, Figure> figureIndexMap;
        private List<AtomicRegion> figureAtoms;

        //
        // Initialize with the list of figures in the drawing. Do we need that knowledge?
        //
        public AreaSolutionGenerator(List<Figure> forest, List<AtomicRegion> atoms)
        {
            topShapeForest = forest;
            figureAtoms = atoms;
            figureIndexMap = new Dictionary<IndexList, Figure>();

            // The initial size of the solution dictionary.
            int initSize = (int)Math.Min(Math.Pow(2, atoms.Count), Math.Pow(2, 12));

            solutions = new SolutionDatabase(initSize);
        }

        //
        // Assuming all solutions have been generated, acquire a single solution equation and area value.
        //
        public KeyValuePair<ComplexRegionEquation, double> GetSolution(List<AtomicRegion> desiredRegions)
        {
            return solutions.GetSolution(figureAtoms, desiredRegions);
        }

        //
        // Count the number of calculable regions.
        //
        public int GetNumComputable()
        {
            return solutions.GetNumComputable();
        }

        //
        // Count the number of calculable regions.
        //
        public int GetNumAtomicComputable()
        {
            return solutions.GetNumAtomicComputable();
        }

        //
        // Count the number of calculable regions.
        //
        public int GetNumIncomputable()
        {
            return solutions.GetNumIncomputable();
        }

        //
        // Catalyst routine to the recursive solver: returns solution equation and actual area.
        //
        public void SolveAll(KnownMeasurementsAggregator known, List<Figure> allFigures)
        {
            PreprocessAtomAreas(known);
            PreprocessShapeHierarchyAreas(known, allFigures);

            //
            // Using the atomic regions, explore all of the top-most shapes recursively.
            //
            for (int a = 0; a < figureAtoms.Count; a++)
            {
                IndexList atomIndexList = new IndexList(a);
                SolutionAgg agg = null;

                solutions.TryGetValue(atomIndexList, out agg);

                if (agg == null)
                {
                    Figure topShape = figureAtoms[a].GetTopMostShape();

                    // Shape Region?
                    ComplexRegionEquation startEq = new ComplexRegionEquation(null, new ShapeRegion(topShape));
                    double outerArea = topShape.GetArea(known);

                    // Invoke the recursive solver using the outermost region and catalyst.
                    //ProcessChildrenShapes(a, new ShapeRegion(topShape), topShape.Hierarchy(),
                    //             new List<TreeNode<Figure>>(),
                    //             startEq, outerArea, known);
                    SolveHelper(new ShapeRegion(topShape),
                                topShape.Hierarchy().Children(),
                                startEq, outerArea, known);
                }
                else if (agg.solType == SolutionAgg.SolutionType.COMPUTABLE)
                {
                    //solutions[atomIndexList] = agg;
                }
                else if (agg.solType == SolutionAgg.SolutionType.INCOMPUTABLE)
                {
                    //solutions[atomIndexList] = agg;
                }
                else if (agg.solType == SolutionAgg.SolutionType.UNKNOWN)
                {
                    //TBD
                }
            }

            //
            // Subtraction of shapes extracts as many atomic regions as possible of the strict atomic regions, now compose those together.
            //
            ComposeAllRegions();
        }

        //
        // Given a shape that owns the atomic region, recur through the resulting atomic region
        //
        // From 
        //
        public void SolveHelper(Region currOuterRegion, List<TreeNode<Figure>> currHierarchyRoots,
                                ComplexRegionEquation currEquation, double currArea, KnownMeasurementsAggregator known)
        {
            IndexList currOuterRegionIndices = IndexList.AcquireAtomicRegionIndices(figureAtoms, currOuterRegion.atoms);

            // There is no outer region
            if (currOuterRegionIndices.IsEmpty()) return;

            //
            // We have reached this point by subtracting shapes, therefore, we have an equation.
            //
            SolutionAgg agg = new SolutionAgg();

            agg.solEq = new ComplexRegionEquation(currEquation);
            agg.solEq.SetTarget(currOuterRegion);
            agg.solType = currArea < 0 ? SolutionAgg.SolutionType.INCOMPUTABLE : SolutionAgg.SolutionType.COMPUTABLE;
            agg.solArea = currArea;
            agg.atomIndices = currOuterRegionIndices;

            //
            // Add this solution to the database.
            //
            solutions.AddSolution(agg);

            // Was this equation solving for a single atomic region? If so, leave. 
            if (currOuterRegion.IsAtomic()) return;

            //
            // Recursively explore EACH sub-shape root inside of the outer region.
            //
            foreach (TreeNode<Figure> shapeNode in currHierarchyRoots)
            {
                // A list omitting this shape
                List<TreeNode<Figure>> updatedHierarchy = new List<TreeNode<Figure>>(currHierarchyRoots);
                updatedHierarchy.Remove(shapeNode);

                // Process this shape
                ProcessShape(currOuterRegion, shapeNode, updatedHierarchy, currEquation, currArea, known);

                // Process the children
                ProcessChildrenShapes(currOuterRegion, shapeNode, updatedHierarchy, currEquation, currArea, known);
            }
        }

        //
        // Create the actual equation and continue processing recursively.
        //
        private void ProcessShape(Region currOuterRegion, TreeNode<Figure> currShape,
                                 List<TreeNode<Figure>> currHierarchyRoots, ComplexRegionEquation currEquation,
                                 double currArea, KnownMeasurementsAggregator known)
        {
            // Acquire the sub-shape.
            Figure currentFigure = currShape.GetData();

            // See what regions compose the subshape.
            ShapeRegion childShapeRegion = new ShapeRegion(currentFigure);

            // Make a copy of the current outer regions
            Region regionCopy = new Region(currOuterRegion);

            // Remove all regions from the outer region; if successful, recur on the new outer shape.
            if (regionCopy.Remove(childShapeRegion.atoms))
            {
                // Update the equation: copy and modify
                ComplexRegionEquation eqCopy = new ComplexRegionEquation(currEquation);
                eqCopy.AppendSubtraction(childShapeRegion);

                // Compute new area
                double currAreaCopy = currArea;
                if (currAreaCopy > 0)
                {
                    double currShapeArea = currentFigure.GetArea(known);

                    currAreaCopy = currShapeArea < 0 ? -1 : currAreaCopy - currShapeArea;
                }

                // Recur.
                SolveHelper(regionCopy, currHierarchyRoots, eqCopy, currAreaCopy, known);
            }
        }

        //
        // Process the child's shapes.
        //
        private void ProcessChildrenShapes(Region currOuterRegion, TreeNode<Figure> currShape,
                                           List<TreeNode<Figure>> currHierarchyRoots, ComplexRegionEquation currEquation,
                                           double currArea, KnownMeasurementsAggregator known)
        {
            foreach (TreeNode<Figure> childNode in currShape.Children())
            {
                // A copy of the children minus this shape.
                List<TreeNode<Figure>> childHierarchy = new List<TreeNode<Figure>>(currShape.Children());
                childHierarchy.Remove(childNode);

                // Add the hierarchy to the list of topmost hierarchical shapes.
                childHierarchy.AddRange(currHierarchyRoots);

                ProcessShape(currOuterRegion, childNode, childHierarchy, currEquation, currArea, known);
            }
        }

        private void PreprocessAtomAreas(KnownMeasurementsAggregator known)
        {
            //
            // Preprocess any of the shape atoms to see if the area is computable.
            //
            for (int a = 0; a < figureAtoms.Count; a++)
            {
                ShapeAtomicRegion shapeAtom = figureAtoms[a] as ShapeAtomicRegion;
                if (shapeAtom != null)
                {
                    double area = shapeAtom.GetArea(known);

                    if (area > 0)
                    {
                        ShapeRegion atomRegion = new ShapeRegion(shapeAtom.shape);

                        SolutionAgg agg = new SolutionAgg();

                        // The equation is the identity equation.
                        agg.solEq = new ComplexRegionEquation(atomRegion, atomRegion);
                        agg.solType = SolutionAgg.SolutionType.COMPUTABLE;
                        agg.solArea = area;
                        agg.atomIndices = new IndexList(a);

                        // Add this solution to the database.
                        solutions.AddSolution(agg);
                    }
                }
            }
        }

        //
        // Recur through all of the shapes to pre-calculate their areas.
        //
        private void PreprocessShapeHierarchyAreas(KnownMeasurementsAggregator known, List<Figure> allFigures)
        {
            foreach (Figure theFigure in allFigures)
            {
                // Acquire the indices of the shape.
                IndexList figIndices = IndexList.AcquireAtomicRegionIndices(figureAtoms, theFigure.atoms);
                figureIndexMap[figIndices] = theFigure;

                double area = theFigure.GetArea(known);

                if (area > 0)
                {
                    ShapeRegion atomRegion = new ShapeRegion(theFigure);

                    SolutionAgg agg = new SolutionAgg();

                    // The equation is the identity equation.
                    agg.solEq = new ComplexRegionEquation(atomRegion, atomRegion);
                    agg.solType = SolutionAgg.SolutionType.COMPUTABLE;
                    agg.solArea = area;
                    agg.atomIndices = IndexList.AcquireAtomicRegionIndices(figureAtoms, theFigure.atoms);

                    // Add this solution to the database.
                    solutions.AddSolution(agg);
                }
            }
        }
        //private void PreprocessShapeHierarchyAreas(KnownMeasurementsAggregator known, TreeNode<Figure> currentRoot)
        //{
        //    Figure theFigure = currentRoot.GetData();

        //    // Acquire the indices of the shape.
        //    IndexList figIndices = IndexList.AcquireAtomicRegionIndices(figureAtoms, theFigure.atoms);
        //    figureIndexMap[figIndices] = theFigure;

        //    double area = theFigure.GetArea(known);

        //    if (area > 0)
        //    {
        //        ShapeRegion atomRegion = new ShapeRegion(theFigure);

        //        SolutionAgg agg = new SolutionAgg();

        //        // The equation is the identity equation.
        //        agg.solEq = new ComplexRegionEquation(atomRegion, atomRegion);
        //        agg.solType = SolutionAgg.SolutionType.COMPUTABLE;
        //        agg.solArea = area;
        //        agg.atomIndices = IndexList.AcquireAtomicRegionIndices(figureAtoms, theFigure.atoms);

        //        // Add this solution to the database.
        //        solutions.AddSolution(agg);
        //    }

        //    foreach (TreeNode<Figure> child in currentRoot.Children())
        //    {
        //        PreprocessShapeHierarchyAreas(known, child);
        //    }
        //}

        //
        // Combine (through addition) any / all set of two equations in which there is no shared atomic region:
        //    (0 1 2) + (5 6) = (0 1 2 5 6)
        //
        // worklist-style fixpoint construction
        //
        private void ComposeAllRegions()
        {
            List<SolutionAgg> worklist = new List<SolutionAgg>(solutions.GetComputableSolutions());

            while (worklist.Any())
            {
                SolutionAgg currentSol = worklist[0];
                worklist.RemoveAt(0);

                foreach (SolutionAgg otherSol in solutions.GetSolutions())
                {
                    HandleComposition(worklist, currentSol, otherSol);
                }

                solutions.AddSolution(currentSol);
            }
        }

        private void HandleComposition(List<SolutionAgg> worklist, SolutionAgg first, SolutionAgg second)
        {
            // Addition of the two regions.
            SolutionAgg unionSol = HandleUnion(first, second);

            if (unionSol != null)
            {
                AddToWorklist(worklist, unionSol);
            }
            else
            {
                // Subtraction of the two regions.
                SolutionAgg diffSol = HandleDifference(first, second);
                if (diffSol != null) AddToWorklist(worklist, diffSol);
            }
        }

        //
        // If the solution is already in the database, update the solution in the database (if needed).
        // otherwise, add this solution to the worklist.
        //
        private void AddToWorklist(List<SolutionAgg> worklist, SolutionAgg solution)
        {
            //// Guarantee we may add this to the aorklist for processing.
            //if (!solutions.Contains(solution))
            //{
            //    worklist.Add(solution);
            //    return;
            //}

            //
            // If the solution is already in the database, update the solution in the database (if needed). 
            //
            if (solutions.AddSolution(solution))
            {
                // The solution may be in the worklist for processing; update accordingly.
                int worklistIndex = worklist.IndexOf(solution);
                if (worklistIndex != -1)
                {
                    worklist[worklistIndex] = solution;
                }
            }
        }

        private SolutionAgg HandleUnion(SolutionAgg first, SolutionAgg second)
        {
            // Can we combine the currentSol with this existent solution?
            IndexList unionIndices = IndexList.UnionIndices(first.atomIndices, second.atomIndices);

            // Disjoint union not possible.
            if (unionIndices == null) return null;

            //
            // Can combine; create a new solution / equation with addition.
            //
            SolutionAgg newSum = new SolutionAgg();
            newSum.atomIndices = unionIndices;
            newSum.solType = first.solType == SolutionAgg.SolutionType.COMPUTABLE &&
                             second.solType == SolutionAgg.SolutionType.COMPUTABLE ? SolutionAgg.SolutionType.COMPUTABLE : SolutionAgg.SolutionType.INCOMPUTABLE;
            newSum.solArea = newSum.solType == SolutionAgg.SolutionType.COMPUTABLE ? first.solArea + second.solArea : -1;
            newSum.solEq = new ComplexRegionEquation(MakeRegion(unionIndices.orderedIndices),
                                                     new ComplexRegionEquation.Binary(first.solEq.target, OperationT.ADDITION, second.solEq.target));
            return newSum;
        }

        private SolutionAgg HandleDifference(SolutionAgg first, SolutionAgg second)
        {
            // Can we combine the currentSol with this existent solution?
            IndexList diffIndices = IndexList.DifferenceIndices(first.atomIndices, second.atomIndices);

            // Disjoint union not possible.
            if (diffIndices == null) return null;

            //
            // Can combine; create a new solution / equation with addition.
            //
            SolutionAgg newSum = new SolutionAgg();
            newSum.atomIndices = diffIndices;
            newSum.solType = first.solType == SolutionAgg.SolutionType.COMPUTABLE &&
                             second.solType == SolutionAgg.SolutionType.COMPUTABLE ? SolutionAgg.SolutionType.COMPUTABLE : SolutionAgg.SolutionType.INCOMPUTABLE;
            if (newSum.solType == SolutionAgg.SolutionType.INCOMPUTABLE)
            {
                newSum.solArea = -1;
            }
            else
            {
                newSum.solArea = first.solArea > second.solArea ? first.solArea - second.solArea : second.solArea - first.solArea;
            }

            if (first.atomIndices.Count > second.atomIndices.Count)
            {
                newSum.solEq = new ComplexRegionEquation(MakeRegion(diffIndices.orderedIndices),
                                                         new ComplexRegionEquation.Binary(first.solEq.target, OperationT.SUBTRACTION, second.solEq.target));
            }
            else
            {
                newSum.solEq = new ComplexRegionEquation(MakeRegion(diffIndices.orderedIndices),
                                                         new ComplexRegionEquation.Binary(second.solEq.target, OperationT.SUBTRACTION, first.solEq.target));
            }

            return newSum;
        }

        //
        // Makes a region out of a list of indices (of atomic regions).
        //
        private Region MakeRegion(List<int> indices)
        {
            //
            // Find a shape, if it applies.
            //
            Figure fig = null;
            IndexList indicesList = new IndexList(indices);

            if (figureIndexMap.TryGetValue(indicesList, out fig)) return new ShapeRegion(fig);

            //
            // Default to a sequence of atoms.
            //
            List<AtomicRegion> atoms = new List<AtomicRegion>();
            
            foreach (int index in indices)
            {
                atoms.Add(figureAtoms[index]);
            }

            return new Region(atoms);
        }

        public void PrintAllSolutions()
        {
            List<SolutionAgg> theSolutions = solutions.GetSolutions();

            for (int s = 0; s < theSolutions.Count; s++)
            {
#if HARD_CODED_UI
                UIDebugPublisher.getInstance().publishString((s + 1) + " (" + string.Format("{0:N4}", theSolutions[s].solArea) + "): " + theSolutions[s].solEq.CheapPrettyString());
#else
                Debug.WriteLine((s + 1) + " (" + string.Format("{0:N4}", theSolutions[s].solArea) + "): " + theSolutions[s].solEq.CheapPrettyString());
#endif
            }
        }

        //
        // Determine which atomic regions touch which root shapes.
        // Returns the indices of covered shapes.
        //
        private List<int>[] DetermineAtomCoverageOfShapes(List<Figure> shapes)
        {
            List<int>[] atomsCoverShapes = new List<int>[figureAtoms.Count];

            for (int a = 0; a < atomsCoverShapes.Length; a++)
            {
                atomsCoverShapes[a] = new List<int>();

                for (int s = 0; s < shapes.Count; s++)
                {
                    if (shapes[s].Covers(figureAtoms[a])) atomsCoverShapes[a].Add(s);
                }
            }

            return atomsCoverShapes;
        }

        //
        // Does this one solution cover all root shapes.
        //
        private bool SolutionCoversRoots(List<int>[] coverage, SolutionAgg solution, int numShapes)
        {
            List<int> shapesCovered = new List<int>();

            // Collect all shape-covered indices
            foreach (int index in solution.atomIndices.orderedIndices)
            {
                Utilities.AddUniqueList<int>(shapesCovered, coverage[index]);
            }

            // If we covered all indices, the solution covers.
            return shapesCovered.Count == numShapes;
        }

        //
        // For each compuable region, does the region touch all root-shapes in the hierarchy?
        //
        public void ComputableInterestingCount(List<Figure> roots, out int interestingComputable, out int uninterestingComputable,
                                                                   out int interestingIncomputable, out int uninterestingIncomputable)
        {
            interestingComputable = 0;
            uninterestingComputable = 0;
            interestingIncomputable = 0;
            uninterestingIncomputable = 0;

            //
            // Determine which atomic regions touch which root shapes.
            //
            List<SolutionAgg> computable = solutions.GetComputableSolutions();
            List<SolutionAgg> incomputable = solutions.GetIncomputableSolutions();

            List<int>[] coverage = DetermineAtomCoverageOfShapes(roots);

            foreach (SolutionAgg comp in computable)
            {
                if (SolutionCoversRoots(coverage, comp, roots.Count)) interestingComputable++;
                else uninterestingComputable++;
            }

            foreach (SolutionAgg incomp in incomputable)
            {
                if (SolutionCoversRoots(coverage, incomp, roots.Count)) interestingIncomputable++;
                else uninterestingIncomputable++;
            }
        }

        //
        // Is this problem (defined by the set of atomic regions) covered / defined by all the root shapes?
        //
        public bool IsProblemInteresting(List<Figure> roots, List<AtomicRegion> regions)
        {
            SolutionAgg solution = solutions.GetSolutionAgg(this.figureAtoms, regions);

            List<int>[] coverage = DetermineAtomCoverageOfShapes(roots);

            return SolutionCoversRoots(coverage, solution, roots.Count);
        }
    }
}