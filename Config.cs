using JiwaFinancials.Jiwa.JiwaServiceModel;
using JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields;
using ServiceStack.Html;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

namespace JiwaCustomerPortal
{
    public static class Config
    {
        public static bool ShowDiagnostics { get; set; }
        // JiwaAPIURL is the URL of the remote Jiwa API.
        public static string? JiwaAPIURL { get; set; }
        // JiwaAPIKey is the API Key to use to perform some requests (such as getting the list of debtor contacts for a given email address to disambiguate identities at login time)
        // The key should be attached to a user with minimal permisssions, and does not need an interactive Jiwa licence.
        public static string? JiwaAPIKey { get; set; }
        public static bool AllowCustomerLogin { get; set; }
        public static bool AllowStaffLogin { get; set; }

        public static SystemInformationGETResponse JiwaAPISystemInformation { get; set; }

        public static string SalesOrderReport { get; set; }
        public static string SalesQuoteReport { get; set; }
        public static string DebtorStatementReport { get; set; }
        public static string CustomerWebPortalPluginVersion { get; set; }
        public static string DocketNumHeader { get; set; }
        public static string IN_LogicalID { get; set; }
        public static string LogicalWarehouseDescription { get; set; }
        public static string IN_PhysicalID { get; set; }
        public static string PhysicalWarehouseDescription { get; set; }

        public static string _ServiceStackJsonAPIClientVersion;
        public static string ServiceStackJsonAPIClientVersion
        {
            get
            {
                if (_ServiceStackJsonAPIClientVersion == null)
                {
                    System.Reflection.Assembly assembly = typeof(ServiceStack.JsonApiClient).Assembly;
                    System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
                    _ServiceStackJsonAPIClientVersion = fvi.FileVersion;
                }

                return _ServiceStackJsonAPIClientVersion;
            }
        }
        
        public static System.Collections.Generic.Dictionary<string, JiwaFinancials.Jiwa.JiwaServiceModel.Tables.FX_Currency> _Currencies;
        public static System.Collections.Generic.Dictionary<string, JiwaFinancials.Jiwa.JiwaServiceModel.Tables.FX_Currency> Currencies
        {
            get 
            {                 
                return _Currencies;
            }            
        }

        private static JiwaFinancials.Jiwa.JiwaServiceModel.Tables.FX_Currency _LocalCurrency;
        public static JiwaFinancials.Jiwa.JiwaServiceModel.Tables.FX_Currency LocalCurrency
        {
            get
            {
                return _LocalCurrency;
            }
        }

        private static CustomFieldsToDisplay _CustomFieldsToDisplay;
        public static CustomFieldsToDisplay CustomFieldsToDisplay
        {
            get
            {
                return _CustomFieldsToDisplay;
            }
            set
            {
                _CustomFieldsToDisplay = value;
            }
        }

        #region "Sales Order"
        private static List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> _SalesOrderCustomFields;

        public static List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> SalesOrderCustomFields
        {
            get
            {
                return _SalesOrderCustomFields;
            }
        }

        private static List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> _StaffLogin_SalesOrderCustomFields = new List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField>();

        public static List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> StaffLogin_SalesOrderCustomFields
        {
            get
            {
                return _StaffLogin_SalesOrderCustomFields;
            }
        }

        private static List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> _CustomerLogin_SalesOrderCustomFields = new List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField>();

        public static List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> CustomerLogin_SalesOrderCustomFields
        {
            get
            {
                return _CustomerLogin_SalesOrderCustomFields;
            }
        }

        private static List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> _SalesOrderHistoryCustomFields;

        public static List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> SalesOrderHistoryCustomFields
        {
            get
            {
                return _SalesOrderHistoryCustomFields;
            }
        }

        private static List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> _StaffLogin_SalesOrderHistoryCustomFields = new List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField>();

        public static List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> StaffLogin_SalesOrderHistoryCustomFields
        {
            get
            {
                return _StaffLogin_SalesOrderHistoryCustomFields;
            }
        }

        private static List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> _CustomerLogin_SalesOrderHistoryCustomFields = new List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField>();

        public static List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> CustomerLogin_SalesOrderHistoryCustomFields
        {
            get
            {
                return _CustomerLogin_SalesOrderHistoryCustomFields;
            }
        }

