﻿using ICDebug.Conversion;
using ItemChanger;
using ItemChanger.Modules;
using ModTerminal.Commands;
using System;
using System.Linq;

namespace ICDebug.Commands
{
    internal static class LoadModule
    {
        public static string LoadModuleCommand(
            [HelpDocumentation("The type of module to load.")]
            [ParameterConverter<ModuleTypeConverter>]
            Type moduleType,
            [HelpDocumentation("Whether multiple instances of this module type can exist.")]
            bool allowMultiple = false
        )
        {
            Module? existingModule = ItemChangerMod.Modules.Modules
                .Where(m => moduleType.IsAssignableFrom(m.GetType()))
                .FirstOrDefault();

            if (existingModule != null && !allowMultiple)
            {
                return $"A module of type {moduleType.Name} has already been loaded.";
            }
            else
            {
                try
                {
                    ItemChangerMod.Modules.Add(moduleType);
                }
                catch
                {
                    return $"Unable to create/load module of type {moduleType.Name}.";
                }
            }
            return $"Successfully created and loaded a new module of type {moduleType.Name}.";
        }
    }
}
