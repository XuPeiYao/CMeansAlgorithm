using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMeans {
    public class Program {
        public static void Main(string[] args) {
            Random rand = new Random();
            List<double[]> data = Enumerable.Range(1, 40).Select(x => new double[] { rand.NextDouble(0,999)}).ToList();
            
            List<Classify> result = CMeans(3, data);

            foreach(var item in result) {
                Console.WriteLine($"===== {item.Id} (Count:{item.Values.Count}) =====");
                foreach(var item2 in item.Values) {
                    Console.WriteLine(string.Join(", ", item2));
                }
            }
            Console.ReadKey();
        }

        public static List<Classify> CMeans(int classCount, List<double[]> Values) => CMeans(classCount, 2, 0.001, Values);
        public static List<Classify> CMeans(int classCount, double m, List<double[]> Values) => CMeans(classCount,m, 0.001, Values);
        public static List<Classify> CMeans(int classCount, double m,double diff, List<double[]> Values) {
            Random rand = new Random(DateTime.Now.Millisecond);//每個值在各組的機率
            double[][] u = Enumerable.Range(0, Values.Count).Select(x => {
                double[] c = new double[classCount];
                for(int i = 0;i < c.Length; i++) {
                    c[i] = rand.NextDouble(0, 1 - c.Sum());
                    if (i == c.Length - 1) c[i] = 1 - c.Sum();
                }
                return c;
            }).ToArray();

            double _j = -1;List<double[]> Centers;
            while (true) {
                Centers = GetCenters(classCount, m, u, Values);
                double j_new = J(m, u, Centers, Values);
                if(_j != -1 && Math.Abs(j_new - _j) < diff) break;
                _j = j_new;
                
                for(int i = 0; i < u.Length ; i++) {
                    for(int j = 0; j < u[i].Length; j++) {
                        u[i][j] = 1/ Enumerable.Range(0,classCount).Select(x=>
                            Math.Pow(
                                Math.Sqrt(Dist(Values[i],Centers[j])) / Math.Sqrt(Dist(Values[i],Centers[x]))
                                ,2/(m-1)
                            )
                        ).Sum();
                    }
                }
            }

            List<Classify> result = Enumerable.Range(0, classCount).Select((x, i) => new Classify() { Id = i }).ToList();
            for(int i = 0; i < Values.Count; i++) {
                var index = Array.IndexOf(u[i],u[i].Max());

                result[index].Values.Add(Values[i]);
            }

            return result;
        }

        public static List<double[]> GetCenters(int classCount,double m,double[][] u,List<double[]> Values) {
            return Enumerable.Range(0, classCount).Select(i => //List<double[]>
                Enumerable.Range(0, Values.First().Length).Select(x =>//double[]
                    Enumerable.Range(0, Values.Count).Select(j =>
                        Math.Pow(u[j][i], m) * Values[j][x]
                    ).Sum()
                    /
                    Enumerable.Range(0, Values.Count).Select(j =>
                        Math.Pow(u[j][i], m)
                    ).Sum()
                ).ToArray()
            ).ToList();
        }

        public static double J(double m,double[][] u,List<double[]> centers,List<double[]> values) {
            return centers.Select((x, i) =>
                       values.Select((y, j) =>
                           Math.Pow(u[j][i], m) * Dist(y,x)
                       ).Sum()
                   ).Sum();
        }

        public static double Dist(double[] value, double[] center) {
            return value.Select((x, i) =>
                Math.Pow(x - center[i],2)
            ).Sum();
        }
    }
}