        private static List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> _SalesOrderLineCustomFields;

        public static List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> SalesOrderLineCustomFields
        {
            get
            {
                return _SalesOrderLineCustomFields;
            }
        }

        private static List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> _StaffLogin_SalesOrderLineCustomFields = new List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField>();

        public static List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> StaffLogin_SalesOrderLineCustomFields
        {
            get
            {
                return _StaffLogin_SalesOrderLineCustomFields;
            }
        }

        private static List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> _CustomerLogin_SalesOrderLineCustomFields = new List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField>();

        public static List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> CustomerLogin_SalesOrderLineCustomFields
        {
            get
            {
                return _CustomerLogin_SalesOrderLineCustomFields;
            }
        }
        #endregion

        #region "Sale Quote Custom Fields"
        private static List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> _SalesQuoteCustomFields;

        public static List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> SalesQuoteCustomFields
        {
            get
            {
                return _SalesQuoteCustomFields;
            }
        }

        private static List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> _StaffLogin_SalesQuoteCustomFields = new List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField>();

        public static List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> StaffLogin_SalesQuoteCustomFields
        {
            get
            {
                return _StaffLogin_SalesQuoteCustomFields;
            }
        }

        private static List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> _CustomerLogin_SalesQuoteCustomFields = new List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField>();

        public static List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> CustomerLogin_SalesQuoteCustomFields
        {
            get
            {
                return _CustomerLogin_SalesQuoteCustomFields;
            }
        }

        private static List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> _SalesQuoteLineCustomFields;

        public static List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> SalesQuoteLineCustomFields
        {
            get
            {
                return _SalesQuoteLineCustomFields;
            }
        }

        private static List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> _StaffLogin_SalesQuoteLineCustomFields = new List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField>();

        public static List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> StaffLogin_SalesQuoteLineCustomFields
        {
            get
            {
                return _StaffLogin_SalesQuoteLineCustomFields;
            }
        }

        private static List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> _CustomerLogin_SalesQuoteLineCustomFields = new List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField>();

        public static List<JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields.CustomField> CustomerLogin_SalesQuoteLineCustomFields
        {
            get
            {
                return _CustomerLogin_SalesQuoteLineCustomFields;
            }
        }
        #endregion

