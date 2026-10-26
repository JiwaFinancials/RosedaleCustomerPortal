using Microsoft.AspNetCore.Components;
using ServiceStack;

namespace JiwaCustomerPortal.Components.Grid.CustomField
{
    public interface ICustomFieldPageHost
    {
        // CustomFieldLookupRenderFragment is the renderfragment of the Parent where a lookup dialog is to be rendered.
        public RenderFragment? CustomFieldLookupRenderFragment { get; set; }
        public Task<TResponse> SendToAPI<TResponse>(IReturn<TResponse> requestDTO);
        public Task SetCustomFieldValue(ICustomFieldValuesHost CustomFieldValuesHost, JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField CustomField, ChangeEventArgs e);
    }
}
