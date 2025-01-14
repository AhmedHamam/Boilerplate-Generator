﻿using BoilerplateGenerator.Models.RoslynWrappers;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.SyntaxDefinitionModels
{
    public class CompilationUnitDefinitionModel
    {
        public SyntaxKind AccessModifier { get; set; } = SyntaxKind.PublicKeyword;

        public SyntaxKind Type { get; set; } = SyntaxKind.ClassDeclaration;

        public IEnumerable<EntityClassWrapper> DefinedInheritanceTypes { get; set; } = Enumerable.Empty<EntityClassWrapper>();

        public IEnumerable<AttributeDefinitionModel> DefinedAttributes { get; set; } = Enumerable.Empty<AttributeDefinitionModel>();
    }
}
