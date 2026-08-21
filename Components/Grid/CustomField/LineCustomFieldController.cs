using JiwaCustomerPortal.Components.Grid.CellType;
using JiwaCustomerPortal.Components.Pages.SalesOrder;
using JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields;
using JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders;
using Microsoft.AspNetCore.Components;

namespace JiwaCustomerPortal.Components.Grid.CustomField
{
    /*
     * This is not to be confused with a traditional controller in the MVC sense. 
     * This is a controller for the LineCustomField component, which is used to manage the state and behavior of the component.
     */
    public class LineCustomFieldController<TCustomFieldPageHost, TCustomFieldValuesHost> where TCustomFieldPageHost : ICustomFieldPageHost where TCustomFieldValuesHost : ICustomFieldValuesHost
    {
        public Components.Grid.CellArray GridCells { get; set; }
        public List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> CustomFields { get; set; }

        public LineCustomFieldController(Components.Grid.CellArray GridCells, List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> CustomFields)
        {
            this.GridCells = GridCells;
            this.CustomFields = CustomFields;

            AddLineCustomFields();
        }

        private void AddLineCustomFields()
        {
            // Add the line custom fields
            foreach (JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField customField in CustomFields)
            {
                switch (customField.CellType)
                {
                    case CellTypes.Checkbox:
                        GridCells.Columns.Add(new Column($"{customField.PluginName}.{customField.SettingName}", new Components.Grid.CellType.CheckboxCellType(), customField.SettingName) { Width = 50 });
                        break;

                    case CellTypes.Combo:
                        ComboCellType comboCellType = new ComboCellType();
                        comboCellType.ComboKeyValuePairs = customField.ComboKeyValuePairs;
                        GridCells.Columns.Add(new Column($"{customField.PluginName}.{customField.SettingName}", comboCellType, customField.SettingName) { Width = 50 });
                        break;

                    case CellTypes.Date:
                        GridCells.Columns.Add(new Column($"{customField.PluginName}.{customField.SettingName}", new Components.Grid.CellType.DateCellType(), customField.SettingName) { Width = 50 });
                        break;

                    case CellTypes.Float:
                        DecimalCellType decimalCellType = new DecimalCellType();
                        decimalCellType.MinDecimalPlaces = customField.DecimalPlaces ?? 2;
                        decimalCellType.MaxDecimalPlaces = customField.DecimalPlaces ?? 2;
                        decimalCellType.MinValue = customField.DecimalMinValue ?? decimal.MinValue;
                        decimalCellType.MaxValue = customField.DecimalMaxValue ?? decimal.MaxValue;
                        GridCells.Columns.Add(new Column($"{customField.PluginName}.{customField.SettingName}", decimalCellType, customField.SettingName) { Width = 50 });
                        break;

                    case CellTypes.Integer:
                        IntegerCellType integerCellType = new IntegerCellType();
                        integerCellType.MinValue = customField.IntegerMinValue ?? int.MinValue;
                        integerCellType.MaxValue = customField.IntegerMaxValue ?? int.MaxValue;
                        GridCells.Columns.Add(new Column($"{customField.PluginName}.{customField.SettingName}", integerCellType, customField.SettingName) { Width = 50 });
                        break;

                    case CellTypes.Lookup:
                        GridCells.Columns.Add(new Column($"{customField.PluginName}.{customField.SettingName}", new Components.Grid.CellType.TextCellType(), customField.SettingName) { Width = 50 });
                        ButtonCellType lookupButton = new ButtonCellType();
                        lookupButton.ButtonType = ButtonCellType.ButtonTypes.Lookup;
                        GridCells.Columns.Add(new Column($"{customField.PluginName}.{customField.SettingName}.Button", lookupButton, customField.SettingName) { Width = 50 });
                        break;

                    case CellTypes.Text:
                        TextCellType textCellType = new TextCellType();
                        textCellType.MaxLength = customField.TextMaxLength ?? 255;
                        GridCells.Columns.Add(new Column($"{customField.PluginName}.{customField.SettingName}", textCellType, customField.SettingName) { Width = 50 });
                        break;
                }
            }
        }

        public async void DisplayCustomFieldValues(TCustomFieldPageHost PageHost, TCustomFieldValuesHost customFieldValuesHost, int row, bool isEditable)
        {
            foreach (JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField customField in CustomFields)
            {
                JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomFieldValue customFieldValue = customFieldValuesHost.CustomFieldValues?.Where(x => x.SettingID == customField.SettingID).FirstOrDefault();
                
                if (customField.CellType == CellTypes.Lookup)
                {
                    GridCells[$"{customField.PluginName}.{customField.SettingName}.Button", row].IsEditable = isEditable;                    

                    if (customFieldValue != null)
                    {
                        if (customFieldValue.DisplayValue == null)
                        {
                            customFieldValue.DisplayValue = await CustomFieldLookupDisplayResolver.ResolveCustomFieldDisplayValue(PageHost, customField, customFieldValue);
                        }                        
                        
                        GridCells[$"{customField.PluginName}.{customField.SettingName}", row].Value = customFieldValue.DisplayValue;
                    }
                    else
                    {
                        GridCells[$"{customField.PluginName}.{customField.SettingName}", row].Value = null;
                    }
                }
                else
                {
                    GridCells[$"{customField.PluginName}.{customField.SettingName}", row].IsEditable = isEditable;
                    if (customFieldValue != null)
                    {
                        GridCells[$"{customField.PluginName}.{customField.SettingName}", row].Value = customFieldValue.Contents;
                    }
                    else
                    {
                        GridCells[$"{customField.PluginName}.{customField.SettingName}", row].Value = null;
                    }
                }
            }
        }

        public async Task HandleCellChanged(TCustomFieldValuesHost customFieldValuesHost, Cell cell, ChangeEventArgs e, Func<TCustomFieldValuesHost, Cell, JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField, ChangeEventArgs, Task> SetLineCustomFieldValueMethod)
        {
            // see if the cell is a custom field cell            
            JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField customField = CustomFields.FirstOrDefault(x => $"{x.PluginName}.{x.SettingName}" == cell.Column.Id);

            if (customField != null)
            {                
                await SetLineCustomFieldValueMethod.Invoke(customFieldValuesHost, cell, customField, e);
            }
        }

        public async Task HandleCellButtonClicked(TCustomFieldPageHost PageHost, TCustomFieldValuesHost customFieldValuesHost, Cell cell)
        {
            // see if the cell is a custom field cell            
            JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField customField = CustomFields.FirstOrDefault(x => $"{x.PluginName}.{x.SettingName}.Button" == cell.Column.Id);

            if (customField != null)
            {
                CustomFieldLookupRenderFragmentGlobals customFieldLookupRenderFragmentGlobals = new CustomFieldLookupRenderFragmentGlobals() { CustomFieldLookupRenderFragment = PageHost.CustomFieldLookupRenderFragment, Host = PageHost, CustomField = customField, CustomFieldValuesHost = customFieldValuesHost };
                await CustomFieldLookupButtonClickHandler.CustomFieldLookupButtonClick(PageHost, customField, customFieldLookupRenderFragmentGlobals);
            }
        }
    }
}
