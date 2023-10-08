using System.Text.RegularExpressions;

namespace PCKStudio_Updater
{
    public sealed class GithubParams
    {
        public readonly string RepositoryOwnerName;
        public readonly string RepositoryName;
        public readonly string TargetExecutableName;
        public readonly bool UsePreRelease;
        public readonly Regex VersionMatcher;

        public GithubParams(string repositoryOwnerName, string repositoryName, string targetExecutableName, bool usePreRelease, Regex versionMatcher)
        {
            RepositoryOwnerName = repositoryOwnerName;
            RepositoryName = repositoryName;
            TargetExecutableName = targetExecutableName;
            UsePreRelease = usePreRelease;
            VersionMatcher = versionMatcher;
        }
    }
}
