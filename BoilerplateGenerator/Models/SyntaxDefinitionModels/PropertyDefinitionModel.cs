﻿using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.RoslynWrappers;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.SyntaxDefinitionModels
{
    public partial class PropertyDefinitionModel
    {
        private readonly IDictionary<string, string> _lookupAttributes = new Dictionary<string, string>
        {
            { nameof(CommonTokens.Key), nameof(CommonTokens.Required) },
            { nameof(CommonTokens.Required), nameof(CommonTokens.Required) },
        };

        public string Name { get; set; }

        public ICollection<AttributeDefinitionModel> Attributes { get; } = new List<AttributeDefinitionModel>();

        public string ReturnType { get; set; }

        public bool IsPrimaryKey { get; set; }

        public IEnumerable<SyntaxKind> Modifiers { get; set; } = new SyntaxKind[] { SyntaxKind.PublicKeyword };

        public IEnumerable<PropertyAccessorDefinitionModel> Accessors { get; set; } = new PropertyAccessorDefinitionModel[]
        {
            new PropertyAccessorDefinitionModel
            {
                AccessorType = SyntaxKind.GetAccessorDeclaration,
            },
            new PropertyAccessorDefinitionModel
            {
                AccessorType = SyntaxKind.SetAccessorDeclaration,
            }
        };

        public PropertyDefinitionModel()
        {
        }

        public PropertyDefinitionModel(EntityPropertyWrapper entityPropertyWrapper, bool appendParentClassName)
        {
            ReturnType = entityPropertyWrapper.Type;
            IsPrimaryKey = entityPropertyWrapper.IsPrimaryKey;
            Name = GenerateName(entityPropertyWrapper, appendParentClassName);

            foreach (string attribute in from attribute in entityPropertyWrapper.Attributes
                                         where _lookupAttributes.Keys.Contains(attribute)
                                         select _lookupAttributes[attribute])
            {
                Attributes.Add(new AttributeDefinitionModel(attribute));
            }
        }

        private string GenerateName(EntityPropertyWrapper entityPropertyWrapper, bool appendParentClassName)
        {
            return appendParentClassName ? $"{entityPropertyWrapper.ParentName}{entityPropertyWrapper.Name}" : entityPropertyWrapper.Name;
        }
    }
}
