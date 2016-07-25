namespace GitLink.Build.Construction
{
    using System.IO;

    public interface ISolutionParser
    {
        StreamReader SolutionReader { get; set; }
        object[] Projects { get; }
        void ParseSolution();
    }
}