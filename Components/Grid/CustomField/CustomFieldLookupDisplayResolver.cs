using JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields;

namespace JiwaCustomerPortal.Components.Grid.CustomField
{
    public static class CustomFieldLookupDisplayResolver
    {
        public static async Task<string> ResolveCustomFieldDisplayValue(ICustomFieldPageHost Host, JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField CustomField, JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomFieldValue CustomFieldValue)
        {
            // The purpose of this method is to resolve the display value for a custom field of type Lookup - we look to see if the plugin has provided a resolver for this custom field, and if so we execute the code in that resolver to get the display value.  If not, then we just return the contents as is.
            CustomField.LookupDisplayValueResolverScriptException = null;

            if (CustomField.LookupDisplayValueResolverScript == null)
            {
                string? fragment = null;
                if (CustomField.LookupDisplayValueResolvers != null && CustomField.LookupDisplayValueResolvers.TryGetValue("Jiwa Customer Web Portal", out fragment))
                {
                    if (!string.IsNullOrWhiteSpace(fragment))
                    {
                        // Example of a resolver fragment for resolving an InventoryID to PartNo - Description
                        // This would be returned by the LookupDisplayValueResolvers method of the IJiwaLookupCustomFieldPlugin implemention of the Jiwa plugin
                        // IN_MainQuery inMainQuery = new IN_MainQuery() { InventoryID = CustomFieldValue.Contents };
                        // var response = await SendToAPI<QueryResponse<IN_Main>>(inMainQuery);
                        // if (response.Results.Count == 1)
                        // {
                        //     CustomFieldValue.DisplayValue = $"{response.Results[0].PartNo} - {response.Results[0].Description}";
                        // }
                        // else
                        // {
                        //     // The provided inventoryID was not found, we use a blank display value for now.
                        //     CustomFieldValue.DisplayValue = "";
                        // }

                        // Compile the script and store it against the custom field - we only want to compile it once, and then we can just run it to get the display value each time after that without the overhead of compiling the code each time.
                        var scriptOptions = Microsoft.CodeAnalysis.Scripting.ScriptOptions.Default;
                        scriptOptions = scriptOptions.WithImports("System", "JiwaCustomerPortal", "JiwaCustomerPortal.Components", "Microsoft.AspNetCore.Components", "JiwaFinancials.Jiwa.JiwaServiceModel.Tables", "JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields", "ServiceStack");
                        scriptOptions = scriptOptions.AddReferences(System.Reflection.Assembly.GetExecutingAssembly());
                        CustomField.LookupDisplayValueResolverScript = Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.Create(fragment, scriptOptions, globalsType: typeof(CustomFieldLookupDisplayResolverGlobals));
                        try
                        {
                            CustomField.LookupDisplayValueResolverScript.Compile();
                        }
                        catch (Exception compileException)
                        {
                            CustomField.LookupDisplayValueResolverScriptException = compileException;
                        }
                    }
                }
            }

            if (CustomField.LookupDisplayValueResolverScript != null && CustomField.LookupDisplayValueResolverScriptException == null)
            {
                CustomFieldLookupDisplayResolverGlobals lookupGlobals = new CustomFieldLookupDisplayResolverGlobals() { Host = Host, CustomFieldValue = CustomFieldValue };
                Microsoft.CodeAnalysis.Scripting.ScriptState? result = null;
                
                try
                {
                    result = await CustomField.LookupDisplayValueResolverScript.RunAsync(globals: lookupGlobals);
                }
                catch (Exception runException)
                {
                    CustomField.LookupDisplayValueResolverScriptException = runException;
                }

                if (result != null && result.Exception != null)
                {
                    CustomField.LookupDisplayValueResolverScriptException = result.Exception;
                }
                return lookupGlobals.CustomFieldValue.DisplayValue;
            }

            return CustomFieldValue.Contents ?? string.Empty;
        }
    }
}
