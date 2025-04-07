using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;

namespace BlackBoxSolutions.Diagnostics
{
    /// <summary>
    /// Based on https://dave-black.blogspot.com/2011/12/how-to-tell-if-assembly-is-debug-or.html, this class returns
    /// methods to determine if an assembly is JIT-optimized or not.
    /// </summary>
    public static class DiagnosticMechanics
    {
        /// <summary>
        /// Determines whether the specified assembly is a Debug build.
        /// </summary>
        /// <param name="assembly">The assembly to check.</param>
        /// <returns>True if the assembly is a Debug build; otherwise, false.</returns>
        public static bool IsDebugBuild(Assembly assembly)
        {
            var debuggableAttr = assembly.GetCustomAttribute<DebuggableAttribute>();
            if (debuggableAttr == null)
                return false; // No attribute => Optimized (Release)

            return debuggableAttr.IsJITTrackingEnabled && debuggableAttr.IsJITOptimizerDisabled;
        }

        /// <summary>
        /// Determines whether the specified assembly is an Optimized (Release) build.
        /// </summary>
        /// <param name="assembly">The assembly to check.</param>
        /// <returns>True if the assembly is optimized (i.e., a Release build); otherwise, false.</returns>
        public static bool IsOptimizedBuild(Assembly assembly)
        {
            var debuggableAttr = assembly.GetCustomAttribute<DebuggableAttribute>();
            if (debuggableAttr == null)
                return true; // No attribute => Optimized (Release)

            return !debuggableAttr.IsJITOptimizerDisabled;
        }

        /// <summary>
        /// Gets a human-readable string for the detected build configuration.
        /// </summary>
        /// <param name="assembly">The assembly to check.</param>
        /// <returns>"Debug", "Release (Optimized)", or "Unknown"</returns>
        public static string GetBuildConfiguration(Assembly assembly)
        {
            var debuggableAttr = assembly.GetCustomAttribute<DebuggableAttribute>();
            if (debuggableAttr == null)
                return "Release (Optimized)";

            if (debuggableAttr.IsJITTrackingEnabled && debuggableAttr.IsJITOptimizerDisabled)
                return "Debug";

            return "Release (Optimized)";
        }

        /// <summary>
        /// Checks if tiered compilation is enabled in the runtime environment. It evaluates the setting from the
        /// application context.
        /// </summary>
        /// <returns>Returns <c>true</c> if tiered compilation is enabled or not set; otherwise, <c>false</c>.</returns>
        public static bool IsTieredCompilationEnabled()
        {
            return !AppContext.TryGetSwitch("System.Runtime.TieredCompilation", out var enabled) || enabled;
        }
    }
}