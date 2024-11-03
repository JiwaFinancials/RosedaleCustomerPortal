using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using ServiceStack;
using System.IO;
using Microsoft.JSInterop;
using System.Diagnostics;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;
using ServiceStack.DataAnnotations;
using System.Reflection;

namespace JiwaCustomerPortal.Components
{
    public partial class JiwaAPIAutoQueryGrid<Model, QueryType> where QueryType : ServiceStack.IQuery
    {
        [Inject]
        WebPortalUserSessionStateContainer WebPortalUserSessionStateContainer { get; set; }

        [Parameter]
        public AuthTypes AuthType { get; set; } = AuthTypes.JiwaAPISessionId;
        [Parameter]
        public IQuery AutoQuery { get; set; }
        [Parameter]
        public bool ShowPageNavigationHeader { get; set; } = true;
        [Parameter]
        public bool ShowPageNavigation { get; set; } = true;
        [Parameter]
        public List<string> HiddenColumns { get; set; } = new List<string>();
        [Parameter]
        public Dictionary<string, string> CaptionMaps { get; set; } = new Dictionary<string, string>();  
        [Parameter]
        public bool AddSelectButtonColumn { get; set; } = false;
        [Parameter]
        public EventCallback NoAuthenticationTokenCallbackMethod { get; set; }
        [Parameter]
        public EventCallback<Exception> APIExceptionCallbackMethod { get; set; }
        [Parameter]
        public EventCallback<Model> ItemSelectedCallbackMethod { get; set; }
        [Parameter]
        public Func<JiwaAutoQueryColumn<Model>, RenderFragment> HeaderCellRenderFragmentCallbackMethod { get; set; }
        [Parameter]
        public Func<Model, string, RenderFragment> DataCellRenderFragmentCallbackMethod { get; set; }

        public List<JiwaAutoQueryColumn<Model>> Columns { get; set; } = new List<JiwaAutoQueryColumn<Model>>();
        private ServiceStack.QueryResponse<Model> Response { get; set; }
        public Model SelectedItem { get; set; }
        private bool APIRequestInPogress;

        DOMRect? tableRect;
        [Inject] public IJSRuntime JS { get; set; }
        ElementReference? refResults;

        JiwaAutoQueryColumn<Model> ShowFilterDialogColumn { get; set; }

        private System.Reflection.PropertyInfo[] ModelProperties;
        private System.Reflection.PropertyInfo[] QueryModelProperties;

        private Dictionary<string, object> ImmutableFilters { get; set; } = new Dictionary<string, object>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (AutoQuery == null)
            {
                return;
            }

            List<string> Fields = new List<String>();
            List<string> OrderBys = new List<string>();
            List<string> OrderByDescs = new List<string>();

            if (AutoQuery.Fields != null)
            {
                Fields = AutoQuery.Fields.Split(",").ToList();
            }

            if (AutoQuery.OrderBy != null)
            {
                OrderBys = AutoQuery.OrderBy.Split(",").ToList();
            }

            if (AutoQuery.OrderByDesc != null)
            {
                OrderByDescs = AutoQuery.OrderByDesc.Split(",").ToList();
            }

            ModelProperties = typeof(Model).GetProperties();
            QueryModelProperties = typeof(QueryType).GetProperties();

