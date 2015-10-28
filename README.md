GitLink
==========

[![Join the chat at https://gitter.im/GitTools/GitLink](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/GitTools/GitLink?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

![License](https://img.shields.io/github/license/gittools/gitlink.svg)
![NuGet downloads](https://img.shields.io/nuget/dt/gitlink.svg)
![Version](https://img.shields.io/nuget/v/gitlink.svg)
![Pre-release version](https://img.shields.io/nuget/vpre/gitlink.svg)
![Chocolatey count](https://img.shields.io/chocolatey/dt/gitlink.svg)
![Chocolatey version](https://img.shields.io/chocolatey/v/gitlink.svg)

![GitLink](design/logo/logo_64.png)

GitLink let's users step through your code hosted on GitHub! **Help make .NET open source projects more accessible by enabling this for your .NET projects, it's just a single additional step in your build**. See the list of [projects using GitLink](#projects-using-gitlink).

<a href="https://pledgie.com/campaigns/26957"><img alt="Click here to lend your support to: GitLink and make a donation at pledgie.com !" src="https://pledgie.com/campaigns/26957.png?skin_name=chrome" border="0" /></a>

-- 

**Important note** 

*GitLink* was formerly named *GitHubLink*. By adding support to more Git hosting services the name seemed not covering the whole package. Note that the old GitHubLink packages on NuGet and Chocolatey will no long be updated / maintained.

-- 

GitLink makes symbol servers obsolete which saves you both time with uploading source files with symbols and the user no longer has to specify custom symbol servers (such as symbolsource.org).

![Stepping through external source code](doc/images/GitLink_example.gif)  


This application is based on the <a href="https://github.com/ctaggart/SourceLink"  target="_blank">SourceLink project</a>. SourceLink requires FAKE to run and not everyone likes to write code in F#. GitLink is a wrapper around SourceLink specifically written to be easily used from any build system (locally or a build server) and in any .NET language. It also provides new features such as standard integration with GitHub and BitBucket and the possibility to use remote repositories. GitLink is available as console application and can be referenced as assembly to be used in other .NET assemblies.

The advantage of GitLink is that it is fully customized for Git. It also works with GitHub or BitBucket urls so it **does not require a local git repository to work**. This makes it perfectly usable in continuous integration servers such as <a href="http://www.finalbuilder.com/Continua-CI" target="_blank">Continua CI</a>.

Updating all the pdb files is very fast. A solution with over 85 projects will be handled in less than 30 seconds.

When using GitLink, the user no longer has to specify symbol servers. He/she only has to enable the support for source servers in Visual Studio as shown in the image below:

![Enabling source server support](doc/images/visualstudio_enablesourceserversupport.png)  

# Troubleshooting

**Note that Visual Studio 2012 needs to run elevated in order to download the source server files due to a bug in Visual Studio 2012.**

If the source stepping is not working, double check that Visual Studio has a valid symbol cache directory to store the source files being downloaded:

![Enabling source server support](doc/images/visualstudio_symbolslocation.png)

# Supported git providers

GitLink supports the following providers out of the box (will auto-detect based on the url):

* <a href="https://bitbucket.org/" target="_blank">BitBucket</a>
* <a href="https://github.com/" target="_blank">GitHub</a>

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

1. Build the software (in release mode with pdb files enabled)
2. Run the console application with the right command line parameters

Below are a few examples.

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

## Ignoring projects

When specific projects should be ignored, use the *-ignore* option. This option accepts a comma separated list of projects to ignore: 

	GitLink.exe c:\source\catel -u https://github.com/catel/catel -f Catel.sln -ignore Catel.Core.WP80,Catel.MVVM.WP80

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

```csharp
var context = new GitLink.Context();
context.SolutionDirectory = @"c:\source\catel";
context.TargetUrl = "https://github.com/catel/catel";
context.TargetBranch = "develop";
```

It is possible to create a context based on command line arguments:

```csharp
var context = ArgumentParser.Parse(@"c:\source\catel -u https://github.com/catel/catel -b develop");
```

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
- <a href="https://github.com/fluentribbon/Fluent.Ribbon" target="_blank">Fluent.Ribbon</a>
- <a href="https://github.com/GitTools/GitLink" target="_blank">GitLink</a>
- <a href="https://github.com/MahApps/MahApps.Metro" target="_blank">MahApps.Metro</a>
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
- <a href="https://github.com/orcomp/Orchestra" target="_blank">Orchestra</a>
- <a href="https://github.com/oxyplot/oxyplot" target="_blank">OxyPlot</a>
- <a href="http://romanticweb.net" target="_blank">Romantic Web</a>
- <a href="https://github.com/xunit/xunit" target="_blank">xUnit.net</a>
- <a href="https://github.com/xunit/visualstudio.xunit" target="_blank">xUnit.net Visual Studio Runner</a>

Are you using GitLink in your projects? Let us know and we will add your project to the list.

*Note that you can also create a pull request on this document and add it yourself.* 
 

# Icon

Link by Dominic Whittle from The Noun Project
