using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModIF
{
    public interface IModdingInterface
    {
        string GetBaseGamePath();
        bool RegisterHandler<T>(string handlerName, EventHandler<T> handler) where T:EventArgs;
    }
}
