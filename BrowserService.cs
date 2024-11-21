using Microsoft.JSInterop;

namespace JiwaCustomerPortal
{
    public class BrowserService
    {
        private readonly IJSRuntime _js;

        public BrowserService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task<bool> IsDarkMode()
        {
            return await _js.InvokeAsync<bool>("IsDarkMode");
        }

        public async Task<string> BootstrapVersion()
        {
            return await _js.InvokeAsync<string>("BootstrapVersion");
        }

        // Function to return a date in the users locale format
        public async Task<string> DateToLocaleString(DateTime date)
        {
            return await _js.InvokeAsync<string>("DateToLocaleString", new object[] { date.Year, date.Month, date.Day });
        }

        public async Task GetUserLocalDateFormat()
        {
            try
            {
                // we just ask javascript to convert an unambiguous date to a string, and we use either US or rest of the world date format based on 
                // whether the day is the first part of the string or not
                string userdate = await DateToLocaleString(new DateTime(2024, 12, 30));

                if (userdate.StartsWith("30") || userdate.StartsWith("31"))
                {
                    DateFormat = "dd/MM/yyyy";
                }
                else
                {
                    DateFormat = "MM/dd/yyyy";
                }
            }
            catch (Exception ex)
            {
                // We don't care if this had an issue - we already have the default set and we were just trying to be super nice in displaying dates in
                // the users date format, but we don't care.
            }
        }

        public string DateFormat { get; set; } = "dd/MM/yyyy";
    }
}
