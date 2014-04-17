// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntExtensions.cs" company="CatenaLogic">
//   Copyright (c) 2012 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitHubLink
{
    public static class ExtensionMethods
    {
        public static bool IsOdd(this int number)
        {
            return number %2 != 0;
        }
    }
}