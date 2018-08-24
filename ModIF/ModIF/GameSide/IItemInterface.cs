using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModIF
{
    public interface IItemInterface
    {
        IItemDesc CreateNewItem();
        bool RegisterNewItem(IItemDesc item);
    }
}
