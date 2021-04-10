using AutoMapper;
using System;
using System.Linq;
using System.Reflection;

namespace TenderManagement.Application.Common.Mappings
{
    /// <summary>
    /// Auto build mapping profile from any class implement <see cref="IMapDef{T}"/>
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile() => ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            var types = assembly.GetExportedTypes()
                .Where(t => t.GetInterfaces().Any(i => 
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapDef<>)))
                .ToList();

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);

                var methodInfo = type.GetMethod("Mapping") 
                    ?? type.GetInterface("IMapDef`1").GetMethod("Mapping");
                
                methodInfo?.Invoke(instance, new object[] { this });

            }
        }
    }
}