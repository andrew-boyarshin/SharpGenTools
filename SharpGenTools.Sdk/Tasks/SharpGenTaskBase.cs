using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Logger = SharpGen.Logging.Logger;

namespace SharpGenTools.Sdk.Tasks
{
    public abstract class SharpGenTaskBase : Task
    {
        // ReSharper disable MemberCanBeProtected.Global, UnusedAutoPropertyAccessor.Global
        [Required] public string[] CastXmlArguments { get; set; }
        [Required] public ITaskItem CastXmlExecutable { get; set; }
        [Required] public ITaskItem[] ConfigFiles { get; set; }
        [Required] public ITaskItem DocumentationCache { get; set; }
        [Required] public ITaskItem[] ExtensionAssemblies { get; set; }
        [Required] public ITaskItem[] ExternalDocumentation { get; set; }
        [Required] public string GeneratedCodeFolder { get; set; }
        [Required] public ITaskItem[] GlobalNamespaceOverrides { get; set; }
        [Required] public ITaskItem InputsCache { get; set; }
        [Required] public string[] Macros { get; set; }
        [Required] public string OutputPath { get; set; }
        [Required] public ITaskItem[] Platforms { get; set; }
        public ITaskItem ConsumerBindMappingConfig { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global, MemberCanBeProtected.Global

#if DEBUG
        public bool DebugWaitForDebuggerAttach { get; set; }
#endif

        protected Logger SharpGenLogger { get; set; }

        protected void PrepareExecute()
        {
            BindingRedirectResolution.Enable();

#if DEBUG
            if (DebugWaitForDebuggerAttach)
                WaitForDebuggerAttach();
#endif

            SharpGenLogger = new Logger(new MSBuildSharpGenLogger(Log));
        }

        [Conditional("DEBUG")]
        protected internal static void WaitForDebuggerAttach()
        {
            while (!Debugger.IsAttached)
                Thread.Sleep(TimeSpan.FromSeconds(1));
        }
    }
}