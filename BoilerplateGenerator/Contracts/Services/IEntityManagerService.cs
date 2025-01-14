﻿using BoilerplateGenerator.Collections;
using BoilerplateGenerator.Contracts.RoslynWrappers;
using BoilerplateGenerator.Models.RoslynWrappers;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoilerplateGenerator.Contracts.Services
{
    public interface IEntityManagerService
    {
        bool IsEntityClassTypeValid { get; }

        Task<string> LoadSelectedEntityDetails();

        IEnumerable<IProjectWrapper> RetrieveAllModules();

        Task<ISolutionWrapper> RetrieveSolution();

        Task FindSelectedFileClassType();

        Task<ITreeNode<IBaseSymbolWrapper>> PopulateClassHierarchy();

        Task<INamedTypeSymbol[]> RetrieveAllAvailableProjectTypes();

        Task<IEnumerable<EntityClassWrapper>> RetrieveAbstractClasses();
    }
}