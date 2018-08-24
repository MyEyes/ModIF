using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace ModIF
{
    [Serializable]
    public struct ModInfo : ISerializable
    {
        public readonly string Name;
        public readonly Version ModVersion;
        public readonly Version MinGameVersion;
        public readonly Version MaxGameVersion;

        //Automatically detected properties
        public readonly string ClassName;
        public readonly string AssemblyFile;
        public readonly string ModDirectory;

        public ModInfo(string name, Version version, Version minGameVersion, Version maxGameVersion)
        {
            //Grab calling routine from stackframe to detect class that initialized
            Type callingType = new StackFrame(2).GetMethod().DeclaringType;
            ClassName = callingType.FullName;
            AssemblyFile = callingType.Assembly.Location;
            ModDirectory = Path.GetDirectoryName(callingType.Assembly.Location);

            Name = name;
            ModVersion = version;
            MinGameVersion = minGameVersion;
            MaxGameVersion = maxGameVersion;
        }

        [SecurityCritical]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("ModVersion", ModVersion.ToString());
            if (MinGameVersion != null)
                info.AddValue("MinGameVersion", MinGameVersion.ToString());
            else
                info.AddValue("MinGameVersion", "null");
            if (MaxGameVersion != null)
                info.AddValue("MaxGameVersion", MaxGameVersion.ToString());
            else
                info.AddValue("MaxGameVersion", "null");
            info.AddValue("ClassName", ClassName);
            info.AddValue("AssemblyFile", AssemblyFile);
            info.AddValue("ModDirectory", ModDirectory);
        }

        public ModInfo(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
            ModVersion = new Version(info.GetString("ModVersion"));
            string minVS = info.GetString("MinGameVersion");
            if (minVS == "null")
                MinGameVersion = null;
            else
                MinGameVersion = new Version(minVS);
            string maxVS = info.GetString("MaxGameVersion");
            if(maxVS == "null")
                MaxGameVersion = null;
            else
                MaxGameVersion = new Version(maxVS);
            ClassName = info.GetString("ClassName");
            AssemblyFile = info.GetString("AssemblyFile");
            ModDirectory = info.GetString("ModDirectory");
        }
    }
}
