﻿using BoilerplateGenerator.Extensions;
using BoilerplateGenerator.Models.Enums;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.RoslynWrappers
{
    public class EntityPropertyWrapper : BaseSymbolWrapper<IPropertySymbol>
    {
        public string Type { get; set; }

        public IEnumerable<string> Attributes { get; set; }

        public bool IsPrimaryKey => Attributes.Contains(nameof(CommonTokens.Key));

        public bool IsRequired => Attributes.Contains(nameof(CommonTokens.Required));

        public string ParentName { get; }

        public override bool IsEnabled => !IsPrimaryKey;

        public EntityPropertyWrapper(IPropertySymbol symbol) : base(symbol)
        {
            ParentName = symbol.ContainingSymbol.Name;
            Type = symbol.Type.ToTypeAlias();
            Attributes = symbol.GetAttributes().Select(x => x.AttributeClass.Name.Split(new string[] { nameof(CommonTokens.Attribute) }, StringSplitOptions.RemoveEmptyEntries).First()).ToArray();
        }
    }
}
