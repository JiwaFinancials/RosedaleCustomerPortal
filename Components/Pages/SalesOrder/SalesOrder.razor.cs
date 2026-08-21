using JiwaCustomerPortal.Components.AutoQueryGrid.Inventory;
using JiwaCustomerPortal.Components.Grid;
using JiwaCustomerPortal.Components.Grid.CustomField;
using JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields;
using JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using ServiceStack;
using System.Runtime.CompilerServices;

namespace JiwaCustomerPortal.Components.Pages.SalesOrder
{
    public partial class SalesOrder : ICustomFieldPageHost
    {
        [Parameter]
        public string InvoiceId { get; set; }

        [SupplyParameterFromQuery(Name = "CreateForDebtorID")]
        public string CreateForDebtorID { get; set; }

        public string SnapshotNo { get; set; }

        // APIRequestInProgress cannot be simply set to true and restored to original state, due to race conditions arising from asynchronous
        // calls - so we use a counter instead, and increment or decrement that - and we look at the APIRequestInProgressCount to determine if a request is currently in progress or not.    
        private int _APIRequestInProgressCount = 0;
        public int APIRequestInProgressCount
        {
            get
            {
                return _APIRequestInProgressCount;
            }
            set
            {
                int previousValue = _APIRequestInProgressCount;
                _APIRequestInProgressCount = value;

                if ((previousValue > 0 && value == 0) || (previousValue == 0 && value > 0))
                {
                    // only trigger a render if we're changing from 0 to >0 or >0 to 0 - otherwise we don't need to trigger a render to show the APIRequestInProgress indicator
                    InvokeAsync(StateHasChanged);
                }
            }
        }
        public bool APIRequestInProgress => APIRequestInProgressCount > 0;
        private string? statusMessage;
        private JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrder salesOrder { get; set; }
        private string SelectedTabId { get; set; } = "Items-tab";
        private string CustomFieldsSelectedTabId { get; set; } = "OrderCustomFields-tab";
        private JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderHistory _SelectedHistory;
        private JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderHistory SelectedHistory
        {
            get
            {
                return _SelectedHistory;
            }
            set
            {
                _SelectedHistory = value;
                // whenever the selected history changes, we need to re-display everything - this occurs on read of sales order, not just selection of snapshot by the user
                InvokeAsync(DisplaySalesOrder);
            }
        }
        private JiwaInventoryAutoQueryModalSelectionDialog AddItemDialog { get; set; }
        private RenderFragment? _CustomFieldLookupRenderFragment;
        public RenderFragment? CustomFieldLookupRenderFragment
        {
            get
            {
                return _CustomFieldLookupRenderFragment;
            }
            set
            {
                _CustomFieldLookupRenderFragment = value;
                InvokeAsync(StateHasChanged);
            }
        }

