using ServiceStack;
using ServiceStack.Text.Pools;

namespace JiwaCustomerPortal.Components.Pages
{
    public partial class Events
    {
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
        private bool DisplayDeregisterConfirmationModalDialog { get; set; } = false;
        private bool DisplayRegisterConfirmationModalDialog { get; set; } = false;

        List <EligibleEvent> EligibleEvents { get; set; } = new List<EligibleEvent>();

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

            CustomerWebPortalEligibleEventsGETRequest customerWebPortalEligibleEventsGETRequest = new CustomerWebPortalEligibleEventsGETRequest();
            EligibleEvents = await SendToAPI<List<EligibleEvent>>(customerWebPortalEligibleEventsGETRequest);
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

        private async Task SendToAPI(IReturnVoid requestDTO)
        {
            APIRequestInProgressCount++;
            try
            {
                await JiwaAPI.PostAsync<ServiceStack.IReturnVoid>(requestDTO, WebPortalUserSessionStateContainer.WebPortalUserSession.Id, null);
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

        private async Task ToggleViewSessions(EligibleEvent eligibleEvent)
        {
            eligibleEvent.isExpanded = !eligibleEvent.isExpanded;
            //StateHasChanged();
        }

        private async Task RegisterAttendance(EligibleEvent eligibleEvent, EventSession eventSession)
        {
            CustomerWebPortalRegisterSessionPOSTRequest customerWebPortalRegisterSessionPOSTRequest = new CustomerWebPortalRegisterSessionPOSTRequest() 
            { 
                EventID = eligibleEvent.EventID.ToString(),
                SessionID = eventSession.SessionID.ToString()
            };
            await SendToAPI(customerWebPortalRegisterSessionPOSTRequest);

            // store the expanded states of the events, so when we re-read we can re-displaythem with the same states

            DisplayRegisterConfirmationModalDialog = true;
            //StateHasChanged();
        }

        private async Task DeregisterAttendance(EligibleEvent eligibleEvent, EventSession eventSession)
        {
            CustomerWebPortalDeregisterSessionDELETERequest customerWebPortalDeregisterSessionDELETERequest = new CustomerWebPortalDeregisterSessionDELETERequest()
            {
                EventID = eligibleEvent.EventID.ToString(),
                SessionID = eventSession.SessionID.ToString()
            };
            await DeleteFromAPI(customerWebPortalDeregisterSessionDELETERequest);
            DisplayDeregisterConfirmationModalDialog = true;
            RereadEventsAndPreserveExpandedState();
            //StateHasChanged();
        }

        public async void AttendanceConfirmationDialogClosed()
        {
            DisplayDeregisterConfirmationModalDialog = false;
            DisplayRegisterConfirmationModalDialog = false;
            RereadEventsAndPreserveExpandedState();
            //StateHasChanged();
        }

        public async void RereadEventsAndPreserveExpandedState()
        {
            List<Guid> ExpandedEventIDs = EligibleEvents.Where(x => x.isExpanded).Select(x => x.EventID).ToList();

            CustomerWebPortalEligibleEventsGETRequest customerWebPortalEligibleEventsGETRequest = new CustomerWebPortalEligibleEventsGETRequest();
            EligibleEvents = await SendToAPI<List<EligibleEvent>>(customerWebPortalEligibleEventsGETRequest);

            foreach(EligibleEvent eligibleEvent in EligibleEvents)
            {
                eligibleEvent.isExpanded = ExpandedEventIDs.Contains(eligibleEvent.EventID);
            }
            StateHasChanged();
        }
    }
}
