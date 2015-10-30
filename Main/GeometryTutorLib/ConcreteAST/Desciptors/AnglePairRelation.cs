using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Describes a point that lies on a angmant.
    /// </summary>
    public abstract class AnglePairRelation : Descriptor
    {
        public Angle angle1 { get; protected set; }
        public Angle angle2 { get; protected set; }

        public AnglePairRelation(Angle ang1, Angle ang2) : base()
        {
            angle1 = ang1;
            angle2 = ang2;
        }

        // Return the shared angle in both congruences
        public Angle AngleShared(AnglePairRelation relation)
        {
            if (angle1.Equates(relation.angle1) || angle1.Equates(relation.angle2)) return angle1;
            if (angle2.Equates(relation.angle1) || angle2.Equates(relation.angle2)) return angle2;

            return null;
        }

        // Return the shared angle in both congruences
        public Angle AngleShared(CongruentAngles cas)
        {
            if (angle1.Equates(cas.ca1) || angle1.Equates(cas.ca2)) return angle1;
            if (angle2.Equates(cas.ca1) || angle2.Equates(cas.ca2)) return angle2;

            return null;
        }

        // Return the shared angle in both congruences
        public Angle OtherAngle(Angle thatAngle)
        {
            if (angle1.Equates(thatAngle)) return angle2;
            if (angle2.Equates(thatAngle)) return angle1;

            return null;
        }

        // Return the shared angle in both congruences
        public bool HasAngle(Angle thatAngle)
        {
            return angle1.Equates(thatAngle) || angle2.Equates(thatAngle);
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override bool StructurallyEquals(Object obj)
        {
            AnglePairRelation relation = obj as AnglePairRelation;
            if (relation == null) return false;
            return (angle1.StructurallyEquals(relation.angle1) && angle2.StructurallyEquals(relation.angle2)) ||
                   (angle1.StructurallyEquals(relation.angle2) && angle2.StructurallyEquals(relation.angle1));
        }

        public override bool Equals(Object obj)
        {
            AnglePairRelation relation = obj as AnglePairRelation;
            if (relation == null) return false;
            return (angle1.Equals(relation.angle1) && angle2.Equals(relation.angle2)) ||
                   (angle1.Equals(relation.angle2) && angle2.Equals(relation.angle1)) && base.Equals(relation);
        }
    }
}
