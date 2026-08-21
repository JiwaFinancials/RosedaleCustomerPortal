namespace JiwaCustomerPortal.Components.Grid.CellType
{
    public class DateCellType : ICellType
    {                
        public bool IsDrilldown { get; set; }
        public ICellType.HAlignment HorizontalAlignment { get; set; } = ICellType.HAlignment.Left;
        public string DateFormat { get; set; } = "yyyy-MM-dd";
    }
}
