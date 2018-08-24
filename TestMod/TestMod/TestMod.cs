using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ModIF;

namespace TestMod
{
    public class TestMod:Mod
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
            IItemInterface itemInterface = moddingIf.GetItemInterface();
            IItemDesc itemDesc = itemInterface.CreateNewItem();
            string[] itemProps = itemDesc.GetProperties();
            for (int i = 0; i < itemProps.Length; i++)
                Console.WriteLine(itemProps[i]);
            return base.Load(moddingIf);
        }
    }
}
