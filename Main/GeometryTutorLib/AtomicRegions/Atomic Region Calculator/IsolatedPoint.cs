using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.Area_Based_Analyses.Atomizer
{
    public class IsolatedPoint : Primitive
    {
        Point thePoint;

        public IsolatedPoint(Point pt)
        {
            thePoint = pt;
        }

        public override string ToString()
        {
            return "Point { " + thePoint.ToString() + " }";
        }
    }
}