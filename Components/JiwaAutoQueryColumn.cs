using JiwaCustomerPortal.Components;

namespace JiwaCustomerPortal
{
    public enum AuthTypes
    {
        JiwaAPISessionId,
        JiwaAPIKey
    }

    public class JiwaAutoQueryColumn<Model>
    {
        public string Id { get; set; }
        public int _DisplayOrder;
        public string Caption { get; set; }
        public bool IsHidden { get; set; }
        public Type ColumnDataType { get; set; }
        public List<JiwaAutoQueryColumnFilterOperator> FilterOperators { get; set; } = new List<JiwaAutoQueryColumnFilterOperator>();
        public List<JiwaAutoQueryColumnFilter> Filters { get; set; } = new List<JiwaAutoQueryColumnFilter>();

        public SortOrders? SortOrder { get; set; }

        public enum SortOrders
        {
            Ascending,
            Descending
        }

        public JiwaAutoQueryColumn(string Id)
        {
            this.Id = Id;
            IsHidden = true;
        }        

        public int DisplayOrder
        {
            get
            {
                return _DisplayOrder;
            }
            set
            {
                // TODO: renumber all displayorders 
                _DisplayOrder = value;
            }
        }
    }
}
