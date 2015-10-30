using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;
using System.Diagnostics;

namespace GeometryTutorLib.GenericInstantiator
{
    public class TransitiveCongruentTriangles : GenericRule
    {
        private static readonly string NAME = "Transitivity of Congruent Triangles";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.TRANSITIVE_CONGRUENT_TRIANGLES);

        // Congruences imply equations: AB \cong CD -> AB = CD
        private static List<GeometricCongruentTriangles> candidateGeoCongruentTriangles = new List<GeometricCongruentTriangles>();
        private static List<AlgebraicCongruentTriangles> candidateAlgCongruentTriangles = new List<AlgebraicCongruentTriangles>();


        // Resets all saved data.
        public static void Clear()
        {
            candidateGeoCongruentTriangles.Clear();
            candidateAlgCongruentTriangles.Clear();
        }

        //
        // Implements transitivity with equations
        // Congruent(Triangle(A, B, C), Triangle(D, E, F)), Congruent(Triangle(L, M, N), Triangle(D, E, F)) -> Congruent(Triangle(A, B, C), Triangle(L, M, N))
        //
        // This includes CongruentSegments and CongruentAngles
        //
        // Generation of new equations is restricted to the following rules; let G be Geometric and A algebriac
        //     G + G -> A
        //     G + A -> A
        //     A + A -X> A  <- Not allowed
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.TRANSITIVE_CONGRUENT_TRIANGLES;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is GeometricCongruentTriangles)
            {
                GeometricCongruentTriangles newGCTS = clause as GeometricCongruentTriangles;

                if (newGCTS.IsReflexive()) return newGrounded;

                foreach (GeometricCongruentTriangles oldGCTS in candidateGeoCongruentTriangles)
                {
                    newGrounded.AddRange(InstantiateTransitive(oldGCTS, newGCTS));
                }

                foreach (AlgebraicCongruentTriangles oldACTS in candidateAlgCongruentTriangles)
                {
                    newGrounded.AddRange(InstantiateTransitive(oldACTS, newGCTS));
                }

                candidateGeoCongruentTriangles.Add(newGCTS);
            }
            else if (clause is AlgebraicCongruentTriangles)
            {
                AlgebraicCongruentTriangles newACTS = clause as AlgebraicCongruentTriangles;

                if (newACTS.IsReflexive()) return newGrounded;

                foreach (GeometricCongruentTriangles oldGCTS in candidateGeoCongruentTriangles)
                {
                    newGrounded.AddRange(InstantiateTransitive(oldGCTS, newACTS));
                }

                candidateAlgCongruentTriangles.Add(newACTS);
            }

            return newGrounded;
        }

        public static List<EdgeAggregator> InstantiateTransitive(CongruentTriangles cts1, CongruentTriangles cts2)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            Dictionary<Point, Point> firstTriangleCorrespondence = cts1.HasTriangle(cts2.ct1);
            Dictionary<Point, Point> secondTriangleCorrespondence = cts1.HasTriangle(cts2.ct2);

            // Same Congruence
            if (firstTriangleCorrespondence != null && secondTriangleCorrespondence != null) return newGrounded;

            // No relationship between congruences
            if (firstTriangleCorrespondence == null && secondTriangleCorrespondence == null) return newGrounded;

            // Acquiring the triangle that links the congruences
            Triangle linkTriangle = firstTriangleCorrespondence != null ? cts2.ct1 : cts2.ct2;
            List<Point> linkPts = linkTriangle.points;

            Dictionary<Point, Point> otherCorrGCTSpts = cts1.OtherTriangle(linkTriangle);
            Dictionary<Point, Point> otherCorrCTSpts = cts2.OtherTriangle(linkTriangle);

            // Link the other triangles together in a new congruence
            Dictionary<Point, Point> newCorrespondence = new Dictionary<Point,Point>();
            foreach (Point linkPt in linkPts)
            {
                Point otherGpt;
                if (!otherCorrGCTSpts.TryGetValue(linkPt, out otherGpt)) throw new ArgumentException("Something strange happened in Triangle correspondence.");

                Point otherCpt;
                if (!otherCorrCTSpts.TryGetValue(linkPt, out otherCpt)) throw new ArgumentException("Something strange happened in Triangle correspondence.");

                newCorrespondence.Add(otherGpt, otherCpt);
            }

            List<Point> triOne = new List<Point>(); 
            List<Point> triTwo = new List<Point>();
            foreach (KeyValuePair<Point, Point> pair in newCorrespondence)
            {
                triOne.Add(pair.Key);
                triTwo.Add(pair.Value);
            }

            //
            // Create the new congruence
            //
            AlgebraicCongruentTriangles acts = new AlgebraicCongruentTriangles(new Triangle(triOne), new Triangle(triTwo));

            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(cts1);
            antecedent.Add(cts2);

            newGrounded.Add(new EdgeAggregator(antecedent, acts, annotation));

            return newGrounded;
        }
    }
}