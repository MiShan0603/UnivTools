using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnivTools
{
    static public class Utils
    {
        static public bool IsDigit(string text)
        {
            foreach (char c in text)
            {
                if (!char.IsDigit(c))
                    return false;
            }

            return true;
        }

    }
}
