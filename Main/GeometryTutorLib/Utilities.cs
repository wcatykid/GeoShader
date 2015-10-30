using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace GeometryTutorLib
{
    public static class Utilities
    {
        public static readonly bool OVERRIDE_DEBUG = false;

        public static readonly bool DEBUG              = OVERRIDE_DEBUG && true;
        public static readonly bool CONSTRUCTION_DEBUG = OVERRIDE_DEBUG && true;   // Generating clauses when analyzing input figure
        public static readonly bool PEBBLING_DEBUG     = OVERRIDE_DEBUG && false;   // Hypergraph edges and pebbled nodes
        public static readonly bool PROBLEM_GEN_DEBUG = OVERRIDE_DEBUG && true;   // Generating the actual problems
        public static readonly bool BACKWARD_PROBLEM_GEN_DEBUG = OVERRIDE_DEBUG && true;   // Generating backward problems
        public static readonly bool ATOMIC_REGION_GEN_DEBUG = OVERRIDE_DEBUG && true;   // Generating atomic regions
        public static readonly bool SHADED_AREA_SOLVER_DEBUG = OVERRIDE_DEBUG && true;   // Solving a shaded area problem.
        public static readonly bool FIGURE_SYNTHESIZER_DEBUG = true;   // Solving a shaded area problem.

        // If the user specifies that an axiom, theorem, or definition is not to be used.
        public static readonly bool RESTRICTING_AXS_DEFINITIONS_THEOREMS = true;

        // Handles negatives and positives.
        public static int Modulus(int x, int m) { return (x % m + m) % m; }

        //
        // Given a list, remove duplicates
        //
        public static List<T> RemoveDuplicates<T>(List<T> list) where T : ConcreteAST.GroundedClause
        {
            List<T> cleanList = new List<T>();

            for (int i = 0; i < list.Count - 1; i++)
            {
                if (list[i] != null)
                {
                    for (int j = i + 1; j < list.Count; j++)
                    {
                        if (list[j] != null && IsDuplicate<T>(list[i], list[j]))
                        {
                            list[j] = null;
                        }
                    }
                }
            }

            foreach (T t in list)
            {
                if (t != null)
                {
                    cleanList.Add(t);
                }
            }

            return cleanList;
        }

        private static bool IsDuplicate<T>(T clause1, T clause2) where T : ConcreteAST.GroundedClause
        {
            if (clause1 is GeometryTutorLib.ConcreteAST.Angle)
            {
                //Do not want to remove angles that are different but have coinciding rays (would cause problems when trying to find
                //inscribed and central angles inside circles)
                GeometryTutorLib.ConcreteAST.Angle angle = clause1 as GeometryTutorLib.ConcreteAST.Angle;
                return angle.EqualRays(clause2);
            }
            else return clause1.StructurallyEquals(clause2);
        }

        //
        // Given a list of grounded clauses, add a new value which is structurally unique.
        //
        public static int StructuralIndex<T>(List<T> list, T t) where T : ConcreteAST.GroundedClause
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].StructurallyEquals(t)) return i;
            }

            return -1;
        }

        //
        // Given a list of grounded clauses, add a new value which is structurally unique.
        //
        public static bool HasStructurally<T>(List<T> list, T t) where T : ConcreteAST.GroundedClause
        {
            return Utilities.StructuralIndex<T>(list, t) != -1;
        }

        //
        // Given a list of grounded clauses, get the structurally unique.
        //
        public static T GetStructurally<T>(List<T> list, T t) where T : ConcreteAST.GroundedClause
        {
            foreach (T oldT in list)
            {
                if (oldT.StructurallyEquals(t)) return oldT;
            }

            return null;
        }

        //
        // Given a list of grounded clauses, add a new value which is structurally unique.
        //
        public static bool AddStructurallyUnique<T>(List<T> list, T t) where T : ConcreteAST.GroundedClause
        {
            if (HasStructurally<T>(list, t)) return false;

            list.Add(t);

            return true;
        }

        //
        // Given a list of grounded clauses, add a new value which is structurally unique.
        //
        public static int StructuralIndex<T, A>(List<KeyValuePair<T, A>> list, T t) where T : ConcreteAST.GroundedClause
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Key.StructurallyEquals(t)) return i;
            }

            return -1;
        }


        // Given a sorted list, insert the element from the front to the back.
        public static void InsertAscendingOrdered(List<int> list, int value)
        {
            // Special Cases
            if (!list.Any())
            {
                list.Add(value);
                return;
            }

            if (value > list[list.Count-1])
            {
                list.Add(value);
                return;
            }

            // General Case
            for (int i = 0; i < list.Count; i++)
            {
                if (value < list[i])
                {
                    list.Insert(i, value);
                    return;
                }
            }
        }

        // Acquire the index of the clause in the hypergraph based only on structure
        public static int StructuralIndex(Hypergraph.Hypergraph<ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> graph, ConcreteAST.GroundedClause g)
        {
            //
            // Handle general case
            //
            List<Hypergraph.HyperNode<ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation>> vertices = graph.vertices;

            for (int v = 0; v < vertices.Count; v++)
            {
                if (vertices[v].data.StructurallyEquals(g)) return v;

                if (vertices[v].data is ConcreteAST.Strengthened)
                {
                    if ((vertices[v].data as ConcreteAST.Strengthened).strengthened.StructurallyEquals(g)) return v;
                }
            }

            //
            // Handle strengthening by seeing if the clause is found without a 'strengthening' component
            //
            ConcreteAST.Strengthened streng = g as ConcreteAST.Strengthened;
            if (streng != null)
            {
                int index = StructuralIndex(graph, streng.strengthened);
                if (index != -1) return index;
            }

            return -1;
        }

        //
        // Acquires the hypergraph index value of the given nodes using structural equality
        //
        public static List<int> CollectGraphIndices(Hypergraph.Hypergraph<ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> graph, List<ConcreteAST.GroundedClause> clauses)
        {
            List<int> indices = new List<int>();

            foreach (ConcreteAST.GroundedClause gc in clauses)
            {
                int index = Utilities.StructuralIndex(graph, gc);
                if (index != -1)
                {
                    indices.Add(index);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("We expect to find the given node (we did not): " + gc.ToString());
                }
            }

            return indices;
        }

        // Ensure uniqueness of additions
        public static void AddUniqueStructurally(List<GeometryTutorLib.ConcreteAST.Figure> figures, GeometryTutorLib.ConcreteAST.Figure f)
        {
            foreach (GeometryTutorLib.ConcreteAST.Figure figure in figures)
            {
                if (figure.StructurallyEquals(f)) return;
            }
            figures.Add(f);
        }

        // -1 is an error
        public static int IntegerRatio(double x, double y)
        {
            return Utilities.CompareValues(x / y, Math.Floor(x / y)) ? (int)Math.Floor(x / y) : -1;
        }

        // -1 is an error
        // A reasonable value for geometry problems must be less than 10 for a ratio
        // This is arbitrarily chosen and can be modeified
        private static readonly int RATIO_MAX = 10;
        public static KeyValuePair<int, int> RationalRatio(double x, double y)
        {
            for (int numer = 2; numer < RATIO_MAX; numer++)
            {
                for (int denom = 1; denom < RATIO_MAX; denom++)
                {
                    if (numer != denom)
                    {
                        if (Utilities.CompareValues(x / y, (double)(numer) / denom))
                        {
                            int gcd = GCD(numer, denom);
                            return numer > denom ? new KeyValuePair<int, int>(numer / gcd, denom / gcd)
                                                 : new KeyValuePair<int, int>(denom / gcd, numer / gcd);
                        }
                    }
                }
            }

            return new KeyValuePair<int,int>(-1, -1);
        }
        public static KeyValuePair<int, int> RationalRatio(double x)
        {
            for (int val = 2; val < RATIO_MAX; val++)
            {
                // Do we acquire an integer?
                if (Utilities.CompareValues(x * val, Math.Floor(x * val)))
                {
                    int gcd = Utilities.GCD(val, (int)Math.Round(x * val));
                    return x < 1 ? new KeyValuePair<int, int>(val / gcd, (int)Math.Round(x * val) / gcd) :
                                   new KeyValuePair<int, int>((int)Math.Round(x * val) / gcd, val / gcd);
                }
            }

            return new KeyValuePair<int, int>(-1, -1);
        }

        public static bool IsInteger(double x)
        {
            return Utilities.CompareValues(x, (int)x);
        }

        // Makes a list containing a single element
        public static List<T> MakeList<T>(T obj)
        {
            List<T> l = new List<T>();

            l.Add(obj);

            return l;
        }

        // Makes a list containing a single element
        public static bool AddUnique<T>(List<T> list, T obj)
        {
            if (list.Contains(obj)) return false;

            list.Add(obj);
            return true;
        }

        // Are the sets disjoint? Like the merge part of a merge-sort.
        public static List<int> DifferenceIfSubsetOrderedSets(List<int> set1, List<int> set2)
        {
            if (EqualOrderedSets(set1, set2)) return null;

            //
            // Acquire the larger set of integer values.
            //
            List<int> larger = null;
            List<int> smaller = null;

            if (set1.Count < set2.Count)
            {
                larger = set2;
                smaller = set1;
            }
            else
            {
                larger = set1;
                smaller = set2;
            }

            //
            // Find the difference of the larger minus the smaller.
            //
            List<int> diff = new List<int>();

            int s = 0;
            int ell = 0;
            for (int d = 0; s < smaller.Count && d < larger.Count; d++)
            {
                if (smaller[s] < larger[ell])
                {
                    return null;
                }
                else if (larger[ell] < smaller[s])
                {
                    diff.Add(larger[ell]);
                    ell++;
                }
                else if (smaller[s] == larger[ell])
                {
                    s++;
                    ell++;
                }
            }

            if (s < smaller.Count) return null;

            // Pick up the rest of the larger set.
            for ( ; ell < larger.Count; ell++)
            {
                diff.Add(larger[ell]);
            }

            return diff;
        }


        // Are the sets disjoint? Like the merge part of a merge-sort.
        public static List<int> UnionIfDisjointOrderedSets(List<int> set1, List<int> set2)
        {
            if (EqualOrderedSets(set1, set2)) return null;

            List<int> union = new List<int>();

            int s1 = 0;
            int s2 = 0;
            for (int u = 0; s1 < set1.Count && s2 < set2.Count && u < set1.Count + set2.Count; u++)
            {
                if (set1[s1] < set2[s2])
                {
                    union.Add(set1[s1]);
                    s1++;
                }
                else if (set2[s2] < set1[s1])
                {
                    union.Add(set2[s2]);
                    s2++;
                }
                else if (set1[s1] == set2[s2]) return null;
            }

            //
            // Pick up remainder of values from one of the lists.
            //
            for ( ; s1 < set1.Count; s1++)
            {
                union.Add(set1[s1]);
            }

            for ( ; s2 < set2.Count; s2++)
            {
                union.Add(set2[s2]);
            }

            return union;
        }

        public static List<int> ComplementList(List<int> theList, int size)
        {
            // Ensure smallest to largest ordering.
            theList.Sort();

            // Eventual complementary list.
            List<int> complement = new List<int>();

            int index, ell;
            for (index = 0, ell = 0; index < size && ell < theList.Count; index++)
            {
                if (index != theList[ell])
                {
                    complement.Add(index);
                }
                else ell++;
            }

            // Picking up remaining values up to size
            for (; index < size; index++)
            {
                complement.Add(index);
            }

            return complement;
        }

        // Makes a list containing a single element
        public static void AddUniqueList<T>(List<T> list, List<T> objList)
        {
            foreach (T o in objList)
            {
                AddUnique<T>(list, o);
            }
        }

        // Is smaller \subseteq larger
        public static bool Subset<T>(List<T> larger, List<T> smaller)
        {
            foreach (T o in smaller)
            {
                if (!larger.Contains(o)) return false;
            }

            return true;
        }

        // Is the list a subset of any of the sets in the list of lists?
        public static bool ListHasSubsetOfSet<T>(List<List<T>> sets, List<T> theSet)
        {
            // Do not consider a new subset which contains an existent polygon.
            foreach (List<T> set in sets)
            {
                if (Subset<T>(theSet, set)) return true;
            }

            return false;
        }

        // Is set1 \equals set2
        public static bool EqualSets<T>(List<T> set1, List<T> set2)
        {
            if (set1.Count != set2.Count) return false;

            return Subset<T>(set1, set2);      // redundant since we checked same size && Subset<T>(set2, set1);
        }

        // Is set1 \equals set2 (and the input sets are ordered for efficiency)
        public static bool EqualOrderedSets(List<int> set1, List<int> set2)
        {
            if (set1.Count != set2.Count) return false;

            for (int i = 0; i < set1.Count; i++)
            {
                if (set1[i] != set2[i]) return false;
            }

            return true;
        }

        // set1 \cap set2
        public static List<T> Intersection<T>(List<T> set1, List<T> set2)
        {
            List<T> inter = new List<T>();

            foreach (T t in set1)
            {
                if (set2.Contains(t)) inter.Add(t);
            }

            return inter;
        }

        public static int GCD(int a, int b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }

        public static readonly double EPSILON = 0.000001;

        public static bool CompareValues(double a, double b)
        {
            return Math.Abs(a - b) < EPSILON;
        }
        public static bool LooseCompareValues(double a, double b)
        {
            return Math.Abs(a - b) < 0.0001;
        }
        public static bool LessThan(double a, double b)
        {
            if (CompareValues(a, b)) return false;
            else return a - b - EPSILON < 0;
        }
        public static bool GreaterThan(double a, double b)
        {
            if (CompareValues(a, b)) return false;
            return a - b + EPSILON > 0;
        }

        private static long[] memoizedFactorials = new long[30];
        public static long Factorial(long x)
        {
            if (x <= 1) return 1;

            // Return saved
            if (memoizedFactorials[x] != 0) return memoizedFactorials[x];

            long result = 1;

            for (int i = 2; i <= x; i++)
            {
                result *= i;
            }

            // Save it
            memoizedFactorials[x] = result;
            return result;
        }

        public static long Permutation(long n, long r)
        {
            if (r == 0) return 0;

            if (n == 0) return 0;

            return ((r >= 0) && (r <= n)) ? Factorial(n) / Factorial(n - r) : 0;
        }

        public static long Combination(long a, long b)
        {
            if (a <= 1) return 1;

            return Factorial(a) / (Factorial(b) * Factorial(a - b));
        }


        //
        // Constructs an integer representation of the powerset based on input value integer n
        // e.g. 4 -> { {}, {0}, {1}, {2}, {3}, {0, 1}, {0, 2}, {0, 3}, {1, 2}, {1, 3}, {2, 3}, {0, 1, 2}, {0, 1, 3}, {0, 2, 3}, {1, 2, 3}, {0, 1, 2, 3} }
        //
        private static readonly int GREATER = 1;
        private static readonly int EQUAL = 0;
        private static readonly int LESS = -1;
        private static int CompareTwoSets(List<int> set1, List<int> set2)
        {
            // Discriminate based on set size foremost
            if (set1.Count < set2.Count) return LESS;
            if (set1.Count > set2.Count) return GREATER;

            for (int i = 0; i < set1.Count && i < set2.Count; i++)
            {
                if (set1[i] < set2[i]) return LESS;
                if (set1[i] > set2[i]) return GREATER;
            }

            return EQUAL;
        }

        private static List<List<int>> ConstructRestrictedPowerSet(int n, int maxCardinality)
        {
            if (n <= 0) return Utilities.MakeList<List<int>>(new List<int>());

            List<List<int>> powerset = ConstructRestrictedPowerSet(n - 1, maxCardinality);
            List<List<int>> newCopies = new List<List<int>>();

            foreach (List<int> intlist in powerset)
            {
                if (intlist.Count < maxCardinality)
                {
                    // Make a copy, add to copy, add to overall list
                    List<int> copy = new List<int>(intlist);
                    copy.Add(n - 1); // We are dealing with indices, subtract 1
                    newCopies.Add(copy);
                }
            }

            powerset.AddRange(newCopies);

            return powerset;
        }

        // A memoized copy of all the powersets. 10 is large for this, we expect max of 5.
        // Note, we use a matrix since maxCardinality may change
        // We maintain ONLY an array because we are using this for a specific purpse in this project
        public static List<List<int>>[] memoized = new List<List<int>>[14];
        public static List<string>[] memoizedCompressed = new List<string>[14];
        public static List<List<int>>[] memoizedWithSingletons = new List<List<int>>[14];
        public static List<string>[] memoizedCompressedWithSingletons = new List<string>[14];
        private static void ConstructPowerSetWithNoEmptyHelper(int n, int maxCardinality)
        {
            if (memoized[n] != null) return;

            // Construct the powerset and remove the emptyset
            List<List<int>> powerset = ConstructRestrictedPowerSet(n, maxCardinality);
            powerset.RemoveAt(0);

            // Sort so the smallest sets are first and sets of the same size are compared based on elements.
            powerset.Sort(CompareTwoSets);

            // Now remove the singleton sets
            powerset.RemoveRange(0, n);

            // Save this construction
            memoized[n] = powerset;

            // Save the compressed versions
            List<string> compressed = new List<string>();
            powerset.ForEach(subset => compressed.Add(CompressUniqueIntegerList(subset)));
            memoizedCompressed[n] = compressed;
        }
        private static void ConstructPowerSetWithNoEmptyHelperWithSingletons(int n, int maxCardinality)
        {
            if (memoizedWithSingletons[n] != null) return;

            // Construct the powerset and remove the emptyset
            List<List<int>> powerset = ConstructRestrictedPowerSet(n, maxCardinality);
            powerset.RemoveAt(0);

            // Sort so the smallest sets are first and sets of the same size are compared based on elements.
            powerset.Sort(CompareTwoSets);

            // Save this construction
            memoizedWithSingletons[n] = powerset;

            // Save the compressed versions
            List<string> compressed = new List<string>();
            powerset.ForEach(subset => compressed.Add(CompressUniqueIntegerList(subset)));
            memoizedCompressedWithSingletons[n] = compressed;
        }
        public static List<List<int>> ConstructPowerSetWithNoEmpty(int n, int maxCardinality)
        {
            ConstructPowerSetWithNoEmptyHelper(n, maxCardinality);

            return memoized[n];
        }
        public static List<List<int>> ConstructPowerSetWithNoEmpty(int n)
        {
            ConstructPowerSetWithNoEmptyHelperWithSingletons(n, n);

            return memoizedWithSingletons[n];
        }

        public static List<string> ConstructPowerSetStringsWithNoEmpty(int n, int maxCardinality)
        {
            ConstructPowerSetWithNoEmptyHelper(n, maxCardinality);

            return memoizedCompressed[n];
        }
        // Unchecked, we assume a unique list of integers
        // Takes an integer list and compresses it into a string: { 0 1 2 } -> 012
        // Note, this is only a useful encoding for unit digits (like with powersets above)
        public static string CompressUniqueIntegerList(List<int> list)
        {
            string compressed = "";
            list.ForEach(item => compressed += item);
            return compressed;
        }
        // Splits a compressed string (from above) into two parts: the substring we have already processed and the tail we have yet to process
        // 012 -> < 01, 2>
        public static KeyValuePair<string, int> SplitStringIntoKnownToProcess(string s)
        {
            return new KeyValuePair<string, int>(s.Substring(0, s.Length - 1), Convert.ToInt32(s[s.Length - 1]) - 48);
        }
        // Decompresses a string of integers directly into an integer list: 012 -> { 0, 1, 2 }
        public static List<int> DecompressStringToList(string s)
        {
            List<int> intList = new List<int>();
            foreach (char c in s) intList.Add(Convert.ToInt32(c) - 48);
            return intList;
        }

        //
        // Get a point in the given list OR create a new list.
        //
        public static ConcreteAST.Point AcquirePoint(List<ConcreteAST.Point> points, ConcreteAST.Point that)
        {
            if (that == null) return null;

            // Avoid parallel line intersections at infinity
            if (double.IsInfinity(that.X) || double.IsInfinity(that.Y) || double.IsNaN(that.X) || double.IsNaN(that.Y)) return null;

            ConcreteAST.Point pt = GetStructurally<ConcreteAST.Point>(points, that);
            if (pt == null) pt = PointFactory.GeneratePoint(that.X, that.Y);
            return pt;
        }

        //
        // Get a point in the given list OR create a new list that is internal to the given segments.
        //
        public static ConcreteAST.Point AcquireRestrictedPoint(List<ConcreteAST.Point> points, ConcreteAST.Point that,
                                                               ConcreteAST.Segment seg1, ConcreteAST.Segment seg2)
        {
            ConcreteAST.Point pt = AcquirePoint(points, that);

            if (pt == null) return null;

            return !seg1.PointLiesOnAndBetweenEndpoints(pt) || !seg2.PointLiesOnAndBetweenEndpoints(pt) ? null : pt;
        }
        public static ConcreteAST.Point AcquireRestrictedPoint(List<ConcreteAST.Point> points, ConcreteAST.Point that,
                                                               ConcreteAST.Arc arc1, ConcreteAST.Arc arc2)
        {
            ConcreteAST.Point pt = AcquirePoint(points, that);

            if (pt == null) return null;

            return !arc1.PointLiesOn(pt) || !arc2.PointLiesOn(pt) ? null : pt;
        }

        public static ConcreteAST.Point AcquireRestrictedPoint(List<ConcreteAST.Point> points, ConcreteAST.Point that,
                                                               ConcreteAST.Segment seg, ConcreteAST.Arc arc)
        {
            ConcreteAST.Point pt = AcquirePoint(points, that);

            if (pt == null) return null;

            return !seg.PointLiesOnAndBetweenEndpoints(pt) || !arc.PointLiesOn(pt) ? null : pt;
        }

        public static string TimeToString(System.TimeSpan span)
        {
            return System.String.Format("{0:00}:{1:00}.{2:00}", span.Minutes, span.Seconds, span.Milliseconds / 10);
        }

        public static List<ConcreteAST.Segment> FilterForMinimal(List<ConcreteAST.Segment> segments)
        {
            List<ConcreteAST.Segment> minimal = new List<ConcreteAST.Segment>();

            for (int s1 = 0; s1 < segments.Count; s1++)
            {
                bool min = true;
                for (int s2 = 0; s2 < segments.Count; s2++)
                {
                    if (s1 != s2)
                    {
                        if (segments[s1].HasSubSegment(segments[s2]))
                        {
                            min = false;
                            break;
                        }
                    }
                }
                if (min) minimal.Add(segments[s1]);
            }

            return minimal;
        }

        public static List<ConcreteAST.Segment> FilterForMaximal(List<ConcreteAST.Segment> segments)
        {
            List<ConcreteAST.Segment> maximal = new List<ConcreteAST.Segment>();

            for (int s1 = 0; s1 < segments.Count; s1++)
            {
                bool max = true;
                for (int s2 = 0; s2 < segments.Count; s2++)
                {
                    if (s1 != s2)
                    {
                        if (segments[s2].HasSubSegment(segments[s1]))
                        {
                            max = false;
                            break;
                        }
                    }
                }
                if (max) maximal.Add(segments[s1]);
            }

            return maximal;
        }

        public static List<ConcreteAST.Segment> FilterShared(List<ConcreteAST.Segment> segments, out List<ConcreteAST.Segment> shared)
        {
            List<ConcreteAST.Segment> unique = new List<ConcreteAST.Segment>();
            shared = new List<ConcreteAST.Segment>();

            bool[] marked = new bool[segments.Count];
            for (int s1 = 0; s1 < segments.Count; s1++)
            {
                if (!marked[s1])
                {
                    for (int s2 = 0; s2 < segments.Count; s2++)
                    {
                        if (s1 != s2)
                        {
                            if (segments[s1].HasSubSegment(segments[s2]))
                            {
                                marked[s1] = true;
                                marked[s2] = true;
                            }
                        }
                    }
                }
            }

            //
            // Pick up all unmarked segments.
            //
            for (int m = 0; m < marked.Length; m++)
            {
                if (!marked[m]) unique.Add(segments[m]);
                else shared.Add(segments[m]);
            }

            return unique;
        }
    }

    public class Stopwatch
    {
        public static readonly bool IsHighResolution = false;
        public static readonly long Frequency = TimeSpan.TicksPerSecond;

        public TimeSpan Elapsed
        {
            get
            {
                if (!this.StartUtc.HasValue)
                {
                    return TimeSpan.Zero;
                }
                if (!this.EndUtc.HasValue)
                {
                    return (DateTime.UtcNow - this.StartUtc.Value);
                }
                return (this.EndUtc.Value - this.StartUtc.Value);
            }
        }

        public long ElapsedMilliseconds
        {
            get
            {
                return this.ElapsedTicks / TimeSpan.TicksPerMillisecond;
            }
        }
        public long ElapsedTicks { get { return this.Elapsed.Ticks; } }
        public bool IsRunning { get; private set; }
        private DateTime? StartUtc { get; set; }
        private DateTime? EndUtc { get; set; }

        public static long GetTimestamp()
        {
            return DateTime.UtcNow.Ticks;
        }

        public void Reset()
        {
            Stop();
            this.EndUtc = null;
            this.StartUtc = null;
        }

        public void Start()
        {
            if (this.IsRunning)
            {
                return;
            }
            if ((this.StartUtc.HasValue) &&
                (this.EndUtc.HasValue))
            {
                // Resume the timer from its previous state
                this.StartUtc = this.StartUtc.Value +
                    (DateTime.UtcNow - this.EndUtc.Value);
            }
            else
            {
                // Start a new time-interval from scratch
                this.StartUtc = DateTime.UtcNow;
            }
            this.IsRunning = true;
            this.EndUtc = null;
        }

        public void Stop()
        {
            if (this.IsRunning)
            {
                this.IsRunning = false;
                this.EndUtc = DateTime.UtcNow;
            }
        }

        public static Stopwatch StartNew()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            return stopwatch;
        }
    }
}
