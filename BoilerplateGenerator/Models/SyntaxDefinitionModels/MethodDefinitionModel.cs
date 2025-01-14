﻿using BoilerplateGenerator.Models.Enums;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.SyntaxDefinitionModels
{
    public class MethodDefinitionModel
    {
        public string Name { get; set; }

        public bool IsEnabled { get; set; } = true;

        public MemberConflictResolutionKind MemberConflictResolutionKind { get; set; } = MemberConflictResolutionKind.Replace;

        public IEnumerable<ParameterDefinitionModel> Parameters { get; set; } = Enumerable.Empty<ParameterDefinitionModel>();

        public IEnumerable<AttributeDefinitionModel> Attributes { get; set; } = Enumerable.Empty<AttributeDefinitionModel>();

        public string ReturnType { get; set; } = $"{CommonTokens.Task}<{CommonTokens.IActionResult}> ";

        public SyntaxKind[] Modifiers { get; set; } = new SyntaxKind[] { SyntaxKind.PublicKeyword, SyntaxKind.AsyncKeyword };

        public IEnumerable<string> Body { get; set; } = Enumerable.Empty<string>();
    }
}
