using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lab1
{
    public class RandomFlow
    {
        public RandomFlow(RandomFlowParameters parameters)
        {
            _flowLength = parameters.FlowLength;
            _a = parameters.A;
            _b = parameters.B;
            _n = parameters.N;
            _N = parameters.Count;
            _h = parameters.H;
            _avg = parameters.MathExpect;
            _correlationFunc = parameters.CorrelationFunc;
            _flowsFilename = parameters.FlowsFilename;
            _resultsFilename = parameters.ResultsFilename;

            init();
            generateFlow();
        }
        public void ExportResults()
        {
            using (FileStream fs = new FileStream(_flowsFilename, FileMode.Create, FileAccess.ReadWrite))
            {
                StreamWriter sw = new StreamWriter(fs);
                for(int i = 0; i < _randomProcess.Count; i++)
                {
                    List<double> process = _randomProcess[i];
                    string processStr = $"process {i + 1}";
                    foreach (double val in process)
                    {
                        processStr += $";{val}";
                    }
                    processStr += "\n";
                    sw.Write(processStr);
                }
                sw.Close();
            }

            using (FileStream fs = new FileStream(_resultsFilename, FileMode.Create, FileAccess.ReadWrite))
            {
                StreamWriter sw = new StreamWriter(fs);
                string data = "avg";
                foreach(var avg in _mEmp)
                {
                    data += $";{avg}";
                }

                data += "\ndispersion";
                foreach(var disp in _dispersion)
                {
                    data += $";{disp}";
                }

                data += "\nK";
                foreach(var k in _K)
                {
                    data += $";{k}";
                }

                data += "\nKemp";
                foreach(var k in _kEmp)
                {
                    data += $";{k}";
                }

                data += "\n";

                sw.Write(data);
                sw.Close();
            }
        }
        private void generateFlow()
        {
            generateTimeTicks();
            generateDispersion();
            generateSigma();
            generateRandomProcess();
            calculateMEmp();
            calculateKEmp();
        }

        private void init()
        {
            _ndg = new NormalDistributionGenerator(_a, _b, _n);
            _timeTicks = new List<double>();
            _ft = new List<List<double>>();
            _sigma = new List<List<double>>();
            _randomProcess = new List<List<double>>();
            _dispersion = new List<double>();
            _K = new List<double>();
            _kEmp = new List<double>();
            _mEmp = new List<double>();
        }
        private void generateTimeTicks()
        {
            for (int i = 1; i < _flowLength + 1; i++)
            {
                _timeTicks.Add((i - 1) * _h);
            }
        }
        private void generateDispersion()
        {
            _dispersion.Add(_correlationFunc(_timeTicks[0], _timeTicks[0]));
            for (int i = 0; i < _flowLength; i++)
            {
                _ft.Add(new List<double>());
            }
            for (int i = 0; i < _flowLength; i++)
            {
                _ft[0].Add(_correlationFunc(_timeTicks[0], _timeTicks[i]) / _dispersion[0]);
            }
            for (int i = 1; i < _flowLength - 1; i++)
            {
                _dispersion.Add(calculateDispersion(i));
                if (_dispersion[_dispersion.Count - 1] == 0)
                {
                    return;
                }
                for (int j = 0; j < i; j++)
                {
                    _ft[i].Add(0);
                }
                for (int j = i; j < _flowLength; j++)
                {
                    _ft[i].Add(makeUnknownFuncVal(i, j));
                }
            }
            for (int i = 0; i < _flowLength; i++)
            {
                _ft[_flowLength - 1].Add(0);
            }
            _ft[_flowLength - 1].Add(1);
            _dispersion.Add(calculateDispersion(_flowLength - 1));
        }

        private double calculateDispersion(int tickIndex)
        {
            double sum = 0;
            for (int i = 0; i < tickIndex; i++)
            {
                sum += _ft[i][tickIndex] * _ft[i][tickIndex] * _dispersion[i];
            }
            return _correlationFunc(_timeTicks[tickIndex], _timeTicks[tickIndex]) - sum;
        }

        private double makeUnknownFuncVal(int tickIndex, int stepIndex)
        {
            double sum = 0;
            for (int i = 0; i <= tickIndex - 1; i++)
            {
                sum += _ft[i][tickIndex] * _ft[i][stepIndex] * _dispersion[i];
            }
            return (_correlationFunc(_timeTicks[tickIndex], _timeTicks[stepIndex]) - sum) / _dispersion[tickIndex];
        }

        private void generateSigma()
        {
            for (int i = 0; i < _N; i++)
            {
                _sigma.Add(new List<double>());
            }
            for (int j = 0; j < _N; j++)
            {
                for (int i = 0; i < _flowLength; i++)
                {
                    _sigma[j].Add(_ndg.generateNextPair(0, Math.Pow(_dispersion[i], 0.5)).ElementAt(0));
                }
            }
        }

        private void generateRandomProcess()
        {
            for (int i = 0; i < _N; i++)
            {
                _randomProcess.Add(new List<double>());
            }
            for (int i = 0; i < _N; i++)
            {
                for (int j = 0; j < _flowLength; j++)
                {
                    double sum = 0;
                    for (int k = 0; k <= j; k++)
                    {
                        sum += _sigma[i][k] * _ft[k][j];
                    }
                    _randomProcess[i].Add(_avg + sum);
                }
            }
        }

        private void calculateKEmp()
        {
            for (int i = 0; i < _flowLength; i++)
            {
                double sum = 0;
                for (int j = 0; j < _N; j++)
                {
                    sum += _randomProcess[j][0] * _randomProcess[j][i];
                }

                double sum2 = 0;
                for (int j = 0; j < _N; j++)
                {
                    sum2 += _randomProcess[j][i];
                }

                double sum3 = 0;
                for (int j = 0; j < _N; j++)
                {
                    sum3 += _randomProcess[j][i];
                }

                _kEmp.Add(Math.Abs(sum - (sum3 * sum2) / _N) / (_N - 1));
            }
            
            for (int i = 0; i < _flowLength; i++)
            {
                _K.Add(_correlationFunc(_timeTicks[0], _timeTicks[i]));
            }
        }

        private void calculateMEmp()
        {
            for (int i = 0; i < _flowLength; i++)
            {
                double sum = 0;
                for (int j = 0; j < _N; j++)
                {
                    sum += _randomProcess[j][i];
                }
                _mEmp.Add(sum / _N);
            }
        }

        List<double> _timeTicks;
        List<double> _dispersion;
        List<List<double>> _ft;
        List<List<double>> _sigma;
        List<List<double>> _randomProcess;
        List<double> _kEmp;
        List<double> _mEmp;
        List<double> _K;
        NormalDistributionGenerator _ndg;
        private int _a;
        private int _b;
        private int _n;
        private int _N;
        private Func<double, double, double> _correlationFunc;
        private int _flowLength;
        private double _h;
        private double _avg;
        private string _flowsFilename;
        private string _resultsFilename;
    }
}
