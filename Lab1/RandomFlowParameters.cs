using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1
{
    public class RandomFlowParameters
    {
        public double MathExpect { get; set; }
        public Func<double, double, double> CorrelationFunc { get; set; }
        public int A { get; set; }
        public int B { get; set; }
        public int N { get; set; }
        public int FlowLength { get; set; }
        public int Count { get; set; }
        public double H { get; set; }
        public string FlowsFilename { get; set; }
        public string ResultsFilename { get; set; }

    }
}
