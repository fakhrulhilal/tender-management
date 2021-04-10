using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TenderManagement.Application.IntegrationTests
{
    internal static class ClassComparer
    {
        internal static IComparer ByPublicProperty => new CompareObjectPublicProperty();

        internal static IComparer ByPublicPropertyExcept(params string[] ignored) =>
            new CompareObjectPublicProperty(ignored);

        public static IComparer ByPublicPropertyWithEnumAsString(params string[] ignoredProperties) =>
            new CompareObjectPublicProperty(true, ignoredProperties);
        internal class CompareObjectPublicProperty : IComparer
        {
            private readonly string[] _ignoredProperties;
            private readonly bool _useEnumAsString;

            public CompareObjectPublicProperty(params string[] ignore)
            {
                _ignoredProperties = ignore ?? new[] { string.Empty };
            }

            public CompareObjectPublicProperty(bool useEnumAsString, params string[] ignore)
            {
                _useEnumAsString = useEnumAsString;
                _ignoredProperties = ignore;
            }

            /// <inheritdoc />
            public int Compare(object source, object target)
            {
                if (source == null || target == null) return source == target ? 0 : 1;

                var properties =
                    source.GetType()
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance);
            
                var targetType = target.GetType();
                if (!properties.Any()) return source == target ? 0 : 1;

                var ignoreList = new List<string>(_ignoredProperties);

                foreach (var prop in properties)
                {
                    if (ignoreList.Contains(prop.Name)) continue;

                    var sourceValue = prop.GetValue(source, null);
                    var targetValue = targetType.GetProperty(prop.Name)?.GetValue(target, null);
                    if (prop.PropertyType.IsEnum && _useEnumAsString)
                    {
                        sourceValue = sourceValue.ToString();
                        targetValue = targetValue?.ToString();
                    }
                    if (sourceValue != targetValue && (sourceValue == null || !sourceValue.Equals(targetValue)))
                        return 1;
                }
                return 0;
            }
        }
    }
}
