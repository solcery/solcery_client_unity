using System;

namespace Solcery.Utils
{
    public static class EnumUtils
    {
        public static T GetEnumAttribute<T>(object value) where T: Attribute
        {
            var type = value.GetType();
            var name = System.Enum.GetName(type, value);
            
            if (name == null)
            {
                return null;
            }

            var field = type.GetField(name);
            if (field != null)
            {
                return Attribute.GetCustomAttribute(field, typeof(T)) as T;
            }

            return null;
        }
    }
}