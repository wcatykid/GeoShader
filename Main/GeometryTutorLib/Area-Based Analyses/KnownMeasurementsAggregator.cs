using System;
using System.Collections.Generic;
using System.Linq;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.Area_Based_Analyses
{
    /// <summary>
    /// A aggregator of all known measurements for a particular problem (angles, segment lengths, etc.)
    /// </summary>
    public class KnownMeasurementsAggregator
    {
        public enum UNITS
        {
            INCH,
            FT,
            MI,
            MM,
            CM,
            M,
            KM
        }

        private List<KeyValuePair<Segment, double>> segments;
        private List<KeyValuePair<Angle, double>> angles;
        private List<KeyValuePair<Arc, double>> arcs;
        private UNITS units;

        public KnownMeasurementsAggregator()
        {
            segments = new List<KeyValuePair<Segment, double>>();
            angles = new List<KeyValuePair<Angle, double>>();
            arcs = new List<KeyValuePair<Arc, double>>();
            units = UNITS.CM;
        }

        public List<KeyValuePair<Segment, double>> GetKnownSegments() { return segments; }

        public double GetSegmentLength(Segment thatSeg)
        {
            if (thatSeg == null)
            {
                // throw new ArgumentException("Why is the angle null?");
                return -1;
            }

            foreach (KeyValuePair<Segment, double> segPair in segments)
            {
                if (thatSeg.StructurallyEquals(segPair.Key)) return segPair.Value;
            }

            return -1;
        }

        public bool SegmentLengthKnown(Segment thatSeg)
        {
            if (thatSeg == null) return false;

            return GetSegmentLength(thatSeg) > 0;
        }

        public double GetAngleMeasure(Angle thatAngle)
        {
            if (thatAngle == null)
            {
                // throw new ArgumentException("Why is the angle null?");
                return -1;
            }

            foreach (KeyValuePair<Angle, double> anglePair in angles)
            {
                if (thatAngle.Equates(anglePair.Key)) return anglePair.Value;
            }

            return -1;
        }

        public double GetArcMeasure(Arc thatArc)
        {
            if (thatArc == null)
            {
                // throw new ArgumentException("Why is the angle null?");
                return -1;
            }

            foreach (KeyValuePair<Arc, double> arcPair in arcs)
            {
                if (thatArc.StructurallyEquals(arcPair.Key)) return arcPair.Value;
            }

            return -1;
        }

        public bool AngleMeasureKnown(Angle thatAngle)
        {
            return GetAngleMeasure(thatAngle) > 0;
        }

        public bool AddAngleMeasureDegree(Angle thatAngle, double measure)
        {
            if (thatAngle == null) return false;

            if (AngleMeasureKnown(thatAngle)) return false;

            if (!Utilities.CompareValues(thatAngle.measure, measure))
            {

                System.Diagnostics.Debug.WriteLine("Error in known measurements.");
            }

            angles.Add(new KeyValuePair<Angle, double>(thatAngle, measure));

            return true;
        }

        public bool ArcMeasureKnown(Arc thatArc)
        {
            return GetArcMeasure(thatArc) > 0;
        }

        public bool AddArcMeasureDegree(Arc thatArc, double measure)
        {
            if (ArcMeasureKnown(thatArc)) return false;

            arcs.Add(new KeyValuePair<Arc, double>(thatArc, measure));

            return true;
        }

        public bool AddAngleMeasureRadian(Angle thatAngle, double measure)
        {
            return AddAngleMeasureDegree(thatAngle, Angle.toDegrees(measure));
        }

        public bool AddSegmentLength(Segment thatSegment, double length)
        {
            if (thatSegment == null) return false;

            if (SegmentLengthKnown(thatSegment)) return false;

            if (!Utilities.CompareValues(thatSegment.Length, length))
            {
                System.Diagnostics.Debug.WriteLine("Error in known measurements.");
            }

            segments.Add(new KeyValuePair<Segment, double>(thatSegment, length));

            return true;
        }

        public void SetMeasurementType(UNITS u)
        {
            units = u;
        }
    }
}
