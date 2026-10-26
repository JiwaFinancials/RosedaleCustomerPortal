using JiwaCustomerPortal.Components.Grid.CellType;
using JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields;
using JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders;
using Microsoft.AspNetCore.Components;

namespace JiwaCustomerPortal.Components.Grid
{
    public partial class Grid : ComponentBase
    {
        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public CellArray Cells { get; set; }
        [Parameter]
        public EventCallback<(Cell, ChangeEventArgs)> CellChanged { get; set; }
        [Parameter]
        public EventCallback<Cell> CellButtonClicked { get; set; }

        [Parameter]
        public Func<Task> DisplayMethod { get; set; }

        public async Task OnCellValueChanged(Cell cell, ChangeEventArgs e)
        {
            var eventData = (cell, e);
            await CellChanged.InvokeAsync(eventData);
        }

        public async Task OnCellButtonClicked(Cell cell)
        {            
            await CellButtonClicked.InvokeAsync(cell);
        }               
                      
        public static string ClassHorizontalAlignment(Components.Grid.CellType.ICellType.HAlignment HorizontalAlignment)
        {
            switch (HorizontalAlignment)
            {
                case Components.Grid.CellType.ICellType.HAlignment.Left:
                    return "text-start";
                case Components.Grid.CellType.ICellType.HAlignment.Center:
                    return "text-center";
                case Components.Grid.CellType.ICellType.HAlignment.Right:
                    return "text-end";
                default:
                    return "text-start";
            }
        }

        public static string StyleHorizontalAlignment(Components.Grid.CellType.ICellType.HAlignment HorizontalAlignment)
        {
            switch (HorizontalAlignment)
            {
                case Components.Grid.CellType.ICellType.HAlignment.Left:
                    return "text-align: left;";
                case Components.Grid.CellType.ICellType.HAlignment.Center:
                    return "text-align: center;";
                case Components.Grid.CellType.ICellType.HAlignment.Right:
                    return "text-align: right;";
                default:
                    return "text-align: left;";
            }
        }
    }
}
