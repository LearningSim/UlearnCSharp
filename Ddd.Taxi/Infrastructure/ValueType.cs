using System;
using System.Linq;
using System.Reflection;

namespace Ddd.Infrastructure
{
    /// <summary>
    /// Базовый класс для всех Value типов.
    /// </summary>
    public class ValueType<T>
    {
        private readonly PropertyInfo[] properties;
        private readonly Type thisType;
        protected ValueType()
        {
            thisType = this.GetType();
            properties = thisType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is T other))
            {
                return false;
            }

            return ReferenceEquals(this, other) || Equals(other);
        }

        public bool Equals(T other)
        {
            foreach (var p in properties)
            {
                var thisVal = GetPropValue(this, p.Name);
                var otherVal = GetPropValue(other, p.Name);
                if (!Equals(thisVal, otherVal)) return false;
            }

            return true;
        }

        public override string ToString()
        {
            var props = properties.OrderBy(p => p.Name);
            var propsStrings = props
                .Select(p => $"{p.Name}: {GetPropValue(this, p.Name)}");
            return $"{typeof(T).Name}({string.Join("; ", propsStrings)})";
        }

        public override int GetHashCode()
        {
            var vals = properties.Select(p => GetPropValue(this, p.Name));
            var result = 0;
            foreach (var val in vals)
            {
                var hash = val?.GetHashCode() ?? 0;
                unchecked
                {
                    result = (result << 5) + 3 + result ^ hash;
                }
            }

            return result;
        }

        private object GetPropValue(object src, string propName)
        {
            return src != null ? thisType.GetProperty(propName)?.GetValue(src, null) : null;
        }
    }
}