// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProviderManager.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink.Providers
{
    using System;
    using Catel.Reflection;

    public static class ProviderManager
    {
        public static ProviderBase GetProvider(string url)
        {
            var providerTypes = TypeCache.GetTypes(x => typeof(ProviderBase).IsAssignableFromEx(x) && !x.IsAbstract);

            foreach (var providerType in providerTypes)
            {
                var provider = (ProviderBase)Activator.CreateInstance(providerType);

                if (provider.Initialize(url))
                    return provider;
            }

            return null;
        }
    }
}