using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSimLib.util
{
    public static class ListHelper
    {
        // Most efficient way to remove multiple items from a IList<T>
        // http://stackoverflow.com/questions/18027575/most-efficient-way-to-remove-multiple-items-from-a-ilistt
        public static void RemoveAll<T>(this IList<T> iList, IEnumerable<T> itemsToRemove)
        {
            var set = new HashSet<T>(itemsToRemove);

            var list = iList as List<T>;
            if (list == null)
            {
                int i = 0;
                while (i < iList.Count)
                {
                    if (set.Contains(iList[i])) iList.RemoveAt(i);
                    else i++;
                }
            }
            else
            {
                list.RemoveAll(set.Contains);
            }
        }
    }
}
