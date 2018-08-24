using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModIF.GameSide.Item
{
    static class ItemInterfaceHelper
    {
        public static string[] GetItemSubTypes(Type itemBaseClass)
        {
            return itemBaseClass.Assembly.ExportedTypes.Where((t) => t.IsSubclassOf(itemBaseClass)).Select((t) => t.FullName).ToArray();
        }

        public static object InstantiateItemDescFromType(string typeName, params object[] parameters)
        {

        }
    }
}
