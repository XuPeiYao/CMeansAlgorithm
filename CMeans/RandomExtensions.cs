using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMeans {
    public static class RandomExtensions {
        public static double NextDouble(this Random rand,double min,double max) {
            return (rand.NextDouble() * (max - min)) + min;
        }
    }
}
