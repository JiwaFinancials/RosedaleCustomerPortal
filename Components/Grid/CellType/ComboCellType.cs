namespace JiwaCustomerPortal.Components.Grid.CellType
{
    public class ComboCellType : ICellType
    {
        public bool IsDrilldown { get; set; }
        public ICellType.HAlignment HorizontalAlignment { get; set; } = ICellType.HAlignment.Center;
        public Dictionary<string, string> ComboKeyValuePairs { get; set; }
    }
}
