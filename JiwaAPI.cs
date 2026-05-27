using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using ServiceStack;
using System.Runtime.CompilerServices;

namespace JiwaCustomerPortal
{
    public static class JiwaAPI
    {
        // Helper method to call the Jiwa API.
        // Uses the JiwaAPIURL from the appsettings.json to determine the API Host name
        // if passed a jiwaAPISessionId, adds an ss-id cookie to the request for auth, otherwise it if an APIKey is provided, it uses that
        // Requests to authenticate will provide neither jiwaAPISessionId or jiwaAPIKey
        // caller should be try catching this and taking appropriate if there was an exception

        public static async Task<TResponse> SendAsync<TResponse>(ServiceStack.IReturn<TResponse> requestDTO, string? jiwaAPISessionId = null, string? jiwaAPIKey = null, CancellationToken cancellationToken = default)
        {                 
            using (ServiceStack.JsonApiClient client = new ServiceStack.JsonApiClient(Config.JiwaAPIURL))
            {
                if (!string.IsNullOrWhiteSpace(jiwaAPISessionId))
                {
                    client.SetCookie("ss-id", jiwaAPISessionId, null);
                }
                else
                {
                    client.BearerToken = jiwaAPIKey;
                }

                return await client.SendAsync(requestDTO, cancellationToken);
            }
        }

        public static async Task<TResponse> GetAsync<TResponse>(ServiceStack.IReturn<TResponse> requestDTO, string? jiwaAPISessionId = null, string? jiwaAPIKey = null, CancellationToken cancellationToken = default)
        {
            using (ServiceStack.JsonApiClient client = new ServiceStack.JsonApiClient(Config.JiwaAPIURL))
            {
                if (! string.IsNullOrWhiteSpace(jiwaAPISessionId))
                {                    
                    client.SetCookie("ss-id", jiwaAPISessionId, null);
                }
                else
                {
                    client.BearerToken = jiwaAPIKey;
                }
               
                return await client.GetAsync(requestDTO, cancellationToken);               
            }
        }

        public static async Task<TResponse> GetAsync<TResponse>(object requestDto, string? jiwaAPISessionId = null, string? jiwaAPIKey = null, CancellationToken cancellationToken = default)
        {
            using (ServiceStack.JsonApiClient client = new ServiceStack.JsonApiClient(Config.JiwaAPIURL))
            {
                if (!string.IsNullOrWhiteSpace(jiwaAPISessionId))
                {
                    client.SetCookie("ss-id", jiwaAPISessionId, null);
                }
                else
                {
                    client.BearerToken = jiwaAPIKey;
                }
                
                return await client.GetAsync<TResponse>(requestDto, cancellationToken);
            }
        }
        
        public static async Task<TResponse> PostAsync<TResponse>(IReturn<TResponse> requestDto, string? jiwaAPISessionId = null, string? jiwaAPIKey = null, CancellationToken cancellationToken = default)
        {
            using (ServiceStack.JsonApiClient client = new ServiceStack.JsonApiClient(Config.JiwaAPIURL))
            {
                if (!string.IsNullOrWhiteSpace(jiwaAPISessionId))
                {
                    client.SetCookie("ss-id", jiwaAPISessionId, null);
                }
                else
                {
                    client.BearerToken = jiwaAPIKey;
                }

                return await client.PostAsync<TResponse>(requestDto, cancellationToken);
            }
        }

        public static async Task<TResponse> PostAsync<TResponse>(object requestDto, string? jiwaAPISessionId = null, string? jiwaAPIKey = null, CancellationToken cancellationToken = default)
        {
            using (ServiceStack.JsonApiClient client = new ServiceStack.JsonApiClient(Config.JiwaAPIURL))
            {
                if (!string.IsNullOrWhiteSpace(jiwaAPISessionId))
                {
                    client.SetCookie("ss-id", jiwaAPISessionId, null);
                }
                else
                {
                    client.BearerToken = jiwaAPIKey;
                }

                return await client.PostAsync<TResponse>(requestDto, cancellationToken);
            }
        }

        public static async Task<TResponse> PatchAsync<TResponse>(ServiceStack.IReturn<TResponse> requestDTO, string? jiwaAPISessionId = null, string? jiwaAPIKey = null, CancellationToken cancellationToken = default)
        {
            using (ServiceStack.JsonApiClient client = new ServiceStack.JsonApiClient(Config.JiwaAPIURL))
            {
                if (!string.IsNullOrWhiteSpace(jiwaAPISessionId))
                {
                    client.SetCookie("ss-id", jiwaAPISessionId, null);
                }
                else
                {
                    client.BearerToken = jiwaAPIKey;
                }

                return await client.PatchAsync(requestDTO, cancellationToken);
            }
        }

        public static async Task<TResponse> PutAsync<TResponse>(ServiceStack.IReturn<TResponse> requestDTO, string? jiwaAPISessionId = null, string? jiwaAPIKey = null, CancellationToken cancellationToken = default)
        {
            using (ServiceStack.JsonApiClient client = new ServiceStack.JsonApiClient(Config.JiwaAPIURL))
            {
                if (!string.IsNullOrWhiteSpace(jiwaAPISessionId))
                {
                    client.SetCookie("ss-id", jiwaAPISessionId, null);
                }
                else
                {
                    client.BearerToken = jiwaAPIKey;
                }

                return await client.PutAsync(requestDTO, cancellationToken);
            }
        }

        public static async Task<TResponse> DeleteAsync<TResponse>(object requestDto, string? jiwaAPISessionId = null, string? jiwaAPIKey = null, CancellationToken cancellationToken = default)
        {
            using (ServiceStack.JsonApiClient client = new ServiceStack.JsonApiClient(Config.JiwaAPIURL))
            {
                if (!string.IsNullOrWhiteSpace(jiwaAPISessionId))
                {
                    client.SetCookie("ss-id", jiwaAPISessionId, null);
                }
                else
                {
                    client.BearerToken = jiwaAPIKey;
                }

                return await client.DeleteAsync<TResponse>(requestDto, cancellationToken);
            }
        }
    }

    public static class DateTimeExtensions
    {
        public static DateTime LastDayOfMonth(this DateTime date)
        {
            return date.AddDays(1 - (date.Day)).AddMonths(1).AddDays(-1);
        }
    }

    public static class ContactNameExtensions
    {
        public static string DisplayName(this JiwaFinancials.Jiwa.JiwaServiceModel.Debtors.DebtorContactName contactName)
        {
            return String.Join(" ", new string[] { contactName.Title, contactName.FirstName, contactName.Surname }.Where(x => !String.IsNullOrEmpty(x)));
        }
    }
}
