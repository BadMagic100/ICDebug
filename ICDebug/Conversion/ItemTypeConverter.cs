using ItemChanger;
using ModTerminal.Commands;
using System;

namespace ICDebug.Conversion
{
    internal class ItemTypeConverter : IValueConverter
    {
        public object? Convert(string value)
        {
            // check fully-qualified names first, then cascade to IC type names
            Type? type = Type.GetType(value);
            if (type == null)
            {
                type = Type.GetType($"ItemChanger.Items.{value},ItemChanger");
            }

            if (type == null || !typeof(AbstractItem).IsAssignableFrom(type))
            {
                throw new InvalidCastException();
            }
            return type;
        }
    }
}
