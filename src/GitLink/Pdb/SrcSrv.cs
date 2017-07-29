// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SrcSrv.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitLink.Pdb
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Catel;

    internal static class SrcSrv
    {
        private static string CreateTarget(string rawUrl, string revision)
        {
            return string.Format(rawUrl, revision);
        }

        internal static byte[] Create(string rawUrl, string revision, IEnumerable<Tuple<string, string>> paths, bool downloadWithPowershell)
        {
            Argument.IsNotNullOrWhitespace(() => rawUrl);
            Argument.IsNotNullOrWhitespace(() => revision);

            using (var ms = new MemoryStream())
            {
                using (var sw = new StreamWriter(ms))
                {
                    var scheme = new Uri(rawUrl).Scheme;

                    sw.WriteLine("SRCSRV: ini ------------------------------------------------");
                    sw.WriteLine("VERSION=2");
                    sw.WriteLine("SRCSRV: variables ------------------------------------------");
                    sw.WriteLine("RAWURL={0}", CreateTarget(rawUrl, revision));
                    if (downloadWithPowershell)
                    {
                        sw.WriteLine("TRGFILE=%fnbksl%(%targ%%var2%)");
                        sw.WriteLine("SRCSRVTRG=%TRGFILE%");
                        sw.WriteLine("SRCSRVCMD=powershell invoke-command -scriptblock {$webClient = New-Object System.Net.WebClient; $webClient.UseDefaultCredentials = $true; $webClient.DownloadFile('%RAWURL%', '%TRGFILE%');}");
                    }
                    else
                    {
                        sw.WriteLine("SRCSRVVERCTRL={0}", scheme);
                        sw.WriteLine("SRCSRVTRG=%RAWURL%");
                    }

                    sw.WriteLine("SRCSRV: source files ---------------------------------------");

                    foreach (var tuple in paths)
                    {
                        sw.WriteLine("{0}*{1}", tuple.Item1, tuple.Item2);
                    }

                    sw.WriteLine("SRCSRV: end ------------------------------------------------");

                    sw.Flush();

                    return ms.ToArray();
                }
            }
        }

        internal static byte[] CreateVsts(string revision, IEnumerable<Tuple<string, string>> paths, Dictionary<string, string> vstsData = null)
        {
            Argument.IsNotNullOrWhitespace(() => revision);

            using (var ms = new MemoryStream())
            {
                using (var sw = new StreamWriter(ms))
                {
                    sw.WriteLine("SRCSRV: ini ------------------------------------------------");
                    sw.WriteLine("VERSION=3");
                    sw.WriteLine("INDEXVERSION=2");
                    sw.WriteLine("VERCTRL=Team Foundation Server");
                    sw.WriteLine("DATETIME={0}", string.Format("{0:ddd MMM hh:mm:ss yyyy}", DateTime.Now));
                    sw.WriteLine("INDEXER=TFSTB");
                    sw.WriteLine("SRCSRV: variables ------------------------------------------");
                    sw.WriteLine("TFS_EXTRACT_TARGET=%targ%\\%var5%\\%fnvar%(%var6%)\\%fnbksl%(%var7%)");
                    sw.WriteLine("TFS_EXTRACT_CMD=tf.exe git view /collection:%fnvar%(%var2%) /teamproject:\"%fnvar%(%var3%)\" /repository:\"%fnvar%(%var4%)\" /commitId:%fnvar%(%var5%) /path:\"%var7%\" /output:%SRCSRVTRG% %fnvar%(%var8%)");

                    string tfs_collection;
                    if (vstsData.TryGetValue("TFS_COLLECTION", out tfs_collection))
                    {
                        sw.WriteLine("TFS_COLLECTION={0}", tfs_collection);
                    }

                    string tfs_team_project;
                    if (vstsData.TryGetValue("TFS_TEAM_PROJECT", out tfs_team_project))
                    {
                        sw.WriteLine("TFS_TEAM_PROJECT={0}", tfs_team_project);
                    }

                    string tfs_repo;
                    if (vstsData.TryGetValue("TFS_REPO", out tfs_repo))
                    {
                        sw.WriteLine("TFS_REPO={0}", tfs_repo);
                    }

                    sw.WriteLine("TFS_COMMIT={0}", revision);
                    sw.WriteLine("TFS_SHORT_COMMIT={0}", revision.Substring(0, 8));
                    sw.WriteLine("TFS_APPLY_FILTERS=/applyfilters");
                    sw.WriteLine("SRCSRVVERCTRL=git");
                    sw.WriteLine("SRCSRVERRDESC=access");
                    sw.WriteLine("SRCSRVERRVAR=var2");
                    sw.WriteLine("SRCSRVTRG=%TFS_EXTRACT_TARGET%");
                    sw.WriteLine("SRCSRVCMD=%TFS_EXTRACT_CMD%");
                    sw.WriteLine("SRCSRV: source files ---------------------------------------");

                    foreach (var tuple in paths)
                    {
                        sw.WriteLine("{0}*TFS_COLLECTION*TFS_TEAM_PROJECT*TFS_REPO*TFS_COMMIT*TFS_SHORT_COMMIT*{1}*TFS_APPLY_FILTERS", tuple.Item1, tuple.Item2);
                    }

                    sw.WriteLine("SRCSRV: end ------------------------------------------------");

                    sw.Flush();

                    return ms.ToArray();
                }
            }
        }
    }
}
