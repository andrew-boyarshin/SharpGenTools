using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using SharpGen.Generator;
using SharpGen.Model;
using System.Linq;

namespace SharpGenTools.Sdk.Tasks
{
    public class GetGeneratedCSharpFiles : Task
    {
        [Required]
        public ITaskItem Model { get; set; }

        [Required]
        public string GeneratedCodeFolder { get; set; }

        [Output]
        public ITaskItem[] GeneratedFiles { get; set; }

        public override bool Execute()
        {
            var asm = CsAssembly.Read(Model.ItemSpec);

            GeneratedFiles = RoslynGenerator.GetFilePathsForGeneratedFiles(asm, GeneratedCodeFolder)
                .Select(Utilities.CreateTaskItem)
                .ToArray<ITaskItem>();

            return true;
        }
    }
}