            if (ModelProperties != null)
            {
                if (Fields != null && Fields.Count > 0)
                {
                    // if the user provided the comma separated Fields property, then they don't want to see all the columns, just the ones they provided - and in the order they provided them                

                    foreach (string fieldName in Fields)
                    {
                        System.Reflection.PropertyInfo propertyInfo = ModelProperties.FirstOrDefault(x => x.Name == fieldName);
                        if (propertyInfo != null)
                        {
                            string caption = null;
                            CaptionMaps.TryGetValue(fieldName, out caption);
                            JiwaAutoQueryColumn<Model> column = new JiwaAutoQueryColumn<Model>(fieldName) { Caption = (caption ?? fieldName), IsHidden = HiddenColumns.Contains(fieldName), ColumnDataType = propertyInfo.PropertyType };
                            if (OrderBys.Contains(column.Id))
                            {
                                column.SortOrder = JiwaAutoQueryColumn<Model>.SortOrders.Ascending;
                            }
                            else if (OrderByDescs.Contains(column.Id))
                            {
                                column.SortOrder = JiwaAutoQueryColumn<Model>.SortOrders.Descending;
                            }

                            column.DisplayOrder = Columns.Count + 1;
                            Columns.Add(column);
                        }
                    }
                }
                else
                {
                    foreach (System.Reflection.PropertyInfo propertyInfo in ModelProperties)
                    {
                        string caption = null;
                        CaptionMaps.TryGetValue(propertyInfo.Name, out caption);
                        JiwaAutoQueryColumn<Model> column = new JiwaAutoQueryColumn<Model>(propertyInfo.Name) { Caption = (caption ?? propertyInfo.Name), IsHidden = HiddenColumns.Contains(propertyInfo.Name), ColumnDataType = propertyInfo.PropertyType };
                        if (OrderBys.Contains(column.Id))
                        {
                            column.SortOrder = JiwaAutoQueryColumn<Model>.SortOrders.Ascending;
                        }
                        else if (OrderByDescs.Contains(column.Id))
                        {
                            column.SortOrder = JiwaAutoQueryColumn<Model>.SortOrders.Descending;
                        }

                        column.DisplayOrder = Columns.Count + 1;
                        Columns.Add(column);
                    }
                }                
            }

            // Autoquery query models have property names which start with the model property name,
            // so if the model has a column named "OrderNo" and it's a string, then the following properties are present in the autoquery model
            //  public virtual string OrderNo { get; set; }
            //  public virtual string OrderNoStartsWith { get; set; }
            //  public virtual string OrderNoEndsWith { get; set; }
            //  public virtual string OrderNoContains { get; set; }
            //  public virtual string OrderNoLike { get; set; }
            //  public virtual string[] OrderNoBetween { get; set; }
            //  public virtual string[] OrderNoIn { get; set; }
            //
            // Each of the filters for each column can be one of these properties.
            
            foreach (JiwaAutoQueryColumn<Model> column in Columns)
            {
                foreach (System.Reflection.PropertyInfo queryPropertyInfo in QueryModelProperties)
                {
                    if (queryPropertyInfo.Name.StartsWith(column.Id))
                    {
                        // Properties already set which are filters we store away now, because when we execute we clear and re-add filters
                        object value = AutoQuery.GetType().GetProperty(queryPropertyInfo.Name).GetValue(AutoQuery, null);
                        if (value != null)
                        {
                            ImmutableFilters.Add(queryPropertyInfo.Name, value);
                        }

                        JiwaAutoQueryColumnFilterOperator queryColumnFilterOperator = new JiwaAutoQueryColumnFilterOperator();
                        queryColumnFilterOperator.QueryModelProperty = queryPropertyInfo.Name;
                        // remove the property name and put a space before each capitalised letter,
                        // so OrderNoEndsWith becomes Ends With
                        queryColumnFilterOperator.DisplayValue = Regex.Replace(queryColumnFilterOperator.QueryModelProperty.Replace(column.Id, ""), "([a-z])([A-Z])", "$1 $2");
                        if (queryColumnFilterOperator.DisplayValue.IsEmpty())
                        {
                            queryColumnFilterOperator.DisplayValue = "Equals";
                        }
                        column.FilterOperators.Add(queryColumnFilterOperator);
                    }
                }
            }

            if (AutoQuery.Skip == null)
            {
                AutoQuery.Skip = 0;
            }

            if (AutoQuery.Take == null)
            {
                AutoQuery.Take = 100;
            }            

            await ExecuteAutoQuery();
        }

