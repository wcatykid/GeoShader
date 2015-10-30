using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;
using GeometryTutorLib.GenericInstantiator;
using GeometryTutorLib.Pebbler;

namespace GeometryTutorLib.Area_Based_Analyses
{
    public class AreaPowersetHypergraph
    {
        // The area hypergraph has nodes which are regions and annotated edges which relate with \pm (+ / -)
        public Hypergraph.Hypergraph<Region, SimpleRegionEquation> graph { get; private set; }

        private ComplexRegionEquation[] memoizedSolutions;

        //
        private bool[] visited;

        public AreaPowersetHypergraph(List<Atomizer.AtomicRegion> atoms,
                                      List<Circle> circles, List<Polygon>[] polygons,
                                      List<Sector> minorSectors, List<Sector> majorSectors)
        {
            // Ensure the capacity of the powerset.
            graph = new Hypergraph.Hypergraph<Region, SimpleRegionEquation>((int)Math.Pow(2, atoms.Count));

            BuildNodes(atoms, circles, polygons, minorSectors, majorSectors);
            BuildEdges(atoms);

            visited = new bool[graph.Size()];

            // Precompute any shapes in which we know we can compute the area; this is done in BuildNodes()
            memoizedSolutions = new ComplexRegionEquation[graph.Size()];
        }

        //
        // Reset the fact we have not visited any nodes.
        //
        public void Clear()
        {
            visited = new bool[graph.Size()];
        }

        //
        // The graph nodes are the powerset of atomic nodes
        //
        private void BuildNodes(List<Atomizer.AtomicRegion> atoms,
                                List<Circle> circles, List<Polygon>[] polygons,
                                List<Sector> minorSectors, List<Sector> majorSectors)
        {
            // Acquire an integer representation of the powerset of atomic nodes
            List<List<int>> powerset = Utilities.ConstructPowerSetWithNoEmpty(atoms.Count);

            // Construct each element of the powerset
            for (int s = 0; s < powerset.Count; s++)
            {
                List<Atomizer.AtomicRegion> theseAtoms = new List<Atomizer.AtomicRegion>();

                // Construct the individual element (set)
                foreach (int e in powerset[s])
                {
                    theseAtoms.Add(atoms[e]);
                }

                // Create the region; If this set of atoms union together to make a single shape, a ShapeRegion is constructed
//                Region newRegion = Region.MakeRegion(theseAtoms, circles, polygons, minorSectors, majorSectors);

                // Add the new node to the area hypergraph
//                graph.AddNode(newRegion);
            }
        }

        //
        // Find the index of the given set; search only between the given indices for speed.
        //
        private int GetPowerSetIndex(List<List<int>> powerset, List<int> wanted, int startIndex, int stopIndex)
        {
            for (int i = startIndex; i < stopIndex && i < powerset.Count; i++)
            {
                if (Utilities.EqualOrderedSets(powerset[i], wanted)) return i;
            }

            return -1;
        }

