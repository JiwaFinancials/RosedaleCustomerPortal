using JiwaCustomerPortal.Components.Pages.SalesOrder;
using Microsoft.AspNetCore.Components;

namespace JiwaCustomerPortal.Components.Grid.CustomField
{
    public class CustomFieldLookupDisplayResolverGlobals
    {
        public ICustomFieldPageHost Host { get; set; }
        public JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomFieldValue CustomFieldValue { get; set; }
    }
}
