using System;
using System.Collections.Generic;
using System.Text;

namespace Worker
{
    public static class DictionaryHelpers
    {
        public static int CountValues(this Dictionary<string, int> dictionary)
        {
            int count = 0;
            foreach(var item in dictionary)
            {
                count += item.Value;
            }

            return count;
        }
    }
}
