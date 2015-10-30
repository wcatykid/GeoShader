using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Threading;
using GeometryTutorLib.ConcreteAST;
using System.Linq;
using System;

namespace GeometryTutorLib.Area_Based_Analyses.Atomizer
{
    /// <summary>
    /// Converts from FacetCalculator Primitives to actual Atomic Regions.
    /// </summary>
    public static class PrimitiveToRegionConverter
    {
        //
        // Take the cycle-based representation and convert in into AtomicRegion objects.
        //
        public static List<AtomicRegion> Convert(UndirectedPlanarGraph.PlanarGraph graph,
                                                 List<Primitive> primitives, List<Circle> circles)
        {
            List<MinimalCycle> cycles = new List<MinimalCycle>();
            List<Filament> filaments = new List<Filament>();

            foreach (Primitive primitive in primitives)
            {
                if (primitive is MinimalCycle) cycles.Add(primitive as MinimalCycle);
                if (primitive is Filament) filaments.Add(primitive as Filament);
            }

            //
            // Convert the filaments to atomic regions.
            //
            List<AtomicRegion> regions = new List<AtomicRegion>();
            if (filaments.Any())
            {
                throw new Exception("A filament occurred in conversion to atomic regions.");
            }
            // regions.AddRange(HandleFilaments(graph, circles, filaments));




            ComposeCycles(graph, cycles);

            if (GeometryTutorLib.Utilities.ATOMIC_REGION_GEN_DEBUG)
            {
                Debug.WriteLine("Composed:");
                foreach (MinimalCycle cycle in cycles)
                {
                    Debug.WriteLine("\t" + cycle.ToString());
                }
            }

            //
            // Convert all cycles (perimeters) to atomic regions
            //
            foreach (MinimalCycle cycle in cycles)
            {
                List<AtomicRegion> temp = cycle.ConstructAtomicRegions(circles, graph);
                foreach (AtomicRegion atom in temp)
                {
                    if (!regions.Contains(atom)) regions.Add(atom);
                }
            }

            return regions;
        }

        //
        // A filament is a path from one node to another; it does not invoke a cycle.
        // In shaded-area problems this can only be accomplished with arcs of circles.
        //
        private static List<AtomicRegion> HandleFilaments(UndirectedPlanarGraph.PlanarGraph graph, List<Circle> circles, List<Filament> filaments)
        {
            List<AtomicRegion> atoms = new List<AtomicRegion>();

            foreach (Filament filament in filaments)
            {
                atoms.AddRange(filament.ExtractAtomicRegions(graph, circles));
            }

            return atoms;
        }

        //
        // If a cycle has an edge that is EXTENDED, there exist two regions, one on each side of the segment; compose the two segments.
        //
        // Fixed point algorithm: while there exists a cycle with an extended segment, compose.
        private static void ComposeCycles(UndirectedPlanarGraph.PlanarGraph graph, List<MinimalCycle> cycles)
        {
            for (int cycleIndex = HasComposableCycle(graph, cycles); cycleIndex != -1; cycleIndex = HasComposableCycle(graph, cycles))
            {
                // Get the cycle and remove it from the list.
                MinimalCycle thisCycle = cycles[cycleIndex];

                cycles.RemoveAt(cycleIndex);

                // Get the extended segment which is the focal segment of composition.
                GeometryTutorLib.ConcreteAST.Segment extendedSeg = thisCycle.GetExtendedSegment(graph);

                // Find the matching cycle that has the same Extended segment
                int otherIndex = GetComposableCycleWithSegment(graph, cycles, extendedSeg);
                MinimalCycle otherCycle = cycles[otherIndex];
                cycles.RemoveAt(otherIndex);

                // Compose the two cycles into a single cycle.
                MinimalCycle composed = thisCycle.Compose(otherCycle, extendedSeg);

                // Add the new, composed cycle
                cycles.Add(composed);
            }
        }

        private static int HasComposableCycle(UndirectedPlanarGraph.PlanarGraph graph, List<MinimalCycle> cycles)
        {
            for (int c = 0; c < cycles.Count; c++)
            {
                if (cycles[c].HasExtendedSegment(graph)) return c;
            }
            return -1;
        }

        private static int GetComposableCycleWithSegment(UndirectedPlanarGraph.PlanarGraph graph,
                                                         List<MinimalCycle> cycles,
                                                         GeometryTutorLib.ConcreteAST.Segment segment)
        {
            for (int c = 0; c < cycles.Count; c++)
            {
                if (cycles[c].HasThisExtendedSegment(graph, segment)) return c;
            }

            return -1;
        }
    }
}