﻿using BoilerplateGenerator.Contracts.RoslynWrappers;
using Microsoft.CodeAnalysis;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BoilerplateGenerator.Models.RoslynWrappers
{
    public class BaseSymbolWrapper<T> : IBaseSymbolWrapper where T : ISymbol
    {
        public T ReferencedSymbol { get; }

        public string Name { get; set; }

        public string Namespace { get; set; }

        private bool? _isChecked;
        public bool? IsChecked
        {
            get
            {
                return _isChecked;
            }

            set
            {
                if (value == _isChecked)
                {
                    return;
                }

                if (!_isChecked.HasValue)
                {
                    value = true;
                }

                _isChecked = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsPropertyChanging { get; set; }

        public bool SymbolWasFound { get; set; }

        public virtual bool IsEnabled { get; }

        public BaseSymbolWrapper(T symbol)
        {
            ReferencedSymbol = symbol;
            Name = symbol?.Name;
            Namespace = symbol?.ContainingNamespace.ToString();
            SymbolWasFound = symbol != null;

            IsChecked = true;
            IsEnabled = true;
        }

        public BaseSymbolWrapper(string name)
        {
            Name = name;
            
            SymbolWasFound = false;
            IsChecked = true;
            IsEnabled = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
