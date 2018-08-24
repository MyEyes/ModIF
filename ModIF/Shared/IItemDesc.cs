using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModIF
{
    public interface IItemDesc : IGameEntity
    {
        string Name { get; set; }
        string Description { get; set; }
        string Type { get; set; }

        bool SetGenericProperty(string property, object value);
        string[] GetProperties();
    }
}
