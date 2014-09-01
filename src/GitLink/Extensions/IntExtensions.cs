// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntExtensions.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink
{
    public static class ExtensionMethods
    {
        public static bool IsOdd(this int number)
        {
            return number % 2 != 0;
        }
    }
}