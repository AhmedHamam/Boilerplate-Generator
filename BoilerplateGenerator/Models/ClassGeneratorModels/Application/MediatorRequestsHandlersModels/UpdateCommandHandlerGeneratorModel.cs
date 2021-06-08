﻿using BoilerplateGenerator.Contracts;
using BoilerplateGenerator.Domain;
using BoilerplateGenerator.Models.Enums;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.Application.MediatorRequestsHandlersModels
{
    public class UpdateCommandHandlerGeneratorModel : BaseMediatorHandlerGeneratorModel
    {
        public UpdateCommandHandlerGeneratorModel(IViewModelBase viewModelBase, IMetadataGenerationService metadataGenerationService)
            : base(viewModelBase, metadataGenerationService)
        {
        }

        public override AssetKind GeneratedClassKind => AssetKind.UpdateCommandHandler;
    }
}
