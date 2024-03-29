using System;
using Solcery.Utils;

namespace Solcery.Widgets_new.Attributes.Enum
{
    public sealed class EnumPlaceWidgetPrefabPathAttribute : Attribute
    {
        private readonly string _prefabPath;

        public static string GetPrefabPath(object value)
        {
            var attr = EnumUtils.GetEnumAttribute<EnumPlaceWidgetPrefabPathAttribute>(value);
            return attr != null ? attr._prefabPath ?? string.Empty : string.Empty;
        }

        public static bool TryGetPrefabPath(object value, out string prefabPath)
        {
            prefabPath = string.Empty;
            
            if (EnumUtils.TryGetEnumAttribute(value, out EnumPlaceWidgetPrefabPathAttribute attribute))
            {
                prefabPath = attribute._prefabPath;
                return true;
            }

            return false;
        }
        
        public EnumPlaceWidgetPrefabPathAttribute(string text) 
        {
            _prefabPath = text ?? string.Empty;
        }
    }
}