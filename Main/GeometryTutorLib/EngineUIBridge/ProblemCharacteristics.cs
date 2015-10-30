using System.Collections.Generic;

namespace GeometryTutorLib.EngineUIBridge
{
    public class ProblemCharacteristics
    {
        private static ProblemCharacteristics instance = null;

        public List<Relationship> Relationships { get; private set; }
        public int LowerWidth { get; set; }
        public int UpperWidth { get; set; }
        public int LowerLength { get; set; }
        public int UpperLength { get; set; }

        /// <summary>
        /// Create a new ProblemCharacteristics.
        /// widths and legths default to 5.
        /// </summary>
        private ProblemCharacteristics()
        {
            Relationships = Relationship.GetRelationships();
            LowerWidth = 5;
            UpperWidth = 5;
            LowerLength = 5;
            UpperLength = 5;
        }

        /// <summary>
        /// Get an instance of the problem characteristics.
        /// </summary>
        /// <returns>The problem characteristics.</returns>
        public static ProblemCharacteristics GetInstance()
        {
            if (instance == null)
            {
                instance = new ProblemCharacteristics();
            }
            return instance;
        }
    }
}