        public async Task ExecuteAutoQuery()
        {            
            string? jiwaAPIKey = null;

            if (AuthType == AuthTypes.JiwaAPISessionId)
            {                
                if (WebPortalUserSessionStateContainer.WebPortalUserSession == null)
                {
                    // not authenticated - no token found in session storage
                    if (NoAuthenticationTokenCallbackMethod.HasDelegate)
                    {
                        await NoAuthenticationTokenCallbackMethod.InvokeAsync();
                    }
                    else
                    {
                        NavigationManager.NavigateTo("User/SignIn");
                    }
                    return;
                }                
            }
            else
            {
                jiwaAPIKey = Config.JiwaAPIKey;
            }

            if (AutoQuery.Include == null || !AutoQuery.Include.Contains("Total"))
            {
                // we need the total for pagination, so add that
                if (!string.IsNullOrEmpty(AutoQuery.Include))
                {
                    AutoQuery.Include += ",Total";
                }
                else
                {
                    AutoQuery.Include = "Total";
                }
            }

            // Sorting
            string orderByAscending = string.Join(",", Columns.Where(p => p.SortOrder != null && p.SortOrder == JiwaAutoQueryColumn<Model>.SortOrders.Ascending).Select(p => p.Id));
            string orderByDescending = string.Join(",", Columns.Where(p => p.SortOrder != null && p.SortOrder == JiwaAutoQueryColumn<Model>.SortOrders.Descending).Select(p => "-" + p.Id));
            AutoQuery.OrderBy = string.Join(",", orderByAscending, orderByDescending);

            // Filters
            foreach (JiwaAutoQueryColumn<Model> column in Columns)
            {
                // Set initial value to null for all properties which start with a column name
                // So OrderNo, OrderNoContains, OrderNoStartsWith and so on are all nulled first
                // We need to null them first or else once we apply a filter it will stay there set in the AutoQuery property until we set it to something else.
                foreach (System.Reflection.PropertyInfo propertyInfo in QueryModelProperties.Where(x => x.Name.StartsWith(column.Id)))
                {                    
                    propertyInfo.SetValue(AutoQuery, null, null);
                    object value = null;
                    ImmutableFilters.TryGetValue(propertyInfo.Name, out value);
                    if (value != null)
                    {
                        propertyInfo.SetValue(AutoQuery, value, null);
                    }
                }

                // Now set values corresponding to the selected filters
                foreach (JiwaAutoQueryColumnFilter filter in column.Filters)
                {
                    System.Reflection.PropertyInfo propertyInfo = AutoQuery.GetType().GetProperty(filter.FilterOperator.QueryModelProperty);
                    if (propertyInfo != null)
                    {
                        Type dataType = column.ColumnDataType;
                        if (dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            dataType = Nullable.GetUnderlyingType(dataType);
                        }

                        if (dataType == typeof(string))
                        {
                            propertyInfo.SetValue(AutoQuery, filter.FilterValue, null);
                        }
                        else if (dataType == typeof(decimal))
                        {
                            decimal value = 0;
                            if (decimal.TryParse(filter.FilterValue, out value))
                            {
                                propertyInfo.SetValue(AutoQuery, value, null);
                            }
                            
                        }
                        else if (dataType == typeof(int))
                        {
                            int value = 0;
                            if (int.TryParse(filter.FilterValue, out value))
                            {
                                propertyInfo.SetValue(AutoQuery, value, null);
                            }

                        }
                        else if (dataType == typeof(bool))
                        {
                            bool value = false;
                            if (bool.TryParse(filter.FilterValue, out value))
                            {
                                propertyInfo.SetValue(AutoQuery, value, null);
                            }

                        }
                        else if (dataType == typeof(DateTime))
                        {
                            DateTime value = DateTime.Now;
                            if (DateTime.TryParse(filter.FilterValue, out value))
                            {
                                propertyInfo.SetValue(AutoQuery, value, null);
                            }
                        }
                    }
                }
            }

            bool oldAPIRequestInPogress = APIRequestInPogress;
            APIRequestInPogress = true;
            // Signal the page has changed so the spinner starts animating
            StateHasChanged();

            try
            {
                if (AuthType == AuthTypes.JiwaAPIKey)
                {
                    Response = await JiwaAPI.GetAsync<QueryResponse<Model>>(AutoQuery, jiwaAPIKey: jiwaAPIKey);
                }
                else
                {
                    Response = await JiwaAPI.GetAsync<QueryResponse<Model>>(AutoQuery, jiwaAPISessionId: WebPortalUserSessionStateContainer?.WebPortalUserSession?.Id);
                }                
            }            
            catch (Exception ex)
            {
                if (APIExceptionCallbackMethod.HasDelegate)
                {
                    await APIExceptionCallbackMethod.InvokeAsync(ex);
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                APIRequestInPogress = oldAPIRequestInPogress;
            }

            // Signal the page has changed again, as many properties of the AutoQuery response are used to render the page
            StateHasChanged();
        }

        public T PropertyValue<T>(Model item, string propertyName)
        {
            return (T)PropertyValue(item, propertyName);
        }

        public object PropertyValue(Model item, string propertyName)
        {
            return item.GetType().GetProperty(propertyName).GetValue(item, null);
        }

        #region "Pagination stuff"
        public string PageButtonStyle(int pageNumber)
        {
            if (pageNumber == CurrentPageNumber())
            {
                return "btn btn-primary";
            }
            else
            {
                return "btn btn-secondary";
            }
        }

        public int RecordCount()
        {
            if (Response != null)
            {
                return (int)Response.Total;
            }
            else
            {
                return 0;
            }
        }

        public int FirstRecordNumber()
        {
            if (AutoQuery?.Skip != null)
            {
                return (int)AutoQuery.Skip + 1;
            }
            else
            {
                return 0;
            }
        }

        public int LastRecordNumber()
        {
            if (Response == null)
            {
                return 0;
            }
            else
            {
                if (AutoQuery.Skip != null)
                {
                    return ((int)AutoQuery.Skip + (int)AutoQuery.Take > Response.Total) ? Response.Total : (int)AutoQuery.Skip + (int)AutoQuery.Take;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int CurrentPageDisplayNumber()
        {
            return CurrentPageNumber() + 1;
        }

        public int CurrentPageNumber()
        {
            if (AutoQuery?.Skip != null)
            {
                return (int)AutoQuery.Skip / (int)AutoQuery.Take;
            }
            else
            {
                return 0;
            }
        }

        public int PageDisplayCount()
        {
            return PageCount() + 1;
        }

        public int PageCount()
        {
            if (Response != null)
            {
                return (Response.Total - 1) / (int)AutoQuery.Take;
            }
            else
            {
                return 0;
            }
        }

        public bool TakePreviousDisbled()
        {
            return (CurrentPageNumber() == 0);
        }

        public bool TakeNextDisbled()
        {
            return (CurrentPageNumber() == PageCount());
        }

        public async void OnTakeNextClick()
        {
            if (AutoQuery.Skip == null)
            {
                AutoQuery.Skip = 0;
            }
            AutoQuery.Skip += AutoQuery.Take;

            if (AutoQuery.Skip > Response.Total)
            {
                AutoQuery.Skip = Response.Total - AutoQuery.Take;
            }

            await ExecuteAutoQuery();
        }

        public async void OnTakePreviousClick()
        {
            AutoQuery.Skip -= AutoQuery.Take;

            if (AutoQuery.Skip < 0)
            {
                AutoQuery.Skip = 0;
            }

            await ExecuteAutoQuery();
        }

        public async void OnPageBlockClick(int pageNumberBlock)
        {
            AutoQuery.Skip = AutoQuery.Take * pageNumberBlock;
            await ExecuteAutoQuery();
        }
        #endregion

        public async Task OnColumnHeaderClick(JiwaAutoQueryColumn<Model> column)
        {
            ShowFilterDialogColumn = column;
        }

        public async void FilterClosed(bool resultOK)
        {
            if (resultOK)
            {
                // Need to reset the skip when filters change, because we have a new record set so current record pointers are invalid otherwise pagination doesn't work right
                AutoQuery.Skip = 0;
                await ExecuteAutoQuery();
            }

            ShowFilterDialogColumn = null;
            StateHasChanged();
        }

        public string RowClass(Model item)
        {
            if (SelectedItem != null && SelectedItem.Equals(item))
            {
                return "table-success";
            }
            else
            {
                return "";
            }
        }
       
        public async void OnSelectItem(Model item)
        {
            SelectedItem = item;
            if (ItemSelectedCallbackMethod.HasDelegate)
            {
                await ItemSelectedCallbackMethod.InvokeAsync(item);
            }
        }
    }

    public struct DOMRect
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Top { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }
        public double Left { get; set; }
    }
}
