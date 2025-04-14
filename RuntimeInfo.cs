using System;
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
        /// <summary>
        /// Gets the architecture of the process (e.g., x64, x86, Arm).
        /// </summary>
        public string Architecture { get; private set; }

        /// <summary>
        /// Gets the version of the core library (mscorlib or System.Private.CoreLib).
        /// </summary>
        public string CoreLibInfoVersion { get; private set; }

        /// <summary>
        /// Gets the file location of the core library (mscorlib or System.Private.CoreLib).
        /// </summary>
        public string CoreLibLocation { get; private set; }

        /// <summary>
        /// Gets the maximum generation supported by the garbage collector.
        /// </summary>
        public int GCMaxGeneration { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the operating system is 64-bit.
        /// </summary>
        public bool Is64BitOperatingSystem { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the process is running in 64-bit mode.
        /// </summary>
        public bool Is64BitProcess { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the JIT compiler is optimizing the code.
        /// </summary>
        public bool IsJITOptimized { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the runtime is .NET Core or later.
        /// </summary>
        public bool IsNetCore { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the garbage collector is using server mode.
        /// </summary>
        public bool IsServerGC { get; private set; }

        /// <summary>
        /// Gets the description of the operating system.
        /// </summary>
        public string OSDescription { get; private set; }

        /// <summary>
        /// Gets the number of processors available on the machine.
        /// </summary>
        public int ProcessorCount { get; private set; }

#if NET5_0_OR_GREATER
        /// <summary>
        /// Gets the runtime identifier (RID) of the current platform.
        /// </summary>
        public string RuntimeIdentifier { get; private set; }
#endif

        /// <summary>
        /// Gets the description of the runtime environment (e.g., .NET Core, .NET Framework).
        /// </summary>
        public string RuntimeDescription { get; private set; }

        /// <summary>
        /// Gets the base directory of the runtime environment.
        /// </summary>
        public string RuntimeDirectory { get; private set; }

        /// <summary>
        /// Gets the version of the runtime environment.
        /// </summary>
        public string RuntimeVersion { get; private set; }

        /// <summary>
        /// Gets a value indicating whether tiered compilation is enabled.
        /// </summary>
        public bool TieredCompilationEnabled { get; private set; }

        /// <summary>
        /// Retrieves runtime information about the current environment.
        /// </summary>
        /// <returns>A <see cref="RuntimeInfo"/> object containing runtime details.</returns>
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

        /// <summary>
        /// Returns a string representation of the runtime information.
        /// </summary>
        /// <returns>A string containing the runtime information properties and their values.</returns>
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
