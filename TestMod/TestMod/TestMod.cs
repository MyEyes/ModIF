using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TestMod
{
    public class TestMod:ModIF.Mod
    {
        public TestMod()
        {
            SetInfo("Testmod", new Version(1, 0), null, null);
        }

        public override bool Load(ModIF.IModdingInterface moddingIf)
        {
            File.OpenRead(Path.Combine(Info.ModDirectory, "Test.txt"));
            File.OpenRead("Content/Text.xml");
            File.OpenRead("settings.lua");
            return base.Load(moddingIf);
        }
    }
}
