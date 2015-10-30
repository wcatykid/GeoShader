using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib;

namespace StatisticsGenerator
{
    public class ShadedAreaFigureStatisticsAggregator : FigureStatisticsAggregator
    {
        public int numShapes;
        public int numRootShapes;
        public int numAtomicRegions;

        public bool originalProblemInteresting;


        public int length;
        public int width;
        public int numAreaFacts;
        public int numGeometricFacts;
        public int numDeductions;


        public ShadedAreaFigureStatisticsAggregator() : base()
        {
            numShapes = 0;
            numRootShapes = 0;
            numAtomicRegions = 0;

            originalProblemInteresting = false;

            length = -1;
            width = -1;
            numAreaFacts = -1;
            numGeometricFacts = -1;
            numDeductions = -1;
        }
    }
}
