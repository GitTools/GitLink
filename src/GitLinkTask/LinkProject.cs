using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace GitLinkTask
{
    public class LinkProject : ToolTask
    {
        [Required]
        public ITaskItem SolutionDirectory { get; set; }

        [Required]
        public string ProjectName { get; set; }

        protected override string ToolName => "GitLink.exe";

        protected override string GenerateFullPathToTool() => Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), "GitLink.exe");

        protected override string GenerateCommandLineCommands() => $"{SolutionDirectory.FullPath()} -include {ProjectName}";
    }
}