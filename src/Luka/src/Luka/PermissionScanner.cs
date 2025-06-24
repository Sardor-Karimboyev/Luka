using System.Reflection;
using Luka.Attributes;

namespace Luka;

public static class PermissionScanner
{
    public static IEnumerable<Type> FindPermissionEnums()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsEnum && t.GetCustomAttribute<PermissionAttribute>() != null);
    }

    public static IEnumerable<string> GetPermissionsFromEnum(Type enumType)
    {
        return enumType.GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(f => f.Name); 
    }
}