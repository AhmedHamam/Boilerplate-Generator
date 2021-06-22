﻿using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.ExtraFeatures.UnitOfWork;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using BoilerplateGenerator.ViewModels;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.Infrastructure
{
    public class EntityRepositoryImplementationGeneratorModel : BaseGenericGeneratorModel
    {
        private readonly IViewModelBase _viewModelBase;
        private readonly IMetadataGenerationService _metadataGenerationService;
        private readonly IUnitOfWorkRequirements _unitOfWorkRequirements;

        public EntityRepositoryImplementationGeneratorModel
        (
            IViewModelBase viewModelBase, 
            IMetadataGenerationService metadataGenerationService,
            IUnitOfWorkRequirements unitOfWorkRequirements
        )
            : base(viewModelBase, metadataGenerationService)
        {
            _viewModelBase = viewModelBase;
            _metadataGenerationService = metadataGenerationService;
            _unitOfWorkRequirements = unitOfWorkRequirements;
        }

        public override bool CanBeCreated => _viewModelBase.UseUnitOfWork;

        public override AssetKind Kind => AssetKind.EntityRepositoryImplementation;

        protected override IEnumerable<string> UsingsBuilder => new string[]
        {
           $"{_metadataGenerationService.NamespaceByAssetKind(AssetKind.EntityRepositoryInterface)}",
           $"{_viewModelBase.EntityTree.PrimaryEntityNamespace()}",
           _unitOfWorkRequirements.BaseRepositoryClass.Namespace,
           _unitOfWorkRequirements.DbContextClass.Namespace
        };

        public override CompilationUnitDefinitionModel CompilationUnitDefinition => new CompilationUnitDefinitionModel
        {
            AccessModifier = SyntaxKind.InternalKeyword,
            DefinedInheritanceTypes = new string[]
            {
                $"{CommonTokens.BaseRepository}<{_viewModelBase.EntityTree.PrimaryEntityName()}>",
                $"{_metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.EntityRepositoryInterface]}",
            }
        };

        protected override IEnumerable<PropertyDefinitionModel> AvailablePropertiesBuilder => Enumerable.Empty<PropertyDefinitionModel>();

        protected override IEnumerable<ConstructorDefinitionModel> ConstructorsBuilder
        {
            get
            {
                return new ConstructorDefinitionModel[]
                {
                    new ConstructorDefinitionModel
                    {
                        CallBaseConstructor = true,
                        Parameters = new ParameterDefinitionModel[]
                        {
                            new ParameterDefinitionModel
                            {
                                ReturnType = _unitOfWorkRequirements.DbContextClass.Name,
                                Name = $"{nameof(CommonTokens.Context).ToLowerCamelCase()}"
                            }
                        },
                    }
                };
            }
        }
    }
}