namespace JiwaCustomerPortal.Components.Grid.CellType
{
    public class TextCellType : ICellType
    {                
        public bool IsDrilldown { get; set; }
        public ICellType.HAlignment HorizontalAlignment { get; set; } = ICellType.HAlignment.Left;
        public int MaxLength { get; set; } = 255;
    }
}
