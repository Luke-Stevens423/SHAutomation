﻿using FlaUI.Core;
using FlaUI.UIA3.Elements;
using FlaUI.UIA3.Identifiers;
using UIA = interop.UIAutomationCore;

namespace FlaUI.UIA3.Patterns
{
    public class TableItemPattern : PatternBaseWithInformation<TableItemPatternInformation>
    {
        public static readonly PatternId Pattern = PatternId.Register(UIA.UIA_PatternIds.UIA_TableItemPatternId, "TableItem");
        public static readonly PropertyId ColumnHeaderItemsProperty = PropertyId.Register(UIA.UIA_PropertyIds.UIA_TableItemColumnHeaderItemsPropertyId, "ColumnHeaderItems");
        public static readonly PropertyId RowHeaderItemsProperty = PropertyId.Register(UIA.UIA_PropertyIds.UIA_TableItemRowHeaderItemsPropertyId, "RowHeaderItems");

        internal TableItemPattern(AutomationElement automationElement, UIA.IUIAutomationTableItemPattern nativePattern)
            : base(automationElement, nativePattern, (element, cached) => new TableItemPatternInformation(element, cached))
        {
        }

        public new UIA.IUIAutomationTableItemPattern NativePattern
        {
            get { return (UIA.IUIAutomationTableItemPattern)base.NativePattern; }
        }
    }

    public class TableItemPatternInformation : InformationBase
    {
        public TableItemPatternInformation(AutomationElement automationElement, bool cached)
            : base(automationElement, cached)
        {
        }

        public AutomationElement[] ColumnHeaderItems
        {
            get { return NativeElementArrayToElements(TableItemPattern.ColumnHeaderItemsProperty); }
        }

        public AutomationElement[] RowHeaderItems
        {
            get { return NativeElementArrayToElements(TableItemPattern.RowHeaderItemsProperty); }
        }
    }
}