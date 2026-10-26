namespace JiwaCustomerPortal.Components.Grid.CellType
{
    public class DecimalCellType : ICellType
    {                
        public bool IsDrilldown { get; set; }
        public ICellType.HAlignment HorizontalAlignment { get; set; } = ICellType.HAlignment.Right;
        public decimal MinValue { get; set; } = decimal.MinValue;
        public decimal MaxValue { get; set; } = decimal.MaxValue;
        public short MinDecimalPlaces { get; set; } = 2;
        public short MaxDecimalPlaces { get; set; } = 6;        
    }
}
