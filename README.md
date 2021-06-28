# Getting Started

### Latest Release: [Download and Install the Extension from Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=Strongbytes.boilerplate-code-generator)
![image](https://user-images.githubusercontent.com/2210051/123601678-a08b1080-d800-11eb-9794-e81e31fc0462.png)


## Prerequisites: [Strongbytes ASP.NET Core modular architecture](https://github.com/Strongbytes/Knowledge-Spread)
* The extension is created to work together with this modular architecture, that is the base for every Strongbytes new project;
* You should be familiar with how [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html) works together with [Mediator Pattern](https://refactoring.guru/design-patterns/mediator) to reduce dependencies between objects and separate read and update operations for a data store. We use [MediatR](https://github.com/jbogard/MediatR) in our modular architecture to accomplish this requirement;
* You should be familiar with [Repository and Unit of Work Pattern](https://www.programmingwithwolfgang.com/repository-and-unit-of-work-pattern/). We implemented a Base Repository and a Base Unit of Work in the modular architecture, that can be easily extended;
* You should be familiar with [IoC](https://en.wikipedia.org/wiki/Inversion_of_control). We use [Autofac IoC](https://github.com/autofac/Autofac) as the IoC Container;
* We use [AutoMapper](https://github.com/AutoMapper/AutoMapper) to simplify object mapping;

## Usage:
##### Open your solution and right-click any existing model (eg. `LearningPath.cs`). Choose `Generate Boilerplate Code...` from the menu:
![image](https://user-images.githubusercontent.com/2210051/123590754-be05ad80-d7f3-11eb-80a7-d2be1b5a7097.png)
##### A new Window will appear, that contains 3 sections:
![image](https://user-images.githubusercontent.com/2210051/123591067-353b4180-d7f4-11eb-9821-e2534479fe8d.png)

### Entity Hierarchy Section
#### Contains a TreeView of the selected entity hierarchy (including all containing models and the base class). 
> ##### Inside this TreeView you can select which properties cab be used to generate the *"Domain Model"*
> ##### *Target Module* is used to specify which application module will contain all the generated files (except for Controller and DbContext);
> ##### *Controller Location* us used to specify which module is the main application module (the one where all Controllers are located);
> ##### You can specify if you want for the genrated CQRS files to use Unit of Work and Repository implementations (also generated by the extension). If this option is not selected, all CQRS classes will have the application DbContext as a dependency;
> ##### You can specify which Queries and which Commands should be generated;
> ##### Note: In order for "GetPaginated" operation to be available, the project has to implement `IPaginatedDataQuery` and `IPaginatedDataResponse<T>`;
> ![image](https://user-images.githubusercontent.com/2210051/123592068-6ff1a980-d7f5-11eb-8762-053f4cc76cd6.png)
> ##### After you fill all the required data, the `"Generate Code"` button is available.

### Directory Structure Section
#### Contains a TreeView of all the generated (or updated) classes;
#### All classes are mappend to the corresponding directories;
> ![image](https://user-images.githubusercontent.com/2210051/123592556-06be6600-d7f6-11eb-898a-281b857b33f6.png)
>
>
> #### Generated Files:
> ##### `LearningPathDomainModel.cs` is the generated class that will be sent to the client as the result for all Controller Requests. It contains all the selected properties from Entity Hierarchy. The same properties are used to generated models specific for CQRS Commands (in this case only for `CreateLearningPathRequestModel.cs`)
> ![image](https://user-images.githubusercontent.com/2210051/123593077-c57a8600-d7f6-11eb-9deb-e7232a558474.png)
>
> ##### `ApplicationDbContext.cs` will append (if doesn't already exist) the DbSet;
> ![image](https://user-images.githubusercontent.com/2210051/123594003-d1b31300-d7f7-11eb-9182-5371747d28f1.png)
> 
> ##### `LearningPathsController.cs` is the generated API Controller, that contains all the selected Queries and Commands;
> ![image](https://user-images.githubusercontent.com/2210051/123594120-feffc100-d7f7-11eb-9659-ea5ba07fe920.png)
>
> ##### `LearningPathsMapper.cs` contains all the generated object mappings. If the file does not exist, it will be created from scratch. If the file exists, the extension will find the existing mappings and will not replace them;
> ![image](https://user-images.githubusercontent.com/2210051/123594302-366e6d80-d7f8-11eb-9235-399c84bbf114.png)
>
> ##### `ILearningPathsRepository.cs` is the generated Repository Interface, that implements `IBaseRepository<LearningPath>`;
> ![image](https://user-images.githubusercontent.com/2210051/123594529-7d5c6300-d7f8-11eb-8e1f-1998f31d627a.png)
>
> ##### `LearningPathsRepository.cs` is the implementation of the `ILearningPathsRepository` interface, that inherits from `BaseRepository<LearningPath>`
> ![image](https://user-images.githubusercontent.com/2210051/123594846-daf0af80-d7f8-11eb-91e4-0081418f0c1d.png)
>
> ##### `IUnitOfWork.cs` is an interface that contains all the existing repositories (as interfaces). If the new generated repository (`ILearningPathsRepository`) does not exist as a property, will be appended. This interface implements `IBaseUnitOfWork`
> ![image](https://user-images.githubusercontent.com/2210051/123595013-125f5c00-d7f9-11eb-89d6-a8720e3f2085.png)
>
> ##### `UnitOfWork.cs` is the class that implements `IUnitOfWork` interface. 
> ![image](https://user-images.githubusercontent.com/2210051/123595182-46d31800-d7f9-11eb-9b25-d5401c00718e.png)
>
> ##### `LearningPathsModule.cs` is the Autofac module that registers all the services required for the *Target Module*. The extension will automatically register `ILearningPathsRepository`, `LearningPathsRepository`, `IUnitOfWork` and `UnitOfWork` (if not already registered);
> ![image](https://user-images.githubusercontent.com/2210051/123595477-9f0a1a00-d7f9-11eb-8328-8fa33ed20fb3.png)
> 
> ##### Commands `CreateLearningPathCommand.cs`, `DeleteLearningPathCommand.cs` and Queries `GetLearningPathByIdQuery.cs`, `GetPaginatedLearningPathsQuery.cs` implement `IRequest<>` from `MediatR`. Every class of this type has a specific CQRS Handler;
> ![image](https://user-images.githubusercontent.com/2210051/123596816-4dfb2580-d7fb-11eb-9277-7ec6dd22eaab.png)
> ![image](https://user-images.githubusercontent.com/2210051/123596853-55223380-d7fb-11eb-9b64-1337b26ebb76.png)
> 
> ##### All CQRS Handlers provide a `Handle()` method, where you will need to write your business logic. All handlers have the same dependencies (`IUnitOfWork` / `DbContext` for data access and `IMapper` for objects mapping)
> ![image](https://user-images.githubusercontent.com/2210051/123596925-68cd9a00-d7fb-11eb-9599-e22d85f5594f.png)

## Extensibility
### In order to add a new autogenerated class to the extension, folow these steps:
1. Create a new class that inherits `BaseGenericGeneratorModel` (eg. `EntityRepositoryInterfaceGeneratorModel.cs`). Make sure that your class ends with `GeneratorModel`, to match the naming guidelines for the other models;
2. Set the `Kind` property to a new enum value (eg. `EntityRepositoryInterface`), that needs to be added to `AssetKind` enum;
3. Go to `MetadataGenerationService` and add two new entries:
    > AssetToCompilationUnitNameMapping (the name of the generated asset), exaple:
    ```
    { AssetKind.EntityRepositoryInterface, $"{CommonTokens.I}{BaseEntityPluralizedName}{CommonTokens.Repository}" }
    ```
    >
    > AssetToNamespaceMapping (the containing namespace), example:
    ```
     {
           AssetKind.EntityRepositoryInterface,
           new NamespaceDefinitionModel
           {
               Content = $"{NamespaceTokens.Domain}.{NamespaceTokens.Repositories}",
           }
      }
      ```
4. Override `UsingsBuilder` and include all the required usings;
5. Override `CompilationUnitDefinition` in order to:
> #### Set Compilation Unit Type (choose between `class` or `interface` generation);
> #### Set Access Modifier (eg: `public`, `internal`);
> #### Set Class Attributes;
> #### Set Inheritance Types;
```
public override CompilationUnitDefinitionModel CompilationUnitDefinition => new CompilationUnitDefinitionModel
{
     Type = SyntaxKind.InterfaceDeclaration,
     DefinedInheritanceTypes = new string[]
     {
         $"{CommonTokens.IBaseRepository}<{_viewModelBase.EntityTree.PrimaryEntityName()}>"
     }
};
```


