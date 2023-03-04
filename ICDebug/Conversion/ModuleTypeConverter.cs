using ItemChanger.Modules;
using ModTerminal.Commands;
using System;

namespace ICDebug.Conversion
{
    internal class ModuleTypeConverter : IValueConverter
    {
        public object? Convert(string value)
        {
            // check fully-qualified names first, then cascade to IC type names
            Type? type = Type.GetType(value);
            if (type == null)
            {
                type = Type.GetType($"ItemChanger.Modules.{value},ItemChanger");
            }

            if (type == null || !typeof(Module).IsAssignableFrom(type))
            {
                throw new InvalidCastException();
            }
            return type;
        }
    }
}
