using JiwaCustomerPortal.Components.AutoQueryGrid;
using JiwaCustomerPortal.Components.AutoQueryGrid.Debtor;
using JiwaCustomerPortal.Components.Grid;
using JiwaFinancials.Jiwa.JiwaServiceModel;
using JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders;
using Microsoft.AspNetCore.Components;
using ServiceStack;
using System.Linq;

namespace JiwaCustomerPortal.Components.Pages
{
    public partial class Customers
    {
            private int APIRequestInProgressCount = 0;
        
            // APIRequestInProgress cannot be simply set to true and restored to original state, due to race conditions arising from asynchronous
            // calls - so we use a counter instead, and increment or decrement that - and we look at the APIRequestInProgressCount to determine if a request is currently in progress or not.
            public bool APIRequestInProgress => APIRequestInProgressCount > 0;
        
            private string? statusMessage;
            private bool CanManageContacts => WebPortalUserSessionStateContainer?.WebPortalUserSession?.IsAdminRole == true || WebPortalUserSessionStateContainer?.WebPortalUserSession?.AuthProvider == "credentials";
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
                    // whenever the debtor changes, we need to re-display everything.
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
            private JiwaAPIAutoQueryGrid<JiwaFinancials.Jiwa.JiwaServiceModel.Tables.v_Jiwa_Debtor_Transactions_List, JiwaFinancials.Jiwa.JiwaServiceModel.Tables.v_Jiwa_Debtor_Transactions_ListQuery> DebtorsTransactionsAutoQueryGrid { get; set; }
        
            private JiwaFinancials.Jiwa.JiwaServiceModel.Tables.v_Jiwa_SalesOrder_ListQuery SalesOrdersAutoQuery { get; set; }
            private JiwaAPIAutoQueryGrid<JiwaFinancials.Jiwa.JiwaServiceModel.Tables.v_Jiwa_SalesOrder_List, JiwaFinancials.Jiwa.JiwaServiceModel.Tables.v_Jiwa_SalesOrder_ListQuery> SalesOrdersAutoQueryGrid { get; set; }
        
            private JiwaFinancials.Jiwa.JiwaServiceModel.Tables.v_Jiwa_SalesQuote_ListQuery SalesQuotesAutoQuery { get; set; }
            private JiwaAPIAutoQueryGrid<JiwaFinancials.Jiwa.JiwaServiceModel.Tables.v_Jiwa_SalesQuote_List, JiwaFinancials.Jiwa.JiwaServiceModel.Tables.v_Jiwa_SalesQuote_ListQuery> SalesQuotesAutoQueryGrid { get; set; }
        
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
                else
                {
                    if (WebPortalUserSessionStateContainer.WebPortalUserSession.AuthProvider != "credentials")
                    {
                        // Should not be here unless you're a staff member, so redirect to logon
                        // TODO: Give user a message that they need to log on with a staff member account to access this page, as opposed to just redirecting them to the login page with no explanation.
                        NavigationManager.NavigateTo($"User/SignIn?returnUrl={NavigationManager.Uri}");
                        return;
                    }
                }

                await InitialiseContactsGrid();
                await InitialiseDeliveryAddressesGrid();
                await InitialiseBalancesGrid();
                await InitialiseBackOrdersGrid();

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

                if (CanManageContacts)
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

        private async Task DisplayContact(JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.DebtorContactName contactName, int row)
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

            if (ContactsGridCells.Columns["Edit"] != null)
            {
                ContactsGridCells["Edit", row].IsEditable = CanManageContacts;
            }

            if (ContactsGridCells.Columns["Delete"] != null)
            {
                ContactsGridCells["Delete", row].IsEditable = CanManageContacts;
            }

            await Task.CompletedTask;
        }

        private async Task ContactsGrid_CellButtonClicked(Cell cell)
        {
            if (Debtor != null && cell.Row >= 0 && cell.Row < Debtor.ContactNames.Count)
            {
                JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.DebtorContactName contactName = Debtor.ContactNames[cell.Row];
                if (cell.Column.Id == "Edit")
                {
                    EditContactName(contactName);
                }
                else if (cell.Column.Id == "Delete")
                {
                    DeleteContactName(contactName);
                }
            }

            await Task.CompletedTask;
        }

