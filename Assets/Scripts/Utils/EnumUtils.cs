using System;

namespace Solcery.Utils
{
    public static class EnumUtils
    {
        public static T GetEnumAttribute<T>(object value) where T: Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            
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

        public static bool TryGetEnumAttribute<T>(object value, out T attribute) where T: Attribute
        {
            attribute = default;
            
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            
            if (name != null)
            {
                var field = type.GetField(name);
                if (field != null)
                {
                    attribute = Attribute.GetCustomAttribute(field, typeof(T)) as T;
                    return true;
                }
            }

            return false;
        }
    }
}