        private Grid.Grid SalesOrderLinesGrid { get; set; }
        private Grid.CellArray SalesOrderLinesGridCells { get; set; }
        private Grid.Grid FreightGrid { get; set; }
        private Grid.CellArray FreightGridCells { get; set; }
        private Grid.Grid TotalsGrid { get; set; }
        private Grid.CellArray TotalsGridCells { get; set; }
        private CustomFieldGrid SalesOrderCustomFieldsGrid { get; set; }
        private Grid.CellArray SalesOrderCustomFieldsGridCells { get; set; }
        private CustomFieldGrid SalesOrderHistoryCustomFieldsGrid { get; set; }
        private Grid.CellArray SalesOrderHistoryCustomFieldsGridCells { get; set; }
        private Components.Grid.CustomField.LineCustomFieldController<JiwaCustomerPortal.Components.Pages.SalesOrder.SalesOrder, JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderLine> SalesOrderLineCustomFieldController { get; set; }
        public bool IsEditable
        {
            get
            {
                return salesOrder.Status == JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrder.SalesOrderStatuses.e_SalesOrderEntered && WebPortalUserSessionStateContainer.WebPortalUserSession?.AuthProvider == "credentials";
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (WebPortalUserSessionStateContainer.ProtectedLocalStore == null)
            {
                try
                {
                    await WebPortalUserSessionStateContainer.SetProtectedLocalStore(ProtectedLocalStore);
                }
                catch (Exception ex)
                {
                    statusMessage = ex.Message;
                    return;
                }
            }

            if (WebPortalUserSessionStateContainer.WebPortalUserSession == null)
            {
                // not authenticated
                NavigationManager.NavigateTo($"User/SignIn?returnUrl={NavigationManager.Uri}");
                return;
            }

            if (CreateForDebtorID != null)
            {
                // create a new Order, and then proceed to show that Order
                JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrderPOSTRequest salesOrderPOSTRequest = new JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrderPOSTRequest() { DebtorID = CreateForDebtorID };
                salesOrder = await SendToAPI<JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrder>(salesOrderPOSTRequest);
            }
            else if (InvoiceId != null)
            {
                JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrderGETRequest salesOrderGETRequest = new JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrderGETRequest() { InvoiceID = InvoiceId };
                salesOrder = await SendToAPI<JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrder>(salesOrderGETRequest);
            }

            if (salesOrder != null)
            {
                await InitialiseSalesOrderLinesGrid();
                await InitialiseFreightGrid();
                await InitialiseTotalsGrid();

                short snapno = 1;

                if (QueryHelpers.ParseQuery(NavigationManager.ToAbsoluteUri(NavigationManager.Uri).Query).TryGetValue("SnapshotNo", out var snapshotNoParam))
                {
                    snapno = short.Parse(snapshotNoParam);

                    if (snapno > salesOrder.Histories.Count)
                    {
                        snapno = 1;
                    }
                }

                SelectedHistory = salesOrder.Histories[snapno - 1];
            }
        }

        public async Task DisplaySalesOrder()
        {
            await DisplaySalesOrderLines();
            await DisplayFreight();
            await DisplayTotals();
        }

        #region "Sales Order Lines Grid"
        public Task InitialiseSalesOrderLinesGrid()
        {
            Components.Grid.CellType.DecimalCellType currencyCellType = new Components.Grid.CellType.DecimalCellType();
            currencyCellType.MinDecimalPlaces = Config.Currencies[salesOrder.CurrencyID].DecimalPlaces ?? 2;
            currencyCellType.MaxDecimalPlaces = Config.Currencies[salesOrder.CurrencyID].DecimalPlaces ?? 2;

            SalesOrderLinesGridCells = new Grid.CellArray();

            SalesOrderLinesGridCells.Columns.Add(new Column("InvoiceLineID", new Components.Grid.CellType.TextCellType(), "InvoiceLineID") { IsVisible = false, Width = 100 });
            SalesOrderLinesGridCells.Columns.Add(new Column("ItemNo", new Components.Grid.CellType.IntegerCellType() { MinValue = 1 }, "Item No.") { Width = 10 });
            SalesOrderLinesGridCells.Columns.Add(new Column("PartNo", new Components.Grid.CellType.TextCellType(), "Part No.") { Width = 50 });
            SalesOrderLinesGridCells.Columns.Add(new Column("Description", new Components.Grid.CellType.TextCellType(), "Description") { Width = 100 });
            SalesOrderLinesGridCells.Columns.Add(new Column("Unit", new Components.Grid.CellType.TextCellType(), "Unit") { Width = 50 });
            SalesOrderLinesGridCells.Columns.Add(new Column("QuantityOrdered", new Components.Grid.CellType.DecimalCellType() { MaxDecimalPlaces = 0 }, "Quantity Ordered") { Width = 50 });
            SalesOrderLinesGridCells.Columns.Add(new Column("QuantityPreviouslyShipped", new Components.Grid.CellType.DecimalCellType() { MaxDecimalPlaces = 0 }, "Quantity Previously Shipped") { Width = 50 });
            SalesOrderLinesGridCells.Columns.Add(new Column("QuantityThisShipment", new Components.Grid.CellType.DecimalCellType() { MaxDecimalPlaces = 0 }, "Quantity This Shipment") { Width = 50 });
            SalesOrderLinesGridCells.Columns.Add(new Column("QuantityBackordered", new Components.Grid.CellType.DecimalCellType() { MaxDecimalPlaces = 0 }, "Quantity Backordered") { Width = 50 });
            SalesOrderLinesGridCells.Columns.Add(new Column("FXDiscountedPrice", currencyCellType, "Price Ex") { Width = 50 });
            SalesOrderLinesGridCells.Columns.Add(new Column("TotalEx", currencyCellType, "Total Ex") { Width = 50 });
            SalesOrderLinesGridCells.Columns.Add(new Column("Tax", currencyCellType, "Tax") { Width = 50 });
            SalesOrderLinesGridCells.Columns.Add(new Column("TotalInc", currencyCellType, "Total Inc") { Width = 50 });

            if (IsEditable)
            {
                SalesOrderLinesGridCells.Columns.Add(new Column("Bin", new Components.Grid.CellType.ButtonCellType() { ButtonType = Components.Grid.CellType.ButtonCellType.ButtonTypes.Bin }, "") { Width = 10 });
            }

            // Add sales order line custom field controller             
            SalesOrderLineCustomFieldController = new Components.Grid.CustomField.LineCustomFieldController<JiwaCustomerPortal.Components.Pages.SalesOrder.SalesOrder, JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderLine>(SalesOrderLinesGridCells, (WebPortalUserSessionStateContainer.WebPortalUserSession?.AuthProvider == "credentials") ? Config.StaffLogin_SalesOrderLineCustomFields : Config.CustomerLogin_SalesOrderLineCustomFields);

            return Task.CompletedTask;
        }

        public async Task DisplaySalesOrderLines()
        {
            SalesOrderLinesGridCells.RowCount = 0;
            foreach (JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderLine salesOrderLine in SelectedHistory.Lines)
            {
                SalesOrderLinesGridCells.RowCount++;
                await DisplaySalesOrderLine(salesOrderLine, SalesOrderLinesGridCells.RowCount - 1);
            }
        }

        public Task DisplaySalesOrderLine(JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderLine salesOrderLine, int row = -1)
        {
            if (row == -1)
            {
                // find the row for this salesOrderLine
                row = SalesOrderLinesGridCells.RowFromCellKeyValue("InvoiceLineID", salesOrderLine.InvoiceLineID);
            }

            if (row != -1)
            {
                SalesOrderLinesGridCells["InvoiceLineID", row].Value = salesOrderLine.InvoiceLineID;
                SalesOrderLinesGridCells["ItemNo", row].Value = salesOrderLine.ItemNo;

                if (salesOrderLine.CommentLine ?? false)
                {
                    SalesOrderLinesGridCells["PartNo", row].Value = salesOrderLine.CommentText;
                    SalesOrderLinesGridCells["PartNo", row].IsEditable = IsEditable;
                    // set a cell span
                    SalesOrderLinesGridCells["PartNo", row].ColSpan = 10 + ((IsEditable) ? 1 : 0);

                    SalesOrderLinesGridCells["Bin", row].IsEditable = IsEditable;
                }
                else
                {
                    SalesOrderLinesGridCells["PartNo", row].Value = salesOrderLine.PartNo;
                    SalesOrderLinesGridCells["Description", row].Value = salesOrderLine.Description;
                    SalesOrderLinesGridCells["Unit", row].Value = (salesOrderLine.UnitOfMeasure != null) ? salesOrderLine.UnitOfMeasure.Name : salesOrderLine.SKUUnitName;

                    Components.Grid.CellType.DecimalCellType quantityCellType = new Components.Grid.CellType.DecimalCellType();
                    quantityCellType.MinValue = 0;
                    quantityCellType.MinDecimalPlaces = salesOrderLine.QuantityDecimalPlaces ?? 0;
                    quantityCellType.MaxDecimalPlaces = salesOrderLine.QuantityDecimalPlaces ?? 0;

                    SalesOrderLinesGridCells["QuantityOrdered", row].CellType = quantityCellType;
                    SalesOrderLinesGridCells["QuantityOrdered", row].Value = salesOrderLine.QuantityOrdered;
                    SalesOrderLinesGridCells["QuantityOrdered", row].IsEditable = IsEditable;

                    SalesOrderLinesGridCells["QuantityPreviouslyShipped", row].CellType = quantityCellType;
                    SalesOrderLinesGridCells["QuantityPreviouslyShipped", row].Value = salesOrderLine.QuantityPreviousDelivery;

                    SalesOrderLinesGridCells["QuantityThisShipment", row].CellType = quantityCellType;
                    SalesOrderLinesGridCells["QuantityThisShipment", row].Value = salesOrderLine.QuantityThisDel;

                    SalesOrderLinesGridCells["QuantityBackordered", row].CellType = quantityCellType;
                    SalesOrderLinesGridCells["QuantityBackordered", row].Value = salesOrderLine.QuantityBackOrd;

                    SalesOrderLinesGridCells["FXDiscountedPrice", row].Value = salesOrderLine.FXDiscountedPrice;
                    SalesOrderLinesGridCells["FXDiscountedPrice", row].IsEditable = IsEditable;

                    SalesOrderLinesGridCells["TotalEx", row].Value = salesOrderLine.FXLineTotal - salesOrderLine.FXTaxToCharge;
                    SalesOrderLinesGridCells["Tax", row].Value = salesOrderLine.FXTaxToCharge;
                    SalesOrderLinesGridCells["TotalInc", row].Value = salesOrderLine.FXLineTotal;

                    if (SalesOrderLinesGridCells.Columns["Bin"] != null)
                    {
                        SalesOrderLinesGridCells["Bin", row].IsEditable = IsEditable;
                    }

                    SalesOrderLineCustomFieldController.DisplayCustomFieldValues(this, salesOrderLine, row, IsEditable);
                }
            }

            return Task.CompletedTask;
        }

        public async void SalesOrderLinesGrid_CellButtonClicked(Cell cell)
        {
            string? invoiceLineID = SalesOrderLinesGridCells["InvoiceLineID", cell.Row].Value?.ToString();
            JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderLine? salesOrderLine = SelectedHistory.Lines.FirstOrDefault(x => x.InvoiceLineID == invoiceLineID);

            if (cell.Column.Id == "Bin")
            {
                if (salesOrderLine != null)
                {
                    await DeleteSalesOrderLine(salesOrderLine);
                }
            }
            else
            {
                if (salesOrderLine != null)
                {
                    // This may be a line custom field, call the controller and it will determine if it is a custom field and handle it accordingly                    
                    await SalesOrderLineCustomFieldController.HandleCellButtonClicked(this, salesOrderLine, cell);
                }
            }
        }

        public async Task SalesOrderLinesGrid_CellChanged((Cell cell, ChangeEventArgs e) args)
        {
            string? invoiceLineID = SalesOrderLinesGridCells["InvoiceLineID", args.cell.Row].Value?.ToString();
            JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderLine? salesOrderLine = SelectedHistory.Lines.FirstOrDefault(x => x.InvoiceLineID == invoiceLineID);

            if (salesOrderLine != null)
            {
                switch (args.cell.Column.Id)
                {
                    case "PartNo":
                        await SalesOrderLineChanged(args.e, salesOrderLine.InvoiceLineID, (dtoLine) =>
                        {
                            dtoLine.CommentText = args.e.Value.ToString();
                        });
                        break;

                    case "QuantityOrdered":
                        await SalesOrderLineChanged(args.e, salesOrderLine.InvoiceLineID, (dtoLine) =>
                        {
                            decimal value = 0;
                            decimal.TryParse(args.e.Value.ToString(), out value);
                            // We don't test the result of the decimal.TryPase - anything invalid is 0
                            dtoLine.QuantityOrdered = value;
                        });

                        break;
                    case "FXDiscountedPrice":
                        await SalesOrderLineChanged(args.e, salesOrderLine.InvoiceLineID, (dtoLine) =>
                        {
                            decimal value = 0;
                            decimal.TryParse(args.e.Value.ToString(), out value);
                            // We don't test the result of the decimal.TryPase - anything invalid is 0
                            dtoLine.FXDiscountedPrice = value;
                        });

                        break;
                    default:
                        // This may be a line custom field, call the controller and it will determine if it is a custom field and handle it accordingly                        
                        await SalesOrderLineCustomFieldController.HandleCellChanged(salesOrderLine, args.cell, args.e, async (host, cell, customField, e) =>
                        {
                            await SetLineCustomFieldValue(host, customField, e);
                        });
                        break;
                }
            }
        }
        #endregion

        #region "Freight Grid"

        public Task InitialiseFreightGrid()
        {
            Components.Grid.CellType.DecimalCellType currencyCellType = new Components.Grid.CellType.DecimalCellType();
            currencyCellType.MinDecimalPlaces = Config.Currencies[salesOrder.CurrencyID].DecimalPlaces ?? 2;
            currencyCellType.MaxDecimalPlaces = Config.Currencies[salesOrder.CurrencyID].DecimalPlaces ?? 2;

            FreightGridCells = new Grid.CellArray();

            FreightGridCells.Columns.Add(new Column("FreightLabel", new Components.Grid.CellType.TextCellType(), "") { Width = 8 });
            FreightGridCells.Columns.Add(new Column("ExTax", currencyCellType, "Ex Tax") { Width = 15 });
            FreightGridCells.Columns.Add(new Column("Tax", currencyCellType, "Tax") { Width = 10 });
            FreightGridCells.Columns.Add(new Column("IncTax", currencyCellType, "Inc Tax") { Width = 10 });

            FreightGridCells.RowCount = 3;

            FreightGridCells["FreightLabel", 0].Value = "Courier";
            FreightGridCells["FreightLabel", 1].Value = "Freight1";
            FreightGridCells["FreightLabel", 2].Value = "Freight2";

            return Task.CompletedTask;
        }

        public async Task FreightGrid_CellChanged((Cell cell, ChangeEventArgs e) args)
        {
            if (args.cell.Row == 0)
            {
                await SalesOrderChanged(args.e, (dtoSalesOrder) =>
                {
                    dtoSalesOrder.Cartage1ExGst = decimal.Parse(args.e.Value.ToString());
                });
            }
            else if (args.cell.Row == 1)
            {
                await SalesOrderChanged(args.e, (dtoSalesOrder) =>
                {
                    dtoSalesOrder.Cartage2ExGst = decimal.Parse(args.e.Value.ToString());
                });
            }
            else if (args.cell.Row == 2)
            {
                await SalesOrderChanged(args.e, (dtoSalesOrder) =>
                {
                    dtoSalesOrder.Cartage3ExGst = decimal.Parse(args.e.Value.ToString());
                });
            }
        }

        public Task DisplayFreight()
        {
            FreightGridCells["ExTax", 0].Value = salesOrder.FXCartage1ExGst;
            FreightGridCells["Tax", 0].Value = salesOrder.FXCartage1Gst;
            FreightGridCells["IncTax", 0].Value = salesOrder.FXCartage1ExGst + salesOrder.FXCartage1Gst;

            FreightGridCells["ExTax", 1].Value = salesOrder.FXCartage2ExGst;
            FreightGridCells["Tax", 1].Value = salesOrder.FXCartage2Gst;
            FreightGridCells["IncTax", 1].Value = salesOrder.FXCartage2ExGst + salesOrder.FXCartage2Gst;

            FreightGridCells["ExTax", 2].Value = salesOrder.FXCartage3ExGst;
            FreightGridCells["Tax", 2].Value = salesOrder.FXCartage3Gst;
            FreightGridCells["IncTax", 2].Value = salesOrder.FXCartage3ExGst + salesOrder.FXCartage3Gst;

            FreightGridCells["ExTax", 0].IsEditable = IsEditable;
            FreightGridCells["ExTax", 1].IsEditable = IsEditable;
            FreightGridCells["ExTax", 2].IsEditable = IsEditable;

            return Task.CompletedTask;
        }
        #endregion

        #region "Totals Grid"
        public Task InitialiseTotalsGrid()
        {
            Components.Grid.CellType.DecimalCellType currencyCellType = new Components.Grid.CellType.DecimalCellType();
            currencyCellType.MinDecimalPlaces = Config.Currencies[salesOrder.CurrencyID].DecimalPlaces ?? 2;
            currencyCellType.MaxDecimalPlaces = Config.Currencies[salesOrder.CurrencyID].DecimalPlaces ?? 2;

            TotalsGridCells = new Grid.CellArray();

            TotalsGridCells.Columns.Add(new Column("TotalLabel", new Components.Grid.CellType.TextCellType(), "Totals") { Width = 10 });
            TotalsGridCells.Columns.Add(new Column("ExTax", currencyCellType, "Ex Tax") { Width = 10 });
            TotalsGridCells.Columns.Add(new Column("Tax", currencyCellType, "Tax") { Width = 10 });
            TotalsGridCells.Columns.Add(new Column("IncTax", currencyCellType, "Inc Tax") { Width = 10 });

            TotalsGridCells.RowCount = 3;

            TotalsGridCells["TotalLabel", 0].Value = "Ordered";
            TotalsGridCells["TotalLabel", 1].Value = "Previous";
            TotalsGridCells["TotalLabel", 2].Value = "This Del.";

            return Task.CompletedTask;
        }

        public Task DisplayTotals()
        {
            TotalsGridCells["ExTax", 0].Value = salesOrder.FXOrderedExGSTTotal;
            TotalsGridCells["Tax", 0].Value = salesOrder.FXOrderedGSTTotal;
            TotalsGridCells["IncTax", 0].Value = salesOrder.FXOrderedExGSTTotal + salesOrder.FXOrderedGSTTotal;

            TotalsGridCells["ExTax", 1].Value = salesOrder.Histories.Where(x => x.HistoryNo < SelectedHistory.HistoryNo).Sum(x => x.FXExGSTTotal);
            TotalsGridCells["Tax", 1].Value = salesOrder.Histories.Where(x => x.HistoryNo < SelectedHistory.HistoryNo).Sum(x => x.FXGSTTotal);
            TotalsGridCells["IncTax", 1].Value = salesOrder.Histories.Where(x => x.HistoryNo < SelectedHistory.HistoryNo).Sum(x => x.FXIncGSTTotal);

            TotalsGridCells["ExTax", 2].Value = SelectedHistory.FXExGSTTotal;
            TotalsGridCells["Tax", 2].Value = SelectedHistory.FXGSTTotal;
            TotalsGridCells["IncTax", 2].Value = SelectedHistory.FXIncGSTTotal;

            return Task.CompletedTask;
        }
        #endregion

        #region "Custom Fields Grids"
        public async Task SalesOrderCustomFieldsGrid_CustomFieldChanged((JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField customField, Cell cell, ChangeEventArgs e) args)
        {
            await SetSalesOrderCustomFieldValue(args.customField, args.e);
        }

        public async Task SalesOrderHistoryCustomFieldsGrid_CustomFieldChanged((JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField customField, Cell cell, ChangeEventArgs e) args)
        {
            await SetSalesOrderHistoryCustomFieldValue(args.customField, args.e);
        }
        #endregion

        private Task OnSelectTab(string tabId)
        {
            SelectedTabId = tabId;

            return Task.CompletedTask;
        }

        private Task OnSelectCustomFieldsTab(string tabId)
        {
            CustomFieldsSelectedTabId = tabId;

            return Task.CompletedTask;
        }

        private Task SelectedHistoryChange(ChangeEventArgs e)
        {
            SelectedHistory = salesOrder.Histories.FirstOrDefault(x => x.InvoiceHistoryID == e.Value.ToString());

            return Task.CompletedTask;
        }

        public void NoAuthenticationToken()
        {
            // user had no auth token to provide - redirect to logon
            NavigationManager.NavigateTo($"User/SignIn?returnUrl={NavigationManager.Uri}");
        }

        public void APIException(Exception ex)
        {
            // first we need to work out what type of exception this was.  If it as a 401 (Not Authenticated), then we just redirect to to the login page
            // Anything else, we set the errorMessage property so the component displays the error to the user.
            if (ex is ServiceStack.WebServiceException)
            {
                ServiceStack.WebServiceException webServiceException = (ServiceStack.WebServiceException)ex;
                if (webServiceException.StatusCode == 401)
                {
                    // We are either not authenticated, or our token we have expired, go log on - but first clear any session info as it's no good no more.
                    WebPortalUserSessionStateContainer.SetWebPortalUserSession(null);
                    NavigationManager.NavigateTo($"User/SignIn?returnUrl={NavigationManager.Uri}");
                }
                else
                {
                    statusMessage = webServiceException.ErrorMessage;
                }
            }
            else
            {
                statusMessage = ex.Message;
            }
        }

        private async Task SalesOrderChanged(ChangeEventArgs e, Action<JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrderPATCHRequest> patchDTOAction)
        {
            JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrderPATCHRequest salesOrderPATCHRequest = new JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrderPATCHRequest();
            salesOrderPATCHRequest.InvoiceID = salesOrder.InvoiceID;
            patchDTOAction.Invoke(salesOrderPATCHRequest);

            int? currentSnap = SelectedHistory.HistoryNo;

            JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrder response = await SendToAPI<JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrder>(salesOrderPATCHRequest);
            if (response != null)
            {
                salesOrder = response;
                SelectedHistory = salesOrder.Histories[currentSnap.Value - 1];
            }
        }

        private async Task SalesOrderLineChanged(ChangeEventArgs e, string InvoiceLineID, Action<JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderLine> patchDTOAction)
        {
            // You may be wondering why do use salesOrderPATCHRequest instead of using the existing route to PATCH a sales order line
            // It's because we need to GET the whole sales order back in the response, not just the line as we have no idea
            // what business logic rules at the other side have done to the sales order
            // So we construct a salesOrderPATCHRequest with only the fields we are changing, but the response will contain the whole sales order with all changes applied and we can update our UI with that

            JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrderPATCHRequest salesOrderPATCHRequest = new JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrderPATCHRequest();
            salesOrderPATCHRequest.InvoiceID = salesOrder.InvoiceID;
            JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderLine OrderLine = new JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderLine();
            OrderLine.InvoiceLineID = InvoiceLineID;
            patchDTOAction.Invoke(OrderLine);
            salesOrderPATCHRequest.Lines = new List<JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderLine>();
            salesOrderPATCHRequest.Lines.Add(OrderLine);

            int? currentSnap = SelectedHistory.HistoryNo;

            JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrder response = await SendToAPI<JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrder>(salesOrderPATCHRequest);
            if (response != null)
            {
                salesOrder = response;
                SelectedHistory = salesOrder.Histories[currentSnap.Value - 1];
            }
        }

        private async Task DeleteSalesOrderLine(JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderLine salesOrderLine)
        {
            JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrderLineDELETERequest salesOrderLineDELETERequest = new JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrderLineDELETERequest();
            salesOrderLineDELETERequest.InvoiceID = salesOrder.InvoiceID;
            salesOrderLineDELETERequest.InvoiceHistoryID = SelectedHistory.InvoiceHistoryID;
            salesOrderLineDELETERequest.InvoiceLineID = salesOrderLine.InvoiceLineID;
            await DeleteFromAPI(salesOrderLineDELETERequest);

            // re-read the Order
            int? currentSnap = SelectedHistory.HistoryNo;
            JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrderGETRequest salesOrderGETRequest = new JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrderGETRequest() { InvoiceID = salesOrder.InvoiceID };
            salesOrder = await SendToAPI<JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrder>(salesOrderGETRequest);

            if (salesOrder != null)
            {
                SelectedHistory = salesOrder.Histories[currentSnap.Value - 1];
            }
            await InvokeAsync(StateHasChanged); // necessary otherwise the throbber does not go away for some reason.            
        }

        private async Task AddInventoryItem(JiwaFinancials.Jiwa.JiwaServiceModel.Tables.Or.v_Jiwa_Inventory_Item_ListOR InventoryItem)
        {
            // You may be wondering why do use salesOrderPATCHRequest instead of using the existing route to POST a new sales order line
            // It's because we need to GET the whole sales order back in the response, not just the line as we have no idea
            // what business logic rules at the other side have done to the sales order
            // So we construct a salesOrderPATCHRequest with only the fields we are changing, but the response will contain the whole sales order with all changes applied and we can update our UI with that

            AddItemDialog.Close(); // don't really need to close the dialog explicitly, as the dialog does this itself, but if we don't then we don't see the throbber during the API call below.

            if (InventoryItem != null)
            {
                JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrderPATCHRequest salesOrderPATCHRequest = new JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrderPATCHRequest();
                salesOrderPATCHRequest.InvoiceID = salesOrder.InvoiceID;
                JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderLine OrderLine = new JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderLine();
                OrderLine.InventoryID = InventoryItem.InventoryID;
                salesOrderPATCHRequest.Lines = new List<JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderLine>();
                salesOrderPATCHRequest.Lines.Add(OrderLine);

                int? currentSnap = SelectedHistory.HistoryNo;

                JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrder response = await SendToAPI<JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrder>(salesOrderPATCHRequest);
                if (response != null)
                {
                    salesOrder = response;
                    SelectedHistory = salesOrder.Histories[currentSnap.Value - 1];
                }
            }
        }

        private async Task AddSalesOrderCommentLine()
        {
            // You may be wondering why do use salesOrderPATCHRequest instead of using the existing route to POST a new comment line
            // It's because we need to GET the whole sales order back in the response, not just the comment line as we have no idea
            // what business logic rules at the other side have done to the sales order
            // So we construct a salesOrderPATCHRequest with only the fields we are changing, but the response will contain the whole sales order with all changes applied and we can update our UI with that

            JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrderPATCHRequest salesOrderPATCHRequest = new JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrderPATCHRequest();
            salesOrderPATCHRequest.InvoiceID = salesOrder.InvoiceID;
            JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderLine OrderLine = new JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderLine();
            OrderLine.CommentLine = true;
            salesOrderPATCHRequest.Lines = new List<JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderLine>();
            salesOrderPATCHRequest.Lines.Add(OrderLine);

            int? currentSnap = SelectedHistory.HistoryNo;

            JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrder response = await SendToAPI<JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrder>(salesOrderPATCHRequest);
            if (response != null)
            {
                salesOrder = response;
                SelectedHistory = salesOrder.Histories[currentSnap.Value - 1];
            }
        }

        #region "Custom Fields"
        public async Task SetCustomFieldValue(ICustomFieldValuesHost CustomFieldValuesHost, JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField CustomField, ChangeEventArgs e)
        {
            if (CustomFieldValuesHost is JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderLine)
            {
                await SetLineCustomFieldValue(CustomFieldValuesHost, CustomField, e);
            }
            else if (CustomFieldValuesHost is JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderHistory)
            {
                await SetSalesOrderHistoryCustomFieldValue(CustomField, e);
            }
            else if (CustomFieldValuesHost is JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrder)
            {
                await SetSalesOrderCustomFieldValue(CustomField, e);
            }
        }

        public async Task SetSalesOrderCustomFieldValue(JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField CustomField, ChangeEventArgs e)
        {
            // You may be wondering why do use salesOrderPATCHRequest instead of using the existing route to PATCH the custom field value
            // It's because we need to GET the whole sales order back in the response, not just the line as we have no idea
            // what business logic rules at the other side have done to the sales order
            // So we construct a salesOrderPATCHRequest with only the fields we are changing, but the response will contain the whole sales order with all changes applied and we can update our UI with that

            JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrderPATCHRequest salesOrderPATCHRequest = new JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrderPATCHRequest();
            salesOrderPATCHRequest.InvoiceID = salesOrder.InvoiceID;
            salesOrderPATCHRequest.CustomFieldValues = new List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomFieldValue>();
            JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomFieldValue customFieldValue = new JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomFieldValue();
            customFieldValue.SettingID = CustomField.SettingID;
            customFieldValue.Contents = e.Value.ToString();
            salesOrderPATCHRequest.CustomFieldValues.Add(customFieldValue);
            int? currentSnap = SelectedHistory.HistoryNo;
            JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrder response = await SendToAPI<JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrder>(salesOrderPATCHRequest);
            if (response != null)
            {
                salesOrder = response;
                SelectedHistory = salesOrder.Histories[currentSnap.Value - 1];
            }
        }

        public async Task SetSalesOrderHistoryCustomFieldValue(JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField CustomField, ChangeEventArgs e)
        {
            // You may be wondering why do use salesOrderPATCHRequest instead of using the existing route to PATCH the custom field value
            // It's because we need to GET the whole sales order back in the response, not just the line as we have no idea
            // what business logic rules at the other side have done to the sales order

            JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrderPATCHRequest salesOrderPATCHRequest = new JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrderPATCHRequest();
            salesOrderPATCHRequest.InvoiceID = salesOrder.InvoiceID;

            salesOrderPATCHRequest.Histories = new List<JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderHistory>();
            JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderHistory history = new JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderHistory() { InvoiceHistoryID = SelectedHistory.InvoiceHistoryID };
            salesOrderPATCHRequest.Histories.Add(history);

            history.CustomFieldValues = new List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomFieldValue>();

            JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomFieldValue customFieldValue = new JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomFieldValue();
            customFieldValue.SettingID = CustomField.SettingID;
            customFieldValue.Contents = e.Value.ToString();
            history.CustomFieldValues.Add(customFieldValue);

            int? currentSnap = SelectedHistory.HistoryNo;
            JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrder response = await SendToAPI<JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrder>(salesOrderPATCHRequest);
            if (response != null)
            {
                salesOrder = response;
                SelectedHistory = salesOrder.Histories[currentSnap.Value - 1];
            }
        }

        public async Task SetLineCustomFieldValue(ICustomFieldValuesHost CustomFieldValuesHost, JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField CustomField, ChangeEventArgs e)
        {
            // You may be wondering why do use salesOrderPATCHRequest instead of using the existing route to PATCH a sales order line custom field value
            // It's because we need to GET the whole sales order back in the response, not just the line as we have no idea
            // what business logic rules at the other side have done to the sales order
            // So we construct a salesOrderPATCHRequest with only the fields we are changing, but the response will contain the whole sales order with all changes applied and we can update our UI with that

            JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderLine SalesOrderLine = (JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderLine)CustomFieldValuesHost;

            JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrderPATCHRequest salesOrderPATCHRequest = new JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrderPATCHRequest();
            salesOrderPATCHRequest.InvoiceID = salesOrder.InvoiceID;
            salesOrderPATCHRequest.Lines = new List<JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderLine>();
            JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderLine line = new JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderLine() { InvoiceLineID = SalesOrderLine.InvoiceLineID };

            line.CustomFieldValues = new List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomFieldValue>();
            JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomFieldValue customFieldValue = new JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomFieldValue();
            customFieldValue.SettingID = CustomField.SettingID;
            customFieldValue.Contents = e.Value.ToString();
            line.CustomFieldValues.Add(customFieldValue);

            salesOrderPATCHRequest.Lines.Add(line);

            int? currentSnap = SelectedHistory.HistoryNo;
            JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrder response = await SendToAPI<JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrder>(salesOrderPATCHRequest);
            if (response != null)
            {
                salesOrder = response;
                SelectedHistory = salesOrder.Histories[currentSnap.Value - 1];
            }
        }
        #endregion

        private async Task DeleteFromAPI(IReturnVoid requestDTO)
        {
            APIRequestInProgressCount++;
            try
            {
                await JiwaAPI.DeleteAsync<ServiceStack.IReturnVoid>(requestDTO, WebPortalUserSessionStateContainer.WebPortalUserSession.Id, null);
            }
            catch (ServiceStack.WebServiceException webServiceException)
            {
                if (webServiceException.StatusCode == 401)
                {
                    // We are either not authenticated, or our token we have expired, go log on - but first clear any session info as it's no good no more.
                    WebPortalUserSessionStateContainer.SetWebPortalUserSession(null);
                    NavigationManager.NavigateTo($"User/SignIn?returnUrl={NavigationManager.Uri}");
                }
                else
                {
                    statusMessage = webServiceException.ErrorMessage;
                }
            }
            catch (Exception ex)
            {
                statusMessage = ex.Message;
            }
            finally
            {
                APIRequestInProgressCount--;
            }
        }

        public async Task<TResponse> SendToAPI<TResponse>(IReturn<TResponse> requestDTO)
        {
            APIRequestInProgressCount++;
            try
            {
                return await JiwaAPI.SendAsync(requestDTO, WebPortalUserSessionStateContainer.WebPortalUserSession.Id, null);
            }
            catch (ServiceStack.WebServiceException webServiceException)
            {
                if (webServiceException.StatusCode == 401)
                {
                    // We are either not authenticated, or our token we have expired, go log on - but first clear any session info as it's no good no more.
                    WebPortalUserSessionStateContainer.SetWebPortalUserSession(null);
                    NavigationManager.NavigateTo($"User/SignIn?returnUrl={NavigationManager.Uri}");
                }
                else
                {
                    statusMessage = webServiceException.ErrorMessage;
                }
            }
            catch (Exception ex)
            {
                statusMessage = ex.Message;
            }
            finally
            {
                APIRequestInProgressCount--;
            }

            return default(TResponse);
        }
    }
}
