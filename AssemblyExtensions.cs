using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReMod.Core
{
    internal static class AssemblyExtensions
    {
        public static IEnumerable<Type> TryGetTypes(this Assembly asm)
        {
            try
            {
                return asm.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                try
                {
                    return asm.GetExportedTypes();
                }
                catch
                {
                    return e.Types.Where(t => t != null);
                }
            }
            catch
            {
                return Enumerable.Empty<Type>();
            }
        }
    }
}
