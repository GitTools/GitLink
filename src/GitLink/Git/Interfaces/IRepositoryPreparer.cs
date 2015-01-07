// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepositoryPreparer.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink.Git
{
    public interface IRepositoryPreparer
    {
        bool IsPreparationRequired(Context context);
        string Prepare(Context context, TemporaryFilesContext temporaryFilesContext);
    }
}