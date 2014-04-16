// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextFacts.cs" company="CatenaLogic">
//   Copyright (c) 2012 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitHubLink.Test
{
    using Catel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class ContextFacts
    {
        #region Nested type: TheDefaultValues

        [TestClass]
        public class TheDefaultValues
        {
            #region Methods

            [TestMethod]
            public void SetsRightDefaultValues()
            {
                var context = new Context();

                Assert.AreEqual("Release", context.ConfigurationName);
                Assert.IsFalse(context.IsHelp);
            }

            #endregion
        }

        #endregion
    }
}