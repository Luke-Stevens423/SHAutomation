﻿using System;
using System.Xml;
using System.Xml.XPath;
using SHAutomation.Core.AutomationElements;
#if NET35
using SHAutomation.Core.Tools;
#endif

namespace SHAutomation.Core
{
    /// <summary>
    /// Custom implementation of a <see cref="XPathNavigator" /> which allows
    /// selecting items by xpath by using the <see cref="ITreeWalker" />.
    /// </summary>
    public class SHAutomationElementXPathNavigator : XPathNavigator
    {
        private const int NoAttributeValue = -1;
        private readonly SHAutomationElement _rootElement;
        private readonly ITreeWalker _treeWalker;
        private SHAutomationElement _currentElement;
        private int _attributeIndex = NoAttributeValue;

        public SHAutomationElementXPathNavigator(SHAutomationElement rootElement)
        {
            _treeWalker = rootElement.Automation.TreeWalkerFactory.GetControlViewWalker();
            _rootElement = rootElement;
            _currentElement = rootElement;
        }

        private bool IsInAttribute => _attributeIndex != NoAttributeValue;

        /// <inheritdoc />
        public override bool HasAttributes => !IsInAttribute;

        /// <inheritdoc />
        public override string Value => IsInAttribute ? GetAttributeValue(_attributeIndex) : _currentElement.ToString();

        /// <inheritdoc />
        public override object UnderlyingObject => _currentElement;

        /// <inheritdoc />
        public override XPathNodeType NodeType
        {
            get
            {
                if (IsInAttribute)
                {
                    return XPathNodeType.Attribute;
                }
                if (_currentElement.Equals(_rootElement))
                {
                    return XPathNodeType.Root;
                }
                return XPathNodeType.Element;
            }
        }

        /// <inheritdoc />
        public override string LocalName => IsInAttribute ? GetAttributeName(_attributeIndex) : _currentElement.Properties.ControlType.ValueOrDefault.ToString();

        /// <inheritdoc />
        public override string Name => LocalName;

        /// <inheritdoc />
        public override XmlNameTable NameTable => throw new NotImplementedException();

        /// <inheritdoc />
        public override string NamespaceURI => string.Empty;

        /// <inheritdoc />
        public override string Prefix => string.Empty;

        /// <inheritdoc />
        public override string BaseURI => string.Empty;

        /// <inheritdoc />
        public override bool IsEmptyElement => false;

        /// <inheritdoc />
        public override XPathNavigator Clone()
        {
            var clonedObject = new SHAutomationElementXPathNavigator(_rootElement)
            {
                _currentElement = _currentElement,
                _attributeIndex = _attributeIndex
            };
            return clonedObject;
        }

        /// <inheritdoc />
        public override bool MoveToFirstAttribute()
        {
            if (IsInAttribute)
            {
                return false;
            }
            _attributeIndex = 0;
            return true;
        }

        /// <inheritdoc />
        public override bool MoveToNextAttribute()
        {
            if (_attributeIndex >= Enum.GetNames(typeof(ElementAttributes)).Length - 1)
            {
                // No more attributes
                return false;
            }
            if (!IsInAttribute)
            {
                return false;
            }
            _attributeIndex++;
            return true;
        }

        /// <inheritdoc />
        public override string GetAttribute(string localName, string namespaceUri)
        {
            if (IsInAttribute)
            {
                return string.Empty;
            }
            var attributeIndex = GetAttributeIndexFromName(localName);
            if (attributeIndex != NoAttributeValue)
            {
                return GetAttributeValue(attributeIndex);
            }
            return string.Empty;
        }

        /// <inheritdoc />
        public override bool MoveToAttribute(string localName, string namespaceUri)
        {
            if (IsInAttribute)
            {
                return false;
            }
            var attributeIndex = GetAttributeIndexFromName(localName);
            if (attributeIndex != NoAttributeValue)
            {
                _attributeIndex = attributeIndex;
                return true;
            }
            return false;
        }

        /// <inheritdoc />
        public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope) => throw new NotImplementedException();

