using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Flux.SDK
{
    internal sealed class DeserializationBinder : System.Runtime.Serialization.SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            //Removing version and PublicKeyTocken form type name. Reqired for compatibility with SDK 1.*
            var type = Regex.Replace(typeName, @"(, Version=[\d\.]+)", string.Empty);
            type = Regex.Replace(type, @"(, PublicKeyToken=[a-z0-9]+)", string.Empty);

            //trying to find type in standard way
            Type res = Type.GetType(typeName);
            if (res != null)
                return res;

            //trying to find type in specific assembly
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName == assemblyName);
            if (assembly == null)
            {
                //trying to get version independent assembly
                var targetName = new AssemblyName(assemblyName);
                assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name == targetName.Name);
            }

            if (assembly != null)
                res = assembly.GetType(typeName);

            if (res == null)
            {
                //trying to get type from current assembly
                String currentAssembly = Assembly.GetExecutingAssembly().FullName;
                return Type.GetType(String.Format("{0}, {1}", typeName, currentAssembly));
            }
            else
                return res;
        }
    }
}