        private async Task InitialiseDeliveryAddressesGrid()
        {
            DeliveryAddressesGridCells = new Grid.CellArray();
            DeliveryAddressesGridCells.Columns.Add(new Column("Address1", new Components.Grid.CellType.TextCellType(), "Address 1") { Width = 40 });
            DeliveryAddressesGridCells.Columns.Add(new Column("Address2", new Components.Grid.CellType.TextCellType(), "Address 2") { Width = 40 });
            DeliveryAddressesGridCells.Columns.Add(new Column("Address3", new Components.Grid.CellType.TextCellType(), "Suburb") { Width = 30 });
            DeliveryAddressesGridCells.Columns.Add(new Column("Address4", new Components.Grid.CellType.TextCellType(), "State") { Width = 20 });
            DeliveryAddressesGridCells.Columns.Add(new Column("Postcode", new Components.Grid.CellType.TextCellType(), "Postcode") { Width = 20 });
            DeliveryAddressesGridCells.Columns.Add(new Column("Country", new Components.Grid.CellType.TextCellType(), "Country") { Width = 20 });

            await Task.CompletedTask;
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

        private async Task InitialiseBalancesGrid()
        {
            BalancesGridCells = new Grid.CellArray();
            BalancesGridCells.Columns.Add(new Column("Currency", new Components.Grid.CellType.TextCellType(), "Currency") { Width = 20 });
            BalancesGridCells.Columns.Add(new Column("Period1", new Components.Grid.CellType.DecimalCellType(), Period1Label) { Width = 15 });
            BalancesGridCells.Columns.Add(new Column("Period2", new Components.Grid.CellType.DecimalCellType(), Period2Label) { Width = 15 });
            BalancesGridCells.Columns.Add(new Column("Period3", new Components.Grid.CellType.DecimalCellType(), Period3Label) { Width = 15 });
            BalancesGridCells.Columns.Add(new Column("Period4", new Components.Grid.CellType.DecimalCellType(), Period4Label) { Width = 15 });
            BalancesGridCells.Columns.Add(new Column("Total", new Components.Grid.CellType.DecimalCellType(), "Total") { Width = 15 });

            await Task.CompletedTask;
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

        private async Task InitialiseBackOrdersGrid()
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

            await Task.CompletedTask;
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
                BackOrdersGridCells["InvoiceNo", row].CustomRenderFragment = async builder =>
                {
                    builder.OpenElement(0, "a");
                    builder.AddAttribute(1, "href", $"SalesOrder/{backorder.InvoiceID}");
                    builder.AddContent(2, backorder.InvoiceNo);
                    builder.CloseElement();
                };
                BackOrdersGridCells["CustomerOrderNo", row].Value = backorder.CustomerOrderNo;
                BackOrdersGridCells["Date", row].Value = backorder.Date;

                Components.Grid.CellType.DecimalCellType quantityCellType = new Components.Grid.CellType.DecimalCellType();
                quantityCellType.MinValue = 0;
                quantityCellType.MinDecimalPlaces = backorder.QuantityDecimalPlaces;
                quantityCellType.MaxDecimalPlaces = backorder.QuantityDecimalPlaces;

                BackOrdersGridCells["Quantity", row].CellType = quantityCellType;
                BackOrdersGridCells["Quantity", row].Value = backorder.Quantity;
                
                BackOrdersGridCells["ExpectedDeliveryDate", row].Value = backorder.ExpectedDeliveryDate;
            }

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
        
        public async void ItemSelected(JiwaFinancials.Jiwa.JiwaServiceModel.Tables.Or.v_Jiwa_Debtor_ListOR item)
        {
            // when a debtor is selected from the customer list, we need to read the debtor
        
            if (item == null)
            {
                return;
            }
        
            LazyLoadTabIds.Clear();
        
            JiwaFinancials.Jiwa.JiwaServiceModel.DebtorGETRequest debtorGETRequest = new JiwaFinancials.Jiwa.JiwaServiceModel.DebtorGETRequest();
            debtorGETRequest.DebtorID = item.DebtorID;
            Debtor = await SendToAPI<JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.Debtor>(debtorGETRequest);      
        
            if (Debtor is not null)
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

                await InitialiseBalancesGrid();
                await DisplayBalances();

                // Re-query the auto-queries with the new DebtorID
                DebtorsTransactionsAutoQuery.DebtorID = Debtor.DebtorID;
                if (DebtorsTransactionsAutoQueryGrid is not null && SelectedTabId == "Transactions-tab")
                {
                    await DebtorsTransactionsAutoQueryGrid.ExecuteAutoQuery();
                }
        
                SalesOrdersAutoQuery.DebtorID = Debtor.DebtorID;
                if (SalesOrdersAutoQueryGrid is not null && SelectedTabId == "SalesOrders-tab")
                {                
                    await SalesOrdersAutoQueryGrid.ExecuteAutoQuery();
                }
        
                SalesQuotesAutoQuery.DebtorID = Debtor.DebtorID;
                if (SalesQuotesAutoQueryGrid is not null && SelectedTabId == "SalesQuotes-tab")
                {                
                    await SalesQuotesAutoQueryGrid.ExecuteAutoQuery();
                }
        
                if (SelectedTabId != null)
                {
                    await OnSelectTab(SelectedTabId);
                }
            }
        
            await InvokeAsync(StateHasChanged);
        }
        
