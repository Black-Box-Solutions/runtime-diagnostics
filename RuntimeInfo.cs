﻿using System;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace BlackBoxSolutions.Diagnostics
{
    /// <summary>
    /// Holds information about the runtime environment, including version, description, OS details, and optimization
    /// settings.
    /// </summary>
    public class RuntimeInfo
    {
        public string Architecture { get; private set; }
        public string CoreLibInfoVersion { get; private set; }
        public string CoreLibLocation { get; private set; }
        public int GCMaxGeneration { get; private set; }
        public bool Is64BitOperatingSystem { get; private set; }
        public bool Is64BitProcess { get; private set; }
        public bool IsJITOptimized { get; private set; }
        public bool IsNetCore { get; private set; }
        public bool IsServerGC { get; private set; }
        public string OSDescription { get; private set; }
        public int ProcessorCount { get; private set; }
#if NET5_0_OR_GREATER
           public string RuntimeIdentifier { get; private set; }
#endif
        public string RuntimeDescription { get; private set; }
        public string RuntimeDirectory { get; private set; }
        public string RuntimeVersion { get; private set; }
        public bool TieredCompilationEnabled { get; private set; }

        [SuppressMessage("Performance", "HAA0102:Non-overridden virtual method call on value type", Justification = "<Pending>")]
        public static RuntimeInfo GetRuntimeInfo()
        {
            var coreLibAssembly = typeof(object).Assembly;
            var executingAssembly = Assembly.GetExecutingAssembly();

            return new RuntimeInfo
            {
                Architecture = RuntimeInformation.ProcessArchitecture.ToString(),
                CoreLibInfoVersion = coreLibAssembly.GetName().Version?.ToString() ?? "Unknown",
                CoreLibLocation = coreLibAssembly.Location,
                GCMaxGeneration = GC.MaxGeneration,
                Is64BitOperatingSystem = Environment.Is64BitOperatingSystem,
                Is64BitProcess = Environment.Is64BitProcess,
                IsJITOptimized = DiagnosticMechanics.IsOptimizedBuild(executingAssembly),
                IsNetCore = Type.GetType("System.Runtime.InteropServices.RuntimeInformation, System.Runtime.InteropServices.RuntimeInformation") != null,
                IsServerGC = GCSettings.IsServerGC,
                OSDescription = RuntimeInformation.OSDescription,
                ProcessorCount = Environment.ProcessorCount,
#if NET5_0_OR_GREATER
                   RuntimeIdentifier = RuntimeInformation.RuntimeIdentifier,  
#endif
                RuntimeDescription = RuntimeInformation.FrameworkDescription,
                RuntimeDirectory = AppContext.BaseDirectory,
                RuntimeVersion = Environment.Version.ToString(),
                TieredCompilationEnabled = DiagnosticMechanics.IsTieredCompilationEnabled()
            };
        }

        [SuppressMessage("Performance", "HAA0301:Closure Allocation Source", Justification = "<Pending>")]
        public override string ToString()
        {
            var properties = GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(p => p.Name);

            return string.Join(", ", properties.Select(p =>
            {
                var value = p.GetValue(this);
                return $"{p.Name}: {value}";
            }));
        }
    }
}