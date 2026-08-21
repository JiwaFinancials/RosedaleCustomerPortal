namespace JiwaCustomerPortal.Components.Grid.CellType
{
    public interface ICellType
    {
        public enum HAlignment
        {
            Left,
            Center,
            Right
        }
        
        public bool IsDrilldown { get; set; }
        public HAlignment HorizontalAlignment { get; set; }
    }
}
