namespace GitLink.Interfaces
{
    using System.IO;

    public interface ISolutionParser
    {
        StreamReader SolutionReader { get; set; }
        object[] Projects { get; }
        void ParseSolution();
    }
}