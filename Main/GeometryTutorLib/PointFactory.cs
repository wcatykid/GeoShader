using System;
using System.Collections.Generic;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib
{
    //
    // Given a pair of coordinates; generate a unique name for it; return that point object.
    // Names go from A..Z..AA..ZZ..AAA...ZZZ
    //
    public static class PointFactory
    {
        private const string prefix = "_*_";
        private static string currentName = "A";
        private static int numLetters = 1;

        private static List<Point> database = new List<Point>();

        public static void Initialize(List<Point> initPoints)
        {
            database.AddRange(initPoints);
        }

        public static bool IsGenerated(Point that)
        {
            if (that == null) return false;

            if (that.name.Length < prefix.Length) return false;

            return prefix == that.name.Substring(0, 3);
        }

        public static Point GeneratePoint(double x, double y)
        {
            int index = database.IndexOf(new Point("", x, y));
            if (index != -1) return database[index];

            Point newPt = new Point(GetCurrentName(), x, y);
            Point oldPt = Utilities.GetStructurally<Point>(database, newPt);

            if (oldPt != null) return oldPt;

            database.Add(newPt);

            return newPt;
        }

        public static Point GeneratePoint(Point pt)
        {
            return GeneratePoint(pt.X, pt.Y);
        }

        // Reset for the next problem
        public static void Reset()
        {
            currentName = "A";
            numLetters = 1;
        }

        private static string GetCurrentName()
        {
            string name = prefix + currentName;

            UpdateName();

            return name;
        }

        private static void UpdateName()
        {
            // Restart at the beginning of the alphabet
            if (currentName[0] == 'Z')
            {
                // We rolled over to more letter.
                numLetters++;

                currentName = "";
                for (int i = 0; i < numLetters; i++)
                {
                    currentName += 'A';
                }
            }
            // Simple increment from A to B, etc.
            else
            {
                char alpha = currentName[0];
                alpha++;

                currentName = "";
                for (int i = 0; i < numLetters; i++)
                {
                    currentName += alpha;
                }
            }
        }
    }
}
