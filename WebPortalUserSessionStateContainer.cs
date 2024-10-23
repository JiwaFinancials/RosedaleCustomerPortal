using JiwaFinancials.Jiwa.JiwaServiceModel;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using ServiceStack;

namespace JiwaCustomerPortal
{
    // We create a container to store the current user session (servicestack session id, user contact details, debtor name
    // and we register this in Program.cs to be dependency injected when referenced - so we can access this session information
    // everywhere fairly easily by either:
    // 1. Adding @inject WebPortalUserSessionStateContainer WebPortalUserSessionStateContainer to the razor page definition
    // or
    // 2. Adding an inject property to a class, eg:
    // [Inject]
    // WebPortalUserSessionStateContainer WebPortalUserSessionStateContainer { get; set; }
    //
    // But note this is a Scoped dependency so once the user opens a new page or tab (or just refreshes) in their browser,
    // they get a new WebPortalUserSessionStateContainer which will be empty.

    public class WebPortalUserSessionStateContainer
    {
        public event Action? OnChange;
        
        public ProtectedLocalStorage? ProtectedLocalStore { get; private set; }

        private WebPortalUserSession? _WebPortalUserSession;
        public WebPortalUserSession? WebPortalUserSession 
        {
            get 
            {
                if (_WebPortalUserSession == null && ProtectedLocalStore != null)
                {
                    // retrieve from local storage
                    GetWebPortalUserSessionFromStorage();
                }
                return _WebPortalUserSession;
            }
            private set 
            {
                _WebPortalUserSession = value;
            } 
        }

        private async Task GetWebPortalUserSessionFromStorage()
        {
            ProtectedBrowserStorageResult<JiwaFinancials.Jiwa.JiwaServiceModel.WebPortalUserSession> storedWebPortalUserSession = await ProtectedLocalStore.GetAsync<JiwaFinancials.Jiwa.JiwaServiceModel.WebPortalUserSession>("JiwaCustomerWebPortalAuthUserSession");
            _WebPortalUserSession = storedWebPortalUserSession.Value;
        }

        public async Task SetProtectedLocalStore(ProtectedLocalStorage value)
        {
            ProtectedLocalStore = value;
            ProtectedBrowserStorageResult<JiwaFinancials.Jiwa.JiwaServiceModel.WebPortalUserSession> storedWebPortalUserSession = await ProtectedLocalStore.GetAsync<JiwaFinancials.Jiwa.JiwaServiceModel.WebPortalUserSession>("JiwaCustomerWebPortalAuthUserSession");
            if (storedWebPortalUserSession.Success && storedWebPortalUserSession.Value != null)
            {
                JiwaFinancials.Jiwa.JiwaServiceModel.WebPortalUserSession webPortalUserSession = storedWebPortalUserSession.Value;

                // re-authenticate with the stored session id - in case something has changed since it was last used, or if it has expired. 
                try
                {
                    JiwaFinancials.Jiwa.JiwaServiceModel.JiwaAuthUserSessionResponse jiwaAuthUserSession = await JiwaAPI.GetAsync(new JiwaFinancials.Jiwa.JiwaServiceModel.AuthCurrentSessionGETRequest(), jiwaAPISessionId: webPortalUserSession.Id);
                    JiwaFinancials.Jiwa.JiwaServiceModel.DebtorContactNameCustomerWebPortalRoleGETResponse debtorContactNameCustomerWebPortalRoleGETResponse = await JiwaAPI.GetAsync(new JiwaFinancials.Jiwa.JiwaServiceModel.DebtorContactNameCustomerWebPortalRoleGETRequest(), jiwaAPISessionId: jiwaAuthUserSession.Id);

                    if (debtorContactNameCustomerWebPortalRoleGETResponse.Roles == null || (!debtorContactNameCustomerWebPortalRoleGETResponse.Roles.Contains("User") && !debtorContactNameCustomerWebPortalRoleGETResponse.Roles.Contains("Admin")))
                    {
                        // no web portal role has been assigned to this debtor contact name; they are not allowed to sign in.
                        throw new Exception("User does not have permission to sign in using the Customer Web Portal. Add the tag 'Customer Web Portal - User' or 'Customer Web Portal - Admin' to the debtor contact name");
                    }

                    webPortalUserSession = jiwaAuthUserSession.ConvertTo<JiwaFinancials.Jiwa.JiwaServiceModel.WebPortalUserSession>();
                    webPortalUserSession.IsAdminRole = debtorContactNameCustomerWebPortalRoleGETResponse.Roles.Contains("Admin");
                    await ProtectedLocalStore.SetAsync("JiwaCustomerWebPortalAuthUserSession", webPortalUserSession);
                    SetWebPortalUserSession(webPortalUserSession);
                }
                catch (Exception ex)
                {
                    // We don't care if this failed - it's more than likely because the JiwaAPISessionId had expired, so we behave as though they never were authenticated.
                    // so just clear the stored JiwaAPIAuthUserSession                    
                    await ProtectedLocalStore.DeleteAsync("JiwaCustomerWebPortalAuthUserSession");
                    SetWebPortalUserSession(null);
                }
            }            
        }

        public void SetWebPortalUserSession(WebPortalUserSession? value)
        {
            WebPortalUserSession = value;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
