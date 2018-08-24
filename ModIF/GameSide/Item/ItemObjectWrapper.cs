using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Permissions;
using System.Reflection;

namespace ModIF.GameSide.Item
{
    /// <summary>
    /// Class to wrap an item to communicate its properties to the isolated mod app domains
    /// Used by the ItemInterfaceHelper class
    /// </summary>
    //Make sure this is only instantiated from trusted code
    [SecurityPermission(System.Security.Permissions.SecurityAction.Demand, Unrestricted=true)]
    public class ItemObjectWrapper : MarshalByRefObject, IItemDesc
    {
        private object wrappedItem;

        private string idProperty;
        private string nameProperty;
        private string descriptionProperty;
        private string typeProperty;

        private string defaultID = null;
        public string ID
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(idProperty))
                    return GetGenericProperty<string>(idProperty);
                else return defaultID;
            }
            set 
            {
                if (!string.IsNullOrWhiteSpace(idProperty))
                    SetGenericProperty<string>(idProperty, value);
                else defaultID = value;
            }
        }

        private string defaultName = null;
        public string Name
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(nameProperty))
                    return GetGenericProperty<string>(nameProperty);
                else
                    return defaultName;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(nameProperty))
                    SetGenericProperty<string>(nameProperty, value);
                else
                    defaultName = value;
            }
        }

        private string defaultDescription = null;
        public string Description
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(descriptionProperty))
                    return GetGenericProperty<string>(descriptionProperty);
                else
                    return defaultDescription;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(descriptionProperty))
                    SetGenericProperty<string>(descriptionProperty, value);
                else
                    defaultDescription = value;
            }
        }
        private string defaultType = null;
        public string Type
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(typeProperty))
                    return GetGenericProperty<string>(typeProperty);
                else
                    return defaultType;
            }
        }

        public ItemObjectWrapper(object wrappedItem, string IDProperty, string NameProperty, string DescriptionProperty, string TypeProperty)
        {
            this.wrappedItem = wrappedItem;
            this.idProperty = IDProperty;
            this.nameProperty = NameProperty;
            this.descriptionProperty = DescriptionProperty;
            this.typeProperty = TypeProperty;
        }

        public T GetGenericProperty<T>(string propertyName)
        {
            Type t = wrappedItem.GetType();
            T result = default(T);
            try
            {
                PropertyInfo propertyInfo = t.GetProperty(propertyName, typeof(T));
                result = (T)propertyInfo.GetValue(wrappedItem);
            }
            catch
            {
                return default(T);
            }
            return result;
        }

        public bool SetGenericProperty<T>(string propertyName, T val)
        {
            Type t = wrappedItem.GetType();
            try
            {
                PropertyInfo propertyInfo = t.GetProperty(propertyName, typeof(T));
                propertyInfo.SetValue(wrappedItem, val);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
