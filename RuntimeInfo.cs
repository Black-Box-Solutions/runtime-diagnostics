using System;
using System.IO;
using System.Linq;
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

        //TODO change this object into a JSON object for easier display and serialization.

        #region Machine-specific properties

        /// <summary>
        /// Gets the description of the operating system.
        /// </summary>
        public string OSDescription { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the operating system architecture of the current environment.
        /// </summary>
        public string OSArchitecture { get; private set; } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether the operating system is 64-bit.
        /// </summary>
        public bool Is64BitOperatingSystem { get; private set; }

        /// <summary>
        /// Gets the number of processors available on the machine.
        /// </summary>
        public int ProcessorCount { get; private set; }

        #endregion Machine-specific properties

        #region .NET Runtime-specific properties

        /// <summary>
        /// Gets the version of the core library (mscorlib or System.Private.CoreLib).
        /// </summary>
        public string CoreLibInfoVersion { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the file location of the core library (mscorlib or System.Private.CoreLib).
        /// </summary>
        public string CoreLibLocation { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the maximum generation supported by the garbage collector.
        /// </summary>
        public int GCMaxGeneration { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the garbage collector is using server mode.
        /// </summary>
        public bool IsServerGC { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the runtime is .NET Core or later.
        /// </summary>
        public bool IsNetCore { get; private set; }

        #endregion .NET Runtime-specific properties

        #region .NET Process-specific properties

        /// <summary>
        /// Gets the architecture of the process (e.g., x64, x86, Arm).
        /// </summary>
        public string ProcessArchitecture { get; private set; } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether the process is running in 64-bit mode.
        /// </summary>
        public bool Is64BitProcess { get; private set; }

        /// <summary>
        /// Gets a human-readable string for the detected build configuration.
        ///     - "Debug" for debug builds
        ///     - "Release (Optimized)" for release builds that are JIT-optimized
        /// </summary>
        public string BuildConfiguration { get; private set; } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether the JIT compiler is optimizing the code.
        /// </summary>
        public bool IsJITOptimized { get; private set; }

#if NET6_0_OR_GREATER
        /// <summary>
        /// Gets or sets the full path to the executable file of the current process.
        /// </summary>
        /// <remarks>On .NET 6.0 or later, this property provides the path to the currently executing
        /// process. If modified, ensure the new value represents a valid file path.</remarks>
        public string ProcessPath { get; private set; } = Environment.ProcessPath;
#endif

#if NET5_0_OR_GREATER
        /// <summary>
        /// Gets or sets the process ID of the current application.
        /// </summary>
        /// <remarks>This property is available only on .NET 5.0 or later.</remarks>
        public int ProcessId { get; private set; } = Environment.ProcessId;

        /// <summary>
        /// Gets the runtime identifier of the current application.
        /// </summary>
        /// <remarks>The runtime identifier is a platform-specific string that identifies the operating
        /// system and architecture  the application is running on. This property is available only in .NET 5.0 or
        /// later.</remarks>
        public string RuntimeIdentifier { get; private set; } = string.Empty;
#endif

        /// <summary>
        /// Gets the description of the runtime environment (e.g., .NET Core, .NET Framework).
        /// </summary>
        public string RuntimeDescription { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the base directory of the runtime environment.
        /// </summary>
        public string RuntimeDirectory { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the version of the runtime environment.
        /// </summary>
        public string RuntimeVersion { get; private set; } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether tiered compilation is enabled.
        /// </summary>
        public bool TieredCompilationEnabled { get; private set; }

        #endregion .NET Process-specific properties

        /// <summary>
        /// Retrieves runtime information about the current environment.
        /// </summary>
        /// <returns>A <see cref="RuntimeInfo"/> object containing runtime details.</returns>
        public static RuntimeInfo GetRuntimeInfo()
        {
            var coreLibAssembly = typeof(object).Assembly;
            var executingAssembly = Assembly.GetExecutingAssembly();

            const string Unknown = "Unknown";
            return new RuntimeInfo
            {
                CoreLibInfoVersion = coreLibAssembly.GetName().Version?.ToString() ?? Unknown,
                CoreLibLocation = string.IsNullOrEmpty(coreLibAssembly.Location)
                    ? "CoreLib location is not available when publishing as a single file - see IL3000"
                    : coreLibAssembly.Location,
                GCMaxGeneration = GC.MaxGeneration,
                Is64BitOperatingSystem = Environment.Is64BitOperatingSystem,
                Is64BitProcess = Environment.Is64BitProcess,
                BuildConfiguration = DiagnosticMechanics.GetBuildConfiguration(executingAssembly),
                IsJITOptimized = DiagnosticMechanics.IsOptimizedBuild(executingAssembly),
                IsNetCore = Type.GetType("System.Runtime.InteropServices.RuntimeInformation, System.Runtime.InteropServices.RuntimeInformation") != null,
                IsServerGC = GCSettings.IsServerGC,
                OSArchitecture = Enum.GetName(typeof(Architecture), RuntimeInformation.OSArchitecture) ?? Unknown,
                OSDescription = RuntimeInformation.OSDescription,
                ProcessArchitecture = Enum.GetName(typeof(Architecture), RuntimeInformation.ProcessArchitecture) ?? Unknown,
                ProcessorCount = Environment.ProcessorCount,
#if NET5_0_OR_GREATER
                ProcessId = Environment.ProcessId,
                RuntimeIdentifier = RuntimeInformation.RuntimeIdentifier,
#endif
#if NET6_0_OR_GREATER
                ProcessPath = Environment.ProcessPath ?? Unknown,
#endif
                RuntimeDescription = RuntimeInformation.FrameworkDescription,
                RuntimeDirectory = AppContext.BaseDirectory,
                RuntimeVersion = Environment.Version.ToString(),
                TieredCompilationEnabled = DiagnosticMechanics.IsTieredCompilationEnabled(),
            };
        }

        /// <summary>
        /// Writes detailed information about the current runtime and environment to the specified <see cref="TextWriter"/>.
        /// </summary>
        /// <remarks>The output includes information such as the runtime version, operating system, and other
        /// relevant environment details. This method is useful for diagnostics, logging, or displaying system information
        /// in applications.</remarks>
        /// <param name="output">The <see cref="TextWriter"/> to which the runtime and environment information will be written. Cannot be
        /// <c>null</c>.</param>
        public static void DisplayRuntimeInformation(TextWriter output)
        {
            output.WriteLine();
            output.WriteLine("==================== Runtime Information ====================");
            output.WriteLine("");
            var runtimeInfoOutput = GetRuntimeInfo().ToString();
            foreach (var prop in runtimeInfoOutput.Split(','))
            {
                output.WriteLine($"\t{prop.Trim()}");
            }
            output.WriteLine("==========================================================");
            output.WriteLine();
        }

        /// <summary>
        /// Returns a string representation of the runtime information.
        /// </summary>
        /// <returns>A string containing the runtime information properties and their values.</returns>
        public override string ToString()
        {
            var properties = GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //.OrderBy(p => p.Name);

            return string.Join(", ", properties.Select(p =>
            {
                var value = p.GetValue(this);
                return $"{p.Name}: {value}";
            }));
        }
    }
}
