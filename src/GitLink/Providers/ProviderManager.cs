// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProviderManager.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink.Providers
{
    using Catel.IoC;
    using Catel.Reflection;

    public class ProviderManager : IProviderManager
    {
        public ProviderBase GetProvider(string url)
        {
            var providerTypes = TypeCache.GetTypes(x => typeof(ProviderBase).IsAssignableFromEx(x) && !x.IsAbstract);

            var typeFactory = TypeFactory.Default;

            foreach (var providerType in providerTypes)
            {
                var provider = (ProviderBase) typeFactory.CreateInstance(providerType);
                if (provider.Initialize(url))
                {
                    return provider;
                }
            }

            return null;
        }
    }
}