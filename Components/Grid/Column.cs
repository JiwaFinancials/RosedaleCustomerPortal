namespace JiwaCustomerPortal.Components.Grid
{
    public class Column
    {
        public string Id { get; set; }
        public string? Caption { get; set; }
        public CellType.ICellType CellType { get; set; }
        public bool IsHidden { get; set; }
        public bool IsVisible { get; set; }
        public bool IsEditable { get; set; }
        public bool IsDrilldown { get; set; }
        public int Width { get; set; }
        public int ColumnNo { get; set; }        
        
        public string Widthpx
        {
            get
            {
                return $"{Width}px";
            }
        }

        public Column(string id, CellType.ICellType cellType, string? caption = null)
        {            
            Id = id;
            Caption = caption;
            CellType = cellType;
            IsHidden = false;
            IsVisible = true;
            IsEditable = false;
            IsDrilldown = false;
            Width = 10;            
        }
    }
}
