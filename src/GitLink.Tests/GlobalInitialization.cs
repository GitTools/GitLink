// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalInitialization.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using Catel.Logging;
using NUnit.Framework;

[SetUpFixture]
public class GlobalInitialization
{
    [SetUp]
    public static void SetUp()
    {
#if DEBUG
        LogManager.AddDebugListener(true);
#endif
    }
}