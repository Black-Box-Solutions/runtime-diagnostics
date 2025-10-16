using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace BlackBoxSolutions.Diagnostics
{
    public static partial class DiagnosticMechanics
    {
        public static void DisplayDotNetEnvironmentVariables(TextWriter output)
        {
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

        private static void DisplayAspNetDataProtectionVariables(TextWriter output)
        {
            output.WriteLine("\tAuthentication & Data Protection:");

            string[] dataProtectionVars = new[]
            {
                "ASPNETCORE_DATAPROTECTION_APPLICATIONNAME"
            };

            WriteEnvVarBlock(output, dataProtectionVars);
        }

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
