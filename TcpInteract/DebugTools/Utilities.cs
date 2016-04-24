using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TcpInteract.DebugTools
{
    /// <summary>
    /// Provides debugging and productivity tools for the <see cref="TcpInteract"/> library.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Checks for conflicting commands in the project. Looks through the possible values of
        /// enumeration types with "command" or "cmd" in the name, as well as the fields in static classes
        /// named similarly.
        /// </summary>
        public static IEnumerable<ReflectedCommand> GetConflictingCommands()
        {
            List<ReflectedCommand> commands = new List<ReflectedCommand>();
            Regex regex = new Regex(@"(cmd)|(command)", RegexOptions.IgnoreCase);

            var assemblies = new[]
            {
                Assembly.GetEntryAssembly(),
                Assembly.GetExecutingAssembly()
            };

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!regex.IsMatch(type.Name)) continue;

                    if (type.IsEnum)
                    {
                        var underType = Enum.GetUnderlyingType(type);

                        foreach (var e in Enum.GetValues(type))
                        {
                            object casted;

                            if (underType == typeof(int))
                                casted = (int)e;
                            else if (underType == typeof(short))
                                casted = (int)(short)e;
                            else if (underType == typeof(uint))
                                casted = (int)(uint)e;
                            else if (underType == typeof(ushort))
                                casted = (int)(ushort)e;
                            else break;

                            var newObj = Convert.ChangeType(e, underType);
                            commands.Add(new ReflectedCommand(Enum.GetName(type, newObj), casted, true));
                        }
                    }
                    else if (type.IsAbstract && type.IsSealed)
                    {
                        // Test for static class fields.

                        foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
                        {
                            if (field.FieldType == typeof(int) || field.FieldType == typeof(uint))
                                commands.Add(new ReflectedCommand(field.Name, field.GetValue(null), false));
                        }
                    }
                }
            }

            return commands.Where(item => commands.Count(i => i == item) >= 2);
        }
    }
}
