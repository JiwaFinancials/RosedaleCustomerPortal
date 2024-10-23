using Microsoft.JSInterop;

namespace JiwaCustomerPortal
{
    public class BrowserDarkModeService
    {
        private readonly IJSRuntime _js;

        public BrowserDarkModeService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task<bool> IsDarkMode()
        {
            return await _js.InvokeAsync<bool>("IsDarkMode");
        }
    }
}
