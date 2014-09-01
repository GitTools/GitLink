// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteHelper.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink
{
    using System.IO;

    public static class DeleteHelper
    {
        public static void DeleteGitRepository(string directory)
        {
            if (string.IsNullOrEmpty(directory))
            {
                return;
            }

            foreach (var fileName in Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories))
            {
                var fileInfo = new FileInfo(fileName)
                {
                    IsReadOnly = false
                };

                fileInfo.Delete();
            }

            Directory.Delete(directory, true);
        }
    }
}