        //
        // There is one addition edge and two subtraction edges per set of 3 nodes.
        // Build the edges top-down from complete set of atoms down to singletons.
        //
        private void BuildEdges(List<Atomizer.AtomicRegion> atoms)
        {
            // Acquire an integer representation of the powerset of atomic nodes
            // This is memoized so it's fast.
            List<List<int>> powerset = Utilities.ConstructPowerSetWithNoEmpty(atoms.Count);

            //
            // For each layer (of particular subset size), establish all links.
            //
            int setIndex = atoms.Count; // Skip the singletons
            int currSetSize = 2;
            int prevLayerStartIndex = 0;
            while (setIndex < powerset.Count)
            {
                //
                // For each layer, look at each individual set and deconstruct
                //
                int layerSize = (int)Utilities.Combination(atoms.Count, currSetSize++);

                for (int layerIndex = 0; layerIndex < layerSize; layerIndex++)
                {
                    int currentIndex = setIndex + layerIndex;

                    // Look at each set
                    // Take away, in turn, each element in the set to construct the desired edges.
                    List<int> currentSet = powerset[currentIndex];
                    foreach (int val in currentSet)
                    {
                        // Make a copy of this set and remove the element
                        List<int> differenceSet = new List<int>(currentSet);
                        differenceSet.Remove(val);

                        int singletonIndex = val; // the index of a singleton corresponds to its value
                        int differenceIndex = GetPowerSetIndex(powerset, differenceSet, prevLayerStartIndex, setIndex + layerSize);

                        //
                        // Build the edge for this 3 node combinations.
                        //

                        //
                        // A = (A \ a) + a
                        //
                        SimpleRegionEquation sumAnn = new SimpleRegionEquation(graph.vertices[currentIndex].data,
                                                                               graph.vertices[differenceIndex].data, OperationT.ADDITION, graph.vertices[singletonIndex].data);
                        List<int> sourceIndices = new List<int>();
                        sourceIndices.Add(differenceIndex);
                        sourceIndices.Add(singletonIndex);
                        graph.AddIndexEdge(sourceIndices, currentIndex, sumAnn);

                        //
                        // (A \ a) = A - a
                        //
                        SimpleRegionEquation diffAnn1 = new SimpleRegionEquation(graph.vertices[differenceIndex].data,
                                                                                 graph.vertices[currentIndex].data, OperationT.SUBTRACTION, graph.vertices[singletonIndex].data);
                        sourceIndices = new List<int>();
                        sourceIndices.Add(currentIndex);
                        sourceIndices.Add(singletonIndex);
                        graph.AddIndexEdge(sourceIndices, differenceIndex, diffAnn1);

                        //
                        // a = A - (A \ a)
                        //
                        SimpleRegionEquation diffAnn2 = new SimpleRegionEquation(graph.vertices[singletonIndex].data,
                                                                                 graph.vertices[currentIndex].data, OperationT.SUBTRACTION, graph.vertices[differenceIndex].data);
                        sourceIndices = new List<int>();
                        sourceIndices.Add(currentIndex);
                        sourceIndices.Add(differenceIndex);
                        graph.AddIndexEdge(sourceIndices, singletonIndex, diffAnn2);
                    }
                }
                prevLayerStartIndex = setIndex;
                setIndex += layerSize;
            }

        }

        //
        // There is one addition edge and two subtraction edges per set of 3 nodes.
        // Build the edges top-down from complete set of atoms down to singletons.
        //
        private void BuildEdges(List<Atomizer.AtomicRegion> atoms, bool[] marked)
        {
            // We don't want edges connecting a singleton region to an 'empty' region.
            if (atoms.Count == 1) return;

            // The node for this set of list of atoms.
            Region atomsRegion = new Region(atoms);

            // Check to see if we have already visited this node and constructed the edges.
            int nodeIndex = graph.GetNodeIndex(atomsRegion);
            if (marked[nodeIndex]) return;

            foreach (Atomizer.AtomicRegion atom in atoms)
            {
                List<Atomizer.AtomicRegion> atomsMinusAtom = new List<Atomizer.AtomicRegion>(atoms);
                atomsMinusAtom.Remove(atom);

                Region aMinus1Region = new Region(atomsMinusAtom);
                Region atomRegion = new Region(atom);

                //
                // A = (A \ a) + a
                //
                SimpleRegionEquation sumAnnotation = new SimpleRegionEquation(atomsRegion, aMinus1Region, OperationT.ADDITION, atomRegion);
                List<Region> sources = new List<Region>();
                sources.Add(aMinus1Region);
                sources.Add(atomRegion);
                graph.AddEdge(sources, atomsRegion, sumAnnotation);

                //
                // (A \ a) = A - a
                //
                SimpleRegionEquation diff1Annotation = new SimpleRegionEquation(aMinus1Region, atomsRegion, OperationT.SUBTRACTION, atomRegion);
                sources = new List<Region>();
                sources.Add(atomsRegion);
                sources.Add(atomRegion);
                graph.AddEdge(sources, aMinus1Region, diff1Annotation);

                //
                // a = A - (A \ a)
                //
                SimpleRegionEquation diff2Annotation = new SimpleRegionEquation(atomRegion, atomsRegion, OperationT.SUBTRACTION, aMinus1Region);
                sources = new List<Region>();
                sources.Add(atomsRegion);
                sources.Add(aMinus1Region);
                graph.AddEdge(sources, atomRegion, diff2Annotation);

                //
                // Recursive call to construct edges with A \ a
                //
                BuildEdges(atomsMinusAtom, marked);
            }

            Debug.WriteLine(graph.EdgeCount());
            marked[nodeIndex] = true;
        }

