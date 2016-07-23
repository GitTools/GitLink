namespace GitLink.Interfaces
{
    public interface IProjectInSolution
    {
        object ProjectType { get; }
        string RelativePath { get; }
        object ProjectConfigurations { get; }
    }
}