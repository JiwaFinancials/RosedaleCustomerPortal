using JiwaFinancials.Jiwa.JiwaServiceModel;
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
        public static string AppSettingsIN_LogicalID { get; set; }
        public static string AppSettingsLogicalWarehouseDescription { get; set; }
        public static string AppSettingsIN_PhysicalID { get; set; }
        public static string AppSettingsPhysicalWarehouseDescription { get; set; }
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

        // Currencies are a dictionary we populate the first time requested.
        // We do this so everyone doesn't need to pull back currency images for each row of data - they can get it from here if they know the CurrencyID
        public static System.Collections.Generic.Dictionary<string, JiwaFinancials.Jiwa.JiwaServiceModel.Tables.FX_Currency> _Currencies;
        public static System.Collections.Generic.Dictionary<string, JiwaFinancials.Jiwa.JiwaServiceModel.Tables.FX_Currency> Currencies
        {
            get 
            { 
                if (_Currencies == null)
                {
                    // lazy load currencies
                    JiwaFinancials.Jiwa.JiwaServiceModel.Tables.FX_CurrencyQuery currencyAutoQuery = new JiwaFinancials.Jiwa.JiwaServiceModel.Tables.FX_CurrencyQuery();
                    currencyAutoQuery.IsEnabled = true;

                    try
                    {
                        Task readCurrenciesTask = Task.Run( async () =>
                        {
                            ServiceStack.QueryResponse<JiwaFinancials.Jiwa.JiwaServiceModel.Tables.FX_Currency> currencyAutoQueryResponse = await JiwaAPI.GetAsync(currencyAutoQuery, jiwaAPIKey: JiwaAPIKey);

                            _Currencies = new System.Collections.Generic.Dictionary<string, JiwaFinancials.Jiwa.JiwaServiceModel.Tables.FX_Currency>();
                            foreach (JiwaFinancials.Jiwa.JiwaServiceModel.Tables.FX_Currency currency in currencyAutoQueryResponse.Results)
                            {
                                _Currencies.Add(currency.RecID, currency);

                                if (currency.IsLocal)
                                {
                                    _LocalCurrency = currency;
                                }   
                            }
                        });

                        readCurrenciesTask.Wait();
                    }
                    catch (Exception ex)
                    {
                        // TODO: we might want to wrap this or somehow indicate to the user why this failed.
                        throw;
                    }
                }

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

        public static async Task ReadSettingsFromAPI()
        {
            CustomerWebPortalSettings response = await JiwaAPI.GetAsync(new CustomerWebPortalSettingsGETRequest(), jiwaAPIKey: JiwaAPIKey);
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

            ApplyWarehouseFallbackFromAppSettings();

            if (string.IsNullOrWhiteSpace(LogicalWarehouseDescription))
            {
                LogicalWarehouseDescription = "All Logical Warehouses";
            }

            if (string.IsNullOrWhiteSpace(PhysicalWarehouseDescription))
            {
                PhysicalWarehouseDescription = "All Physical Warehouses";
            }

            JiwaAPISystemInformation = await JiwaAPI.GetAsync(new SystemInformationGETRequest(), jiwaAPIKey: JiwaAPIKey);
        }

        private static void ApplyWarehouseFallbackFromAppSettings()
        {
            if (string.IsNullOrWhiteSpace(IN_LogicalID))
            {
                IN_LogicalID = AppSettingsIN_LogicalID ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(LogicalWarehouseDescription))
            {
                LogicalWarehouseDescription = AppSettingsLogicalWarehouseDescription ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(IN_PhysicalID))
            {
                IN_PhysicalID = AppSettingsIN_PhysicalID ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(PhysicalWarehouseDescription))
            {
                PhysicalWarehouseDescription = AppSettingsPhysicalWarehouseDescription ?? string.Empty;
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
}
