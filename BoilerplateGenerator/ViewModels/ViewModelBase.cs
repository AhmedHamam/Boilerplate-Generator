﻿using BoilerplateGenerator.Collections;
using BoilerplateGenerator.Contracts.Generators;
using BoilerplateGenerator.Contracts.RoslynWrappers;
using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Models.Pagination;
using BoilerplateGenerator.Models.TreeView;
using BoilerplateGenerator.Services;
using Microsoft.VisualStudio.Shell;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Task = System.Threading.Tasks.Task;

namespace BoilerplateGenerator.ViewModels
{
    public class ViewModelBase : IViewModelBase
    {
        private readonly IEntityManagerService _fileManagerService;
        private readonly IGeneratorModelsManagerService _generatorModelsManagerService;

        public ViewModelBase(IEntityManagerService fileManagerService, IGeneratorModelsManagerService generatorModelsManagerService)
        {
            _fileManagerService = fileManagerService;
            _generatorModelsManagerService = generatorModelsManagerService;
            PaginationRequirements.PropertyChanged += (sender, eventArgs) => NotifyPropertyChanged(nameof(GetPaginatedQueryIsEnabled));
        }

        #region Properties

        #region CQRS Dependencies
        private bool _getAllQueryIsEnabled = true;
        public bool GetAllQueryIsEnabled
        {
            get
            {
                return _getAllQueryIsEnabled;
            }

            set
            {
                if (value == _getAllQueryIsEnabled)
                {
                    return;
                }

                _getAllQueryIsEnabled = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(GenerateCodeCommandIsEnabled));
            }
        }

