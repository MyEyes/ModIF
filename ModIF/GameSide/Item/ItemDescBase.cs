using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ModIF.GameSide
{
    public class ItemDescBase: MarshalByRefObject, IItemDesc
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }

        public virtual bool SetGenericProperty(string property, object value)
        {
            Type t = this.GetType();
            try
            {
                PropertyInfo pi = t.GetProperty(property, value.GetType());
                pi.SetValue(this, value);
                return true;
            }
            catch
            {
                //Return false on any exception
                return false;
            }
        }

        public virtual string[] GetProperties()
        {
            Type t = this.GetType();
            var props = t.GetProperties();
            return props.Select((p) => p.Name).ToArray();
        }
    }
}
