using JiwaCustomerPortal.Components.AutoQueryGrid.Inventory;
using JiwaCustomerPortal.Components.Grid;
using JiwaCustomerPortal.Components.Grid.CustomField;
using JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using ServiceStack;
using System.Diagnostics;

namespace JiwaCustomerPortal.Components.Pages.SalesQuote
{
    public partial class SalesQuote : ICustomFieldPageHost
    {
        [Parameter]
        public string QuoteId { get; set; }

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
        private JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuote salesQuote { get; set; }
        private string SelectedTabId { get; set; } = "Items-tab";
        private JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuoteHistory _SelectedHistory;
        private JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuoteHistory SelectedHistory
        {
            get
            {
                return _SelectedHistory;
            }
            set
            {
                _SelectedHistory = value;
                // whenever the selected history changes, we need to re-display everything - this occurs on read of sales order, not just selection of snapshot by the user
                InvokeAsync(DisplaySalesQuote);
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

        private Grid.Grid SalesQuoteLinesGrid { get; set; }
        private Grid.CellArray SalesQuoteLinesGridCells { get; set; }
        private Grid.Grid FreightGrid { get; set; }
        private Grid.CellArray FreightGridCells { get; set; }
        private Grid.Grid TotalsGrid { get; set; }
        private Grid.CellArray TotalsGridCells { get; set; }
        private CustomFieldGrid SalesQuoteCustomFieldsGrid { get; set; }
        private Grid.CellArray SalesQuoteCustomFieldsGridCells { get; set; }
        private Components.Grid.CustomField.LineCustomFieldController<JiwaCustomerPortal.Components.Pages.SalesQuote.SalesQuote, JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuoteLine> SalesQuoteLineCustomFieldController { get; set; }

        public bool IsEditable
        {
            get
            {
                return salesQuote.Status == JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuote.e_SalesQuoteStatuses.e_SalesQuoteEntered && WebPortalUserSessionStateContainer.WebPortalUserSession?.AuthProvider == "credentials";
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
                // create a new quote, and then proceed to show that quote
                JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotePOSTRequest salesQuotePOSTRequest = new JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotePOSTRequest() { DebtorID = CreateForDebtorID };
                salesQuote = await SendToAPI<JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuote>(salesQuotePOSTRequest);
            }
            else if (QuoteId != null)
            {
                JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuoteGETRequest salesQuoteGETRequest = new JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuoteGETRequest() { QuoteID = QuoteId };
                salesQuote = await SendToAPI<JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuote>(salesQuoteGETRequest);
            }

            if (salesQuote != null)
            {
                await InitialiseSalesQuoteLinesGrid();
                await InitialiseFreightGrid();
                await InitialiseTotalsGrid();

                short snapno = 1;

                if (QueryHelpers.ParseQuery(NavigationManager.ToAbsoluteUri(NavigationManager.Uri).Query).TryGetValue("SnapshotNo", out var snapshotNoParam))
                {
                    snapno = short.Parse(snapshotNoParam);

                    if (snapno > salesQuote.Histories.Count)
                    {
                        snapno = 1;
                    }
                }

                SelectedHistory = salesQuote.Histories[snapno - 1];
            }
        }

        public async Task DisplaySalesQuote()
        {
            await DisplaySalesQuoteLines();
            await DisplayFreight();
            await DisplayTotals();
        }

        #region "Sales Quote Lines Grid"
        public Task InitialiseSalesQuoteLinesGrid()
        {
            Components.Grid.CellType.DecimalCellType currencyCellType = new Components.Grid.CellType.DecimalCellType();
            currencyCellType.MinDecimalPlaces = Config.Currencies[salesQuote.CurrencyID].DecimalPlaces ?? 2;
            currencyCellType.MaxDecimalPlaces = Config.Currencies[salesQuote.CurrencyID].DecimalPlaces ?? 2;

            SalesQuoteLinesGridCells = new Grid.CellArray();

            SalesQuoteLinesGridCells.Columns.Add(new Column("QuoteLineID", new Components.Grid.CellType.TextCellType(), "QuoteLineID") { IsVisible = false, Width = 100 });
            SalesQuoteLinesGridCells.Columns.Add(new Column("ItemNo", new Components.Grid.CellType.IntegerCellType() { MinValue = 1 }, "Item No.") { Width = 10 });
            SalesQuoteLinesGridCells.Columns.Add(new Column("PartNo", new Components.Grid.CellType.TextCellType(), "Part No.") { Width = 50 });
            SalesQuoteLinesGridCells.Columns.Add(new Column("Description", new Components.Grid.CellType.TextCellType(), "Description") { Width = 100 });
            SalesQuoteLinesGridCells.Columns.Add(new Column("Unit", new Components.Grid.CellType.TextCellType(), "Unit") { Width = 50 });
            SalesQuoteLinesGridCells.Columns.Add(new Column("QuantityOrdered", new Components.Grid.CellType.DecimalCellType() { MaxDecimalPlaces = 0 }, "Quantity") { Width = 50 });
            SalesQuoteLinesGridCells.Columns.Add(new Column("FXDiscountedPrice", currencyCellType, "Price Ex") { Width = 50 });
            SalesQuoteLinesGridCells.Columns.Add(new Column("TotalEx", currencyCellType, "Total Ex") { Width = 50 });
            SalesQuoteLinesGridCells.Columns.Add(new Column("Tax", currencyCellType, "Tax") { Width = 50 });
            SalesQuoteLinesGridCells.Columns.Add(new Column("TotalInc", currencyCellType, "Total Inc") { Width = 50 });

            if (IsEditable)
            {
                SalesQuoteLinesGridCells.Columns.Add(new Column("Bin", new Components.Grid.CellType.ButtonCellType() { ButtonType = Components.Grid.CellType.ButtonCellType.ButtonTypes.Bin }, "") { Width = 10 });
            }

            // Add sales quote line custom field controller            
            SalesQuoteLineCustomFieldController = new Components.Grid.CustomField.LineCustomFieldController<JiwaCustomerPortal.Components.Pages.SalesQuote.SalesQuote, JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuoteLine>(SalesQuoteLinesGridCells, (WebPortalUserSessionStateContainer.WebPortalUserSession?.AuthProvider == "credentials") ? Config.StaffLogin_SalesQuoteLineCustomFields : Config.CustomerLogin_SalesQuoteLineCustomFields);
        
            return Task.CompletedTask;
        }

        public async Task DisplaySalesQuoteLines()
        {
            SalesQuoteLinesGridCells.RowCount = 0;
            foreach (JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuoteLine salesQuoteLine in SelectedHistory.Lines)
            {
                SalesQuoteLinesGridCells.RowCount++;
                await DisplaySalesQuoteLine(salesQuoteLine, SalesQuoteLinesGridCells.RowCount - 1);
            }
        }

        public Task DisplaySalesQuoteLine(JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuoteLine salesQuoteLine, int row = -1)
        {
            if (row == -1)
            {
                // find the row for this salesQuoteLine
                row = SalesQuoteLinesGridCells.RowFromCellKeyValue("QuoteLineID", salesQuoteLine.QuoteLineID);
            }

            if (row != -1)
            {
                SalesQuoteLinesGridCells["QuoteLineID", row].Value = salesQuoteLine.QuoteLineID;
                SalesQuoteLinesGridCells["ItemNo", row].Value = salesQuoteLine.ItemNo;

                if (salesQuoteLine.CommentLine ?? false)
                {
                    SalesQuoteLinesGridCells["PartNo", row].Value = salesQuoteLine.CommentText;
                    SalesQuoteLinesGridCells["PartNo", row].IsEditable = IsEditable;
                    // set a cell span
                    SalesQuoteLinesGridCells["PartNo", row].ColSpan = 7 + ((IsEditable) ? 1 : 0);

                    SalesQuoteLinesGridCells["Bin", row].IsEditable = IsEditable;
                }
                else
                {
                    SalesQuoteLinesGridCells["PartNo", row].Value = salesQuoteLine.PartNo;
                    SalesQuoteLinesGridCells["Description", row].Value = salesQuoteLine.Description;
                    SalesQuoteLinesGridCells["Unit", row].Value = (salesQuoteLine.UnitOfMeasure != null) ? salesQuoteLine.UnitOfMeasure.Name : salesQuoteLine.SKUUnitName;

                    Components.Grid.CellType.DecimalCellType quantityCellType = new Components.Grid.CellType.DecimalCellType();
                    quantityCellType.MinValue = 0;
                    quantityCellType.MinDecimalPlaces = salesQuoteLine.QuantityDecimalPlaces ?? 0;
                    quantityCellType.MaxDecimalPlaces = salesQuoteLine.QuantityDecimalPlaces ?? 0;

                    SalesQuoteLinesGridCells["QuantityOrdered", row].CellType = quantityCellType;
                    SalesQuoteLinesGridCells["QuantityOrdered", row].Value = salesQuoteLine.QuantityOrdered;
                    SalesQuoteLinesGridCells["QuantityOrdered", row].IsEditable = IsEditable;

                    SalesQuoteLinesGridCells["FXDiscountedPrice", row].Value = salesQuoteLine.FXDiscountedPrice;
                    SalesQuoteLinesGridCells["FXDiscountedPrice", row].IsEditable = IsEditable;

                    SalesQuoteLinesGridCells["TotalEx", row].Value = salesQuoteLine.FXLineTotal - salesQuoteLine.FXTaxToCharge;
                    SalesQuoteLinesGridCells["Tax", row].Value = salesQuoteLine.FXTaxToCharge;
                    SalesQuoteLinesGridCells["TotalInc", row].Value = salesQuoteLine.FXLineTotal;

                    if (SalesQuoteLinesGridCells.Columns["Bin"] != null)
                    {
                        SalesQuoteLinesGridCells["Bin", row].IsEditable = IsEditable;
                    }

                    SalesQuoteLineCustomFieldController.DisplayCustomFieldValues(this, salesQuoteLine, row, IsEditable);
                }
            }
        
    return Task.CompletedTask;
}

        public async void SalesQuoteLinesGrid_CellButtonClicked(Cell cell)
        {
            string? invoiceLineID = SalesQuoteLinesGridCells["QuoteLineID", cell.Row].Value?.ToString();
            JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuoteLine? salesQuoteLine = SelectedHistory.Lines.FirstOrDefault(x => x.QuoteLineID == invoiceLineID);

            if (cell.Column.Id == "Bin")
            {
                if (salesQuoteLine != null)
                {
                    await DeleteSalesQuoteLine(salesQuoteLine);
                }
            }
            else
            {
                if (salesQuoteLine != null)
                {
                    // This may be a line custom field, call the controller and it will determine if it is a custom field and handle it accordingly                    
                    await SalesQuoteLineCustomFieldController.HandleCellButtonClicked(this, salesQuoteLine, cell);                                        
                }
            }
        }

        public async Task SalesQuoteLinesGrid_CellChanged((Cell cell, ChangeEventArgs e) args)
        {
            string? invoiceLineID = SalesQuoteLinesGridCells["QuoteLineID", args.cell.Row].Value?.ToString();
            JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuoteLine? salesQuoteLine = SelectedHistory.Lines.FirstOrDefault(x => x.QuoteLineID == invoiceLineID);

            if (salesQuoteLine != null)
            {
                switch (args.cell.Column.Id)
                {
                    case "PartNo":
                        await SalesQuoteLineChanged(args.e, salesQuoteLine.QuoteLineID, (dtoLine) =>
                        {
                            dtoLine.CommentText = args.e.Value.ToString();
                        });
                        break;

                    case "QuantityOrdered":
                        await SalesQuoteLineChanged(args.e, salesQuoteLine.QuoteLineID, (dtoLine) =>
                        {
                            decimal value = 0;
                            decimal.TryParse(args.e.Value.ToString(), out value);
                            // We don't test the result of the decimal.TryPase - anything invalid is 0
                            dtoLine.QuantityOrdered = value;
                        });

                        break;
                    case "FXDiscountedPrice":
                        await SalesQuoteLineChanged(args.e, salesQuoteLine.QuoteLineID, (dtoLine) =>
                        {
                            decimal value = 0;
                            decimal.TryParse(args.e.Value.ToString(), out value);
                            // We don't test the result of the decimal.TryPase - anything invalid is 0
                            dtoLine.FXDiscountedPrice = value;
                        });

                        break;
                    default:
                        // This may be a line custom field, call the controller and it will determine if it is a custom field and handle it accordingly                        
                        await SalesQuoteLineCustomFieldController.HandleCellChanged(salesQuoteLine, args.cell, args.e, async (host, cell, customField, e) =>
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
            currencyCellType.MinDecimalPlaces = Config.Currencies[salesQuote.CurrencyID].DecimalPlaces ?? 2;
            currencyCellType.MaxDecimalPlaces = Config.Currencies[salesQuote.CurrencyID].DecimalPlaces ?? 2;

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
                await SalesQuoteChanged(args.e, (dtoSalesQuote) =>
                {
                    dtoSalesQuote.Cartage1ExGst = decimal.Parse(args.e.Value.ToString());
                });
            }
            else if (args.cell.Row == 1)
            {
                await SalesQuoteChanged(args.e, (dtoSalesQuote) =>
                {
                    dtoSalesQuote.Cartage2ExGst = decimal.Parse(args.e.Value.ToString());
                });
            }
            else if (args.cell.Row == 2)
            {
                await SalesQuoteChanged(args.e, (dtoSalesQuote) =>
                {
                    dtoSalesQuote.Cartage3ExGst = decimal.Parse(args.e.Value.ToString());
                });
            }
        }

        public Task DisplayFreight()
        {
            FreightGridCells["ExTax", 0].Value = salesQuote.FXCartage1ExGst;
            FreightGridCells["Tax", 0].Value = salesQuote.FXCartage1Gst;
            FreightGridCells["IncTax", 0].Value = salesQuote.FXCartage1ExGst + salesQuote.FXCartage1Gst;

            FreightGridCells["ExTax", 1].Value = salesQuote.FXCartage2ExGst;
            FreightGridCells["Tax", 1].Value = salesQuote.FXCartage2Gst;
            FreightGridCells["IncTax", 1].Value = salesQuote.FXCartage2ExGst + salesQuote.FXCartage2Gst;

            FreightGridCells["ExTax", 2].Value = salesQuote.FXCartage3ExGst;
            FreightGridCells["Tax", 2].Value = salesQuote.FXCartage3Gst;
            FreightGridCells["IncTax", 2].Value = salesQuote.FXCartage3ExGst + salesQuote.FXCartage3Gst;

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
            currencyCellType.MinDecimalPlaces = Config.Currencies[salesQuote.CurrencyID].DecimalPlaces ?? 2;
            currencyCellType.MaxDecimalPlaces = Config.Currencies[salesQuote.CurrencyID].DecimalPlaces ?? 2;

            TotalsGridCells = new Grid.CellArray();

            TotalsGridCells.Columns.Add(new Column("TotalLabel", new Components.Grid.CellType.TextCellType(), "") { Width = 10 });
            TotalsGridCells.Columns.Add(new Column("ExTax", currencyCellType, "Ex Tax") { Width = 10 });
            TotalsGridCells.Columns.Add(new Column("Tax", currencyCellType, "Tax") { Width = 10 });
            TotalsGridCells.Columns.Add(new Column("IncTax", currencyCellType, "Inc Tax") { Width = 10 });

            TotalsGridCells.RowCount = 1;

            TotalsGridCells["TotalLabel", 0].Value = "Total";
        
            return Task.CompletedTask;
        }

        public Task DisplayTotals()
        {            
            TotalsGridCells["ExTax", 0].Value = (SelectedHistory.Lines.Sum(x => x.FXLineTotal) - SelectedHistory.Lines.Sum(x => x.FXTaxToCharge)) + SelectedHistory.CartageCharge1.FXExTaxAmount + SelectedHistory.CartageCharge2.FXExTaxAmount + SelectedHistory.CartageCharge3.FXExTaxAmount;
            TotalsGridCells["Tax", 0].Value = SelectedHistory.Lines.Sum(x => x.FXTaxToCharge) + SelectedHistory.CartageCharge1.FXTaxAmount + SelectedHistory.CartageCharge2.FXTaxAmount + SelectedHistory.CartageCharge3.FXTaxAmount;
            TotalsGridCells["IncTax", 0].Value = SelectedHistory.FXHistoryTotal;
        
            return Task.CompletedTask;
        }
        #endregion

        #region "Custom Fields Grids"
        public async Task SalesQuoteCustomFieldsGrid_CustomFieldChanged((JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField customField, Cell cell, ChangeEventArgs e) args)
        {
            await SetSalesQuoteCustomFieldValue(args.customField, args.e);
        }
        #endregion

        private Task OnSelectTab(string tabId)
        {
            SelectedTabId = tabId;
        
            return Task.CompletedTask;
        }

        private Task SelectedHistoryChange(ChangeEventArgs e)
        {
            SelectedHistory = salesQuote.Histories.FirstOrDefault(x => x.QuoteHistoryID == e.Value.ToString());
        
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

        private async Task SalesQuoteChanged(ChangeEventArgs e, Action<JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotePATCHRequest> patchDTOAction)
        {
            JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotePATCHRequest salesQuotePATCHRequest = new JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotePATCHRequest();
            salesQuotePATCHRequest.QuoteID = salesQuote.QuoteID;
            patchDTOAction.Invoke(salesQuotePATCHRequest);

            int? currentSnap = SelectedHistory.HistoryNo;

            JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuote response = await SendToAPI<JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuote>(salesQuotePATCHRequest);
            if (response != null)
            {
                salesQuote = response;
                SelectedHistory = salesQuote.Histories[currentSnap.Value - 1];
            }
        }

        private async Task SalesQuoteLineChanged(ChangeEventArgs e, string quoteLineID, Action<JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuoteLine> patchDTOAction)
        {
            // You may be wondering why do use salesQuotePATCHRequest instead of using the existing route to PATCH the quote line
            // It's because we need to GET the whole sales quote back in the response, not just the comment line as we have no idea
            // what business logic rules at the other side have done to the sales quote
            // So we construct a salesQuotePATCHRequest with only the fields we are changing, but the response will contain the whole sales quote with all changes applied and we can update our UI with that

            JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotePATCHRequest salesQuotePATCHRequest = new JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotePATCHRequest();
            salesQuotePATCHRequest.QuoteID = salesQuote.QuoteID;
            JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuoteLine quoteLine = new JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuoteLine();
            quoteLine.QuoteLineID = quoteLineID;
            patchDTOAction.Invoke(quoteLine);
            salesQuotePATCHRequest.Lines = new List<JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuoteLine>();
            salesQuotePATCHRequest.Lines.Add(quoteLine);

            int? currentSnap = SelectedHistory.HistoryNo;

            JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuote response = await SendToAPI<JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuote>(salesQuotePATCHRequest);
            if (response != null)
            {
                salesQuote = response;
                SelectedHistory = salesQuote.Histories[currentSnap.Value - 1];
            }
        }

        private async Task DeleteSalesQuoteLine(JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuoteLine salesQuoteLine)
        {
            JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuoteLineDELETERequest salesQuoteLineDELETERequest = new JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuoteLineDELETERequest();
            salesQuoteLineDELETERequest.QuoteID = salesQuote.QuoteID;
            salesQuoteLineDELETERequest.QuoteHistoryID = SelectedHistory.QuoteHistoryID;
            salesQuoteLineDELETERequest.QuoteLineID = salesQuoteLine.QuoteLineID;
            await DeleteFromAPI(salesQuoteLineDELETERequest);

            // re-read the quote
            int? currentSnap = SelectedHistory.HistoryNo;
            JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuoteGETRequest salesQuoteGETRequest = new JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuoteGETRequest() { QuoteID = salesQuote.QuoteID };
            salesQuote = await SendToAPI<JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuote>(salesQuoteGETRequest);

            if (salesQuote != null)
            {
                SelectedHistory = salesQuote.Histories[currentSnap.Value - 1];
                await InvokeAsync(StateHasChanged);  // required otherwise the UI doesn't update properly - indeterminate progress indicator just keeps spinning and the new line doesn't appear
            }
        }

        private async Task AddInventoryItem(JiwaFinancials.Jiwa.JiwaServiceModel.Tables.Or.v_Jiwa_Inventory_Item_ListOR InventoryItem)
        {
            // You may be wondering why do use salesQuotePATCHRequest instead of using the existing route to POST a new quote line
            // It's because we need to GET the whole sales quote back in the response, not just the comment line as we have no idea
            // what business logic rules at the other side have done to the sales quote
            // So we construct a salesQuotePATCHRequest with only the fields we are changing, but the response will contain the whole sales quote with all changes applied and we can update our UI with that

            AddItemDialog.Close(); // don't really need to close the dialog explicitly, as the dialog does this itself, but if we don't then we don't see the throbber during the API call below.

            if (InventoryItem != null)
            {
                JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotePATCHRequest salesQuotePATCHRequest = new JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotePATCHRequest();
                salesQuotePATCHRequest.QuoteID = salesQuote.QuoteID;
                JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuoteLine quoteLine = new JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuoteLine();
                quoteLine.InventoryID = InventoryItem.InventoryID;
                salesQuotePATCHRequest.Lines = new List<JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuoteLine>();
                salesQuotePATCHRequest.Lines.Add(quoteLine);

                int? currentSnap = SelectedHistory.HistoryNo;

                JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuote response = await SendToAPI<JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuote>(salesQuotePATCHRequest);
                if (response != null)
                {
                    salesQuote = response;
                    SelectedHistory = salesQuote.Histories[currentSnap.Value - 1];                    
                }
            }
        }

        private async Task AddSalesQuoteCommentLine()
        {
            // You may be wondering why do use salesQuotePATCHRequest instead of using the existing route to POST a new comment line
            // It's because we need to GET the whole sales quote back in the response, not just the comment line as we have no idea
            // what business logic rules at the other side have done to the sales quote
            // So we construct a salesQuotePATCHRequest with only the fields we are changing, but the response will contain the whole sales quote with all changes applied and we can update our UI with that

            JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotePATCHRequest salesQuotePATCHRequest = new JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotePATCHRequest();
            salesQuotePATCHRequest.QuoteID = salesQuote.QuoteID;
            JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuoteLine quoteLine = new JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuoteLine();
            quoteLine.CommentLine = true;
            salesQuotePATCHRequest.Lines = new List<JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuoteLine>();
            salesQuotePATCHRequest.Lines.Add(quoteLine);

            int? currentSnap = SelectedHistory.HistoryNo;

            JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuote response = await SendToAPI<JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuote>(salesQuotePATCHRequest);
            if (response != null)
            {
                salesQuote = response;
                SelectedHistory = salesQuote.Histories[currentSnap.Value - 1];
            }
        }

        #region "Custom Fields"
        public async Task SetCustomFieldValue(ICustomFieldValuesHost CustomFieldValuesHost, JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField CustomField, ChangeEventArgs e)
        {
            if (CustomFieldValuesHost is JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuoteLine)
            {
                await SetLineCustomFieldValue(CustomFieldValuesHost, CustomField, e);
            }
            else if (CustomFieldValuesHost is JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuote)
            {
                await SetSalesQuoteCustomFieldValue(CustomField, e);
            }
        }

        public async Task SetSalesQuoteCustomFieldValue(JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField CustomField, ChangeEventArgs e)
        {
            // You may be wondering why do use salesQuotePATCHRequest instead of using the existing route to PATCH the custom field value
            // It's because we need to GET the whole sales order back in the response, not just the line as we have no idea
            // what business logic rules at the other side have done to the sales order
            // So we construct a salesQuotePATCHRequest with only the fields we are changing, but the response will contain the whole sales order with all changes applied and we can update our UI with that

            JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotePATCHRequest salesQuotePATCHRequest = new JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotePATCHRequest();
            salesQuotePATCHRequest.QuoteID = salesQuote.QuoteID;
            salesQuotePATCHRequest.CustomFieldValues = new List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomFieldValue>();
            JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomFieldValue customFieldValue = new JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomFieldValue();
            customFieldValue.SettingID = CustomField.SettingID;
            customFieldValue.Contents = e.Value.ToString();
            salesQuotePATCHRequest.CustomFieldValues.Add(customFieldValue);
            int? currentSnap = SelectedHistory.HistoryNo;
            JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuote response = await SendToAPI<JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuote>(salesQuotePATCHRequest);
            if (response != null)
            {
                salesQuote = response;
                SelectedHistory = salesQuote.Histories[currentSnap.Value - 1];
            }
        }

        public async Task SetLineCustomFieldValue(ICustomFieldValuesHost CustomFieldValuesHost, JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField CustomField, ChangeEventArgs e)
        {
            // You may be wondering why do use salesQuotePATCHRequest instead of using the existing route to PATCH a sales order line custom field value
            // It's because we need to GET the whole sales order back in the response, not just the line as we have no idea
            // what business logic rules at the other side have done to the sales order
            // So we construct a salesQuotePATCHRequest with only the fields we are changing, but the response will contain the whole sales order with all changes applied and we can update our UI with that

            JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuoteLine SalesQuoteLine = (JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuoteLine)CustomFieldValuesHost;

            JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotePATCHRequest salesQuotePATCHRequest = new JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotePATCHRequest();
            salesQuotePATCHRequest.QuoteID = salesQuote.QuoteID;
            salesQuotePATCHRequest.Lines = new List<JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuoteLine>();
            JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuoteLine line = new JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuoteLine() { QuoteLineID = SalesQuoteLine.QuoteLineID };

            line.CustomFieldValues = new List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomFieldValue>();
            JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomFieldValue customFieldValue = new JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomFieldValue();
            customFieldValue.SettingID = CustomField.SettingID;
            customFieldValue.Contents = e.Value.ToString();
            line.CustomFieldValues.Add(customFieldValue);

            salesQuotePATCHRequest.Lines.Add(line);

            int? currentSnap = SelectedHistory.HistoryNo;
            JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuote response = await SendToAPI<JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuote>(salesQuotePATCHRequest);
            if (response != null)
            {
                salesQuote = response;
                SelectedHistory = salesQuote.Histories[currentSnap.Value - 1];
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