        /// <inheritdoc />
        public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope) => throw new NotImplementedException();

        /// <inheritdoc />
        public override void MoveToRoot()
        {
            _attributeIndex = NoAttributeValue;
            _currentElement = _rootElement;
        }

        /// <inheritdoc />
        public override bool MoveToNext()
        {
            if (IsInAttribute) { return false; }
            var nextElement = _treeWalker.GetNextSibling(_currentElement);
            if (nextElement == null)
            {
                return false;
            }
            _currentElement = nextElement;
            return true;
        }

        /// <inheritdoc />
        public override bool MoveToPrevious()
        {
            if (IsInAttribute) { return false; }
            var previousElement = _treeWalker.GetPreviousSibling(_currentElement);
            if (previousElement == null)
            {
                return false;
            }
            _currentElement = previousElement;
            return true;
        }

        /// <inheritdoc />
        public override bool MoveToFirstChild()
        {
            if (IsInAttribute) { return false; }
            var childElement = _treeWalker.GetFirstChild(_currentElement);
            if (childElement == null)
            {
                return false;
            }
            _currentElement = childElement;
            return true;
        }

        /// <inheritdoc />
        public override bool MoveToParent()
        {
            if (IsInAttribute)
            {
                _attributeIndex = NoAttributeValue;
                return true;
            }
            if (_currentElement.Equals(_rootElement))
            {
                return false;
            }
            _currentElement = _treeWalker.GetParent(_currentElement);
            return true;
        }

        /// <inheritdoc />
        public override bool MoveTo(XPathNavigator other)
        {
            var specificNavigator = other as SHAutomationElementXPathNavigator;
            if (specificNavigator == null)
            {
                return false;
            }
            if (!_rootElement.Equals(specificNavigator._rootElement))
            {
                return false;
            }
            _currentElement = specificNavigator._currentElement;
            _attributeIndex = specificNavigator._attributeIndex;
            return true;
        }

        /// <inheritdoc />
        public override bool MoveToId(string id)
        {
            return false;
        }

        /// <inheritdoc />
        public override bool IsSamePosition(XPathNavigator other)
        {
            var specificNavigator = other as SHAutomationElementXPathNavigator;
            if (specificNavigator == null)
            {
                return false;
            }
            if (!_rootElement.Equals(specificNavigator._rootElement))
            {
                return false;
            }
            return _currentElement.Equals(specificNavigator._currentElement)
                && _attributeIndex == specificNavigator._attributeIndex;
        }

        private string GetAttributeValue(int attributeIndex)
        {
            switch ((ElementAttributes)attributeIndex)
            {
                case ElementAttributes.AutomationId:
                    return _currentElement.Properties.AutomationId.ValueOrDefault;
                case ElementAttributes.Name:
                    return _currentElement.Properties.Name.ValueOrDefault;
                case ElementAttributes.ClassName:
                    return _currentElement.Properties.ClassName.ValueOrDefault;
                case ElementAttributes.HelpText:
                    return _currentElement.Properties.HelpText.ValueOrDefault;
                case ElementAttributes.ProcessId:
                    return _currentElement.Properties.ProcessId.ValueOrDefault.ToString();
                case ElementAttributes.ControlType:
                    return _currentElement.Properties.ControlType.ValueOrDefault.ToString();
                case ElementAttributes.LocalizedControlType:
                    return _currentElement.Properties.LocalizedControlType.ValueOrDefault;
                case ElementAttributes.Value:
                    return _currentElement.Properties.Value.ValueOrDefault;
                case ElementAttributes.IsEnabled:
                    return _currentElement.Properties.IsEnabled.ValueOrDefault.ToString();
                case ElementAttributes.IsOffscreen:
                    return _currentElement.Properties.IsOffscreen.ValueOrDefault.ToString();
                case ElementAttributes.HasKeyboardFocus:
                    return _currentElement.Properties.HasKeyboardFocus.ValueOrDefault.ToString();
                case ElementAttributes.IsKeyboardFocusable:
                    return _currentElement.Properties.IsKeyboardFocusable.ValueOrDefault.ToString();
                case ElementAttributes.IsBoundingRectangleEmpty:
                    return _currentElement.Properties.BoundingRectangle.ValueOrDefault.IsEmpty.ToString();
                default:
                    throw new ArgumentOutOfRangeException(nameof(attributeIndex));
            }
        }

        private string GetAttributeName(int attributeIndex)
        {
            var name = Enum.GetName(typeof(ElementAttributes), attributeIndex);
            if (name == null)
            {
                throw new ArgumentOutOfRangeException(nameof(attributeIndex));
            }
            return name;
        }

        private int GetAttributeIndexFromName(string attributeName)
        {
#if NET35
            if (EnumExtensions.TryParse(attributeName, out ElementAttributes parsedValue))
#else
            if (Enum.TryParse(attributeName, out ElementAttributes parsedValue))
#endif
            {
                return (int)parsedValue;
            }
            return NoAttributeValue;
        }

        private enum ElementAttributes
        {
            AutomationId,
            Name,
            ClassName,
            HelpText,
            ProcessId,
            ControlType,
            LocalizedControlType,
            Value,
            IsEnabled,
            IsOffscreen,
            HasKeyboardFocus,
            IsKeyboardFocusable,
            IsBoundingRectangleEmpty

        }
    }
}