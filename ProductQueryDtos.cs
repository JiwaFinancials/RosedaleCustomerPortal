using ServiceStack;

namespace JiwaFinancials.Jiwa.JiwaServiceModel.Tables
{
    [Route("/Queries/InventoryItemList", "GET")]
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    public partial class v_Jiwa_Inventory_Item_ListQuery
        : QueryDb<JiwaFinancials.Jiwa.JiwaServiceModel.Tables.Or.v_Jiwa_Inventory_Item_ListOR>, IReturn<QueryResponse<JiwaFinancials.Jiwa.JiwaServiceModel.Tables.Or.v_Jiwa_Inventory_Item_ListOR>>
    {
        public virtual string InventoryID { get; set; }
        public virtual string PartNoContains { get; set; }
        public virtual string DescriptionContains { get; set; }
        public virtual string IN_LogicalID { get; set; }
        public virtual string Fields { get; set; }
        public virtual string OrderBy { get; set; }
        public virtual int? Take { get; set; }
        public virtual int? Skip { get; set; }
    }
}

namespace JiwaFinancials.Jiwa.JiwaServiceModel.Tables.Or
{
    [Route("/Queries/OR/InventoryItemList", "GET")]
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    public partial class v_Jiwa_Inventory_Item_List_OR_UnscopedQuery
        : v_Jiwa_Inventory_Item_ListORQuery, IReturn<QueryResponse<v_Jiwa_Inventory_Item_ListOR>>
    {
    }
}
