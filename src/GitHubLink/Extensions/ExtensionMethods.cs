// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="CatenaLogic">
//   Copyright (c) 2012 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitHubLink
{
    using System;
    using System.Text;

    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        #region Methods

        public static string TrimToFirstLine(this string s)
        {
            return s.Split(new[]
            {
                "\r\n",
                "\n"
            }, StringSplitOptions.None)[0];
        }

        public static void AppendLineFormat(this StringBuilder stringBuilder, string format, params object[] args)
        {
            stringBuilder.AppendFormat(format, args);
            stringBuilder.AppendLine();
        }

        public static string TrimStart(this string value, string toTrim)
        {
            if (!value.StartsWith(toTrim))
            {
                return value;
            }
            var startIndex = toTrim.Length;
            return value.Substring(startIndex);
        }

        public static bool IsOdd(this int number)
        {
            return number % 2 != 0;
        }

        public static string JsonEncode(this string value)
        {
            if (value != null)
            {
                return value
                    .Replace("\"", "\\\"")
                    .Replace("\\", "\\\\")
                    .Replace("\b", "\\b")
                    .Replace("\f", "\\f")
                    .Replace("\n", "\\n")
                    .Replace("\t", "\\t")
                    .Replace("\r", "\\r");
            }
            return null;
        }

        #endregion
    }
}