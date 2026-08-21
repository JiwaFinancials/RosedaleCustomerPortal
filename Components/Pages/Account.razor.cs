using JiwaCustomerPortal.Components.AutoQueryGrid;
using JiwaCustomerPortal.Components.AutoQueryGrid.Inventory;
using JiwaCustomerPortal.Components.Grid;
using JiwaCustomerPortal.Components.Grid.CustomField;
using JiwaFinancials.Jiwa.JiwaServiceModel;
using JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using ServiceStack;
using System.Linq;

namespace JiwaCustomerPortal.Components.Pages
{
    public partial class Account
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        // APIRequestInProgress cannot be simply set to true and restored to original state, due to race conditions arising from asynchronous
        // calls - so we use a counter instead, and increment or decrement that - and we look at the APIRequestInProgressCount to determine if a request is currently in progress or not.
        private int APIRequestInProgressCount = 0;
        public bool APIRequestInProgress => APIRequestInProgressCount > 0;
        private string? statusMessage;
        private bool IsAdminRole => WebPortalUserSessionStateContainer?.WebPortalUserSession?.IsAdminRole == true;
        private JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.Debtor _Debtor;
        private JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.Debtor Debtor 
        { 
            get
            {
                return _Debtor;
            }
            set
            {
                _Debtor = value;
                // whenever the debtor changes, we need to re-display everything - this occurs on read of debtor, or when populating from the result of a patch
                InvokeAsync(DisplayAccount);
            }
        }
        private string SelectedTabId { get; set; } = "Contacts-tab";

        // For tabs which we lazy load, we keep track of if they've been clicked on with this list. The tab ID is added to the list when they click on it,
        // and we only bother rendering the AutoQuery or calling the API route backing that data when it is clicked the first time.
        private List<String> LazyLoadTabIds = new List<string>();

        private Grid.Grid ContactsGrid { get; set; }
        private Grid.CellArray ContactsGridCells { get; set; }
        private Grid.Grid DeliveryAddressesGrid { get; set; }
        private Grid.CellArray DeliveryAddressesGridCells { get; set; }
        private Grid.Grid BalancesGrid { get; set; }
        private Grid.CellArray BalancesGridCells { get; set; }        
        private Grid.Grid BackOrdersGrid { get; set; }
        private Grid.CellArray BackOrdersGridCells { get; set; }

        private JiwaFinancials.Jiwa.JiwaServiceModel.Tables.v_Jiwa_Debtor_Transactions_ListQuery DebtorsTransactionsAutoQuery { get; set; }
        private JiwaFinancials.Jiwa.JiwaServiceModel.Tables.v_Jiwa_SalesOrder_ListQuery SalesOrdersAutoQuery { get; set;  }
        private JiwaFinancials.Jiwa.JiwaServiceModel.Tables.v_Jiwa_SalesQuote_ListQuery SalesQuotesAutoQuery { get; set; }
        private List<JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.DebtorBackOrder> BackOrders { get; set; }

        private JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.DebtorContactName ContactNameToCreate { get; set; }
        private JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.DebtorContactName ContactNameToEdit { get; set; }    
        private JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.DebtorContactName ContactNameToDelete { get; set; }

        private string Period1Label { get; set; } = "Period 1";
        private string Period2Label { get; set; } = "Period 2";
        private string Period3Label { get; set; } = "Period 3";
        private string Period4Label { get; set; } = "Period 4";
        
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

            await InitialiseContactsGrid();
            await InitialiseDeliveryAddressesGrid();
            await InitialiseBalancesGrid();            
            await InitialiseBackOrdersGrid();            
        
            JiwaFinancials.Jiwa.JiwaServiceModel.CustomerDebtorGETRequest customerDebtorGETRequest = new JiwaFinancials.Jiwa.JiwaServiceModel.CustomerDebtorGETRequest();
            Debtor = await SendToAPI<JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.Debtor>(customerDebtorGETRequest);
        
            if (Debtor != null)
            {
                switch (Debtor.PeriodType)
                {
                    case JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.Debtor.PeriodTypes.Weekly:
                        Period1Label = "Current";
                        Period2Label = "8-14 Days";
                        Period3Label = "15-21 Days";
                        Period4Label = "21+ Days";
                        break;
        
                    case JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.Debtor.PeriodTypes.Fortnightly:
                        Period1Label = "Current";
                        Period2Label = "15-28 Days";
                        Period3Label = "29-42 Days";
                        Period4Label = "42+ Days";
                        break;
        
                    case JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.Debtor.PeriodTypes.Monthly:
                        Period1Label = "Current";
                        Period2Label = "31-60 Days";
                        Period3Label = "61-90 Days";
                        Period4Label = "90+ Days";
                        break;
        
                    case JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.Debtor.PeriodTypes.Custom:
                        Period1Label = "Current";
                        Period2Label = "Period 1";
                        Period3Label = "Period 2";
                        Period4Label = "Period 3";
                        break;
                }
            }
        
