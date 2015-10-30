using System;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.Calculational_Logic
{
    /// <summary>
    /// Class wrapper around a figure.
    /// </summary>
    public class FigureNode : CalculationalASTNode
    {
        public Figure theFigure { get; private set; }

        public FigureNode(Figure f)
        {

        }
    }
}
