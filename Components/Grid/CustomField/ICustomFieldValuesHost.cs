using JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields;

namespace JiwaCustomerPortal.Components.Grid.CustomField
{
    public interface ICustomFieldValuesHost
    {
        public List<CustomFieldValue> CustomFieldValues { get; set; }        
    }
}
