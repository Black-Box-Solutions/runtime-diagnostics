using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace BlackBoxSolutions.Diagnostics
{
    /// <summary>
    /// Provides diagnostic utilities for displaying .NET environment variables and configuration settings.
    /// </summary>
    /// <remarks>This class contains methods to output categorized .NET environment variables and related
    /// configuration details to a specified <see cref="TextWriter"/>. It is designed to assist in diagnosing runtime
    /// configurations, including core runtime settings, ASP.NET Core configurations, networking options,
    /// platform-specific variables, and legacy compatibility settings. <para> The output is organized into sections for
    /// easier readability, with each section focusing on a specific category of environment variables. Sensitive
    /// information, such as passwords, is masked where applicable. </para> <para> This class is intended for diagnostic
    /// and troubleshooting purposes and should not be used in production environments where exposing environment
    /// variables may pose a security risk. </para></remarks>
    public static partial class DiagnosticMechanics
    {
        /// <summary>
        /// Displays categorized .NET environment variables and their values to the specified output stream.
        /// </summary>
        /// <remarks>
        /// The output is organized into sections for readability. Sensitive information such as certificate passwords
        /// is masked. This method is intended for diagnostic and troubleshooting scenarios and should not be used
        /// in environments where exposing environment variables is a security concern.
        /// </remarks>
        /// <param name="output">The <see cref="TextWriter"/> to which the environment variable information is written. This parameter cannot be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="output"/> is <c>null</c>.</exception>
        public static void DisplayDotNetEnvironmentVariables(TextWriter output)
        {
            if (output == null) throw new ArgumentNullException(nameof(output));

            output.WriteLine("==================== .NET Environment Variables ====================");

            // Host Path Information Section
            DisplayHostPathInformation(output);

            // Core Runtime Environment Variables
            output.WriteLine("\tCORE RUNTIME ENVIRONMENT VARIABLES");
            output.WriteLine(new string('-', 70));

            DisplayCoreRuntimeVariables(output);
            DisplayGarbageCollectionVariables(output);
            DisplayThreadingPerformanceVariables(output);

            // ASP.NET Core Environment Variables
            output.WriteLine();
            output.WriteLine("\tASP.NET CORE ENVIRONMENT VARIABLES");
            output.WriteLine(new string('-', 70));

            DisplayAspNetCoreVariables(output);
            DisplayAspNetSecurityVariables(output);
            DisplayAspNetDataProtectionVariables(output);

            // Networking & HTTP Configuration
            output.WriteLine();
            output.WriteLine("\tNETWORKING & HTTP CONFIGURATION");
            output.WriteLine(new string('-', 70));

            DisplayNetworkingVariables(output);

            // Platform-Specific Variables
            output.WriteLine();
            output.WriteLine("\tPLATFORM-SPECIFIC VARIABLES");
            output.WriteLine(new string('-', 70));

            DisplayPlatformSpecificVariables(output);

            // Container & Cloud Environment Variables
            output.WriteLine();
            output.WriteLine("\tCONTAINER & CLOUD ENVIRONMENT VARIABLES");
            output.WriteLine(new string('-', 70));

            DisplayContainerVariables(output);

            // Customer-Relevant SDK Variables
            output.WriteLine();
            output.WriteLine("\tCUSTOMER-RELEVANT SDK VARIABLES");
            output.WriteLine(new string('-', 70));

            DisplaySdkVariables(output);
            DisplayPackageManagementVariables(output);

            // Legacy Variable Compatibility
            output.WriteLine();
            output.WriteLine("\tLEGACY VARIABLE COMPATIBILITY");
            output.WriteLine(new string('-', 70));

            DisplayLegacyVariables(output);

            output.WriteLine();
            output.WriteLine("====================================================================");
            output.WriteLine("");
        }

        /// <summary>
        /// Writes host and entry-point path information for the current process.
        /// </summary>
        /// <param name="output">The <see cref="TextWriter"/> to write the host path information to.</param>
        private static void DisplayHostPathInformation(TextWriter output)
        {
            output.WriteLine("\tHOST PATH INFORMATION");
            output.WriteLine(new string('-', 70));

            string dotnetExePath = Environment.GetCommandLineArgs().FirstOrDefault();
            output.WriteLine($"\tdotnet host path (from args): {dotnetExePath ?? "Not found"}");

            string currentProcessPath = Process.GetCurrentProcess().MainModule?.FileName;
            output.WriteLine($"\tdotnet host path (from process): {currentProcessPath ?? "Not found"}");
            output.WriteLine($"\tdotnet host path (from entry assembly): {Assembly.GetEntryAssembly()?.Location ?? "No entry assembly location found"}");

            // Check for dotnet.exe on PATH
            string dotnetOnPath = Environment.GetEnvironmentVariable("PATH")?
                .Split(Path.PathSeparator)
                .Where(dir => !string.IsNullOrWhiteSpace(dir))
                .Select(dir => Path.Combine(dir, RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "dotnet.exe" : "dotnet"))
                .FirstOrDefault(File.Exists);

            output.WriteLine($"\tdotnet executable found on PATH: {dotnetOnPath ?? "Not found"}");
            output.WriteLine();
        }

        /// <summary>
        /// Writes essential core runtime environment variables and their values.
        /// </summary>
        /// <param name="output">The <see cref="TextWriter"/> to write the variables to.</param>
        private static void DisplayCoreRuntimeVariables(TextWriter output)
        {
            output.WriteLine("\tEssential Runtime Configuration:");

            string[] coreRuntimeVars = new[]
            {
                "DOTNET_ADDITIONAL_DEPS",
                "DOTNET_DefaultDiagnosticPortSuspend",
                "DOTNET_DiagnosticPorts",
                "DOTNET_EnableEventPipe",
                "DOTNET_EventPipeOutputPath",
                "DOTNET_HOST_TRACE",
                "DOTNET_HOST_TRACEFILE",
                "DOTNET_HOST_TRACE_VERBOSITY",
                "DOTNET_MULTILEVEL_LOOKUP",
                "DOTNET_PerfMapEnabled",
                "DOTNET_ReadyToRun",
                "DOTNET_ROLL_FORWARD",
                "DOTNET_ROLL_FORWARD_TO_PRERELEASE",
                "DOTNET_ROOT",
                "DOTNET_SHARED_STORE",
                "DOTNET_STARTUP_HOOKS",
                "DOTNET_TieredCompilation"
            };

            WriteEnvVarBlock(output, coreRuntimeVars);
        }

        /// <summary>
        /// Writes garbage collection related environment variables and their values.
        /// </summary>
        /// <param name="output">The <see cref="TextWriter"/> to write the variables to.</param>
        private static void DisplayGarbageCollectionVariables(TextWriter output)
        {
            output.WriteLine("\tGarbage Collection Configuration:");

            string[] gcVars = new[]
            {
                "DOTNET_GCConserveMemory",
                "DOTNET_GCHeapCount",
                "DOTNET_GCRetainVM",
                "DOTNET_gcServer"
            };

            WriteEnvVarBlock(output, gcVars);
        }

        /// <summary>
        /// Writes threading and performance related environment variables.
        /// </summary>
        /// <param name="output">The <see cref="TextWriter"/> to write the variables to.</param>
        private static void DisplayThreadingPerformanceVariables(TextWriter output)
        {
            output.WriteLine("\tThreading & Performance:");

            string[] threadingVars = new[]
            {
                "DOTNET_SYSTEM_NET_SOCKETS_INLINE_COMPLETIONS",
                "DOTNET_ThreadPool_UnfairSemaphoreSpinLimit"
            };

            WriteEnvVarBlock(output, threadingVars);
        }

        /// <summary>
        /// Writes ASP.NET Core specific environment variables (content root, environment, URLs, etc.).
        /// </summary>
        /// <param name="output">The <see cref="TextWriter"/> to write the variables to.</param>
        private static void DisplayAspNetCoreVariables(TextWriter output)
        {
            output.WriteLine("\tCore Environment Configuration:");

            string[] aspNetVars = new[]
            {
                "ASPNETCORE_CONTENTROOT",
                "ASPNETCORE_DETAILEDERRORS",
                "ASPNETCORE_ENVIRONMENT",
                "ASPNETCORE_FORWARDEDHEADERS_ENABLED",
                "ASPNETCORE_HOSTINGSTARTUPASSEMBLIES",
                "ASPNETCORE_HTTPS_PORT",
                "ASPNETCORE_SHUTDOWNTIMEOUTSECONDS",
                "ASPNETCORE_URLS",
                "ASPNETCORE_WEBROOT",
                "DOTNET_ENVIRONMENT"
            };

            WriteEnvVarBlock(output, aspNetVars);
        }

        /// <summary>
        /// Writes ASP.NET Core security-related variables (certificate paths and passwords). Passwords are masked.
        /// </summary>
        /// <param name="output">The <see cref="TextWriter"/> to write the security variables to.</param>
        /// <remarks>
        /// If an environment variable name includes "Password" and a value is present, the value will be replaced with "***MASKED***".
        /// </remarks>
        private static void DisplayAspNetSecurityVariables(TextWriter output)
        {
            output.WriteLine("\tSSL/TLS & Security Configuration:");

            string[] securityVars = new[]
            {
                "ASPNETCORE_Kestrel__Certificates__Default__Password",
                "ASPNETCORE_Kestrel__Certificates__Default__Path"
            };

            foreach (string variable in securityVars)
            {
                string value = Environment.GetEnvironmentVariable(variable);
                if (variable.Contains("Password") && !string.IsNullOrEmpty(value))
                {
                    value = "***MASKED***";
                }
                output.WriteLine($"\t\t{variable}: {value ?? "(not set)"}");
            }
            output.WriteLine();
        }

        /// <summary>
        /// Writes ASP.NET Core data protection related environment variables.
        /// </summary>
        /// <param name="output">The <see cref="TextWriter"/> to write the variables to.</param>
        private static void DisplayAspNetDataProtectionVariables(TextWriter output)
        {
            output.WriteLine("\tAuthentication & Data Protection:");

            string[] dataProtectionVars = new[]
            {
                "ASPNETCORE_DATAPROTECTION_APPLICATIONNAME"
            };

            WriteEnvVarBlock(output, dataProtectionVars);
        }

        /// <summary>
        /// Writes networking and HTTP client related environment variables (HTTP2/HTTP3 and sockets handler settings).
        /// </summary>
        /// <param name="output">The <see cref="TextWriter"/> to write the networking variables to.</param>
        private static void DisplayNetworkingVariables(TextWriter output)
        {
            output.WriteLine("\tHTTP Client & Protocol Support:");

            string[] networkingVars = new[]
            {
                "DOTNET_SYSTEM_NET_DISABLEIPV6",
                "DOTNET_SYSTEM_NET_HTTP_SOCKETSHTTPHANDLER_HTTP2SUPPORT",
                "DOTNET_SYSTEM_NET_HTTP_SOCKETSHTTPHANDLER_HTTP3SUPPORT",
                "DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER",
                "DOTNET_SYSTEM_NET_HTTP_USEPORTSINURLS"
            };

            WriteEnvVarBlock(output, networkingVars);
        }

        /// <summary>
        /// Writes platform-specific environment flags and prints the detected current platform (Windows/Linux/macOS).
        /// </summary>
        /// <param name="output">The <see cref="TextWriter"/> to write the platform-specific variables and detected platform to.</param>
        private static void DisplayPlatformSpecificVariables(TextWriter output)
        {
            string[] platformVars = new[]
            {
                "DOTNET_SYSTEM_GLOBALIZATION_INVARIANT",
                "DOTNET_SYSTEM_GLOBALIZATION_PREDEFINED_CULTURES_ONLY",
                "DOTNET_LegacyNullReferenceExceptionPolicy",
                "DOTNET_USE_POLLING_FILE_WATCHER",
                "DOTNET_SYSTEM_NET_HTTP_USEKESTREL"
            };

            string platform = "Unknown";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                platform = "Windows";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                platform = "Linux";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                platform = "macOS";

            output.WriteLine($"\tPlatform-Specific Configuration (Current: {platform}):");

            WriteEnvVarBlock(output, platformVars);
        }

        /// <summary>
        /// Writes container-related environment variables used to detect containerized execution and console settings.
        /// </summary>
        /// <param name="output">The <see cref="TextWriter"/> to write the container variables to.</param>
        private static void DisplayContainerVariables(TextWriter output)
        {
            output.WriteLine("\tContainer Detection & Optimization:");

            string[] containerVars = new[]
            {
                "DOTNET_RUNNING_IN_CONTAINER",
                "DOTNET_RUNNING_IN_CONTAINERS",
                "DOTNET_SYSTEM_CONSOLE_ALLOW_ANSI_COLOR_REDIRECTION"
            };

            WriteEnvVarBlock(output, containerVars);
        }

        /// <summary>
        /// Writes commonly used SDK-related environment variables that affect developer experience and CLI behavior.
        /// </summary>
        /// <param name="output">The <see cref="TextWriter"/> to write the SDK variables to.</param>
        private static void DisplaySdkVariables(TextWriter output)
        {
            output.WriteLine("\tEssential Customer Configuration:");

            string[] sdkVars = new[]
            {
                "DOTNET_CLI_TELEMETRY_OPTOUT",
                "DOTNET_CLI_UI_LANGUAGE",
                "DOTNET_NOLOGO",
                "DOTNET_SKIP_FIRST_TIME_EXPERIENCE"
            };

            WriteEnvVarBlock(output, sdkVars);
        }

        /// <summary>
        /// Writes package management related environment variables that influence NuGet behavior and caches.
        /// </summary>
        /// <param name="output">The <see cref="TextWriter"/> to write the package management variables to.</param>
        private static void DisplayPackageManagementVariables(TextWriter output)
        {
            output.WriteLine("\tPackage Management (Customer Impact):");

            string[] packageVars = new[]
            {
                "NUGET_FALLBACK_PACKAGES",
                "NUGET_HTTP_CACHE_PATH",
                "NUGET_PACKAGES",
                "NUGET_SCRATCH"
            };

            WriteEnvVarBlock(output, packageVars);
        }

        /// <summary>
        /// Writes legacy compatibility environment variables such as COREHOST_* and COMPlus_* settings.
        /// </summary>
        /// <param name="output">The <see cref="TextWriter"/> to write the legacy compatibility variables to.</param>
        private static void DisplayLegacyVariables(TextWriter output)
        {
            output.WriteLine("\tCOREHOST_* Variables (Still Supported):");

            string[] corehostVars = new[]
            {
                "COREHOST_TRACE",
                "COREHOST_TRACEFILE"
            };

            // (No trailing blank line here originally; leave as-is)
            foreach (string variable in corehostVars)
            {
                string value = Environment.GetEnvironmentVariable(variable);
                output.WriteLine($"\t\t{variable}: {value ?? "(not set)"}");
            }

            output.WriteLine();
            output.WriteLine("\tCOMPlus_* Variables (Deprecated but Functional):");

            string[] complusVars = new[]
            {
                "COMPlus_EnableEventPipe",
                "COMPlus_gcServer",
                "COMPlus_GCConserveMemory",
                "COMPlus_ReadyToRun",
                "COMPlus_TieredCompilation"
            };

            WriteEnvVarBlock(output, complusVars);
        }

        /// <summary>
        /// Writes a block of environment variables and their values to the provided <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="output">The <see cref="TextWriter"/> to write the variables to.</param>
        /// <param name="variables">A collection of environment variable names to query and print.</param>
        /// <remarks>
        /// For each variable in <paramref name="variables"/>, this method writes a line containing the variable name
        /// and its value or "(not set)" if the variable is not present. After the block it writes a blank line to separate sections.
        /// </remarks>
        private static void WriteEnvVarBlock(TextWriter output, IEnumerable<string> variables)
        {
            foreach (string variable in variables)
            {
                string value = Environment.GetEnvironmentVariable(variable);
                output.WriteLine($"\t\t{variable}: {value ?? "(not set)"}");
            }
            output.WriteLine();
        }
    }
}
