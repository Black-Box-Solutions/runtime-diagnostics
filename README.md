# runtime-diagnostics

A small utility library that collects and prints diagnostic information about the running .NET application at runtime. It provides a strongly-typed `RuntimeInfo` snapshot and a set of environment-variable inspectors that surface useful runtime, GC, JIT, ASP.NET Core, networking, platform and container configuration used for troubleshooting and telemetry.

Supported/validated targets in this repository: .NET Standard 2.0 and modern .NET (NET5/NET6/NET7/NET8/NET9).

## What this package does

- Returns a compact, strongly-typed snapshot of the runtime and process using `RuntimeInfo.GetRuntimeInfo()`.
- Writes a human-readable runtime summary via `RuntimeInfo.DisplayRuntimeInformation(TextWriter)`.
- Dumps categorized .NET-related environment variables via `DiagnosticMechanics.DisplayDotNetEnvironmentVariables(TextWriter)` for quick inspection (core runtime settings, GC config, threading, ASP.NET Core, networking, platform-specific flags, container/cloud cues, SDK and package management variables, and legacy compatibility variables).

This is useful for:
- Collecting runtime telemetry during startup.
- Printing diagnostic headers in logs.
- Producing environment reports when debugging deployment differences (containers, cloud, OS, or CI).

## What information is emitted

Runtime snapshot includes (non-exhaustive):
- OS and platform: `OSDescription`, `OSArchitecture`, `Is64BitOperatingSystem`
- Process: `ProcessArchitecture`, `Is64BitProcess`, `ProcessId` (where available), `ProcessPath` (where available)
- Runtime: `RuntimeDescription`, `RuntimeVersion`, `RuntimeDirectory`, `RuntimeIdentifier` (where available)
- Core library: `CoreLibInfoVersion`, `CoreLibLocation`
- GC / JIT / performance: `GCMaxGeneration`, `IsServerGC`, `IsJITOptimized`, `TieredCompilationEnabled`, `BuildConfiguration`
- Machine: `ProcessorCount`

Environment variable categories printed include (sample variables shown):
- Core runtime: `DOTNET_TieredCompilation`, `DOTNET_ROLL_FORWARD`, `DOTNET_STARTUP_HOOKS`, `DOTNET_ROOT`
- GC tuning: `DOTNET_gcServer`, `DOTNET_GCConserveMemory`, `DOTNET_GCHeapCount`
- Threading / perf: `DOTNET_ThreadPool_UnfairSemaphoreSpinLimit`
- ASP.NET Core: `ASPNETCORE_ENVIRONMENT`, `ASPNETCORE_URLS`, `ASPNETCORE_CONTENTROOT`, `DOTNET_ENVIRONMENT`
- Networking: `DOTNET_SYSTEM_NET_HTTP_SOCKETSHTTPHANDLER_HTTP2SUPPORT`, `DOTNET_SYSTEM_NET_DISABLEIPV6`
- Container detection: `DOTNET_RUNNING_IN_CONTAINER`, `DOTNET_RUNNING_IN_CONTAINERS`
- SDK / package variables: `DOTNET_CLI_TELEMETRY_OPTOUT`, `NUGET_PACKAGES`
- Legacy / compatibility: `COREHOST_TRACE`, `COMPlus_TieredCompilation` (and other `COMPlus_*` keys)

## Quick usage examples

```
examples/Program.cs 

using System; 
using System.IO; 
using BlackBoxSolutions.Diagnostics;

class Program
{
    static void Main()
    {
        // Print a formatted runtime summary to console
        RuntimeInfo.DisplayRuntimeInformation(Console.Out);
        // Print categorized .NET environment variables to console
        DiagnosticMechanics.DisplayDotNetEnvironmentVariables(Console.Out);

        // Obtain a strongly-typed object to inspect or serialize
        var info = RuntimeInfo.GetRuntimeInfo();
    
        // Example: serialize to JSON (requires a JSON serializer)
        // Console.WriteLine(JsonSerializer.Serialize(info, new JsonSerializerOptions { WriteIndented = true }));
    }
}
```
Example snippet of the runtime summary (formatted output; values will vary by host):
```
examples/sample-output.txt

==================== Runtime Information ====================
CoreLibInfoVersion: 9.0.0,
CoreLibLocation: C:\Program Files\dotnet\shared\Microsoft.NETCore.App\9.0.0\System.Private.CoreLib.dll
GCMaxGeneration: 2, IsServerGC: False
IsNetCore: True, RuntimeDescription: .NET 9.0.0
RuntimeVersion: 9.0.0, RuntimeDirectory: C:\app\
OSDescription: Microsoft Windows 10.0.19044, OSArchitecture: X64
ProcessArchitecture: X64, Is64BitProcess: True, ProcessorCount: 32
BuildConfiguration: Release (Optimized), IsJITOptimized: True
==========================================================
```

Example snippet of environment variables output (truncated):
```
examples/env-output.txt
==================== .NET Environment Variables ====================
HOST PATH INFORMATION
------------------------------------------------------
    dotnet host path (from args): C:\Program Files\dotnet\dotnet.exe
    dotnet host path (from process): C:\Program Files\dotnet\dotnet.exe
    dotnet host path (from entry assembly): C:\app\MyApp.dll
    dotnet executable found on PATH: C:\Program Files\dotnet\dotnet.exe
CORE RUNTIME ENVIRONMENT VARIABLES
----------------------------------------------------------------------
    DOTNET_TieredCompilation: 1
    DOTNET_ROOT: C:\Program Files\dotnet
    DOTNET_ReadyToRun: (not set)

====================================================================
```

## Integration notes
- Use the `Display*` helpers with any `TextWriter` (console, file, ASP.NET logs).
- `RuntimeInfo.GetRuntimeInfo()` is intended for programmatic inspection and serialization.
- Some properties (e.g., `ProcessId`, `ProcessPath`, `RuntimeIdentifier`) only exist on newer TFMs; the library uses conditional compilation to expose them where available.

## Contributing
See the repository for examples and unit tests. Create issues or PRs for additional environment checks
or output formats (JSON export is a planned enhancement).