            // AutoQueries need to be a property instantiated in the parent and not new'd in the razor defining component because we want to persist the AutoQuery properties between renders.
            // If we don't do this, what happens is the component does not render propertly - so, for example
            // Where the debtor transactions autoquery component returns multiple pages, selecting page 3 and then clicking on a 
            // different tab and then back to the debtor transactions tab it will still show the transactions last displayed, but the page number will be back to page 1
            // So we instantiate the auto queries here, and in the component razor instantiation we set the AutoQuery parameter of the component to be our local declared instance here.
            DebtorsTransactionsAutoQuery = new JiwaFinancials.Jiwa.JiwaServiceModel.Tables.v_Jiwa_Debtor_Transactions_ListQuery()
                {
                    OrderByDesc = "TranDate",
                    Fields = "InvRemitNo,TranDate,Description,DueDate,CurrencyShortName,Ref,Remark,Note,SourceID,DecimalPlaces,CurrencyID,DebitCredit,DebitAmountIncTax,CreditAmountIncTax,GSTAmount,AllocatedAmount,OutstandingAmount",
                    Take = 10
                };        
        
            SalesOrdersAutoQuery = new JiwaFinancials.Jiwa.JiwaServiceModel.Tables.v_Jiwa_SalesOrder_ListQuery()
                {
                    OrderByDesc = "InvoiceNoDashHistoryNo",
                FXInvoiceTotalIncTaxNotEqualTo = 0,
                Fields = "InvoiceID,InvoiceNoDashHistoryNo,HistoryNo,InvoiceInitDate,OrderNo,SOReference,CurrencyShortName,FXInvoiceTotalIncTax,TotalAllocated,DueDate,CreditNote,CurrencyID",
                    Take = 10
                };
        
