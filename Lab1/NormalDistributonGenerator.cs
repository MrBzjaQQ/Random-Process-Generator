using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1
{
    public class NormalDistributionGenerator
    {
        public NormalDistributionGenerator(int a, int b, int generatorModule)
        {
            this.generatorModule = generatorModule;
            generator = new Generator(a, b, generatorModule);
        }
        public List<double> generate(int amount)
        {
            List<double> numbers = new List<double>();
            for (int i = 0; i < amount / 2; i++)
            {
                foreach (double next in generateNextPair())
                    numbers.Add(next);
            }
            return numbers;
        }
        public IEnumerable<double> generateNextPair()
        {
            double u1, u2, V1, V2, S;
            do
            {
                previousNumberForLCG = generator.getNextNumber(previousNumberForLCG);
                u1 = (double)previousNumberForLCG / generatorModule;
                previousNumberForLCG = generator.getNextNumber(previousNumberForLCG);
                u2 = (double)previousNumberForLCG / generatorModule;
                V1 = 2 * u1 - 1;
                V2 = 2 * u2 - 1;
                S = Math.Pow(V1, 2.0) + Math.Pow(V2, 2.0);
            } while (S > 1);
            yield return V1 * Math.Sqrt(-2 * Math.Log(S) / S);
            yield return V2 * Math.Sqrt(-2 * Math.Log(S) / S);
        }
        public IEnumerable<double> generateNextPair(double avg, double dispersion)
        {
            double u1, u2, V1, V2, S;
            do
            {
                previousNumberForLCG = generator.getNextNumber(previousNumberForLCG);
                u1 = (double)previousNumberForLCG / generatorModule;
                previousNumberForLCG = generator.getNextNumber(previousNumberForLCG);
                u2 = (double)previousNumberForLCG / generatorModule;
                V1 = 2 * u1 - 1;
                V2 = 2 * u2 - 1;
                S = Math.Pow(V1, 2.0) + Math.Pow(V2, 2.0);
            } while (S > 1);
            yield return V1 * Math.Sqrt(-2 * Math.Log(S) / S) * dispersion + avg;
            yield return V2 * Math.Sqrt(-2 * Math.Log(S) / S) * dispersion + avg;
        }
        private Generator generator;
        private int previousNumberForLCG = 1;
        private int generatorModule;
    }

}