        public JiwaFinancials.Jiwa.JiwaServiceModel.Tables.v_Jiwa_Debtor_List InitialSelectedItemMethod(List<JiwaFinancials.Jiwa.JiwaServiceModel.Tables.v_Jiwa_Debtor_List> Results)
        {
            if (Results != null && Results.Count > 0)
            {
                return Results.FirstOrDefault();
            }
            else
            {
                return null;
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
        
        private static RenderFragment TextCell(string text, string cssClass = null) => builder =>
        {
            builder.OpenElement(0, "td");
            if (!string.IsNullOrWhiteSpace(cssClass))
            {
                builder.AddAttribute(1, "class", cssClass);
            }

            builder.AddContent(2, text);
            builder.CloseElement();
        };

        private static RenderFragment EmptyCell(string cssClass = null) => builder =>
        {
            builder.OpenElement(0, "td");
            if (!string.IsNullOrWhiteSpace(cssClass))
            {
                builder.AddAttribute(1, "class", cssClass);
            }

            builder.CloseElement();
        };

        private static RenderFragment BadgeCell(string text, string badgeClass, string tdClass = null) => builder =>
        {
            builder.OpenElement(0, "td");
            if (!string.IsNullOrWhiteSpace(tdClass))
            {
                builder.AddAttribute(1, "class", tdClass);
            }

            builder.OpenElement(2, "span");
            builder.AddAttribute(3, "class", badgeClass);
            builder.AddContent(4, text);
            builder.CloseElement();
            builder.CloseElement();
        };

        private RenderFragment LinkCell(string href, string text) => builder =>
        {
            builder.OpenElement(0, "td");
            builder.OpenElement(1, "a");
            builder.AddAttribute(2, "href", href);
            builder.AddContent(3, text);
            builder.CloseElement();
            builder.CloseElement();
        };

        private RenderFragment CurrencyCell(string currencyId, string currencyShortName) => builder =>
        {
            builder.OpenElement(0, "td");
            builder.OpenElement(1, "img");
            builder.AddAttribute(2, "src", $"data:image/png;base64,{Convert.ToBase64String(Config.Currencies[currencyId].Picture)}");
            builder.AddAttribute(3, "width", "30");
            builder.AddAttribute(4, "height", "20");
            builder.CloseElement();
            builder.AddContent(5, $" {currencyShortName}");
            builder.CloseElement();
        };

        public RenderFragment TransactionsGridDataCellRenderFragment(JiwaFinancials.Jiwa.JiwaServiceModel.Tables.v_Jiwa_Debtor_Transactions_List item, string columnId)
        {
            switch (columnId)
            {
                case "InvRemitNo":
                    return item.Description == "Sales Orders"
                        ? LinkCell($"SalesOrder/{item.SourceID.Trim()}?SnapshotNo={GetSnapshotNoFromInvoiceNoDashDocketNumHeaderHistoryNo(item.InvRemitNo)}", item.InvRemitNo)
                        : TextCell(item.InvRemitNo);
                case "Description":
                    if (item.Description == "Bank Receipts") return BadgeCell("Payment", "badge rounded-pill bg-success", "text-center");
                    if (item.Description == "Debtor Adjustments") return BadgeCell("Adjustment", "badge rounded-pill bg-warning", "text-center");
                    if (item.Description == "Sales Orders") return BadgeCell("Invoice", "badge rounded-pill bg-secondary", "text-center");
                    return TextCell(item.Description, "text-center");
                case "DueDate":
                    return item.DebitCredit ? TextCell(item.DueDate.Value.ToString(BrowserService.DateFormat), "text-end") : EmptyCell("text-end");
                case "CurrencyShortName":
                    return CurrencyCell(item.CurrencyID, item.CurrencyShortName);
                case "DebitAmountIncTax":
                    {
                        decimal debitAmount = item.DebitAmountIncTax ?? 0;
                        return debitAmount != 0 ? TextCell(Config.FormattedCurrency(debitAmount, item.CurrencyID), "text-end") : EmptyCell("text-end");
                    }
                case "CreditAmountIncTax":
                    {
                        decimal creditAmount = item.CreditAmountIncTax ?? 0;
                        return creditAmount != 0 ? TextCell(Config.FormattedCurrency(creditAmount, item.CurrencyID), "text-end") : EmptyCell("text-end");
                    }
                case "GSTAmount":
                    return item.GSTAmount != 0 ? TextCell(Config.FormattedCurrency(item.GSTAmount, item.CurrencyID), "text-end") : EmptyCell("text-end");
                case "AllocatedAmount":
                    return item.DebitCredit && item.AllocatedAmount != null && item.AllocatedAmount.Value != 0
                        ? TextCell(Config.FormattedCurrency(item.AllocatedAmount.Value, item.CurrencyID), "text-end")
                        : EmptyCell();
                case "OutstandingAmount":
                    if (item.DebitCredit && item.OutstandingAmount != null && item.OutstandingAmount.Value != 0)
                    {
                        if (item.DueDate < DateTime.Now)
                        {
                            return builder =>
                            {
                                builder.OpenElement(0, "td");
                                builder.AddAttribute(1, "class", "text-end");
                                builder.OpenElement(2, "span");
                                builder.AddAttribute(3, "class", "badge rounded-pill bg-danger");
                                builder.AddContent(4, "Overdue");
                                builder.CloseElement();
                                builder.OpenElement(5, "br");
                                builder.CloseElement();
                                builder.AddContent(6, Config.FormattedCurrency(item.OutstandingAmount.Value, item.CurrencyID));
                                builder.CloseElement();
                            };
                        }

                        return TextCell(Config.FormattedCurrency(item.OutstandingAmount.Value, item.CurrencyID), "text-end");
                    }

                    if (item.DebitCredit)
                    {
                        return BadgeCell("Fully Paid", "badge rounded-pill bg-success", "text-end");
                    }

                    if (item.OutstandingAmount.Value == 0)
                    {
                        return EmptyCell("text-end");
                    }

                    return TextCell(Config.FormattedCurrency(-item.OutstandingAmount.Value, item.CurrencyID), "text-end");
            }

            return null;
        }

        public RenderFragment SalesOrdersGridDataCellRenderFragment(JiwaFinancials.Jiwa.JiwaServiceModel.Tables.v_Jiwa_SalesOrder_List item, string columnId)
        {
            switch (columnId)
            {
                case "InvoiceNoDashHistoryNo":
                    return LinkCell($"SalesOrder/{item.InvoiceID.Trim()}?SnapshotNo={item.HistoryNo}", item.InvoiceNoDashHistoryNo);
                case "FXInvoiceTotalIncTax":
                    if (item.CreditNote && item.FXInvoiceTotalIncTax != null)
                    {
                        decimal orderTotal = item.FXInvoiceTotalIncTax.Value * -1;
                        return TextCell(Config.FormattedCurrency(orderTotal, item.CurrencyID), "text-end");
                    }

                    if (item.FXInvoiceTotalIncTax > item.TotalAllocated && item.DueDate < DateTime.Now)
                    {
                        return builder =>
                        {
                            builder.OpenElement(0, "td");
                            builder.AddAttribute(1, "class", "text-end");
                            builder.OpenElement(2, "div");
                            builder.AddAttribute(3, "class", "text-danger");
                            builder.AddContent(4, Config.FormattedCurrency(item.FXInvoiceTotalIncTax, item.CurrencyID));
                            builder.CloseElement();
                            builder.CloseElement();
                        };
                    }

                    if (item.TotalAllocated >= item.FXInvoiceTotalIncTax)
                    {
                        return builder =>
                        {
                            builder.OpenElement(0, "td");
                            builder.AddAttribute(1, "class", "text-end");
                            builder.OpenElement(2, "div");
                            builder.AddAttribute(3, "class", "text-success");
                            builder.AddContent(4, Config.FormattedCurrency(item.FXInvoiceTotalIncTax, item.CurrencyID));
                            builder.CloseElement();
                            builder.CloseElement();
                        };
                    }

                    return TextCell(Config.FormattedCurrency(item.FXInvoiceTotalIncTax, item.CurrencyID), "text-end");
                case "TotalAllocated":
                    if (item.CreditNote && item.TotalAllocated != null)
                    {
                        decimal? allocatedTotal = item.TotalAllocated.Value * -1;
                        return TextCell(Config.FormattedCurrency(allocatedTotal, item.CurrencyID), "text-end");
                    }

                    return (item.FXInvoiceTotalIncTax > item.TotalAllocated && item.DueDate < DateTime.Now)
                        ? builder =>
                        {
                            builder.OpenElement(0, "td");
                            builder.AddAttribute(1, "class", "text-end");
                            builder.OpenElement(2, "div");
                            builder.AddAttribute(3, "class", "text-danger");
                            builder.AddContent(4, Config.FormattedCurrency(item.TotalAllocated, item.CurrencyID));
                            builder.CloseElement();
                            builder.CloseElement();
                        }
                        : builder =>
                        {
                            builder.OpenElement(0, "td");
                            builder.AddAttribute(1, "class", "text-end");
                            builder.OpenElement(2, "div");
                            builder.AddAttribute(3, "class", "text-success");
                            builder.AddContent(4, Config.FormattedCurrency(item.TotalAllocated, item.CurrencyID));
                            builder.CloseElement();
                            builder.CloseElement();
                        };
                case "DueDate":
                    if (!item.CreditNote && item.DueDate != null)
                    {
                        if (item.FXInvoiceTotalIncTax > item.TotalAllocated && item.DueDate < DateTime.Now)
                        {
                            return BadgeCell(item.DueDate.Value.ToString(BrowserService.DateFormat), "badge rounded-pill bg-danger", "text-end");
                        }

                        if (item.TotalAllocated >= item.FXInvoiceTotalIncTax)
                        {
                            return BadgeCell("Fully Paid", "badge rounded-pill bg-success", "text-end");
                        }

                        return TextCell(item.DueDate.Value.ToString(BrowserService.DateFormat), "text-end");
                    }

                    return EmptyCell("text-end");
                case "CurrencyShortName":
                    return CurrencyCell(item.CurrencyID, item.CurrencyShortName);
            }

            return null;
        }

        public RenderFragment SalesQuotesGridHeaderCellRenderFragment(JiwaAutoQueryColumn<JiwaFinancials.Jiwa.JiwaServiceModel.Tables.v_Jiwa_SalesQuote_List> Column)
        {
            if (Column.Id == "Status")
            {
                return TextCell(Column.Caption, "text-center");
            }

            return null;
        }

        public RenderFragment SalesQuotesGridDataCellRenderFragment(JiwaFinancials.Jiwa.JiwaServiceModel.Tables.v_Jiwa_SalesQuote_List item, string columnId)
        {
            switch (columnId)
            {
                case "InvoiceNoDashHistoryNo":
                    return LinkCell($"SalesQuote/{item.InvoiceID.Trim()}?SnapshotNo={item.HistoryNo}", item.InvoiceNoDashHistoryNo);
                case "FXInvoiceTotalIncTax":
                    return TextCell(Config.FormattedCurrency(item.FXInvoiceTotalIncTax, item.CurrencyID), "text-end");
                case "CurrencyShortName":
                    return CurrencyCell(item.CurrencyID, item.CurrencyShortName);
                case "Status":
                    return item.Status == 0
                        ? BadgeCell("Open", "badge rounded-pill bg-success", "text-center")
                        : BadgeCell("Closed", "badge rounded-pill bg-danger", "text-center");
            }

            return null;
        }

        public void CreateContactName()
        {
            ContactNameToCreate = new JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.DebtorContactName();
            ContactNameToCreate.TagMemberships = new List<JiwaFinancials.Jiwa.JiwaServiceModel.Tags.Tag>();
        }
        
        public void EditContactName(JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.DebtorContactName ContactName)
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
        
        private Task CreateQuote()
        {
            NavigationManager.NavigateTo($"/SalesQuote/NULL/?CreateForDebtorID={Debtor.DebtorID}&returnUrl={NavigationManager.Uri}");            
            return Task.CompletedTask;
        }
        
        private Task CreateSalesOrder()
        {
            NavigationManager.NavigateTo($"/SalesOrder/NULL/?CreateForDebtorID={Debtor.DebtorID}&returnUrl={NavigationManager.Uri}");
            
            return Task.CompletedTask;
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

        private async Task<TResponse> SendToAPI<TResponse>(IReturn<TResponse> requestDTO)
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

