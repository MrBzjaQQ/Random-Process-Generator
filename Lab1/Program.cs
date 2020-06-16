using System;

namespace Lab1
{
    class Program
    {
        static Func<double, double, double> correlationFunc = (double t1, double t2) => 1 / (1 + Math.Abs(t1 - t2));
        const int flowCount = 1000;
        const double mathExpect = 0;
        static void Main(string[] args)
        {
            RandomFlowParameters parameters = new RandomFlowParameters
            {
                A = 421,
                B = 54773,
                N = 259200,
                Count = 1000,
                MathExpect = 1.5,
                FlowLength = 100,
                H = 0.01,
                CorrelationFunc = correlationFunc,
                FlowsFilename = "Flows.csv",
                ResultsFilename = "Results.csv",
            };
            RandomFlow flow = new RandomFlow(parameters);
            flow.ExportResults();
        }
    }
}
