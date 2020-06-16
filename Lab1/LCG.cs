using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1
{
    public class Generator
    {
        public Generator(int a, int c, int N)
        {
            this.a = a;
            this.c = c;
            this.N = N;
        }
        public List<int> generate(int amount)
        {
            List<int> values = new List<int>();
            int value = X0;
            for (int i = 0; i < amount; i++)
            {
                int next = getNextNumber(value);
                value = next;
                values.Add(next);
            }
            return values;
        }
        public int getNextNumber(int previous)
        {
            return (a * previous + c) % N;
        }
        private const int X0 = 1;
        private int a, c, N;
    }
}
