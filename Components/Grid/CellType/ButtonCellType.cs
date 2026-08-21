namespace JiwaCustomerPortal.Components.Grid.CellType
{
    public class ButtonCellType : ICellType
    {
        public enum ButtonTypes
        {
            Bin = 0,
            Lookup = 1,
            Edit = 2,
            Custom = 3
        }

        public bool IsDrilldown { get; set; }
        public ICellType.HAlignment HorizontalAlignment { get; set; } = ICellType.HAlignment.Center;
        public ButtonTypes ButtonType { get; set; }
    }
}