        public static async Task ReadSettingsFromAPI(CancellationToken cancellationToken = default)
        {
            CustomerWebPortalSettings response = await JiwaAPI.GetAsync(new CustomerWebPortalSettingsGETRequest(), jiwaAPIKey: JiwaAPIKey, cancellationToken: cancellationToken);
            SalesOrderReport = response.SalesOrderReport;
            SalesQuoteReport = response.SalesQuoteReport;
            DebtorStatementReport = response.DebtorStatementReport;
            CustomerWebPortalPluginVersion = response.PluginVersion;
            DocketNumHeader = response.DocketNumHeader;

            // Some plugin/API versions do not populate warehouse fields on CustomerWebPortalSettings.
            IN_LogicalID = response.IN_LogicalID ?? string.Empty;
            LogicalWarehouseDescription = response.LogicalWarehouseDescription ?? string.Empty;
            IN_PhysicalID = response.IN_PhysicalID ?? string.Empty;
            PhysicalWarehouseDescription = response.PhysicalWarehouseDescription ?? string.Empty;

            if (string.IsNullOrWhiteSpace(LogicalWarehouseDescription))
            {
                LogicalWarehouseDescription = "All Logical Warehouses";
            }

            if (string.IsNullOrWhiteSpace(PhysicalWarehouseDescription))
            {
                PhysicalWarehouseDescription = "All Physical Warehouses";
            }

            JiwaAPISystemInformation = await JiwaAPI.GetAsync(new SystemInformationGETRequest(), jiwaAPIKey: JiwaAPIKey, cancellationToken: cancellationToken);

            // Read the currencies
            JiwaFinancials.Jiwa.JiwaServiceModel.Tables.FX_CurrencyQuery currencyAutoQuery = new JiwaFinancials.Jiwa.JiwaServiceModel.Tables.FX_CurrencyQuery();
            currencyAutoQuery.IsEnabled = true;
            ServiceStack.QueryResponse<JiwaFinancials.Jiwa.JiwaServiceModel.Tables.FX_Currency> currencyAutoQueryResponse = await JiwaAPI.GetAsync(currencyAutoQuery, jiwaAPIKey: JiwaAPIKey, cancellationToken: cancellationToken);
            _Currencies = new System.Collections.Generic.Dictionary<string, JiwaFinancials.Jiwa.JiwaServiceModel.Tables.FX_Currency>();
            foreach (JiwaFinancials.Jiwa.JiwaServiceModel.Tables.FX_Currency currency in currencyAutoQueryResponse.Results)
            {
                _Currencies.Add(currency.RecID, currency);

                if (currency.IsLocal)
                {
                    _LocalCurrency = currency;
                }
            }            

            // Read the custom field definitions            
            _SalesOrderCustomFields = await JiwaAPI.GetAsync(new SalesOrderCustomFieldsGETManyRequest(), jiwaAPIKey: JiwaAPIKey, cancellationToken: cancellationToken);
            foreach (CustomField customField in Config.SalesOrderCustomFields)
            {
                if (Config.CustomFieldsToDisplay?.StaffLogin?.Where(x => x.ModuleName == "Sales Order" && x.PluginName == customField.PluginName && x.CustomFieldNames.Contains(customField.SettingName)).FirstOrDefault() != null)
                {
                    _StaffLogin_SalesOrderCustomFields.Add(customField);
                }

                if (Config.CustomFieldsToDisplay?.CustomerLogin?.Where(x => x.ModuleName == "Sales Order" && x.PluginName == customField.PluginName && x.CustomFieldNames.Contains(customField.SettingName)).FirstOrDefault() != null)
                {
                    _CustomerLogin_SalesOrderCustomFields.Add(customField);
                }
            }

            _SalesOrderHistoryCustomFields = await JiwaAPI.GetAsync(new SalesOrderHistoryCustomFieldsGETManyRequest(), jiwaAPIKey: JiwaAPIKey, cancellationToken: cancellationToken);
            foreach (CustomField customField in Config.SalesOrderHistoryCustomFields)
            {
                if (Config.CustomFieldsToDisplay.StaffLogin?.Where(x => x.ModuleName == "Sales Order History" && x.PluginName == customField.PluginName && x.CustomFieldNames.Contains(customField.SettingName)).FirstOrDefault() != null)
                {
                    _StaffLogin_SalesOrderHistoryCustomFields.Add(customField);
                }

                if (Config.CustomFieldsToDisplay.CustomerLogin?.Where(x => x.ModuleName == "Sales Order History" && x.PluginName == customField.PluginName && x.CustomFieldNames.Contains(customField.SettingName)).FirstOrDefault() != null)
                {
                    _CustomerLogin_SalesOrderHistoryCustomFields.Add(customField);
                }
            }

            _SalesOrderLineCustomFields = await JiwaAPI.GetAsync(new SalesOrderLineCustomFieldsGETManyRequest(), jiwaAPIKey: JiwaAPIKey, cancellationToken: cancellationToken);
            foreach (CustomField customField in Config.SalesOrderLineCustomFields)
            {
                if (Config.CustomFieldsToDisplay?.StaffLogin?.Where(x => x.ModuleName == "Sales Order Line" && x.PluginName == customField.PluginName && x.CustomFieldNames.Contains(customField.SettingName)).FirstOrDefault() != null)
                {
                    _StaffLogin_SalesQuoteLineCustomFields.Add(customField);
                }

                if (Config.CustomFieldsToDisplay.CustomerLogin?.Where(x => x.ModuleName == "Sales Order Line" && x.PluginName == customField.PluginName && x.CustomFieldNames.Contains(customField.SettingName)).FirstOrDefault() != null)
                {
                    _CustomerLogin_SalesOrderLineCustomFields.Add(customField);
                }
            }

            _SalesQuoteCustomFields = await JiwaAPI.GetAsync(new SalesQuoteCustomFieldsGETManyRequest(), jiwaAPIKey: JiwaAPIKey, cancellationToken: cancellationToken);
            foreach (CustomField customField in Config.SalesQuoteCustomFields)
            {
                if (Config.CustomFieldsToDisplay?.StaffLogin?.Where(x => x.ModuleName == "Sales Quote" && x.PluginName == customField.PluginName && x.CustomFieldNames.Contains(customField.SettingName)).FirstOrDefault() != null)
                {
                    _StaffLogin_SalesQuoteCustomFields.Add(customField);
                }

                if (Config.CustomFieldsToDisplay?.CustomerLogin?.Where(x => x.ModuleName == "Sales Quote" && x.PluginName == customField.PluginName && x.CustomFieldNames.Contains(customField.SettingName)).FirstOrDefault() != null)
                {
                    _CustomerLogin_SalesQuoteCustomFields.Add(customField);
                }
            }

            _SalesQuoteLineCustomFields = await JiwaAPI.GetAsync(new SalesQuoteLineCustomFieldsGETManyRequest(), jiwaAPIKey: JiwaAPIKey, cancellationToken: cancellationToken);
            
            foreach (CustomField customField in Config.SalesQuoteLineCustomFields)
            {                                
                if (Config.CustomFieldsToDisplay?.StaffLogin?.Where(x => x.ModuleName == "Sales Quote Line" && x.PluginName == customField.PluginName && x.CustomFieldNames.Contains(customField.SettingName)).FirstOrDefault() != null)
                {
                    _StaffLogin_SalesQuoteLineCustomFields.Add(customField);
                }
                                
                if (Config.CustomFieldsToDisplay?.CustomerLogin?.Where(x => x.ModuleName == "Sales Quote Line" && x.PluginName == customField.PluginName && x.CustomFieldNames.Contains(customField.SettingName)).FirstOrDefault() != null)
                {
                    _CustomerLogin_SalesQuoteLineCustomFields.Add(customField);
                }                
            }
        }

