using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Egate_Payroll
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class CloneCopyIgnoreAttribute : Attribute
    {
    }

    public static class CloneCopyExt
    {
        private static Dictionary<Type, IEnumerable<PropertyInfo>> _propertiesCache = new Dictionary<Type, IEnumerable<PropertyInfo>>();

        public static T DeepClone<T>(this T obj)
        {
            return (T)CloneCopyExt.DeepClone((object)obj);
        }

        public static void DeepCopyTo<T>(this T source, T destination) where T : class
        {
            CloneCopyExt.DeepCopyTo((object)source, (object)destination);
        }

        private static object DeepClone(object obj)
        {
            if (obj == null) return null;
            Type type = obj.GetType();
            if (!type.IsClass || Nullable.GetUnderlyingType(type) != null)
            {
                //is struct or nullable
                return obj;
            }
            else if (type.Equals(typeof(string)))
            {
                //is string
                return string.Copy(Convert.ToString(obj));
            }
            else if (typeof(IList).IsAssignableFrom(type) && type.IsGenericType)
            {
                //is list
                Type genericType = type.GetGenericArguments()[0];
                IList lst = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(genericType));
                foreach (var i in (IEnumerable)obj)
                    lst.Add(CloneCopyExt.DeepClone(i));
                return lst;
            }
            else
            {
                //other type
                //get public instance properties, with public setter, not indexed, not marked with ignore attribute
                if (!_propertiesCache.ContainsKey(type))
                {
                    //save properties in cache
                    var cacheProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty)
                        .Where(p => p.CanWrite && p.GetSetMethod(true).IsPublic && p.GetIndexParameters().Length == 0 && p.GetCustomAttributes<CloneCopyIgnoreAttribute>(false).Count() == 0);
                    cacheProperties.Count();
                    _propertiesCache.Add(type, cacheProperties);
                }
                var properties = _propertiesCache[type];
                //begin clone process
                object clone = Activator.CreateInstance(type);
                foreach (var p in properties)
                {
                    //do recursive cloning
                    object value = CloneCopyExt.DeepClone(p.GetValue(obj, null));
                    p.SetValue(clone, value);
                }
                return clone;
            }
        }

        private static void DeepCopyTo(object source, object destination)
        {
            //check if both types are same
            if (!source.GetType().Equals(destination.GetType()))
                throw new ArgumentException("(CloneCopyExt) Source type and Destination type must be same.");
            Type type = source.GetType();
            //get public instance properties, with public setter, and not indexed
            if (!_propertiesCache.ContainsKey(type))
            {
                //save properties in cache
                var cacheProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty)
                    .Where(p => p.CanWrite && p.GetSetMethod(true).IsPublic && p.GetIndexParameters().Length == 0);
                cacheProperties.Count();
                _propertiesCache.Add(type, cacheProperties);
            }
            var properties = _propertiesCache[type];
            //begin copy process
            foreach (var p in properties)
            {
                object value = p.GetValue(source, null);
                p.SetValue(destination, value, null);
            }
        }
    }
}
