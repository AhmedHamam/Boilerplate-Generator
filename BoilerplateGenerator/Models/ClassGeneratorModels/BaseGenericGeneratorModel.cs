﻿using BoilerplateGenerator.Contracts.Generators;
using BoilerplateGenerator.Contracts.RoslynWrappers;
using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using BoilerplateGenerator.ViewModels;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoilerplateGenerator.Models.ClassGeneratorModels
{
    public abstract class BaseGenericGeneratorModel : IGenericGeneratorModel
    {
        private readonly object _locker = new object();
        private readonly IViewModelBase _viewModelBase;
        private readonly IMetadataGenerationService _metadataGenerationService;

        protected BaseGenericGeneratorModel(IViewModelBase viewModelBase, IMetadataGenerationService metadataGenerationService)
        {
            _viewModelBase = viewModelBase;
            _metadataGenerationService = metadataGenerationService;
        }


        #region Model Builder
        public abstract AssetKind Kind { get; }

        public virtual bool CanBeCreated => true;

        public virtual bool MergeWithExistingAsset => false;

        protected virtual IEnumerable<string> UsingsBuilder => new string[] { UsingTokens.System };

        private IEnumerable<PropertyDefinitionModel> _availablePropertiesBuilder;
        protected virtual IEnumerable<PropertyDefinitionModel> AvailablePropertiesBuilder
        {
            get
            {
                lock (_locker)
                {
                    if (_availablePropertiesBuilder != null)
                    {
                        return _availablePropertiesBuilder;
                    }

                    _availablePropertiesBuilder = _viewModelBase.EntityTree.First().FilterTreeProperties();
                    return _availablePropertiesBuilder;
                }
            }
        }

        protected virtual IEnumerable<ParameterDefinitionModel> InjectedDependenciesBuilder { get; } = Enumerable.Empty<ParameterDefinitionModel>();

        protected virtual IEnumerable<ConstructorDefinitionModel> ConstructorsBuilder { get; } = Enumerable.Empty<ConstructorDefinitionModel>();

        protected virtual IEnumerable<MethodDefinitionModel> AvailableMethodsBuilder { get; } = Enumerable.Empty<MethodDefinitionModel>();
        #endregion

        #region Public Model Properties
        private string _namespace;
        public string Namespace
        {
            get
            {
                if (_namespace != null)
                {
                    return _namespace;
                }

                _namespace = _metadataGenerationService.NamespaceByAssetKind(Kind);
                return _namespace;
            }
        }

        private string _name;
        public string Name
        {
            get
            {
                if (_name != null)
                {
                    return _name;
                }

                _name = _metadataGenerationService.AssetToCompilationUnitNameMapping[Kind];
                return _name;
            }
        }

        public virtual CompilationUnitDefinitionModel CompilationUnitDefinition { get; } = new CompilationUnitDefinitionModel();

        public bool FileExistsInProject => TargetModule.GeneratedFileAlreadyExists($"{_metadataGenerationService.NamespaceByAssetKind(Kind)}", $"{_metadataGenerationService.AssetToCompilationUnitNameMapping[Kind]}");

        public string TargetProjectName => TargetModule.Name;

        private IEnumerable<string> _usings;
        public IEnumerable<string> Usings
        {
            get
            {
                if (_usings != null)
                {
                    return _usings;
                }

                _usings = UsingsBuilder.Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x).ToArray();
                return _usings;
            }
        }

        private IEnumerable<PropertyDefinitionModel> _availableProperties;
        public IEnumerable<PropertyDefinitionModel> DefinedProperties
        {
            get
            {
                if (_availableProperties != null)
                {
                    return _availableProperties;
                }

                _availableProperties = AvailablePropertiesBuilder.ToArray();
                return _availableProperties;
            }
        }

        private IEnumerable<ParameterDefinitionModel> _injectedDependencies;
        public IEnumerable<ParameterDefinitionModel> InjectedDependencies
        {
            get
            {
                if (_injectedDependencies != null)
                {
                    return _injectedDependencies;
                }

                _injectedDependencies = InjectedDependenciesBuilder.ToArray();
                return _injectedDependencies;
            }
        }

        private IEnumerable<ConstructorDefinitionModel> _constructors;
        public IEnumerable<ConstructorDefinitionModel> DefinedConstructors
        {
            get
            {
                if (_constructors != null)
                {
                    return _constructors;
                }

                _constructors = ConstructorsBuilder.ToArray();
                return _constructors;
            }
        }

        private IEnumerable<MethodDefinitionModel> _availableMethods;
        public IEnumerable<MethodDefinitionModel> DefinedMethods
        {
            get
            {
                if (_availableMethods != null)
                {
                    return _availableMethods;
                }

                _availableMethods = AvailableMethodsBuilder.ToArray();
                return _availableMethods;
            }
        }
        #endregion

        #region Internal Properties

        private PropertyDefinitionModel _baseEntityPrimaryKey;
        protected PropertyDefinitionModel BaseEntityPrimaryKey
        {
            get
            {
                lock (_locker)
                {
                    if (_baseEntityPrimaryKey != null)
                    {
                        return _baseEntityPrimaryKey;
                    }

                    _baseEntityPrimaryKey = _viewModelBase.EntityTree.First().FilterTreeProperties().FirstOrDefault(x => x.IsPrimaryKey) ?? new PropertyDefinitionModel
                    {
                        Name = $"{CommonTokens.Id}",
                        IsPrimaryKey = true,
                        ReturnType = "int"
                    };

                    return _baseEntityPrimaryKey;
                }
            }
        }

        protected virtual IProjectWrapper TargetModule => _viewModelBase.SelectedTargetModuleProject;
        #endregion

        #region Methods
        public async Task ExportAssetAsFile(string content)
        {
            await TargetModule.ExportFile
            (
                $"{_metadataGenerationService.NamespaceByAssetKind(Kind)}",
                $"{_metadataGenerationService.AssetToCompilationUnitNameMapping[Kind]}",
                content
            );
        }

        public async Task<CompilationUnitSyntax> LoadExistingAssetFromFile()
        {
            return await TargetModule.GetExistingFileClass
            (
                $"{_metadataGenerationService.NamespaceByAssetKind(Kind)}",
                $"{_metadataGenerationService.AssetToCompilationUnitNameMapping[Kind]}"
            );
        }
        #endregion
    }
}