        public static string FormattedDecimals(decimal value, short decimalPlaces, bool useCommas = true)
        {            
            string decimalsFormat = new string('0', decimalPlaces);
            if (useCommas)
            {
                return value.ToString($"###,###,###,###,##0.{decimalsFormat}");
            }
            else
            {
                return value.ToString($"##############0.{decimalsFormat}");
            }
        }

        public static string FormattedDecimals(decimal? value, short decimalPlaces, bool useCommas = true)
        {
            if (value != null)
            {
                return FormattedDecimals(value.Value, decimalPlaces, useCommas);
            }
            else
            {
                return "";
            }
        }

        public static string FormattedDecimals(decimal? value, short? decimalPlaces, bool useCommas = true)
        {
            if (decimalPlaces == null)
            {
                return FormattedDecimals(value, 0, useCommas);
            }
            else
            {
                return FormattedDecimals(value, decimalPlaces.Value, useCommas);
            }
        }

        public static short? CurrencyDecimals(string CurrencyID)
        {
            JiwaFinancials.Jiwa.JiwaServiceModel.Tables.FX_Currency currency = _Currencies[CurrencyID];

            if (currency != null)
            {
                return currency.DecimalPlaces;
            }
            else
            {
                return 0;
            }
        }

        public static string FormattedCurrency(decimal value, string CurrencyID)
        {
            JiwaFinancials.Jiwa.JiwaServiceModel.Tables.FX_Currency currency = _Currencies[CurrencyID];

            if (currency != null)
            {                
                string decimalsFormat = new string('0', (int)currency.DecimalPlaces);
                string currencyFormat = $"###,###,###,###,##0.{decimalsFormat}";
                if (value < 0)
                {
                    return $"-{currency.Symbol}{Math.Abs(value).ToString(currencyFormat)}";
                }
                else
                {
                    return $"{currency.Symbol}{value.ToString(currencyFormat)}";
                }
            }
            else
            {
                return value.ToString();
            }
        }

        public static string FormattedCurrency(decimal? value, string CurrencyID)
        {
            if (value != null)
            {
                return FormattedCurrency(value.Value, CurrencyID);
            }            
            else
            {
                return "";
            }
        }

        public static string? GetTargetFrameworkName()
        {
            return Assembly
                .GetEntryAssembly()?
                .GetCustomAttribute<TargetFrameworkAttribute>()?
                .FrameworkName;
        }

        public static string BootStrapVersion { get; set; }
    }

    public class CustomFieldsToDisplay
    {
        public List<AllowedCustomField> CustomerLogin { get; set; }
        public List<AllowedCustomField>StaffLogin { get; set; }
    }

    public class AllowedCustomField
    {
        public string ModuleName { get; set; }
        public string PluginName { get; set; }
        public List<string> CustomFieldNames { get; set; }
    }
}
