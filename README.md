GitLink
==========

[![Join the chat at https://gitter.im/GitTools/GitLink](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/GitTools/GitLink?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
![License](https://img.shields.io/github/license/gittools/gitlink.svg)
![Version](https://img.shields.io/nuget/v/gitlink.svg)
![Pre-release version](https://img.shields.io/nuget/vpre/gitlink.svg)
![Chocolatey count](https://img.shields.io/chocolatey/dt/gitlink.svg)
![Chocolatey version](https://img.shields.io/chocolatey/v/gitlink.svg)

![GitLink](design/logo/logo_64.png)

GitLink let's users step through your code hosted on GitHub! **Help make .NET open source projects more accessible by enabling this for your .NET projects, it's just a single additional step in your build**. See the list of [projects using GitLink](#projects-using-gitlink).

<a href="https://pledgie.com/campaigns/26957"><img alt="Click here to lend your support to: GitLink and make a donation at pledgie.com !" src="https://pledgie.com/campaigns/26957.png?skin_name=chrome" border="0" /></a>

-- 

**Important** 

*GitLink* was formerly named *GitHubLink*. By adding support to more Git hosting services the name seemed not covering the whole package. The old GitHubLink packages on NuGet and Chocolatey will no longer be updated or maintained.

*Private git repositories* might be problematic. [More info](#source-stepping-returns-html).

--

GitLink makes symbol servers obsolete which saves you both time with uploading source files with symbols and the user no longer has to specify custom symbol servers (such as symbolsource.org).

![Stepping through external source code](doc/images/GitLink_example.gif)  

The advantage of GitLink is that it is fully customized for Git. It also works with GitHub or BitBucket urls so it **does not require a local git repository to work**. This makes it perfectly usable in continuous integration servers such as <a href="http://www.finalbuilder.com/Continua-CI" target="_blank">Continua CI</a>.

Updating all the pdb files is very fast. A solution with over 85 projects will be handled in less than 30 seconds.

When using GitLink, the user no longer has to specify symbol servers. The only requirement is to ensure the check the `Enable source server support` option in Visual Studio as shown below:

![Enabling source server support](doc/images/visualstudio_enablesourceserversupport.png)  

# Troubleshooting

## Source Stepping isn't working

* Visual Studio 2012 needs to run elevated in order to download the source server files

* Specify a value for Visual Studio -> Options -> Debugging -> Symbols -> `Cache Symbols in this directory`

![Enabling source server support](doc/images/visualstudio_symbolslocation.png)

* Verify that there is not a case mismatch in git folder names vs. references in the .csproj that was used during packaging. E.g. GitHub will not serve /Folder/Somefile.cs if Visual Studio requests /FOLdeR/Somefile.cs.

## Source Stepping returns HTML
If your repository is private, you are likely seeing the logon HTML from your git host.

* Log onto your git host in Internet Explorer
* Purge your local symbol cache

Note that this approach is not guaranteed to work.  Visual Studio needs to authenticate to retrieve the source files
but does not ask the user for credentials to do so.  There are ways to work around this, but no mechanism is currently
provided out-of-the-box in *GitLink*.

Possible workarounds
* Include a mechanism in the pdb to retrieve credentials (using PowerShell and Windows credentials store) (see [#37](https://github.com/GitTools/GitLink/issues/37))
* Use a proxy service that does not require authentication (see [#66](https://github.com/GitTools/GitLink/issues/66) and [Source server with Git repository](https://shonnlyga.wordpress.com/2016/05/28/source-server-with-git-repository))


# Supported git providers

GitLink supports the following providers out of the box (will auto-detect based on the url):

* <a href="https://bitbucket.org/" target="_blank">BitBucket</a>
* <a href="https://github.com/" target="_blank">GitHub</a>
* Custom Provider (custom urls)

Providers currently being worked on:

* <a href="https://www.assembla.com/home" target="_blank">Assembla</a>
* <a href="http://beanstalkapp.com/" target="_blank">Beanstalk</a>
* <a href="http://www.cloudforge.com/" target="_blank">CloudForge</a>
* <a href="https://www.codebasehq.com/" target="_blank">Codebase</a>
* <a href="https://www.fogcreek.com/kiln/" target="_blank">FogCreek</a>
* <a href="https://plan.io/" target="_blank">Planio</a>
* <a href="http://projectlocker.com/" target="_blank">ProjectLocker</a> 
* <a href="https://rhodecode.com/" target="_blank">RhodeCode</a>
* <a href="https://unfuddle.com/" target="_blank">Unfuddle</a>   

It is also possible to specify a custom url provider.

# Using GitLink as command line tool

Using GitLink via the command line is very simple:

1. Build the solution - in release mode with pdb files enabled
2. Run the console application with the right command line parameters
3. Include the PDB in your nuget package

See [Oren Novotony's blog post](https://oren.codes/2015/09/23/enabling-source-code-debugging-for-your-nuget-packages-with-gitlink/) for even more detail and examples on build integration.

## Most simple usage

This is the most simple usage available **starting from 2.2.0**. It will automatically determine the url and commit based on a local *.git* directory.

	GitLink.exe c:\source\catel

## Running for the default branch

    GitLink.exe c:\source\catel -u https://github.com/catel/catel 

This will use the default branch (which is in most cases **master**). You can find out the default branch by checking what branch is loaded by default on the GitHub page.

## Running for a specific branch

    GitLink.exe c:\source\catel -u https://github.com/catel/catel -b develop

This will use the develop branch.

## Running for a specific branch and configuration

    GitLink.exe c:\source\catel -u https://github.com/catel/catel -b develop -c debug

This will use the develop branch and the debug configuration.

## Running for a specific solution only

Sometimes a repository contains more than 1 solution file. By default, all solutions will be processed. To only process a single solution file, use the *-f* option: 

	GitLink.exe c:\source\catel -u https://github.com/catel/catel -f Catel.sln

## Ignoring projects and explicitly including them

When specific projects should be ignored, use the *-ignore* option. This option accepts a comma separated list of patterns to ignore. Each pattern is either a literal project name (case-insensitive) or a regex enclosed in slashes. For example: 

	GitLink.exe c:\source\catel -u https://github.com/catel/catel -f Catel.sln -ignore Catel.Core.WP80,Catel.MVVM.WP80
	GitLink.exe c:\source\catel -u https://github.com/catel/catel -f Catel.sln -ignore /^.+\.WP80$/,Catel.Core

In case you want to ignore most of your projects, you can explicitly *-include* only the projects you need - others will be ignored automatically. Same as *-ignore* it accepts list of patterns. For example:

	GitLink.exe c:\source\catel -u https://github.com/catel/catel -f Catel.sln -include Catel.Core
	GitLink.exe c:\source\catel -u https://github.com/catel/catel -f Catel.sln -include /Catel\..*$/,SomeOtherProject

Finally, you can set both *-ignore* and *-include* options. In this case only projects matching one of *-include* patterns will be taken, but if and only if they don't match one of *-ignore*s. For example, the following command line will include only Catel.* projects, except "Catel.Core":

	GitLink.exe c:\source\catel -u https://github.com/catel/catel -f Catel.sln -include /Catel\..*$/ -ignore Catel.Core
 
## Running for an uncommon / customized URL

When working with a repository using uncommon URL you can use placeholders to specifiy where the filename and revision hash should be, use `-u` parameter with the custom URL

    GitLink.exe c:\source\catel -u "https://host/projects/catel/repos/catel/browse/{filename}?at={revision}&raw"
    
The custom url will be used to fill the placeholders with the relative file path and the revision hash.

## Running for a custom raw content URL

When working with a content proxy or an alternative git VCS system that supports direct HTTP access to specific file revisions use the `-u` parameter with the custom raw content root URL

    GitLink.exe c:\source\catel -u https://raw.githubusercontent.com/catel/catel
    
The custom url will be used to fill in the following pattern `{customUrl}/{revision}/{relativeFilePath}` when generating the source mapping.

## Getting help

When you need help about GitLink, use the following command line:

    GitLink.exe -help

## Logging to a file

When you need to log the information to a file, use the following command line:

    GitLink.exe c:\source\catel -u https://github.com/catel/catel -b develop -l GitLinkLog.log

# Using GitLink in code

GitLink is built with 2 usages in mind: command line and code reference. Though most people will use the command line version, it is possible to reference the executable and use the logic in code.

The command line implementation uses the same available API. 

## Creating a context

To link files to a Git project, a context must be created. The command line version does this by using the *ArgumentParser* class. It is also possible to create a context from scratch as shown in the example below:

	var context = new GitLink.Context(new ProviderManager());
	context.SolutionDirectory = @"c:\source\catel";
	context.TargetUrl = "https://github.com/catel/catel";
	context.TargetBranch = "develop";

It is possible to create a context based on command line arguments:

	var context = ArgumentParser.Parse(@"c:\source\catel -u https://github.com/catel/catel -b develop");

## Linking a context

Once a context is created, the *Linker* class can be used to actually link the files:

    Linker.Link(context);

# How to get

There are three general ways to get GitLink:

## Get it from GitHub

The releases will be available as separate executable download on the [releases tab](https://github.com/GitTools/GitLink/releases) of the project.

## Get it via Chocolatey

If you want to install the tool on your (build) computer, the package is available via <a href="https://chocolatey.org/" target="_blank">Chocolatey</a>. To install, use the following command:

    choco install gitlink

## Get it via NuGet

If you want to reference the assembly to use it in code, the recommended way to get it is via <a href="http://www.nuget.org/" target="_blank">NuGet</a>. 

**Note that getting GitLink via NuGet will add it as a reference to the project**

# How does it work

The SrcSrv tool (Srcsrv.dll) enables a client to retrieve the exact version of the source files that were used to build an application. Because the source code for a module can change between versions and over the course of years, it is important to look at the source code as it existed when the version of the module in question was built.

For more information, see the <a href="http://msdn.microsoft.com/en-us/library/windows/hardware/ff558791(v=vs.85).aspx" target="_blank">official documentation of SrcSrv</a>.

GitLink creates a source index file and updates the PDB file so it will retrieve the files from the Git host file handler.

<a name="projects-using-gitlink"></a>
# Projects using GitLink

Below is a list of projects already using GitLink (alphabetically ordered).

- <a href="http://www.catelproject.com" target="_blank">Catel</a>
- <a href="http://www.expandframework.com/" target="_blank">eXpand</a>
- <a href="https://fakeiteasy.github.io/" target="_blank">FakeItEasy</a>
- <a href="https://github.com/fluentribbon/Fluent.Ribbon" target="_blank">Fluent.Ribbon</a>
- <a href="https://github.com/GitTools/GitLink" target="_blank">GitLink</a>
- <a href="https://github.com/MahApps/MahApps.Metro" target="_blank">MahApps.Metro</a>
- <a href="https://github.com/MetacoSA/NBitcoin" target="_blank">NBitcoin</a>
- <a href="https://github.com/MetacoSA/NBitcoin.Indexer" target="_blank">NBitcoin.Indexer</a>
- <a href="https://github.com/elasticsearch/elasticsearch-net" target="_blank">NEST and Elasticsearch.NET</a>
- <a href="https://github.com/orcomp/Orc.Analytics" target="_blank">Orc.Analytics</a>
- <a href="https://github.com/orcomp/Orc.AutomaticSupport" target="_blank">Orc.AutomaticSupport</a>
- <a href="https://github.com/orcomp/Orc.CommandLine" target="_blank">Orc.CommandLine</a>
- <a href="https://github.com/orcomp/Orc.Controls" target="_blank">Orc.Controls</a>
- <a href="https://github.com/orcomp/Orc.CrashReporting" target="_blank">Orc.CrashReporting</a>
- <a href="https://github.com/orcomp/Orc.CsvHelper" target="_blank">Orc.CsvHelper</a>
- <a href="https://github.com/orcomp/Orc.Feedback" target="_blank">Orc.Feedback</a>
- <a href="https://github.com/orcomp/Orc.FileAssociation" target="_blank">Orc.FileAssociation</a>
- <a href="https://github.com/orcomp/Orc.FilterBuilder" target="_blank">Orc.FilterBuilder</a>
- <a href="https://github.com/orcomp/Orc.LicenseManager" target="_blank">Orc.LicenseManager</a>
- <a href="https://github.com/orcomp/Orc.Metadata" target="_blank">Orc.Metadata</a>
- <a href="https://github.com/orcomp/Orc.Notifications" target="_blank">Orc.Notifications</a>
- <a href="https://github.com/orcomp/Orc.NuGetExplorer" target="_blank">Orc.NuGetExplorer</a>
- <a href="https://github.com/orcomp/Orc.ProjectManagement" target="_blank">Orc.ProjectManagement</a>
- <a href="https://github.com/orcomp/Orc.Search" target="_blank">Orc.Search</a>
- <a href="https://github.com/orcomp/Orc.Sort" target="_blank">Orc.Sort</a>
- <a href="https://github.com/orcomp/Orc.Squirrel" target="_blank">Orc.Squirrel</a>
- <a href="https://github.com/orcomp/Orc.SupportPackage" target="_blank">Orc.SupportPackage</a>
- <a href="https://github.com/orcomp/Orc.SystemInfo" target="_blank">Orc.SystemInfo</a>
- <a href="https://github.com/orcomp/Orc.WorkspaceManagement" target="_blank">Orc.WorkspaceManagement</a>
- <a href="https://github.com/orcomp/Orc.Wizard" target="_blank">Orc.Wizard</a>
- <a href="https://github.com/orcomp/Orchestra" target="_blank">Orchestra</a>
- <a href="https://github.com/oxyplot/oxyplot" target="_blank">OxyPlot</a>
- <a href="https://github.com/MetacoSA/QBitNinja" target="_blank">QBitNinja</a>
- <a href="http://reactiveui.net" target="_blank">ReactiveUI</a>
- <a href="http://romanticweb.net" target="_blank">Romantic Web</a>
- <a href="https://github.com/dotnet/roslyn" target="_blank">Roslyn</a>
- <a href="https://github.com/volkanceylan/serenity" target="_blank">Serenity</a>
- <a href="https://github.com/xunit/xunit" target="_blank">xUnit.net</a>
- <a href="https://github.com/xunit/visualstudio.xunit" target="_blank">xUnit.net Visual Studio Runner</a>

Are you using GitLink in your projects? Let us know and we will add your project to the list.

*Note that you can also create a pull request on this document and add it yourself.* 
 

# Icon

Link by Dominic Whittle from The Noun Project
