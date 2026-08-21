namespace JiwaCustomerPortal.Components.Grid.CellType
{
    public class IntegerCellType : ICellType
    {                
        public bool IsDrilldown { get; set; }
        public ICellType.HAlignment HorizontalAlignment { get; set; } = ICellType.HAlignment.Right;
        public int MinValue { get; set; } = int.MinValue;
        public int MaxValue { get; set; } = int.MaxValue;
    }
}
