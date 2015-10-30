using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib;

namespace StatisticsGenerator
{
    public abstract class FigureStatisticsAggregator
    {
        // k-G Counts
        public const int MAX_K = 2;
        public int[] kGcardinalities = new int[MAX_K + 1];

        public bool isComplete;

        // Figure Properties (implicit facts)
        public int totalImplicitFacts;
        public int numInMiddle;
        public int numPoints;
        public int numSegments;
        public int numIntersections;
        public int numTriangles;
        public int numAngles;

        // Later
        public int numQuadrilaterals;
        public int numCircles;

        public int totalExplicitFacts;



        // For timing the processing of each figure
        public GeometryTutorLib.Stopwatch stopwatch;

        public FigureStatisticsAggregator()
        {
            totalImplicitFacts = 0;
            numInMiddle = 0;
            numPoints = 0;
            numSegments = 0;
            numIntersections = 0;
            numTriangles = 0;
            numAngles = 0;
            numQuadrilaterals = 0;
            numCircles = 0;

            totalExplicitFacts = 0;
 
            stopwatch = new GeometryTutorLib.Stopwatch();
        }
    }
}
