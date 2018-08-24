using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;

namespace ModIF
{
    public static class ModLoader
    {
        private static Dictionary<string, AppDomain> ModAppDomains = new Dictionary<string,AppDomain>();


        private static string[] allowedReadDirs;
        private static string[] allowedWriteDirs;
        private static string modBasePath;
        private static bool Initialized = false;

        public static void SetupAppDomain(string modPath, IEnumerable<string> modReadableDirs, IEnumerable<string> modWriteableDirs)
        {
            modBasePath = Path.GetFullPath(modPath);
            allowedReadDirs = modReadableDirs.Select((p) => Path.GetFullPath(p)).ToArray();
            allowedWriteDirs = modWriteableDirs.Select((p) => Path.GetFullPath(p)).ToArray();

            Initialized = true;
        }

        private static AppDomain CreateModAppDomain(string modFile)
        {
            //TODO: Should maybe log error
            if (ModAppDomains.ContainsKey(modFile))
                return ModAppDomains[modFile];

            string modDir = Path.GetDirectoryName(modFile);
            AppDomainSetup appDomainSetup = new AppDomainSetup();
            appDomainSetup.DisallowCodeDownload = true;
            appDomainSetup.DisallowBindingRedirects = true;
            appDomainSetup.PrivateBinPath = string.Format("{0};{1}", modDir.Replace("file:\\", ""), Environment.CurrentDirectory);
            //appDomainSetup.PrivateBinPathProbe = null;
            appDomainSetup.ApplicationName = AppDomain.CurrentDomain.SetupInformation.ApplicationName;
            appDomainSetup.ApplicationBase = Environment.CurrentDirectory;
            appDomainSetup.DynamicBase = Environment.CurrentDirectory;

            PermissionSet permissions = new PermissionSet(null);
            //Make sure mods only read from allowed directories
            if (allowedReadDirs.Length > 0)
            {
                FileIOPermission readPermission = new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery, allowedReadDirs);
                permissions.AddPermission(readPermission);
            }
            //Allow mod to read from its own directory and game base directory
            modDir = modDir.Replace("file:\\", "");
            FileIOPermission readOwnDirectory = new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery, new string[] { Path.GetFullPath(modDir), Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory) });
            permissions.AddPermission(readOwnDirectory);

            //Make sure mods only write to allowed directories
            if (allowedWriteDirs.Length > 0)
            {
                FileIOPermission writePermission = new FileIOPermission(FileIOPermissionAccess.Write | FileIOPermissionAccess.Append, allowedWriteDirs);
                permissions.AddPermission(writePermission);
            }

            //Allow mod to run code
            SecurityPermission sp = new SecurityPermission(SecurityPermissionFlag.Execution | SecurityPermissionFlag.Infrastructure | SecurityPermissionFlag.SerializationFormatter | SecurityPermissionFlag.AllFlags);
            permissions.SetPermission(sp);

            //Restrict mods reflection capabilities
            ReflectionPermission rp = new ReflectionPermission(ReflectionPermissionFlag.NoFlags);
            permissions.SetPermission(rp);

            return AppDomain.CreateDomain(string.Format("{0}AppDomain", Path.GetFileNameWithoutExtension(modFile)), AppDomain.CurrentDomain.Evidence, appDomainSetup, permissions, typeof(Mod).Assembly.Evidence.GetHostEvidence<StrongName>());
        }

        public static List<ModInfo> ScanForMods(string directory, bool recursive)
        {
            if (!Initialized)
                throw new InvalidOperationException("Call SetupAppDomain before doing anything with the ModLoader");

            List<ModInfo> mods = new List<ModInfo>();
            AddModsInFolderToList(directory, mods, recursive);
            return mods;
        }

        private static void AddModsInFolderToList(string directory, List<ModInfo> list, bool recursive)
        {
            string[] files = Directory.GetFiles(directory);
            foreach(string file in files)
            {
                Assembly currentAssembly = GetModAssembly(file);
                if (currentAssembly != null)
                    list.AddRange(GetAssemblyModInfos(currentAssembly));
            }
            if (recursive)
                foreach (string subDirectory in Directory.GetDirectories(directory))
                    AddModsInFolderToList(subDirectory, list, recursive);
        }

        private static Assembly GetModAssembly(string filePath)
        {
            //Only attempt to load dll files
            //TODO: Might cause issues on MacOS
            if (Path.GetExtension(filePath) != ".dll")
                return null;

            try
            {
                //Should automatically put the assembly into our new ModAppDomain
                Assembly assembly = Assembly.LoadFrom(filePath);
                //Return the assembly if it contains a mod, otherwise return null
                if (assembly.ExportedTypes.Any((type) => type.IsSubclassOf(typeof(Mod))))
                    return assembly;
                else
                    return null;
            }
            catch
            {
                //If we already created the domain, clean up
                if(ModAppDomains.ContainsKey(filePath))
                    ModAppDomains.Remove(filePath);
                //If any exception occurs we return null
                return null;
            }
        }

        private static IEnumerable<ModInfo> GetAssemblyModInfos(Assembly assembly)
        {
            //Grab all subtypes of Mod that the assembly exports
            var modTypes = assembly.ExportedTypes.Where((type) => type.IsSubclassOf(typeof(Mod)));
            AppDomain domain = CreateModAppDomain(assembly.CodeBase);
            foreach (Type modType in modTypes)
            {
                Mod tempMod = null;
                try
                {
                    tempMod = domain.CreateInstanceFromAndUnwrap(assembly.CodeBase, modType.FullName) as Mod;
                }
                catch
                {
                    //Do nothing, we skip
                }
                //If we managed to construct a Mod object yield return its info
                if (tempMod != null)
                {
                    if (!ModAppDomains.ContainsKey(modType.Assembly.Location))
                        ModAppDomains.Add(modType.Assembly.Location, domain);
                    yield return tempMod.Info;
                }
            }
            yield break;
        }

        public static Mod LoadMod(string className, string assemblyFile)
        {
            try
            {
                Mod tempMod = ModAppDomains[assemblyFile].CreateInstanceFromAndUnwrap(assemblyFile, className) as Mod;
                //If we managed to construct a Mod object return it
                return tempMod;
            }
            catch
            {
                return null;
            }
        }
    }
}
