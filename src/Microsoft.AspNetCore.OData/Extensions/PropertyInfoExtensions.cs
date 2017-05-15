using System.Reflection;
using Microsoft.AspNetCore.OData.Builder;

namespace Microsoft.AspNetCore.OData.Extensions
{
    public static class PropertyInfoExtensions
    {
        internal static bool IsIgnored(this PropertyInfo property,
            params IEdmTypeConfiguration[] configurations)
        {
            var config = property.GetConfiguration(configurations);
            if (config != null)
            {
                return config.IsIgnored;
            }
            return true;
        }

        internal static PropertyConfiguration GetConfiguration(this PropertyInfo property, params IEdmTypeConfiguration[] configurations)
        {
            foreach (var config in configurations)
            {
                if (config.ClrType == property.DeclaringType)
                {
                    var structuralTypeConfiguration = config as StructuralTypeConfiguration;
                    if (structuralTypeConfiguration != null && structuralTypeConfiguration.ExplicitProperties.ContainsKey(property))
                    {
                        return structuralTypeConfiguration.ExplicitProperties[property];
                    }
                    return null;
                }
            }
            return null;
        }
    }
}