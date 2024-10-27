using JiwaFinancials.Jiwa.JiwaServiceModel;
using ServiceStack.Html;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace JiwaCustomerPortal
{
    public static class Config
    {
        // JiwaAPIURL is the URL of the remote Jiwa API.
        public static string JiwaAPIURL { get; set; }
        // JiwaAPIKey is the API Key to use to perform some requests (such as getting the list of debtor contacts for a given email address to disambiguate identities at login time)
        // The key should be attached to a user with minimal permisssions, and does not need an interactive Jiwa licence.
        public static string JiwaAPIKey { get; set; }

        public static string SalesOrderReport { get; set; }
        public static string SalesQuoteReport { get; set; }
        public static string DebtorStatementReport { get; set; }

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

                    //try
                    //{
                    Task readCurrenciesTask = Task.Run( async () =>
                    {
                        ServiceStack.QueryResponse<JiwaFinancials.Jiwa.JiwaServiceModel.Tables.FX_Currency> currencyAutoQueryResponse = await JiwaAPI.GetAsync(currencyAutoQuery, jiwaAPIKey: JiwaAPIKey);

                        _Currencies = new System.Collections.Generic.Dictionary<string, JiwaFinancials.Jiwa.JiwaServiceModel.Tables.FX_Currency>();
                        foreach (JiwaFinancials.Jiwa.JiwaServiceModel.Tables.FX_Currency currency in currencyAutoQueryResponse.Results)
                        {
                            _Currencies.Add(currency.RecID, currency);
                        }
                    });

                    readCurrenciesTask.Wait();
                    //}
                    //catch (Exception ex)
                    //{
                    //    // TODO: we might want to wrap this or somehow indicate to the user why this failed.
                    //    throw;
                    //}
                }

                return _Currencies;
            }            
        }

        public static async Task ReadSettingsFromAPI()
        {
            CustomerWebPortalSettings response = await JiwaAPI.GetAsync(new CustomerWebPortalSettingsGETRequest(), jiwaAPIKey: JiwaAPIKey);
            SalesOrderReport = response.SalesOrderReport;
            SalesQuoteReport = response.SalesQuoteReport;
            DebtorStatementReport = response.DebtorStatementReport;
        }

        public static string FormattedCurrency(decimal value, string CurrencyID)
        {
            JiwaFinancials.Jiwa.JiwaServiceModel.Tables.FX_Currency currency = _Currencies[CurrencyID];

            if (currency != null)
            {
                string decimalsFormat = new string('0', (int)currency.DecimalPlaces);
                string currencyFormat = $"###,###,###,###,###.{decimalsFormat}";
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
    }
}