            SalesQuotesAutoQuery = new JiwaFinancials.Jiwa.JiwaServiceModel.Tables.v_Jiwa_SalesQuote_ListQuery()
                {
                    OrderByDesc = "InvoiceNoDashHistoryNo",
                FXInvoiceTotalIncTaxNotEqualTo = 0,
                Fields = "InvoiceID,InvoiceNoDashHistoryNo,HistoryNo,InvoiceInitDate,Status,OrderNo,QOReference,CurrencyShortName,FXInvoiceTotalIncTax,CurrencyID",
                    Take = 10
                };    
        }  

        private async Task OpenUrlInNewTab(string url)
        {
            await JSRuntime.InvokeVoidAsync("open", url, "_blank");
        }

        private async Task DisplayAccount()
        {
            await DisplayContacts();
            await DisplayDeliveryAddresses();
            await DisplayBalances();
        }

        private Task InitialiseContactsGrid()
        {
            ContactsGridCells = new Grid.CellArray();
            ContactsGridCells.Columns.Add(new Column("Name", new Components.Grid.CellType.TextCellType(), "Name") { Width = 40 });
            ContactsGridCells.Columns.Add(new Column("Email", new Components.Grid.CellType.TextCellType(), "Email") { Width = 40 });
            ContactsGridCells.Columns.Add(new Column("Mobile", new Components.Grid.CellType.TextCellType(), "Mobile") { Width = 20 });
            ContactsGridCells.Columns.Add(new Column("Tags", new Components.Grid.CellType.TextCellType(), "Tags") { Width = 60 });

            if (IsAdminRole)
            {
                ContactsGridCells.Columns.Add(new Column("Edit", new Components.Grid.CellType.ButtonCellType() { ButtonType = Components.Grid.CellType.ButtonCellType.ButtonTypes.Edit }, "Edit") { Width = 10 });
                ContactsGridCells.Columns.Add(new Column("Delete", new Components.Grid.CellType.ButtonCellType() { ButtonType = Components.Grid.CellType.ButtonCellType.ButtonTypes.Bin }, "Delete") { Width = 10 });
            }

            return Task.CompletedTask;
        }

        private async Task DisplayContacts()
        {
            if (Debtor?.ContactNames == null)
            {
                ContactsGridCells.RowCount = 0;
                return;
            }

            ContactsGridCells.RowCount = Debtor.ContactNames.Count;
            for (int row = 0; row < Debtor.ContactNames.Count; row++)
            {
                await DisplayContact(Debtor.ContactNames[row], row);
            }                
        }

        private Task DisplayContact(JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.DebtorContactName contactName, int row)
        {
            ContactsGridCells["Name", row].Value = contactName.DisplayName();
            ContactsGridCells["Email", row].Value = contactName.EmailAddress;
            ContactsGridCells["Mobile", row].Value = contactName.Mobile;

            ContactsGridCells["Tags", row].CustomRenderFragment = async builder =>
            {
                foreach (JiwaFinancials.Jiwa.JiwaServiceModel.Tags.Tag tag in contactName.TagMemberships)
                {
                    builder.OpenElement(0, "span");
                    builder.AddAttribute(1, "class", "badge rounded-pill bg-primary");
                    builder.AddContent(2, tag.Text);
                    builder.CloseElement();
                }                    
            };

            ContactsGridCells["Tags", row].Value = contactName.TagMemberships != null ? string.Join(", ", contactName.TagMemberships.Select(tag => tag.Text)) : null;

            if (ContactsGridCells.Columns["Edit"] != null)
            {
                ContactsGridCells["Edit", row].IsEditable = IsAdminRole;
            }

            if (ContactsGridCells.Columns["Delete"] != null)
            {
                ContactsGridCells["Delete", row].IsEditable = IsAdminRole;
            }
        
    return Task.CompletedTask;
}

        private async Task ContactsGrid_CellButtonClicked(Cell cell)
        {
            if (Debtor != null && cell.Row >= 0 && cell.Row < Debtor.ContactNames.Count)
            {
                JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.DebtorContactName contactName = Debtor.ContactNames[cell.Row];
                if (cell.Column.Id == "Edit")
                {
                    await EditContactName(contactName);
                }
                else if (cell.Column.Id == "Delete")
                {
                    await DeleteContactName(contactName);
                }
            }

            return;
        }

        private Task InitialiseDeliveryAddressesGrid()
        {
            DeliveryAddressesGridCells = new Grid.CellArray();
            DeliveryAddressesGridCells.Columns.Add(new Column("Address1", new Components.Grid.CellType.TextCellType(), "Address 1") { Width = 40 });
            DeliveryAddressesGridCells.Columns.Add(new Column("Address2", new Components.Grid.CellType.TextCellType(), "Address 2") { Width = 40 });
            DeliveryAddressesGridCells.Columns.Add(new Column("Address3", new Components.Grid.CellType.TextCellType(), "Suburb") { Width = 30 });
            DeliveryAddressesGridCells.Columns.Add(new Column("Address4", new Components.Grid.CellType.TextCellType(), "State") { Width = 20 });
            DeliveryAddressesGridCells.Columns.Add(new Column("Postcode", new Components.Grid.CellType.TextCellType(), "Postcode") { Width = 20 });
            DeliveryAddressesGridCells.Columns.Add(new Column("Country", new Components.Grid.CellType.TextCellType(), "Country") { Width = 20 });

            return Task.CompletedTask;
        }

        private Task DisplayDeliveryAddresses()
        {
            if (Debtor?.DeliveryAddresses == null)
            {
                DeliveryAddressesGridCells.RowCount = 0;
                return Task.CompletedTask;
            }

            DeliveryAddressesGridCells.RowCount = Debtor.DeliveryAddresses.Count;
            for (int row = 0; row < Debtor.DeliveryAddresses.Count; row++)
            {
                JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.DebtorDeliveryAddress deliveryAddress = Debtor.DeliveryAddresses[row];
                DeliveryAddressesGridCells["Address1", row].Value = deliveryAddress.Address1;
                DeliveryAddressesGridCells["Address2", row].Value = deliveryAddress.Address2;
                DeliveryAddressesGridCells["Address3", row].Value = deliveryAddress.Address3;
                DeliveryAddressesGridCells["Address4", row].Value = deliveryAddress.Address4;
                DeliveryAddressesGridCells["Postcode", row].Value = deliveryAddress.Postcode;
                DeliveryAddressesGridCells["Country", row].Value = deliveryAddress.Country;
            }

            return Task.CompletedTask;
        }

        private Task InitialiseBalancesGrid()
        {
            BalancesGridCells = new Grid.CellArray();
            BalancesGridCells.Columns.Add(new Column("Currency", new Components.Grid.CellType.TextCellType(), "Currency") { Width = 20 });
            BalancesGridCells.Columns.Add(new Column("Period1", new Components.Grid.CellType.DecimalCellType(), Period1Label) { Width = 15 });
            BalancesGridCells.Columns.Add(new Column("Period2", new Components.Grid.CellType.DecimalCellType(), Period2Label) { Width = 15 });
            BalancesGridCells.Columns.Add(new Column("Period3", new Components.Grid.CellType.DecimalCellType(), Period3Label) { Width = 15 });
            BalancesGridCells.Columns.Add(new Column("Period4", new Components.Grid.CellType.DecimalCellType(), Period4Label) { Width = 15 });
            BalancesGridCells.Columns.Add(new Column("Total", new Components.Grid.CellType.DecimalCellType(), "Total") { Width = 15 });

            return Task.CompletedTask;
        }

        private Task DisplayBalances()
        {
            if (Debtor?.Balances == null)
            {
                BalancesGridCells.RowCount = 0;
                return Task.CompletedTask;
            }

            BalancesGridCells.RowCount = Debtor.Balances.Count;
            for (int row = 0; row < Debtor.Balances.Count; row++)
            {
                JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.DebtorBalance balance = Debtor.Balances[row];
                BalancesGridCells["Currency", row].Value = balance.CurrencyShortName;

                BalancesGridCells["Currency", row].CustomRenderFragment = async builder =>
                {
                    builder.OpenElement(0, "img");
                    builder.AddAttribute(1, "src", $"data:image/png;base64,{Convert.ToBase64String(Config.Currencies[balance.CurrencyID].Picture)}");
                    builder.AddAttribute(2, "width", "30");
                    builder.AddAttribute(3, "height", "20");
                    builder.CloseElement();
                    builder.AddContent(4, $" {balance.CurrencyShortName}");
                };

                Components.Grid.CellType.DecimalCellType currencyCellType = new Components.Grid.CellType.DecimalCellType();
                currencyCellType.MinDecimalPlaces = balance.CurrencyDecimalPlaces ?? 0;
                currencyCellType.MaxDecimalPlaces = balance.CurrencyDecimalPlaces ?? 0;

                BalancesGridCells["Period1", row].CellType = currencyCellType;
                BalancesGridCells["Period2", row].CellType = currencyCellType;
                BalancesGridCells["Period3", row].CellType = currencyCellType;
                BalancesGridCells["Period4", row].CellType = currencyCellType;
                BalancesGridCells["Total", row].CellType = currencyCellType;

                BalancesGridCells["Period1", row].Value = balance.FXPeriod1;
                BalancesGridCells["Period2", row].Value = balance.FXPeriod2;
                BalancesGridCells["Period3", row].Value = balance.FXPeriod3;
                BalancesGridCells["Period4", row].Value = balance.FXPeriod4;
                BalancesGridCells["Total", row].Value = balance.FXTotal;
            }

            return Task.CompletedTask;
        }
        private async Task StatementsGrid_CellButtonClicked(Cell cell)
        {            
            await OpenUrlInNewTab($"StatementPDF/{cell.Column.Id}");
        }

        private Task InitialiseBackOrdersGrid()
        {
            Components.Grid.CellType.DateCellType dateCellType = new Components.Grid.CellType.DateCellType();
            dateCellType.DateFormat = BrowserService.DateFormat;

            BackOrdersGridCells = new Grid.CellArray();
            BackOrdersGridCells.Columns.Add(new Column("PartNo", new Components.Grid.CellType.TextCellType(), "Part No.") { Width = 20 });
            BackOrdersGridCells.Columns.Add(new Column("Description", new Components.Grid.CellType.TextCellType(), "Description") { Width = 60 });
            BackOrdersGridCells.Columns.Add(new Column("InvoiceNo", new Components.Grid.CellType.TextCellType(), "Invoice No.") { Width = 20 });
            BackOrdersGridCells.Columns.Add(new Column("CustomerOrderNo", new Components.Grid.CellType.TextCellType(), "Order No.") { Width = 20 });
            BackOrdersGridCells.Columns.Add(new Column("Date", dateCellType, "Date") { Width = 20 });
            BackOrdersGridCells.Columns.Add(new Column("Quantity", new Components.Grid.CellType.DecimalCellType(), "Quantity") { Width = 20 });
            BackOrdersGridCells.Columns.Add(new Column("ExpectedDeliveryDate", dateCellType, "ETA") { Width = 20 });
            BackOrdersGridCells.Columns.Add(new Column("Open", new Components.Grid.CellType.ButtonCellType() { ButtonType = Components.Grid.CellType.ButtonCellType.ButtonTypes.Lookup }, "") { Width = 10 });
            return Task.CompletedTask;
        }

        private Task DisplayBackOrders()
        {
            if (BackOrders == null)
            {
                BackOrdersGridCells.RowCount = 0;
                return Task.CompletedTask;
            }

            BackOrdersGridCells.RowCount = BackOrders.Count;
            for (int row = 0; row < BackOrders.Count; row++)
            {
                JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.DebtorBackOrder backorder = BackOrders[row];
                BackOrdersGridCells["PartNo", row].Value = backorder.PartNo;
                BackOrdersGridCells["Description", row].Value = backorder.Description;
                BackOrdersGridCells["InvoiceNo", row].Value = backorder.InvoiceNo;
                BackOrdersGridCells["CustomerOrderNo", row].Value = backorder.CustomerOrderNo;
                BackOrdersGridCells["Date", row].Value = backorder.Date;

                Components.Grid.CellType.DecimalCellType quantityCellType = new Components.Grid.CellType.DecimalCellType();
                quantityCellType.MinValue = 0;
                quantityCellType.MinDecimalPlaces = backorder.QuantityDecimalPlaces;
                quantityCellType.MaxDecimalPlaces = backorder.QuantityDecimalPlaces;

                BackOrdersGridCells["Quantity", row].CellType = quantityCellType;
                BackOrdersGridCells["Quantity", row].Value = backorder.Quantity;

                BackOrdersGridCells["ExpectedDeliveryDate", row].Value = backorder.ExpectedDeliveryDate;
                BackOrdersGridCells["Open", row].IsEditable = true;
            }

            return Task.CompletedTask;
        }

        private async Task BackOrdersGrid_CellButtonClicked(Cell cell)
        {
            if (BackOrders != null && cell.Row >= 0 && cell.Row < BackOrders.Count)
            {
                await OpenUrlInNewTab($"SalesOrder/{BackOrders[cell.Row].InvoiceID}");
            }
        }

        private async Task ReadBackOrders()
        {        
            if (Debtor != null)
            {
                JiwaFinancials.Jiwa.JiwaServiceModel.Tables.DebtorBackordersGETRequest DebtorBackordersGETRequest = new JiwaFinancials.Jiwa.JiwaServiceModel.Tables.DebtorBackordersGETRequest();
                DebtorBackordersGETRequest.DebtorID = Debtor.DebtorID;
                BackOrders = await SendToAPI<List<JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.DebtorBackOrder>>(DebtorBackordersGETRequest);
                await DisplayBackOrders();                
            }
        }
        
        private async Task OnSelectTab(string tabId)
        {
            SelectedTabId = tabId;
        
            if (!LazyLoadTabIds.Contains(tabId))
            {
                if (tabId == "Backorders-tab")
                {
                    await ReadBackOrders();
                }
        
                LazyLoadTabIds.Add(tabId);
            }
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
        
        public RenderFragment TransactionsGridDataCellRenderFragment(JiwaFinancials.Jiwa.JiwaServiceModel.Tables.v_Jiwa_Debtor_Transactions_List item, string columnId)
        {
            string? markup = null;
            switch (columnId)
            {
                case "InvRemitNo":
                    if (item.Description == "Sales Orders")
                    {
                        markup = $"<td><a href='SalesOrder/{item.SourceID.Trim()}?SnapshotNo={GetSnapshotNoFromInvoiceNoDashDocketNumHeaderHistoryNo(item.InvRemitNo)}'>{System.Net.WebUtility.HtmlEncode(item.InvRemitNo)}</a></td>";
                    }
                    else
                    {
                        markup = $"<td>{System.Net.WebUtility.HtmlEncode(item.InvRemitNo)}</td>";
                    }
                    break;

                case "Description":
                    if (item.Description == "Bank Receipts")
                    {
                        markup = "<td class='text-center'><span class='badge rounded-pill bg-success'>Payment</span></td>";
                    }
                    else if (item.Description == "Debtor Adjustments")
                    {
                        markup = "<td class='text-center'><span class='badge rounded-pill bg-warning'>Adjustment</span></td>";
                    }
                    else if (item.Description == "Sales Orders")
                    {
                        markup = "<td class='text-center'><span class='badge rounded-pill bg-secondary'>Invoice</span></td>";
                    }
                    else
                    {
                        markup = $"<td class='text-center'>{System.Net.WebUtility.HtmlEncode(item.Description)}</td>";
                    }
                    break;

                case "DueDate":
                    if (item.DebitCredit)
                    {
                        markup = $"<td class='text-end'>{item.DueDate.Value.ToString(BrowserService.DateFormat)}</td>";
                    }
                    else
                    {
                        markup = "<td class='text-end'></td>";
                    }
                    break;

                case "CurrencyShortName":
                    markup = $"<td><img src='data:image/png;base64,{Convert.ToBase64String(Config.Currencies[item.CurrencyID].Picture)}' width='30' height='20'> {System.Net.WebUtility.HtmlEncode(item.CurrencyShortName)}</td>";
                    break;

                case "DebitAmountIncTax":
                    decimal debitAmount = item.DebitAmountIncTax ?? 0;
                    if (debitAmount != 0)
                    {
                        markup = $"<td class='text-end'>{Config.FormattedCurrency(debitAmount, item.CurrencyID)}</td>";
                    }
                    else
                    {
                        markup = "<td class='text-end'></td>";
                    }
                    break;

                case "CreditAmountIncTax":
                    decimal creditAmount = item.CreditAmountIncTax ?? 0;
                    if (creditAmount != 0)
                    {
                        markup = $"<td class='text-end'>{Config.FormattedCurrency(creditAmount, item.CurrencyID)}</td>";
                    }
                    else
                    {
                        markup = "<td class='text-end'></td>";
                    }
                    break;

                case "GSTAmount":
                    if (item.GSTAmount != 0)
                    {
                        markup = $"<td class='text-end'>{Config.FormattedCurrency(item.GSTAmount, item.CurrencyID)}</td>";
                    }
                    else
                    {
                        markup = "<td class='text-end'></td>";
                    }
                    break;

                case "AllocatedAmount":
                    markup = "<td></td>";
                    if (item.DebitCredit && item.AllocatedAmount != null && item.AllocatedAmount.Value != 0)
                    {
                        markup = $"<td class='text-end'>{Config.FormattedCurrency(item.AllocatedAmount.Value, item.CurrencyID)}</td>";
                    }
                    break;

                case "OutstandingAmount":
                    if (item.DebitCredit && item.OutstandingAmount != null && item.OutstandingAmount.Value != 0)
                    {
                        if (item.DueDate < DateTime.Now)
                        {
                            markup = $"<td class='text-end'><span class='badge rounded-pill bg-danger'>Overdue</span><br />{Config.FormattedCurrency(item.OutstandingAmount.Value, item.CurrencyID)}</td>";
                        }
                        else
                        {
                            markup = $"<td class='text-end'>{Config.FormattedCurrency(item.OutstandingAmount.Value, item.CurrencyID)}</td>";
                        }
                    }
                    else
                    {
                        if (item.DebitCredit)
                        {
                            markup = "<td class='text-end'><div class='badge rounded-pill bg-success'>Fully Paid</div></td>";
                        }
                        else
                        {
                            if (item.OutstandingAmount.Value == 0)
                            {
                                markup = "<td class='text-end'></td>";
                            }
                            else
                            {
                                markup = $"<td class='text-end'>{Config.FormattedCurrency(-item.OutstandingAmount.Value, item.CurrencyID)}</td>";
                            }
                        }
                    }
                    break;
            }

            return string.IsNullOrWhiteSpace(markup) ? null : (builder => builder.AddMarkupContent(0, markup));
        }
        
        public RenderFragment SalesOrdersGridDataCellRenderFragment(JiwaFinancials.Jiwa.JiwaServiceModel.Tables.v_Jiwa_SalesOrder_List item, string columnId)
        {
            string? markup = null;
            switch (columnId)
            {
                case "InvoiceNoDashHistoryNo":
                    markup = $"<td><a href='SalesOrder/{item.InvoiceID.Trim()}?SnapshotNo={item.HistoryNo}'>{System.Net.WebUtility.HtmlEncode(item.InvoiceNoDashHistoryNo)}</a></td>";
                    break;
                case "FXInvoiceTotalIncTax":
                    if (item.CreditNote && item.FXInvoiceTotalIncTax != null)
                    {
                        decimal orderTotal = item.FXInvoiceTotalIncTax.Value * -1;
                        markup = $"<td class='text-end'>{Config.FormattedCurrency(orderTotal, item.CurrencyID)}</td>";
                    }
                    else
                    {
                        if (item.FXInvoiceTotalIncTax > item.TotalAllocated && item.DueDate < DateTime.Now)
                        {
                            markup = $"<td class='text-end'><div class='text-danger'>{Config.FormattedCurrency(item.FXInvoiceTotalIncTax, item.CurrencyID)}</div></td>";
                        }
                        else
                        {
                            if (item.TotalAllocated >= item.FXInvoiceTotalIncTax)
                            {
                                markup = $"<td class='text-end'><div class='text-success'>{Config.FormattedCurrency(item.FXInvoiceTotalIncTax, item.CurrencyID)}</div></td>";
                            }
                            else
                            {
                                markup = $"<td class='text-end'>{Config.FormattedCurrency(item.FXInvoiceTotalIncTax, item.CurrencyID)}</td>";
                            }
                        }
                    }
                    break;
                case "TotalAllocated":
                    if (item.CreditNote && item.TotalAllocated != null)
                    {
                        decimal? allocatedTotal = item.TotalAllocated.Value * -1;
                        markup = $"<td class='text-end'>{Config.FormattedCurrency(allocatedTotal, item.CurrencyID)}</td>";
                    }
                    else
                    {
                        if (item.FXInvoiceTotalIncTax > item.TotalAllocated && item.DueDate < DateTime.Now)
                        {
                            markup = $"<td class='text-end'><div class='text-danger'>{Config.FormattedCurrency(item.TotalAllocated, item.CurrencyID)}</div></td>";
                        }
                        else
                        {
                            markup = $"<td class='text-end'><div class='text-success'>{Config.FormattedCurrency(item.TotalAllocated, item.CurrencyID)}</div></td>";
                        }
                    }
                    break;
                case "DueDate":
                    if (!item.CreditNote && item.DueDate != null)
                    {
                        if (item.FXInvoiceTotalIncTax > item.TotalAllocated && item.DueDate < DateTime.Now)
                        {
                            markup = $"<td class='text-end'><span class='badge rounded-pill bg-danger'>{item.DueDate.Value.ToString(BrowserService.DateFormat)}</span></td>";
                        }
                        else
                        {
                            if (item.TotalAllocated >= item.FXInvoiceTotalIncTax)
                            {
                                markup = "<td class='text-end'><div class='badge rounded-pill bg-success'>Fully Paid</div></td>";
                            }
                            else
                            {
                                markup = $"<td class='text-end'>{item.DueDate.Value.ToString(BrowserService.DateFormat)}</td>";
                            }
                        }
                    }
                    else
                    {
                        markup = "<td class='text-end'></td>";
                    }
                    break;
                case "CurrencyShortName":
                    markup = $"<td><img src='data:image/png;base64,{Convert.ToBase64String(Config.Currencies[item.CurrencyID].Picture)}' width='30' height='20'> {System.Net.WebUtility.HtmlEncode(item.CurrencyShortName)}</td>";
                    break;
            }

            return string.IsNullOrWhiteSpace(markup) ? null : (builder => builder.AddMarkupContent(0, markup));
        }
        
        public RenderFragment SalesQuotesGridHeaderCellRenderFragment(JiwaAutoQueryColumn<JiwaFinancials.Jiwa.JiwaServiceModel.Tables.v_Jiwa_SalesQuote_List> Column)
        {
            string? markup = null;
            switch (Column.Id)
            {
                case "Status":
                    markup = $"<td class='text-center'>{System.Net.WebUtility.HtmlEncode(Column.Caption)}</td>";
                    break;
            }

            return string.IsNullOrWhiteSpace(markup) ? null : (builder => builder.AddMarkupContent(0, markup));
        }

        public RenderFragment SalesQuotesGridDataCellRenderFragment(JiwaFinancials.Jiwa.JiwaServiceModel.Tables.v_Jiwa_SalesQuote_List item, string columnId)
        {
            string? markup = null;
            switch (columnId)
            {
                case "InvoiceNoDashHistoryNo":
                    markup = $"<td><a href='SalesQuote/{item.InvoiceID.Trim()}?SnapshotNo={item.HistoryNo}'>{System.Net.WebUtility.HtmlEncode(item.InvoiceNoDashHistoryNo)}</a></td>";
                    break;
                case "FXInvoiceTotalIncTax":
                    markup = $"<td class='text-end'>{Config.FormattedCurrency(item.FXInvoiceTotalIncTax, item.CurrencyID)}</td>";
                    break;
                case "CurrencyShortName":
                    markup = $"<td><img src='data:image/png;base64,{Convert.ToBase64String(Config.Currencies[item.CurrencyID].Picture)}' width='30' height='20'> {System.Net.WebUtility.HtmlEncode(item.CurrencyShortName)}</td>";
                    break;
                case "Status":
                    if (item.Status == 0)
                    {
                        markup = "<td class='text-center'><span class='badge rounded-pill bg-success'>Open</span></td>";
                    }
                    else
                    {
                        markup = "<td class='text-center'><span class='badge rounded-pill bg-danger'>Closed</span></td>";
                    }
                    break;
            }

            return string.IsNullOrWhiteSpace(markup) ? null : (builder => builder.AddMarkupContent(0, markup));
        }
        
        public Task CreateContactName()
        {
            ContactNameToCreate = new JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.DebtorContactName();
            ContactNameToCreate.TagMemberships = new List<JiwaFinancials.Jiwa.JiwaServiceModel.Tags.Tag>();
        
            return Task.CompletedTask;
        }
        
        public Task EditContactName(JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.DebtorContactName ContactName)
        {
            // setting ContactNameToEdit will trigger the edit contact dialog
            // we set ContactNameToEdit to a copy of the actual contact name, because whatever changes the edit dialog makes we might want to abandon if the user cancels the dialog        
            ContactNameToEdit = new JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.DebtorContactName();
            ContactNameToEdit.ContactNameID = ContactName.ContactNameID;
            ContactNameToEdit.Title = ContactName.Title;
            ContactNameToEdit.FirstName = ContactName.FirstName;
            ContactNameToEdit.Surname = ContactName.Surname;
            ContactNameToEdit.Mobile = ContactName.Mobile;
            ContactNameToEdit.EmailAddress = ContactName.EmailAddress;
            ContactNameToEdit.TagMemberships = ContactName.TagMemberships.ToList();
        
            return Task.CompletedTask;
        }
        
        public Task DeleteContactName(JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.DebtorContactName ContactName)
        {
            // This will trigger the are you sure? dialog
            ContactNameToDelete = ContactName;
        
            return Task.CompletedTask;
        }      
        
        public async void CreateContactDialogClosed(JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.DebtorContactName ContactName)
        {
            ContactNameToCreate = null;
        
            if (ContactName != null)
            {
                // edit the contact
                JiwaFinancials.Jiwa.JiwaServiceModel.DebtorContactNamePOSTRequest debtorContactNamePOSTRequest = new JiwaFinancials.Jiwa.JiwaServiceModel.DebtorContactNamePOSTRequest();
                debtorContactNamePOSTRequest.DebtorID = Debtor.DebtorID;
                debtorContactNamePOSTRequest.Title = ContactName.Title;
                debtorContactNamePOSTRequest.FirstName = ContactName.FirstName;
                debtorContactNamePOSTRequest.Surname = ContactName.Surname;
                debtorContactNamePOSTRequest.Mobile = ContactName.Mobile;
                debtorContactNamePOSTRequest.EmailAddress = ContactName.EmailAddress;
                debtorContactNamePOSTRequest.TagMemberships = ContactName.TagMemberships;

                JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.DebtorContactName createdContact = await SendToAPI<JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.DebtorContactName>(debtorContactNamePOSTRequest);
                // Re-read contact names after post.
                JiwaFinancials.Jiwa.JiwaServiceModel.DebtorContactNamesGETManyRequest debtorContactNamesGETManyRequest = new JiwaFinancials.Jiwa.JiwaServiceModel.DebtorContactNamesGETManyRequest();
                debtorContactNamesGETManyRequest.DebtorID = Debtor.DebtorID;
                Debtor.ContactNames = await SendToAPI<List<JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.DebtorContactName>>(debtorContactNamesGETManyRequest);
                await DisplayContacts();
                statusMessage = $"Contact {ContactName.DisplayName()}, {ContactName.EmailAddress} has been created.";
            }
            else
            {
                // clear any previously set status message when the user cancels the create
                statusMessage = null;
            }
                
            await InvokeAsync(StateHasChanged);
        }
        
        public async void EditContactDialogClosed(JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.DebtorContactName ContactName)
        {
            ContactNameToEdit = null;
        
            if (ContactName != null)
            {
                // edit the contact
                JiwaFinancials.Jiwa.JiwaServiceModel.DebtorContactNamePATCHRequest debtorContactNamePATCHRequest = new JiwaFinancials.Jiwa.JiwaServiceModel.DebtorContactNamePATCHRequest();
                debtorContactNamePATCHRequest.DebtorID = Debtor.DebtorID;
                debtorContactNamePATCHRequest.ContactNameID = ContactName.ContactNameID;
                debtorContactNamePATCHRequest.Title = ContactName.Title;
                debtorContactNamePATCHRequest.FirstName = ContactName.FirstName;
                debtorContactNamePATCHRequest.Surname = ContactName.Surname;
                debtorContactNamePATCHRequest.Mobile = ContactName.Mobile;
                debtorContactNamePATCHRequest.EmailAddress = ContactName.EmailAddress;
                debtorContactNamePATCHRequest.TagMemberships = null; // Tag memberships we ignore in the contact name patch, as we do a subsequent PUT to replace all the tags instead.

                var patchResponse = await SendToAPI<JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.DebtorContactName>(debtorContactNamePATCHRequest);
                JiwaFinancials.Jiwa.JiwaServiceModel.DebtorContactNameTagMembershipPUTRequest debtorContactNameTagMembershipPUTRequest = new JiwaFinancials.Jiwa.JiwaServiceModel.DebtorContactNameTagMembershipPUTRequest();
                debtorContactNameTagMembershipPUTRequest.DebtorID = Debtor.DebtorID;
                debtorContactNameTagMembershipPUTRequest.ContactNameID = ContactName.ContactNameID;
                debtorContactNameTagMembershipPUTRequest.Tags = ContactName.TagMemberships;

                var putResponse = await SendToAPI<List<JiwaFinancials.Jiwa.JiwaServiceModel.Tags.Tag>>(debtorContactNameTagMembershipPUTRequest);

                // Re-read contact names after updating.                
                JiwaFinancials.Jiwa.JiwaServiceModel.DebtorContactNamesGETManyRequest debtorContactNamesGETManyRequest = new JiwaFinancials.Jiwa.JiwaServiceModel.DebtorContactNamesGETManyRequest();
                debtorContactNamesGETManyRequest.DebtorID = Debtor.DebtorID;
                Debtor.ContactNames = await SendToAPI<List<JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.DebtorContactName>>(debtorContactNamesGETManyRequest);
                await DisplayContacts();
                statusMessage = $"Contact {ContactName.DisplayName()}, {ContactName.EmailAddress} has been updated.";
                await InvokeAsync(StateHasChanged);
            }
            else
            {
                // clear any previously set status message when the user cancels the edit
                statusMessage = null;
            }                           
        }
        
        public async void DeleteContactConfirmationDialogClosed(bool resultOK)
        {
            if (resultOK)
            {
                if (ContactNameToDelete.ContactNameID == WebPortalUserSessionStateContainer.WebPortalUserSession.DebtorContactNameID)
                {
                    statusMessage = "We'd rather you didn't delete yourself.";
                    ContactNameToDelete = null;
                    return;
                }
        
                JiwaFinancials.Jiwa.JiwaServiceModel.DebtorContactNameDELETERequest debtorContactNameDELETERequest = new JiwaFinancials.Jiwa.JiwaServiceModel.DebtorContactNameDELETERequest();
                debtorContactNameDELETERequest.DebtorID = Debtor.DebtorID;
                debtorContactNameDELETERequest.ContactNameID = ContactNameToDelete.ContactNameID;
                    
                string deletedMessage = $"Contact {ContactNameToDelete.DisplayName()}, {ContactNameToDelete.EmailAddress} has been deleted.";
                ContactNameToDelete = null;

                await DeleteFromAPI(debtorContactNameDELETERequest);

                // Re-read contact names after updating.                
                JiwaFinancials.Jiwa.JiwaServiceModel.DebtorContactNamesGETManyRequest debtorContactNamesGETManyRequest = new JiwaFinancials.Jiwa.JiwaServiceModel.DebtorContactNamesGETManyRequest();
                debtorContactNamesGETManyRequest.DebtorID = Debtor.DebtorID;
                Debtor.ContactNames = await SendToAPI<List<JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.DebtorContactName>>(debtorContactNamesGETManyRequest);
                await DisplayContacts();
                statusMessage = deletedMessage;
                await InvokeAsync(StateHasChanged);
            }
            else
            {
                // clear any previously set status message when the user cancels the delete
                statusMessage = null;
                ContactNameToDelete = null;
            }                           
        }
        
        public short GetSnapshotNoFromInvoiceNoDashDocketNumHeaderHistoryNo(string InvoiceNoDashDocketNumHeaderHistoryNo)
        {
            short snapshotNo = 1;
            // parse the @item.InvRemitNo to try to obtain the snapshot number
            // Invoices appear in the InvRemitNo in the format {InvoiceNo}-{DocketNumHeader}{HistoryNo}
            // Eg: 110237-D01
            // The DocketNumHeader system setting is set in the CustomerWebPortalSettings when this web app first starts up.  It may be blank or null.
            // So we get everthing to the right of the last hyphen (-) and remove the DocketNumHeader if that was not null or blank and what is left
            // should be the snapshot number.
            int hyphenIndex = InvoiceNoDashDocketNumHeaderHistoryNo.LastIndexOf('-');
            if (hyphenIndex != -1)
            {
                string docketNo = InvoiceNoDashDocketNumHeaderHistoryNo.Substring(hyphenIndex);
        
                if (!string.IsNullOrWhiteSpace(Config.DocketNumHeader))
                {
                    docketNo = docketNo.Replace("-" + Config.DocketNumHeader, "");
                }
        
                short docketNoAsShort = 0;
        
                if (short.TryParse(docketNo, out docketNoAsShort))
                {
                    snapshotNo = docketNoAsShort;
                }
            }
        
            return snapshotNo;
        }

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
