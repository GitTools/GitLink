using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using GitTools.Git;

namespace GitLink.Providers
{
    public class VisualStudioTeamServicesProvider : ProviderBase
    {
        private readonly Regex _visualStudioTeamServicesRegex = new Regex(@"(?<url>(?<companyurl>(?:https://)?(?<accountname>([a-zA-Z0-9\-\.]*)?)\.visualstudio\.com/)(?<project>[a-zA-Z0-9\-\.]*)/?_git/(?<repo>[^/]+))");

        public VisualStudioTeamServicesProvider() 
            : base(new GitPreparer())
        {
        }

        public override string RawGitUrl
        {
            get
            {
                return "";
            }
        }

        public override bool Initialize(string url)
        {
            var match = _visualStudioTeamServicesRegex.Match(url);

            if (!match.Success)
            {
                return false;
            }

            CompanyName = match.Groups["accountname"].Value;
            CompanyUrl = match.Groups["companyurl"].Value + "DefaultCollection/";

            ProjectName = match.Groups["project"].Value;
            if(ProjectName == "")
            {
                ProjectName = match.Groups["repo"].Value;
            }

            ProjectUrl = match.Groups["companyurl"].Value + ProjectName + "/";

            if (!CompanyUrl.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
            {
                CompanyUrl = String.Concat("https://", CompanyUrl);
            }

            if (!ProjectUrl.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
            {
                ProjectUrl = String.Concat("https://", ProjectUrl);
            }

            return true;
        }
    }
}
