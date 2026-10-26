namespace JiwaCustomerPortal.Components.Grid.CustomField
{
    public static class CustomFieldLookupButtonClickHandler
    {
        public static async Task CustomFieldLookupButtonClick(ICustomFieldPageHost PageHost, JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField CustomField, CustomFieldLookupRenderFragmentGlobals lookupScriptGlobals)
        {
            PageHost.CustomFieldLookupRenderFragment = null;

            if (CustomField.LookupButtonScript == null)
            {
                // example of what the fragment looks like for the Inventory lookup - a Jiwa plugin defines this fragment as a string,
                // and provides it to us in the CustomField.LookupProviders dictionary - and we can then compile and run that code to get the RenderFragment
                // to render the lookup dialog for that custom field.  This allows the plugin to define not only which dialog to show, but also what happens
                // when the user selects something in that dialog - in this case we set the value of the custom field to the InventoryID of the item they selected in the dialog.
                // This would be returned by the LookupProviders method of the IJiwaLookupCustomFieldPlugin implemention of the Jiwa plugin
                // @"CustomFieldLookupRenderFragment = async builder =>
                // {
                //     builder.OpenComponent(0, typeof(JiwaInventoryAutoQueryModalSelectionDialog));
                //     builder.AddAttribute(1, ""Title"", ""Select Item"");
                //     builder.AddAttribute(2, ""AutoOpen"", true);
                //     builder.AddAttribute(3, ""OnItemSelected"", EventCallback.Factory.Create<JiwaFinancials.Jiwa.JiwaServiceModel.Tables.Or.v_Jiwa_Inventory_Item_ListOR>(Host, async (i) =>
                //     {
                //         await Host.SetCustomFieldValue(CustomFieldValuesHost, CustomField, new ChangeEventArgs() { Value = i.InventoryID });
                //         CustomFieldLookupRenderFragment = null;
                //     }));
                //     builder.CloseComponent();
                // }";

                string? fragment = null;
                if (CustomField.LookupProviders != null && CustomField.LookupProviders.TryGetValue("Jiwa Customer Web Portal", out fragment))
                {
                    if (!string.IsNullOrWhiteSpace(fragment))
                    {
                        var scriptOptions = Microsoft.CodeAnalysis.Scripting.ScriptOptions.Default;
                        scriptOptions = scriptOptions.WithImports("System", "JiwaCustomerPortal", "JiwaCustomerPortal.Components", "JiwaCustomerPortal.Components.AutoQueryGrid", "JiwaCustomerPortal.Components.AutoQueryGrid.Inventory", "JiwaCustomerPortal.Components.AutoQueryGrid.Debtor", "Microsoft.AspNetCore.Components");
                        scriptOptions = scriptOptions.AddReferences(System.Reflection.Assembly.GetExecutingAssembly());
                        CustomField.LookupButtonScript = Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.Create(fragment, scriptOptions, globalsType: typeof(CustomFieldLookupRenderFragmentGlobals));
                        try
                        {
                            CustomField.LookupButtonScript.Compile();
                        }
                        catch (Exception compileException)
                        {
                            CustomField.LookupButtonClickScriptException = compileException;
                        }
                    }
                }
            }

            if (CustomField.LookupButtonScript != null)
            {
                Microsoft.CodeAnalysis.Scripting.ScriptState? result = null;                
                try
                {
                    result = await CustomField.LookupButtonScript.RunAsync(globals: lookupScriptGlobals);                    
                }
                catch (Exception runException)
                {
                    CustomField.LookupButtonClickScriptException = runException;
                }

                if (result != null && result.Exception != null)
                {
                    CustomField.LookupButtonClickScriptException = result.Exception;
                }
            }
        }
    }
}
