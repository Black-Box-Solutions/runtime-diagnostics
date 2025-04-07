using System;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;

namespace BlackBoxSolutions.Diagnostics
{
    /// <summary>
    /// Holds information about the runtime environment, including version, description, OS details, and optimization
    /// settings.
    /// </summary>
    public class RuntimeInfo
    {
        public string RuntimeVersion { get; private set; }
        public string RuntimeDescription { get; private set; }
#if NET5_0_OR_GREATER
    public string RuntimeIdentifier { get; private set; }
#endif
        public string OSDescription { get; private set; }
        public string Architecture { get; private set; }
        public bool IsJITOptimized { get; private set; }
        public string GCMode { get; private set; }
        public bool TieredCompilationEnabled { get; private set; }
        public string CoreLibInfoVersion { get; private set; }
        public string CoreLibLocation { get; private set; }

        public static RuntimeInfo GetRuntimeInfo()
        {
            var coreLibAssembly = typeof(object).Assembly;
            var executingAssembly = Assembly.GetExecutingAssembly();

            return new RuntimeInfo
            {
                RuntimeVersion = Environment.Version.ToString(),
                RuntimeDescription = RuntimeInformation.FrameworkDescription,
#if NET5_0_OR_GREATER
            RuntimeIdentifier = RuntimeInformation.RuntimeIdentifier,
#endif
                OSDescription = RuntimeInformation.OSDescription,
                Architecture = RuntimeInformation.ProcessArchitecture.ToString(),
                IsJITOptimized = DiagnosticMechanics.IsOptimizedBuild(executingAssembly),
                GCMode = GCSettings.IsServerGC ? "Server" : "Workstation",
                TieredCompilationEnabled = DiagnosticMechanics.IsTieredCompilationEnabled(),
                CoreLibInfoVersion = coreLibAssembly.GetName().Version?.ToString() ?? "Unknown",
                CoreLibLocation = coreLibAssembly.Location
            };
        }

        public override string ToString()
        {
#if NET5_0_OR_GREATER
        return $"{nameof(RuntimeVersion)}: {RuntimeVersion}, {nameof(RuntimeDescription)}: {RuntimeDescription}, {nameof(RuntimeIdentifier)}: {RuntimeIdentifier}, {nameof(OSDescription)}: {OSDescription}, {nameof(Architecture)}: {Architecture}, {nameof(IsJITOptimized)}: {IsJITOptimized}, {nameof(TieredCompilationEnabled)}: {TieredCompilationEnabled}, {nameof(GCMode)}: {GCMode}, {nameof(CoreLibInfoVersion)}: {CoreLibInfoVersion}, {nameof(CoreLibLocation)}: {CoreLibLocation}";
#else
            return $"{nameof(RuntimeVersion)}: {RuntimeVersion}, {nameof(RuntimeDescription)}: {RuntimeDescription}, {nameof(OSDescription)}: {OSDescription}, {nameof(Architecture)}: {Architecture}, {nameof(IsJITOptimized)}: {IsJITOptimized}, {nameof(TieredCompilationEnabled)}: {TieredCompilationEnabled}, {nameof(GCMode)}: {GCMode}, {nameof(CoreLibInfoVersion)}: {CoreLibInfoVersion}, {nameof(CoreLibLocation)}: {CoreLibLocation}";
#endif
        }
    }
}