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
            return attr != null ? attr._prefabPath ?? "" : "";
        }
        
        public EnumPlaceWidgetPrefabPathAttribute(string text) 
        {
            _prefabPath = text ?? "";
        }
    }
}