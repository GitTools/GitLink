namespace GitLink.Build.Construction
{
    public interface IProjectConfigurationInSolution
    {
        bool IncludeInBuild { get; }
    }
}