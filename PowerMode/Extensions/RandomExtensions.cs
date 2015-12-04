using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerMode.Extensions
{
    public static class RandomExtensions
    {
        public static int NextSignSwap(this Random random)
        {
            return random.Next(0, 2) == 1 ? 1 : -1;
        }
    }
}