        //
        // The graph nodes are the powerset of atomic nodes
        //
        private void PrecomputeShapes()
        {
            // Construct each element of the powerset
            for (int r = 0; r < graph.vertices.Count; r++)
            {
                // TODO collect atoms into a shape....



                // Check if we can memoize this directly as a shape
                if (graph.vertices[r].data is ShapeRegion)
                {
                    memoizedSolutions[r] = new ComplexRegionEquation(graph.vertices[r].data, graph.vertices[r].data);
                }
            }
        }

        public ComplexRegionEquation TraceRegionArea(Region thatRegion)
        {
            // Find this region in the hypergraph.
            int startIndex = graph.GetNodeIndex(thatRegion);

            if (startIndex == -1) throw new ArgumentException("Desired region not found in area hypergraph: " + thatRegion);

            // Modifiable region equation: region = region so that we substitute into the RHS. 
            ComplexRegionEquation eq = new ComplexRegionEquation(thatRegion, thatRegion);

            // Traverse depth-first to construct all equations.
            bool success = SimpleVisit(startIndex, eq);

            return success ? memoizedSolutions[startIndex] : null;
        }

        //
        // Graph traversal to find shapes and thus the resulting equation (solution).
        //
        // Depth first: construct along the way
        // Find only the SHORTEST equation (based on the number of regions involved in the equation).
        //
        private bool SimpleVisit(int regionIndex, ComplexRegionEquation currentEq)
        {
            //
            // Deal with memoizing: keep the shortest equation for this particular region (@ regionIndex)
            //
            if (visited[regionIndex]) return true;

            // We have now visited this node.
            visited[regionIndex] = true;

            // Is this partitcular region a shape?
            // If so, save the basis equation in memozied.
            if (graph.vertices[regionIndex].data is ShapeRegion) return true;

            // For all hyperedges leaving this node, follow the edge sources
            foreach (Hypergraph.HyperEdge<SimpleRegionEquation> edge in graph.vertices[regionIndex].targetEdges)
            {
                // Will contain two equations representing expressions for the source node
                ComplexRegionEquation[] edgeEqs = new ComplexRegionEquation[edge.sourceNodes.Count];

                // For actively substituting into.
                ComplexRegionEquation currentEqCopy = new ComplexRegionEquation(currentEq);

                // Area can be calcualted either directly or using the GeoTutor deductive engine.
                bool canCalcArea = true;
                for (int e = 0; e < edge.sourceNodes.Count; e++)
                {
                    // If we have already visited this node, we already have an equation for it; use it.
                    if (visited[edge.sourceNodes[e]])
                    {
                        // Check if we cannot calculate the region area.
                        if (memoizedSolutions[edge.sourceNodes[e]] == null)
                        {
                            canCalcArea = false;
                            break;
                        }

                        // Otherwise, we use the memoized version of this region equation for this source node.
                        edgeEqs[e] = memoizedSolutions[edge.sourceNodes[e]];
                    }
                    // We don't have a memoized version; calculate it.
                    else
                    {
                        // Create an equation: region = region so that we substitute into the RHS.
                        Region srcRegion = graph.vertices[edge.sourceNodes[e]].data;
                        edgeEqs[e] = new ComplexRegionEquation(srcRegion, srcRegion);

                        // This source node is not a shape: we can't directly calculate its area.
                        if (!SimpleVisit(edge.sourceNodes[e], edgeEqs[e]))
                        {
                            canCalcArea = false;
                            break;
                        }
                    }
                }

                //
                // If we have a successful search from this edge, create the corresponding region equation.
                //
                if (canCalcArea)
                {
                    // We can substitute the annotation along the edge into the edge's target region (expression).

                    // to find (val)
                    currentEqCopy.Substitute(graph.vertices[edge.targetNode].data,
                        // to sub (for val)
                                             new ComplexRegionEquation.Binary(edgeEqs[0].expr,
                                                                              edge.annotation.op,
                                                                              edgeEqs[0].expr));
                    // Choose the shortest solution for this region
                    if (currentEq.Length > currentEqCopy.Length)
                    {
                        currentEq = currentEqCopy;
                    }
                }
            }

            // Did we find a solution?
            if (currentEq.Length == int.MaxValue) return false;

            // Success; save the solution.
            memoizedSolutions[regionIndex] = currentEq;

            return true;
        }

        public ComplexRegionEquation TraceRegionArea(List<Atomizer.AtomicRegion> atoms)
        {
            return TraceRegionArea(new Region(atoms));
        }
    }
}