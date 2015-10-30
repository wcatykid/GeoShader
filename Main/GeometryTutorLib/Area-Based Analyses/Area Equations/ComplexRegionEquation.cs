using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.Area_Based_Analyses
{
    //
    // An equation of the form: target = a +- b +- c +- ... +- d
    //
    public class ComplexRegionEquation
    {
        public Region target { get; private set; }
        public Expr expr { get; private set; }
        private double thisArea;

        public ComplexRegionEquation(Region tar, Region r) : base()
        {
            target = tar;
            expr = new Unary(r);
            thisArea = -1;
        }

        public ComplexRegionEquation(SimpleRegionEquation simple) : base()
        {
            target = simple.target;

            expr = new Binary(simple.bigger, simple.op, simple.smaller);

            thisArea = -1;
        }

        public ComplexRegionEquation(ComplexRegionEquation complex) : base()
        {
            target = complex.target;

            expr = complex.expr.Copy();

            thisArea = -1;
        }

        public ComplexRegionEquation(Region tar, Expr exp) : base()
        {
            target = tar;

            expr = exp;

            thisArea = -1;
        }

        public void SetTarget(Region targ)
        {
            this.target = targ;
        }

        public void AppendSubtraction(Region r)
        {
            this.expr = new Binary(this.expr, OperationT.SUBTRACTION, new Unary(r));
        }

        //
        // Large - x_1 - x_2 - ... - x_3 = atom   =>  Large - atom = x_1 + x_2 + ... + x_3
        //
        public void ChangePolarity()
        {
            throw new ArgumentException("Not implemented.");
        }

        public void Substitute(Region toFind, Expr toSub)
        {
            expr.Substitute(toFind, toSub);
        }

        public override string ToString()
        {
            return target.ToString() + " = \n" + expr.ToString();
        }

        public string CheapPrettyString()
        {
            return target.CheapPrettyString() + " = " + expr.CheapPrettyString();
        }

        public int Length
        {
            get
            {
                if (!IsComplete()) return int.MaxValue;

                // If this is an indentity equation (base case in other code),
                // indicate a length of infinity.
                if (IsIdentity()) return int.MaxValue;

                // Otherwise, count the number of regions.
                return expr.NumRegions();
            }
        }

        //
        // Does this equation define both sides of the equation?
        //
        public bool IsComplete()
        {
            return target != null && expr != null;
        }

        //
        // Is the expr of the equation defined by all shapes?
        //
        public bool DefinedByShapes()
        {
            return expr.DefinedByShapes();
        }

        // Is this an equation of the form x = x?
        public bool IsIdentity()
        {
            Unary rhs = expr as Unary;
            if (rhs == null) return false;

            return target.Equals(rhs.theRegion);
        }

        // Get the actual numeric value of the area defined by this equation (-1 if unknwon)
        public double GetArea()
        {
            if (thisArea > 0) return thisArea;

            thisArea = expr.GetArea();

            return thisArea;
        }

        //
        // Class hierarchy to handle expressions (one side of an equation).
        //
        public abstract class Expr
        {
            public Expr() {}
            public abstract Expr Copy();
            public abstract Expr Substitute(Region toFind, Expr toSub);
            public abstract int NumRegions();
            public abstract double GetArea();
            public abstract string CheapPrettyString();
            public abstract bool DefinedByShapes();
        }

        public class Unary : Expr
        {
            public Region theRegion;

            public Unary(Region r) : base() { theRegion = r; }
            public Unary(Unary e) : base() { theRegion = e.theRegion; }
            public override Expr Copy() { return new Unary(this); }
            public override Expr Substitute(Region toFind, Expr toSub) { return theRegion.Equals(toFind) ? toSub : this; }
            public override string ToString() { return theRegion.ToString(); }
            public override string CheapPrettyString() { return theRegion.CheapPrettyString(); }
            public override int NumRegions() { return 1; }
            public override double GetArea() { return theRegion.GetKnownArea(); }
            public override bool DefinedByShapes() { return theRegion is ShapeRegion; }
        }

        public class Binary : Expr
        {
            public Expr leftExp;
            public Expr rightExp;
            public OperationT op;

            public Binary(Expr ell, OperationT op, Expr right) : base()
            {
                leftExp = ell;
                this.op = op;
                rightExp = right;
            }
            public Binary(Region ell, OperationT op, Region r) : this(new Unary(ell), op, new Unary(r)) { }
            public Binary(Binary b) : this (b.leftExp.Copy(), b.op, b.rightExp.Copy()) {}

            public override Expr Copy() { return new Binary(this); }
            public override Expr Substitute(Region toFind, Expr toSub)
            {
                Expr newLeft = leftExp.Substitute(toFind, toSub);
                Expr newRight = leftExp.Substitute(toFind, toSub);
                return new Binary(newLeft, this.op, newRight);
            }
            public override string ToString()
            {
                string operation = op == OperationT.ADDITION ? "+" : "-";

                return "(" + leftExp.ToString() + operation + rightExp.ToString() + ")"; 
            }
            public override string CheapPrettyString()
            {
                string operation = op == OperationT.ADDITION ? "+" : "-";

                return "(" + leftExp.CheapPrettyString() + " " + operation + " " + rightExp.CheapPrettyString() + ")";
            }
            public override int NumRegions() { return leftExp.NumRegions() + rightExp.NumRegions(); }

            public override double GetArea()
            {
                double left = leftExp.GetArea();
                double right = rightExp.GetArea();

                if (left < 0 || right < 0) return -1;
                
                return op == OperationT.ADDITION ? left + right : left - right;
            }
            public override bool DefinedByShapes() { return leftExp.DefinedByShapes() && rightExp.DefinedByShapes(); }
        }
    }
}