        private bool _getByIdQueryIsEnabled = true;
        public bool GetByIdQueryIsEnabled
        {
            get
            {
                return _getByIdQueryIsEnabled;
            }

            set
            {
                if (value == _getByIdQueryIsEnabled)
                {
                    return;
                }

                _getByIdQueryIsEnabled = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(GenerateCodeCommandIsEnabled));
            }
        }

        private bool _getPaginatedQueryIsEnabled = true;
        public bool GetPaginatedQueryIsEnabled
        {
            get
            {
                
                return _getPaginatedQueryIsEnabled && PaginationRequirements.PaginationIsAvailable.HasValue;
            }

            set
            {
                if (value == _getPaginatedQueryIsEnabled)
                {
                    return;
                }

                _getPaginatedQueryIsEnabled = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(GenerateCodeCommandIsEnabled));
            }
        }

        private bool _createCommandIsEnabled = true;
        public bool CreateCommandIsEnabled
        {
            get
            {
                return _createCommandIsEnabled;
            }

            set
            {
                if (value == _createCommandIsEnabled)
                {
                    return;
                }

                _createCommandIsEnabled = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(GenerateCodeCommandIsEnabled));
            }
        }

        private bool _updateCommandIsEnabled = true;
        public bool UpdateCommandIsEnabled
        {
            get
            {
                return _updateCommandIsEnabled;
            }

            set
            {
                if (value == _updateCommandIsEnabled)
                {
                    return;
                }

                _updateCommandIsEnabled = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(GenerateCodeCommandIsEnabled));
            }
        }

        private bool _deleteCommandIsEnabled = true;
        public bool DeleteCommandIsEnabled
        {
            get
            {
                return _deleteCommandIsEnabled;
            }

            set
            {
                if (value == _deleteCommandIsEnabled)
                {
                    return;
                }

                _deleteCommandIsEnabled = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(GenerateCodeCommandIsEnabled));
            }
        }

        public bool GenerateCodeCommandIsEnabled => SelectedControllersProject != null && SelectedTargetModuleProject != null
                                                && (GetByIdQueryIsEnabled || GetAllQueryIsEnabled || GetPaginatedQueryIsEnabled || CreateCommandIsEnabled || UpdateCommandIsEnabled || DeleteCommandIsEnabled);
        #endregion

        private bool _useUnitOfWork = true;
        public bool UseUnitOfWork
        {
            get
            {
                return _useUnitOfWork;
            }

            set
            {
                if (value == _useUnitOfWork)
                {
                    return;
                }

                _useUnitOfWork = value;
                NotifyPropertyChanged();
            }
        }

        private Visibility _loaderVisibility = Visibility.Collapsed;
        public Visibility LoaderVisibility
        {
            get
            {
                return _loaderVisibility;
            }

            set
            {
                if (value == _loaderVisibility)
                {
                    return;
                }

                _loaderVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private string _referencedEntityName;
        public string ReferencedEntityName
        {
            get
            {
                return _referencedEntityName;
            }

            set
            {
                if (value == _referencedEntityName)
                {
                    return;
                }

                _referencedEntityName = value;
                NotifyPropertyChanged();
            }
        }

        private IProjectWrapper _selectedProject;
        public IProjectWrapper SelectedTargetModuleProject
        {
            get
            {
                return _selectedProject;
            }

            set
            {
                if (value == _selectedProject)
                {
                    return;
                }

                _selectedProject = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(GenerateCodeCommandIsEnabled));
            }
        }

        private IProjectWrapper _selectedControllersProject;
        public IProjectWrapper SelectedControllersProject
        {
            get
            {
                return _selectedControllersProject;
            }

            set
            {
                if (value == _selectedControllersProject)
                {
                    return;
                }

                _selectedControllersProject = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(GenerateCodeCommandIsEnabled));
            }
        }

        public ISolutionWrapper Solution { get; set; }

        public IPaginationRequirements PaginationRequirements { get; } = new PaginationRequirements();
        #endregion

        #region Collections
        public ObservableCollection<ITreeNode<IBaseSymbolWrapper>> EntityTree { get; set; } = new ObservableCollection<ITreeNode<IBaseSymbolWrapper>>();

        public ObservableCollection<IProjectWrapper> AvailableModules { get; set; } = new ObservableCollection<IProjectWrapper>();

        public ObservableCollection<ITreeNode<IBaseGeneratedAsset>> DirectoriesTree { get; set; } = new ObservableCollection<ITreeNode<IBaseGeneratedAsset>>();
        #endregion

        #region Commands
        private ICommand _viewEntityHierarchyCommand;
        public ICommand ViewEntityHierarchyCommand
        {
            get
            {
                if (_viewEntityHierarchyCommand != null)
                {
                    return _viewEntityHierarchyCommand;
                }

                _viewEntityHierarchyCommand = new CommandHandler(async (parameter) =>
                {
                    EntityTree.Clear();
                    DirectoriesTree.Clear();

                    await _fileManagerService.FindSelectedFileClassType().ConfigureAwait(false);

                    if (!_fileManagerService.IsEntityClassTypeValid)
                    {
                        return;
                    }

                    ITreeNode<IBaseSymbolWrapper> rootNode = await _fileManagerService.PopulateClassHierarchy();
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    EntityTree.Clear();
                    DirectoriesTree.Clear();
                    EntityTree.Add(rootNode);
                });

                return _viewEntityHierarchyCommand;
            }
        }

        private ICommand _generateCodeCommand;
        public ICommand GenerateCodeCommand
        {
            get
            {
                if (_generateCodeCommand != null)
                {
                    return _generateCodeCommand;
                }

                _generateCodeCommand = new CommandHandler(async (parameter) =>
                {
                    DirectoriesTree.Clear();

                    ITreeNode<IBaseGeneratedAsset> rootNode = new TreeNode<IBaseGeneratedAsset>
                    {
                        Current = new GeneratedDirectory(Solution.Name),
                    };

                    await Task.Run(async () =>
                    {
                        foreach (IGenericGeneratorModel availableModel in await _generatorModelsManagerService.RetrieveAvailableGeneratorModels().ConfigureAwait(false))
                        {
                            IGeneratedClass generatedClass = await new ClassGenerationService(availableModel).GetGeneratedClass().ConfigureAwait(false);
                            rootNode.GenerateDirectoryClassTree(generatedClass);
                        }
                    }).ConfigureAwait(false);

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    DirectoriesTree.Clear();
                    DirectoriesTree.Add(rootNode);
                });

                return _generateCodeCommand;
            }
        }

        private ICommand _exportGeneratedFilesCommand;
        public ICommand ExportGeneratedFilesCommand
        {
            get
            {
                if (_exportGeneratedFilesCommand != null)
                {
                    return _exportGeneratedFilesCommand;
                }

                _exportGeneratedFilesCommand = new CommandHandler(async (parameter) =>
                {
                    await DirectoriesTree.First().ExportGeneratedFiles().ConfigureAwait(false);

                    if (!(parameter is Window mainWindow))
                    {
                        return;
                    }

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    mainWindow.Close();
                });

                return _exportGeneratedFilesCommand;
            }
        }
        #endregion

        #region Business

        public async Task PopulateUiData()
        {
            ReferencedEntityName = await _fileManagerService.LoadSelectedEntityDetails();

            ViewEntityHierarchyCommand.Execute(null);

            Solution = await _fileManagerService.RetrieveSolution();

            foreach (IProjectWrapper item in _fileManagerService.RetrieveAllModules())
            {
                AvailableModules.Add(item);
            }

            await _fileManagerService.RetrievePaginationRequirements(PaginationRequirements);
        }
        #endregion

        #region Event Handlers
        public void ResetInterface()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
