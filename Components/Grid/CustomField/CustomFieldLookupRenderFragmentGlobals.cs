using JiwaCustomerPortal.Components.Pages.SalesOrder;
using Microsoft.AspNetCore.Components;

namespace JiwaCustomerPortal.Components.Grid.CustomField
{
    public class CustomFieldLookupRenderFragmentGlobals
    {
        private RenderFragment? _CustomFieldLookupRenderFragment;
        public RenderFragment? CustomFieldLookupRenderFragment 
        { 
            get
            {
                return _CustomFieldLookupRenderFragment;
            }
            set
            {
                _CustomFieldLookupRenderFragment = value;
                if (Host != null)
                {
                    Host.CustomFieldLookupRenderFragment = value;
                }                
            }
        }
        public ICustomFieldPageHost Host { get; set; }
        public JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField CustomField { get; set; }        

        public ICustomFieldValuesHost CustomFieldValuesHost { get; set; }
    }
}
