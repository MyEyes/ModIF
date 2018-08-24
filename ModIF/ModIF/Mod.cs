using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModIF
{
    public class Mod: MarshalByRefObject
    {
        public ModInfo Info { get; private set; }
        /// <summary>
        /// Constructor for the mod, this should set up the Info property
        /// </summary>
        public Mod()
        {

        }

        protected void SetInfo(string Name, Version ModVersion, Version minGameVersion, Version maxGameVersion)
        {
            Info = new ModInfo(Name, ModVersion, minGameVersion, maxGameVersion);
        }

        public virtual bool Load(IModdingInterface moddingIf)
        {
            return false;
        }

        public virtual bool Activate(IModdingInterface moddinfIf)
        {
            return false;
        }

        public virtual bool Deactivate(IModdingInterface moddingIf)
        {
            return false;
        }
    }
}
