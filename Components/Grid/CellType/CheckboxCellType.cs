namespace JiwaCustomerPortal.Components.Grid.CellType
{
    public class CheckboxCellType : ICellType
    {                
        public bool IsDrilldown { get; set; }
        public ICellType.HAlignment HorizontalAlignment { get; set; } = ICellType.HAlignment.Center;
    }
}
