using JiwaCustomerPortal.Components.Grid.CustomField;
using JiwaCustomerPortal.Components.Pages;
using JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields;
using JiwaFinancials.Jiwa.JiwaServiceModel.Debtors;
using JiwaFinancials.Jiwa.JiwaServiceModel.Notes;
using JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders;
using JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes;
using JiwaFinancials.Jiwa.JiwaServiceModel.Staff;
using JiwaFinancials.Jiwa.JiwaServiceModel.Startup.Diagnostics;
using JiwaFinancials.Jiwa.JiwaServiceModel.Tables;
using JiwaFinancials.Jiwa.JiwaServiceModel.Tags;
using JiwaFinancials.Jiwa.JiwaServiceModel.Tax;
using ServiceStack;
using ServiceStack.DataAnnotations;
using ServiceStack.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Runtime.Serialization;
using static JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrder;
using static JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuote;

#region "Rosedale Licence DTOs"
#region "Licencing"
namespace JiwaFinancials.Jiwa.JiwaServiceModel.Licencing
{
    public partial class Licence
    {
        public virtual string LicenceID { get; set; }
        public virtual string ResourceID { get; set; }
        public virtual string ResourceName { get; set; }
        public virtual DateTime CommenceDate { get; set; }
        public virtual DateTime ExpiryDate { get; set; }
        public virtual string LicenceType { get; set; }
        public virtual bool NonInteractive { get; set; }
        public virtual string Username { get; set; }
        public virtual int CALs { get; set; }
        public virtual string Version { get; set; }
        public virtual DateTime IssueDate { get; set; }
        public virtual string CompanyName { get; set; }
        public virtual string Note { get; set; }
        public virtual string Scope { get; set; }
        public virtual string Signature { get; set; }
        public virtual DateTimeOffset? LastSavedDateTime { get; set; }
    }
}
#endregion
public class CustomerName
{
    public string DebtorID { get; set; }
    public string Name { get; set; }
    public int CALs { get; set; }
    public bool IsMaintained { get; set; }
    public DateTime LicenceExpiration { get; set; }

    // We add this to the DTO for convenience when displaying the licences page.  This gets lazy loaded.
    public List<RegistrationLicences> RegistrationLicences { get; set; }
    public bool LicencesHaveBeenRead { get; set; }
    public bool isExpanded { get; set; }
}

public class RegistrationLicences
{
    public string RecID { get; set; }
    public string RegistrationName { get; set; }
    public string ResourceName { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime CommenceDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int LicenceType { get; set; }
    public int CALs { get; set; }
    public string UserName { get; set; }
    public string Version { get; set; }
    public bool NonInteractive { get; set; }
}

[Route("/LicensedCustomerNames", "GET")]
[ApiResponse(Description = "OK", StatusCode = 200)]
[ApiResponse(Description = "Not authenticated", StatusCode = 401)]
[ApiResponse(Description = "Not authorised", StatusCode = 403)]
[ApiResponse(Description = "No results found", StatusCode = 404)]
public class LicensedCustomerNamesGETRequest : IReturn<List<CustomerName>>
{
}

[Route("/CustomerLicences/{DebtorID}", "GET")]
[ApiResponse(Description = "OK", StatusCode = 200)]
[ApiResponse(Description = "Not authenticated", StatusCode = 401)]
[ApiResponse(Description = "Not authorised", StatusCode = 403)]
[ApiResponse(Description = "No results found", StatusCode = 404)]
public class CustomerLicencesGETRequest : IReturn<List<RegistrationLicences>>
{
    public string DebtorID { get; set; }
}

[Route("/Licence/{LicenceID}", "GET")]
[ApiResponse(Description = "OK", StatusCode = 200)]
[ApiResponse(Description = "Not authenticated", StatusCode = 401)]
[ApiResponse(Description = "Not authorised", StatusCode = 403)]
[ApiResponse(Description = "No Licence with the provided LicenceID was found", StatusCode = 404)]
public class LicenceGETRequest : IReturn<ServiceStack.Web.IHttpResult>
{
    public string LicenceID { get; set; }
    public bool AsAttachment { get; set; }
}

[ApiResponse(Description = "OK", StatusCode = 200)]
[ApiResponse(Description = "Not authenticated", StatusCode = 401)]
[ApiResponse(Description = "Not authorised", StatusCode = 403)]
public class LicenceRenewGETRequest : IReturn<List<JiwaFinancials.Jiwa.JiwaServiceModel.Licencing.Licence>>
{
    public string CompanyName { get; set; }
    public string? Version { get; set; }
}

public class WebDownloadLink
{
    public string GroupDescription { get; set; }
    public int GroupDisplayOrder { get; set; }
    public string DisplayText { get; set; }
    public int DisplayOrder { get; set; }
    public string Size { get; set; }
    public string URL { get; set; }
    public string Comment { get; set; }
    public DateTime PublishDate { get; set; }
    public bool GroupExpanded { get; set; } // Added property only used for rendering downloads page
}

[Route("/Downloads", "GET")]
[ApiResponse(Description = "OK", StatusCode = 200)]
[ApiResponse(Description = "Not authenticated", StatusCode = 401)]
[ApiResponse(Description = "Not authorised", StatusCode = 403)]
[ApiResponse(Description = "No downloads found", StatusCode = 404)]
public class WebDownloadLinksGETRequest : IReturn<List<WebDownloadLink>>
{
}
#endregion

#region "DTOs purpose made for this app"
namespace JiwaFinancials.Jiwa.JiwaServiceModel
{
    public class WebPortalUserSession
    {
        // this is just the standard JiwaAuthUserSessionResponse class with some fields removed and some extra fields - like the Web Portal role IsAdminRole
        // We didn't want to persist in the browser storage all the fields, and we wanted to add the IsAdminRole field, so this class is used instead of the JiwaAuthUserSessionResponse 
        public virtual string Id { get; set; }
        public virtual string DebtorContactNameID { get; set; }
        public virtual string DebtorContactNameTitle { get; set; }
        public virtual string DebtorContactNameFirstName { get; set; }
        public virtual string DebtorContactNameSurname { get; set; }
        public virtual string DebtorContactNameEmailAddress { get; set; }
        public virtual string DebtorID { get; set; }
        public virtual string DebtorName { get; set; }
        public virtual bool IsAdminRole { get; set; }

        public virtual string AuthProvider { get; set; }
        public virtual string JiwaStaffID { get; set; }
        public virtual string JiwaStaffUsername { get; set; }
        public virtual string JiwaStaffTitle { get; set; }
        public virtual string JiwaStaffFirstname { get; set; }
        public virtual string JiwaStaffSurname { get; set; }
        public virtual string JiwaStaffEmailAddress { get; set; }
    }
}
#endregion

// Everything below was obtained by visiting http://{API Hostname}/types/csharp, which generates all the DTO classes for the API, and copy pasting into here only the DTOs needed

#region "Web Portal DTOs - added to the API by the Customer Web Portal Plugin"
namespace JiwaFinancials.Jiwa.JiwaServiceModel
{
    [Route("/CustomerWebPortal/Role", "GET")]
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    [ApiResponse(Description = "No role was found", StatusCode = 404)]
    public partial class DebtorContactNameCustomerWebPortalRoleGETRequest
            : IReturn<DebtorContactNameCustomerWebPortalRoleGETResponse>
    {
    }

    public partial class DebtorContactNameCustomerWebPortalRoleGETResponse
    {
        public virtual List<string>? Roles { get; set; }
    }

    [Route("/CustomerWebPortal/Settings", "GET")]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    public partial class CustomerWebPortalSettingsGETRequest
        : IReturn<CustomerWebPortalSettings>
    {
    }

    public partial class CustomerWebPortalSettings
    {
        public virtual string SalesOrderReport { get; set; }
        public virtual string SalesQuoteReport { get; set; }
        public virtual string DebtorStatementReport { get; set; }
        public virtual string PluginVersion { get; set; }
        public virtual string DocketNumHeader { get; set; }
        public virtual string IN_LogicalID { get; set; }
        public virtual string IN_PhysicalID { get; set; }
        public virtual string LogicalWarehouseDescription { get; set; }
        public virtual string PhysicalWarehouseDescription { get; set; }
    }

    [Route("/Debtors/ContactNames/{ContactNameID}/PasswordReset", "POST")]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    [ApiResponse(Description = "No debtor contact name with the ContactNameID provided was found", StatusCode = 404)]
    [ApiResponse(Description = "Password reset request generated and emailed", StatusCode = 204)]
    public partial class DebtorContactNameResetPasswordPOSTRequest : IReturnVoid
    {
        public virtual string ContactNameID { get; set; }
        public virtual string ResetURL { get; set; }
    }

    [Route("/Debtors/ContactNames/{Token}/TokenisedPasswordChange", "POST")]
    [ApiResponse(401, "Not authenticated")]
    [ApiResponse(403, "Not authorised")]
    [ApiResponse(404, "No debtor contact name with the ContactNameID provided was not found")]
    [ApiResponse(204, "Password changed successfully")]
    public partial class DebtorContactNameTokenisedChangePasswordPOSTRequest : IReturnVoid
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }

    [Route("/Debtors/ContactNames/{ContactNameID}/PasswordChange", "POST")]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    [ApiResponse(Description = "No debtor contact name with the ContactNameID provided was not found", StatusCode = 404)]
    [ApiResponse(Description = "Password changed successfully", StatusCode = 204)]
    public partial class DebtorContactNameChangePasswordPOSTRequest
       : IReturnVoid
    {
        public virtual string ContactNameID { get; set; }
        public virtual string ExistingPassword { get; set; }
        public virtual string NewPassword { get; set; }
    }

    [Route("/Staff/{Username}/PasswordReset", "POST")]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    [ApiResponse(Description = "No staff member with the Username provided was not found", StatusCode = 404)]
    [ApiResponse(Description = "Password reset request generated and emailed", StatusCode = 204)]
    public partial class StaffResetPasswordPOSTRequest
       : IReturnVoid
    {
        public virtual string Username { get; set; }
        public virtual string ResetURL { get; set; }
    }

    [Route("/Staff/{Token}/TokenisedPasswordChange", "POST")]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    [ApiResponse(Description = "No staff member with the StaffID provided was not found", StatusCode = 404)]
    [ApiResponse(Description = "Password changed successfully", StatusCode = 204)]
    public partial class StaffTokenisedChangePasswordPOSTRequest
        : IReturnVoid
    {
        public virtual string Token { get; set; }
        public virtual string NewPassword { get; set; }
    }

    [Route("/Staff/{StaffID}/PasswordChange", "POST")]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    [ApiResponse(Description = "No staff member with the StaffID provided was not found", StatusCode = 404)]
    [ApiResponse(Description = "Password changed successfully", StatusCode = 204)]
    public partial class StaffChangePasswordPOSTRequest
       : IReturnVoid
    {
        public virtual string StaffID { get; set; }
        public virtual string ExistingPassword { get; set; }
        public virtual string NewPassword { get; set; }
    }
}
#endregion

#region "Standard Jiwa API DTOs"
#region "Request DTOs"
namespace JiwaFinancials.Jiwa.JiwaServiceModel
{
    #region "System"
    [Route("/SystemInfo/", "GET")]
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    public partial class SystemInformationGETRequest
        : IReturn<SystemInformationGETResponse>
    {
    }

    public partial class SystemInformationGETResponse
    {
        public virtual string JiwaVersion { get; set; }
        public virtual string JiwaRESTAPIPluginVersion { get; set; }
        public virtual string ServiceStackVersion { get; set; }
        public virtual string DotNETVersion { get; set; }
        public virtual string OSVersion { get; set; }
        public virtual DateTime SQLServerDateTime { get; set; }
        public virtual string CacheProvider { get; set; }
        public virtual string DatabaseName { get; set; }
        public virtual string DatabaseServer { get; set; }
        public virtual string SQLVersion { get; set; }
        public virtual string LicensedCompany { get; set; }
        public virtual string CurrencyName { get; set; }
        public virtual string CurrencyShortName { get; set; }
        public virtual short MoneyDecimalPlaces { get; set; }
    }
    #endregion region

    #region "Authentication"
    [Route("/Sessions/Current", "GET")]
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    public partial class AuthCurrentSessionGETRequest
        : IReturn<JiwaAuthUserSessionResponse>
    {
    }

    public partial class JiwaAuthUserSessionResponse
    {
        public virtual string Id { get; set; }
        public virtual string AuthProvider { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual string UserName { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string APIKey_Type { get; set; }
        public virtual string PrincipalID { get; set; }
        public virtual string JiwaStaffID { get; set; }
        public virtual string JiwaStaffUsername { get; set; }
        public virtual string JiwaStaffTitle { get; set; }
        public virtual string JiwaStaffFirstname { get; set; }
        public virtual string JiwaStaffSurname { get; set; }
        public virtual string JiwaStaffEmailAddress { get; set; }
        public virtual byte[] JiwaStaffPicture { get; set; }
        public virtual string DebtorContactNameID { get; set; }
        public virtual string DebtorContactNameTitle { get; set; }
        public virtual string DebtorContactNameFirstName { get; set; }
        public virtual string DebtorContactNameSurname { get; set; }
        public virtual string DebtorContactNameEmailAddress { get; set; }
        public virtual string DebtorID { get; set; }
        public virtual string DebtorName { get; set; }
    }

    [Route("/auth/logout", "GET")]
    [ApiResponse(Description = "logged out OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    public partial class LogoutGetRequest
        : IReturn<LogoutGetResponse>
    {
    }

    public partial class LogoutGetResponse
    {
        public virtual string Username { get; set; }
        public virtual DateTime LoginDateTime { get; set; }
        public virtual DateTime LogoutDateTime { get; set; }
    }
    #endregion

    #region "Reports"
    [Route("/SalesOrders/{InvoiceHistoryID}/InvoiceSnapshotReport/{ReportID}", "GET")]
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    [ApiResponse(Description = "No Sales Order with the InvoiceHistoryID, or Report with the ReportID provided was found", StatusCode = 404)]
    public partial class InvoiceSnapshotReportGETRequest
        : IReturn<IHttpResult>
    {
        public virtual string InvoiceHistoryID { get; set; }
        public virtual string ReportID { get; set; }
        public virtual bool AsAttachment { get; set; }
    }

    [Route("/SalesQuotes/{QuoteHistoryID}/QuoteSnapshotReport/{ReportID}", "GET")]
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    [ApiResponse(Description = "No Sales Quote with the QuoteHistoryID, or Report with the ReportID provided was found", StatusCode = 404)]
    public partial class SalesQuoteSnapshotReportGETRequest
        : IReturn<IHttpResult>
    {
        public virtual string QuoteHistoryID { get; set; }
        public virtual string ReportID { get; set; }
        public virtual bool AsAttachment { get; set; }
    }
    #endregion

    #region "Debtors"
    [Route("/Debtors/{DebtorID}/StatementReport/{ReportID}/At/{AsAtDate}", "GET")]
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    [ApiResponse(Description = "No Debtor with Debtor ID, or Report with the ReportID provided was found", StatusCode = 404)]
    public partial class DebtorStatementReportGETRequest
        : IReturn<IHttpResult>
    {
        public virtual string DebtorID { get; set; }
        public virtual DateTime AsAtDate { get; set; }
        public virtual string ReportID { get; set; }
        public virtual bool AsAttachment { get; set; }
    }

    [Route("/Debtors", "GET")]
    [ApiResponse(401, "Not authenticated")]
    [ApiResponse(403, "Not authorised")]
    public class CustomerDebtorGETRequest : IReturn<JiwaServiceModel.Debtors.Debtor>
    {
    }

    [Route("/Debtors/{DebtorID}", "GET")]
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    [ApiResponse(Description = "No debtor with the DebtorID provided was found", StatusCode = 404)]
    public partial class DebtorGETRequest
        : IReturn<Debtor>
    {
        public virtual string DebtorID { get; set; }
    }

    [Route("/Debtors/{DebtorID}/ContactNames", "GET")]
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    [ApiResponse(Description = "No debtor contact name with the DebtorID or ContactNameID provided was found", StatusCode = 404)]
    public partial class DebtorContactNamesGETManyRequest
        : IReturn<List<DebtorContactName>>
    {
        public virtual string DebtorID { get; set; }
    }

    [Route("/Debtors/{DebtorID}/ContactNames", "POST")]
    [ApiResponse(Description = "Created OK", StatusCode = 201)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    [ApiResponse(Description = "No debtor contact name with the DebtorID or ContactNameID provided was found", StatusCode = 404)]
    public partial class DebtorContactNamePOSTRequest
        : DebtorContactName, IReturn<DebtorContactName>
    {
        [IgnoreDataMember]
        public virtual string ContactNameID { get; set; }

        [IgnoreDataMember]
        public virtual DateTimeOffset? LastSavedDateTime { get; set; }

        [IgnoreDataMember]
        public virtual bool? LogonCodeChangedByUser { get; set; }

        public virtual string DebtorID { get; set; }
    }

    [Route("/Debtors/{DebtorID}/ContactNames/{ContactNameID}", "PATCH")]
    [ApiResponse(Description = "Updated OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    [ApiResponse(Description = "No debtor contact name with the DebtorID or ContactNameID provided was found", StatusCode = 404)]
    public partial class DebtorContactNamePATCHRequest
        : DebtorContactName, IReturn<DebtorContactName>
    {
        [IgnoreDataMember]
        public virtual DateTimeOffset? LastSavedDateTime { get; set; }

        [IgnoreDataMember]
        public virtual bool? LogonCodeChangedByUser { get; set; }

        public virtual string DebtorID { get; set; }
        public virtual string ContactNameID { get; set; }
    }

    [Route("/Debtors/{DebtorID}/ContactNames/{ContactNameID}", "DELETE")]
    [ApiResponse(Description = "Deleted OK", StatusCode = 204)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    [ApiResponse(Description = "No debtor contact name with the DebtorID or ContactNameID provided was found", StatusCode = 404)]
    public partial class DebtorContactNameDELETERequest
        : DebtorContactName, IReturnVoid
    {
        public virtual string DebtorID { get; set; }
        public virtual string ContactNameID { get; set; }
    }

    [Route("/Debtors/ContactNamesTag", "GET")]
    [ApiResponse(Description = "Read Ok", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    public partial class DebtorContactNameTagGETManyRequest
        : IReturn<List<DebtorContactNameTag>>
    {
    }

    [Route("/Debtors/{DebtorID}/ContactNames/{ContactNameID}/TagMembership", "PUT")]
    [ApiResponse(Description = "Updated OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    [ApiResponse(Description = "No debtor contact name with the DebtorID or ContactNameID provided was found", StatusCode = 404)]
    public partial class DebtorContactNameTagMembershipPUTRequest
    : IReturn<List<Tag>>
    {
        public virtual string DebtorID { get; set; }
        public virtual string ContactNameID { get; set; }
        public virtual List<Tag> Tags { get; set; }
    }
    #endregion

    #region "Inventory"
    [Route("/Inventory/{InventoryID}/Images", "GET")]
    [ApiResponse(Description = "Read Ok", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    [ApiResponse(Description = "No inventory with the InventoryID provided was found", StatusCode = 404)]
    public partial class InventoryImageGETManyRequest
        : IReturn<List<JiwaFinancials.Jiwa.JiwaServiceModel.Inventory.InventoryImage>>
    {
        public virtual string InventoryID { get; set; }
    }

    [Route("/Inventory/Picture", "GET")]
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    [ApiResponse(Description = "No inventory with the InventoryID or PartNo provided was found", StatusCode = 404)]
    public partial class InventoryPictureGETRequest
        : IReturn<IHttpResult>
    {
        public virtual string InventoryID { get; set; }
        public virtual string PartNo { get; set; }
        public virtual bool AsAttachment { get; set; }
    }

    [Route("/Inventory/{InventoryID}/Pricing/{DebtorID}/{IN_LogicalID}/{Date}/{Quantity}", "GET")]
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    public partial class InventoryPriceGETRequest
        : IReturn<JiwaFinancials.Jiwa.JiwaServiceModel.Inventory.InventoryPriceGETResponse>
    {
        public virtual string InventoryID { get; set; }
        public virtual string DebtorID { get; set; }
        public virtual string IN_LogicalID { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual decimal Quantity { get; set; }
    }
    #endregion

    #region "Sales Orders"
    [Route("/SalesOrders/{InvoiceID}", "GET")]
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    [ApiResponse(Description = "No Sales Order with the InvoiceID provided was found", StatusCode = 404)]
    public partial class SalesOrderGETRequest
        : IReturn<JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrder>
    {
        public virtual string InvoiceID { get; set; }
    }

    [Route("/SalesOrders", "POST")]
    [ApiResponse(Description = "Created OK", StatusCode = 201)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    public partial class SalesOrderPOSTRequest
        : JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrder, IReturn<JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrder>
    {
        [IgnoreDataMember]
        public virtual string InvoiceID { get; set; }

        [IgnoreDataMember]
        public virtual SalesOrderStatuses? Status { get; set; }

        [IgnoreDataMember]
        public virtual DateTimeOffset? LastSavedDateTime { get; set; }

        [IgnoreDataMember]
        public virtual DateTime? DeliveredDate { get; set; }

        [IgnoreDataMember]
        public virtual bool? Delivered { get; set; }

        [IgnoreDataMember]
        public virtual DateTime? RCTIDate { get; set; }

        [IgnoreDataMember]
        public virtual string StaffTitle { get; set; }

        [IgnoreDataMember]
        public virtual string DebtorName { get; set; }

        [IgnoreDataMember]
        public virtual string DebtorEmailAddress { get; set; }
    }

    [Route("/SalesOrders/{InvoiceID}", "PATCH")]
    [ApiResponse(Description = "Updated OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    [ApiResponse(Description = "No Sales Order with the InvoiceID provided was found", StatusCode = 404)]
    public partial class SalesOrderPATCHRequest
        : JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrder, IReturn<JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrder>
    {
        [IgnoreDataMember]
        public virtual string Type { get; set; }

        [IgnoreDataMember]
        public virtual string DebtorID { get; set; }

        [IgnoreDataMember]
        public virtual string DebtorAccountNo { get; set; }

        [IgnoreDataMember]
        public virtual DateTimeOffset? LastSavedDateTime { get; set; }

        [IgnoreDataMember]
        public virtual DateTime? DeliveredDate { get; set; }

        [IgnoreDataMember]
        public virtual bool? Delivered { get; set; }

        [IgnoreDataMember]
        public virtual DateTime? RCTIDate { get; set; }

        [IgnoreDataMember]
        public virtual string StaffTitle { get; set; }

        [IgnoreDataMember]
        public virtual string DebtorName { get; set; }

        [IgnoreDataMember]
        public virtual string DebtorEmailAddress { get; set; }

        [IgnoreDataMember]
        public virtual string LogicalID { get; set; }

        [IgnoreDataMember]
        public virtual string LogicalWarehouseDescription { get; set; }

        [IgnoreDataMember]
        public virtual string PhysicalWarehouseDescription { get; set; }

        public virtual string InvoiceID { get; set; }
    }

    [Route("/SalesOrders/{InvoiceID}/Historys/{InvoiceHistoryID}/Lines", "POST")]
    [ApiResponse(Description = "Created OK", StatusCode = 201)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    [ApiResponse(Description = "No Sales Order Line with the InvoiceID or InvoiceLineID provided was found", StatusCode = 404)]
    public partial class SalesOrderLinePOSTRequest
        : JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderLine, IReturn<JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders.SalesOrderLine>
    {
        [IgnoreDataMember]
        public virtual string InvoiceLineID { get; set; }

        [IgnoreDataMember]
        public virtual decimal? PriceExGst { get; set; }

        [IgnoreDataMember]
        public virtual decimal? TaxToCharge { get; set; }

        [IgnoreDataMember]
        public virtual decimal? UnitCost { get; set; }

        [IgnoreDataMember]
        public virtual bool? FixPrice { get; set; }

        [IgnoreDataMember]
        public virtual decimal? LineTotal { get; set; }

        [IgnoreDataMember]
        public virtual decimal? Weight { get; set; }

        [IgnoreDataMember]
        public virtual decimal? Cubic { get; set; }

        [IgnoreDataMember]
        public virtual decimal? QuotedDiscountedPrice { get; set; }

        [IgnoreDataMember]
        public virtual decimal? QuotedDiscountPercentage { get; set; }

        [IgnoreDataMember]
        public virtual short? QuantityDecimalPlaces { get; set; }

        [IgnoreDataMember]
        public virtual decimal? QuantityOriginalOrdered { get; set; }

        [IgnoreDataMember]
        public virtual bool? NonInventory { get; set; }

        [IgnoreDataMember]
        public virtual string CostCenter { get; set; }

        [IgnoreDataMember]
        public virtual string Stage { get; set; }

        [IgnoreDataMember]
        public virtual SalesOrderKitLineTypesEnum? KitLineType { get; set; }

        [IgnoreDataMember]
        public virtual decimal? KitUnits { get; set; }

        [IgnoreDataMember]
        public virtual string KitHeaderLineID { get; set; }

        public virtual string InvoiceID { get; set; }
        public virtual string InvoiceHistoryID { get; set; }
    }

    [Route("/SalesOrders/{InvoiceID}/Historys/{InvoiceHistoryID}/Lines/{InvoiceLineID}", "DELETE")]
    [ApiResponse(Description = "Deleted OK", StatusCode = 204)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    [ApiResponse(Description = "No Sales Order Line with the InvoiceID or InvoiceLineID provided was found", StatusCode = 404)]
    public partial class SalesOrderLineDELETERequest
        : IReturnVoid
    {
        public virtual string InvoiceID { get; set; }
        public virtual string InvoiceHistoryID { get; set; }
        public virtual string InvoiceLineID { get; set; }
    }

    [Route("/SalesOrders/CustomFields", "GET")]
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    public partial class SalesOrderCustomFieldsGETManyRequest
        : IReturn<List<CustomField>>
    {
    }

    [Route("/SalesOrders/Historys/CustomFields", "GET")]
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    public partial class SalesOrderHistoryCustomFieldsGETManyRequest
       : IReturn<List<CustomField>>
    {
    }

    [Route("/SalesOrders/Lines/CustomFields", "GET")]
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    public partial class SalesOrderLineCustomFieldsGETManyRequest
        : IReturn<List<CustomField>>
    {
    }
    #endregion

    #region "Sales Quotes"
    [Route("/SalesQuotes/{QuoteID}", "GET")]
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    [ApiResponse(Description = "No Sales Quote with the QuoteID provided was found", StatusCode = 404)]
    public partial class SalesQuoteGETRequest
        : IReturn<JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuote>
    {
        public virtual string QuoteID { get; set; }
    }

    [Route("/SalesQuotes", "POST")]
    [ApiResponse(Description = "Created OK", StatusCode = 201)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    public class SalesQuotePOSTRequest
        : JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuote, IReturn<JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuote>
    {
        [IgnoreDataMember]
        public string QuoteID { get; set; }

        [IgnoreDataMember]
        public e_SalesQuoteStatuses? Status { get; set; }

        [IgnoreDataMember]
        public DateTimeOffset? LastSavedDateTime { get; set; }

        [IgnoreDataMember]
        public string StaffTitle { get; set; }

        [IgnoreDataMember]
        public string DebtorName { get; set; }

        [IgnoreDataMember]
        public string DebtorEmailAddress { get; set; }
    }

    [Route("/SalesQuotes/{QuoteID}", "PATCH")]
    [ApiResponse(Description = "Updated OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    [ApiResponse(Description = "No Sales Quote with the QuoteID provided was found", StatusCode = 404)]
    public partial class SalesQuotePATCHRequest
        : JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuote, IReturn<JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuote>
    {
        [IgnoreDataMember]
        public virtual string Type { get; set; }

        [IgnoreDataMember]
        public virtual string DebtorID { get; set; }

        [IgnoreDataMember]
        public virtual string DebtorAccountNo { get; set; }

        [IgnoreDataMember]
        public virtual DateTimeOffset? LastSavedDateTime { get; set; }

        [IgnoreDataMember]
        public virtual string StaffTitle { get; set; }

        [IgnoreDataMember]
        public virtual string DebtorName { get; set; }

        [IgnoreDataMember]
        public virtual string DebtorEmailAddress { get; set; }

        [IgnoreDataMember]
        public virtual string LogicalID { get; set; }

        [IgnoreDataMember]
        public virtual string LogicalWarehouseDescription { get; set; }

        [IgnoreDataMember]
        public virtual string PhysicalWarehouseDescription { get; set; }

        public virtual string QuoteID { get; set; }
    }

    [Route("/SalesQuotes/{QuoteID}/Historys/{QuoteHistoryID}/Lines", "POST")]
    [ApiResponse(Description = "Created OK", StatusCode = 201)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    [ApiResponse(Description = "No Sales Quote Line with the QuoteID or QuoteLineID provided was found", StatusCode = 404)]
    public partial class SalesQuoteLinePOSTRequest
        : JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuoteLine, IReturn<JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes.SalesQuoteLine>
    {
        [IgnoreDataMember]
        public virtual string QuoteLineID { get; set; }

        [IgnoreDataMember]
        public virtual decimal? TaxToCharge { get; set; }

        [IgnoreDataMember]
        public virtual decimal? UnitCost { get; set; }

        [IgnoreDataMember]
        public virtual decimal? LineTotal { get; set; }

        [IgnoreDataMember]
        public virtual decimal? Weight { get; set; }

        [IgnoreDataMember]
        public virtual decimal? Cubic { get; set; }

        [IgnoreDataMember]
        public virtual short? QuantityDecimalPlaces { get; set; }

        [IgnoreDataMember]
        public virtual decimal? QuantityOriginalOrdered { get; set; }

        [IgnoreDataMember]
        public virtual SalesQuoteKitLineTypesEnum? KitLineType { get; set; }

        [IgnoreDataMember]
        public virtual decimal? KitUnits { get; set; }

        [IgnoreDataMember]
        public virtual string KitHeaderLineID { get; set; }

        public virtual string QuoteID { get; set; }
        public virtual string QuoteHistoryID { get; set; }
    }

    [Route("/SalesQuotes/{QuoteID}/Historys/{QuoteHistoryID}/Lines/{QuoteLineID}", "DELETE")]
    [ApiResponse(Description = "Deleted OK", StatusCode = 204)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    [ApiResponse(Description = "No Sales Quote Line with the QuoteID or QuoteLineID provided was found", StatusCode = 404)]
    public partial class SalesQuoteLineDELETERequest
        : IReturnVoid
    {
        public virtual string QuoteID { get; set; }
        public virtual string QuoteHistoryID { get; set; }
        public virtual string QuoteLineID { get; set; }
    }

    [Route("/SalesQuotes/CustomFields", "GET")]
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    public partial class SalesQuoteCustomFieldsGETManyRequest
        : IReturn<List<CustomField>>
    {
    }

    [Route("/SalesQuotes/Lines/CustomFields", "GET")]
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    public partial class SalesQuoteLineCustomFieldsGETManyRequest
        : IReturn<List<CustomField>>
    {
    }

    #endregion
}
#endregion

#region "Models"
#region "Custom Fields"
namespace JiwaFinancials.Jiwa.JiwaServiceModel.CustomFields
{
    public enum CellTypes
    {
        Date = 0,
        Text = 1,
        Float = 2,
        Integer = 3,
        Lookup = 7,
        Combo = 8,
        Checkbox = 10,
    }

    public partial class CustomField
    {
        public virtual string SettingID { get; set; }
        public virtual string SettingName { get; set; }
        public virtual string PluginID { get; set; }
        public virtual string PluginName { get; set; }
        public virtual CellTypes CellType { get; set; }
        public virtual int DisplayOrder { get; set; }
        public virtual short? DecimalPlaces { get; set; }
        public virtual decimal? DecimalMinValue { get; set; }
        public virtual decimal? DecimalMaxValue { get; set; }
        public virtual int? IntegerMinValue { get; set; }
        public virtual int? IntegerMaxValue { get; set; }
        public virtual DateTime? DateMinValue { get; set; }
        public virtual DateTime? DateMaxValue { get; set; }
        public virtual int? TextMaxLength { get; set; }
        public virtual Dictionary<string, string> LookupProviders { get; set; }
        public virtual Dictionary<string, string> LookupDisplayValueResolvers { get; set; }
        public virtual Dictionary<string, string> ComboKeyValuePairs { get; set; }

        // LookupButtonScript and LookupDisplayValueResolverScript added for our purposes here - this is not populated or used by the Jiwa REST API
        public virtual Microsoft.CodeAnalysis.Scripting.Script LookupButtonScript { get; set; }
        public virtual Microsoft.CodeAnalysis.Scripting.Script LookupDisplayValueResolverScript { get; set; }
        public virtual Exception? LookupDisplayValueResolverScriptException { get; set; }
        public virtual Exception? LookupButtonClickScriptException { get; set; }
    }

    public partial class CustomFieldValue
    {
        public virtual string SettingID { get; set; }
        public virtual string SettingName { get; set; }
        public virtual string Contents { get; set; }
        public virtual string PluginID { get; set; }
        public virtual string PluginName { get; set; }
        // We added DisplayValue for our use with lookups - this is not populated or used by the Jiwa REST API - we populate it using resolvers.
        public virtual string DisplayValue { get; set; }
    }

}

#endregion

#region "Debtors"
namespace JiwaFinancials.Jiwa.JiwaServiceModel.Debtors
{
    // Properties not relevant are commented out so we don't have to have their dto types defined here either 
    // So Classification is commented out, because we don't use it but if we did we'd need to define the DebtorClassification DTO here also
    #region "Debtors"
    public partial class Debtor
    {
        public virtual decimal? CreditLimit { get; set; }
        public virtual int? EarlyPaymentDiscountDays { get; set; }
        public virtual decimal? EarlyPaymentDiscountAmount { get; set; }
        public virtual DateTime? LastPurchaseDate { get; set; }
        public virtual DateTime? LastPaymentDate { get; set; }
        public virtual decimal? StandingDiscountOnInvoices { get; set; }
        public virtual bool? AccountOnHold { get; set; }
        public virtual decimal? CurrentBalance { get; set; }
        public virtual decimal? Period1Balance { get; set; }
        public virtual decimal? Period2Balance { get; set; }
        public virtual decimal? Period3Balance { get; set; }
        public virtual decimal? Period4Balance { get; set; }
        public virtual bool? NotifyRequired { get; set; }
        public virtual bool? WebAccess { get; set; }
        public virtual DateTime? CommenceDate { get; set; }
        public virtual TradingStatuses? TradingStatus { get; set; }
        public virtual PeriodTypes? PeriodType { get; set; }
        public virtual bool? UsesFX { get; set; }
        public virtual bool? IsCashOnly { get; set; }
        public virtual int? TermsDays { get; set; }
        public virtual TermsTypes? TermsType { get; set; }
        public virtual bool? ExcludeFromAging { get; set; }
        public virtual bool? DebtorIsBranchAccount { get; set; }
        public virtual decimal? RemainingNormalPrepaidLabourPackHours { get; set; }
        public virtual decimal? RemainingSpecialPrepaidLabourPackHours { get; set; }
        public virtual short? FXDecimalPlaces { get; set; }
        public virtual string DebtorID { get; set; }
        public virtual string ProspectID { get; set; }
        public virtual DateTimeOffset? LastSavedDateTime { get; set; }
        public virtual string AccountNo { get; set; }
        public virtual string AltAccountNo { get; set; }
        public virtual string Name { get; set; }
        public virtual string Address1 { get; set; }
        public virtual string Address2 { get; set; }
        public virtual string Address3 { get; set; }
        public virtual string Address4 { get; set; }
        public virtual string Postcode { get; set; }
        public virtual string Country { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Fax { get; set; }
        public virtual string EmailAddress { get; set; }
        public virtual string ACN { get; set; }
        public virtual string ABN { get; set; }
        public virtual string AustPostDPID { get; set; }
        public virtual string AustPostBCSP { get; set; }
        public virtual string BankName { get; set; }
        public virtual string BankAccountNo { get; set; }
        public virtual string BankBSBN { get; set; }
        public virtual string BankAccountName { get; set; }
        public virtual string TaxExemptionNo { get; set; }
        public virtual string NotifyAddress { get; set; }
        public virtual string ParentDebtorID { get; set; }
        public virtual string ParentDebtorAccountNo { get; set; }
        public virtual string ParentDebtorName { get; set; }
        public virtual string PriceSchemeID { get; set; }
        public virtual string PriceSchemeDescription { get; set; }
        public virtual string TradingName { get; set; }
        public virtual string CompanyName { get; set; }
        public virtual string ProprietorsName { get; set; }
        public virtual string FaxHeader { get; set; }
        public virtual string DefaultCurrencyID { get; set; }
        public virtual string DefaultCurrencyName { get; set; }
        public virtual string DefaultCurrencyShortName { get; set; }
        public virtual short? DefaultCurrencyDecimalPlaces { get; set; }
        public virtual string BPayReference { get; set; }
        //public virtual DebtorClassification Classification { get; set; }
        //public virtual DebtorCategory Category1 { get; set; }
        //public virtual DebtorCategory Category2 { get; set; }
        //public virtual DebtorCategory Category3 { get; set; }
        //public virtual DebtorCategory Category4 { get; set; }
        //public virtual DebtorCategory Category5 { get; set; }
        public virtual List<DebtorContactName> ContactNames { get; set; }
        //public virtual List<DebtorGroupMembership> GroupMemberships { get; set; }
        //public virtual List<DebtorBranchDebtor> BranchDebtors { get; set; }
        public virtual List<DebtorDeliveryAddress> DeliveryAddresses { get; set; }
        //public virtual List<DebtorFreightForwarderAddress> FreightForwarderAddresses { get; set; }
        //public virtual List<Note> Notes { get; set; }
        //public virtual List<Note> CreditNotes { get; set; }
        //public virtual List<DebtorDirector> Directors { get; set; }
        //public virtual List<DebtorBudget> Budgets { get; set; }
        //public virtual List<DebtorPartNumber> DebtorPartNumbers { get; set; }
        //public virtual List<CustomFieldValue> CustomFieldValues { get; set; }
        //public virtual List<Document> Documents { get; set; }
        //public virtual List<DebtorSystem> DebtorSystems { get; set; }
        //public virtual List<DebtorLedger> DebtorLedgers { get; set; }
        //public virtual List<Tag> TagMemberships { get; set; }
        public virtual List<DebtorBalance> Balances { get; set; }
        public enum TradingStatuses
        {
            e_DebtorTradingStatusInActive,
            e_DebtorTradingStatusActive,
        }

        public enum PeriodTypes
        {
            Weekly,
            Fortnightly,
            Monthly,
            Custom,
        }

        public enum TermsTypes
        {
            Invoice,
            Statement,
        }

    }

    public partial class DebtorBalance
    {
        public virtual string CurrencyID { get; set; }
        public virtual string CurrencyName { get; set; }
        public virtual string CurrencyShortName { get; set; }
        public virtual short? CurrencyDecimalPlaces { get; set; }
        public virtual decimal? Period1 { get; set; }
        public virtual decimal? Period2 { get; set; }
        public virtual decimal? Period3 { get; set; }
        public virtual decimal? Period4 { get; set; }
        public virtual decimal? Total { get; set; }
        public virtual decimal? FXPeriod1 { get; set; }
        public virtual decimal? FXPeriod2 { get; set; }
        public virtual decimal? FXPeriod3 { get; set; }
        public virtual decimal? FXPeriod4 { get; set; }
        public virtual decimal? FXTotal { get; set; }
    }

    //public partial class DebtorBranchDebtor
    //{
    //    public virtual string DebtorID { get; set; }
    //    public virtual string AccountNo { get; set; }
    //    public virtual string Name { get; set; }
    //    public virtual DateTimeOffset? LastSavedDateTime { get; set; }
    //}

    //public partial class DebtorBudget
    //{
    //    public virtual string BudgetID { get; set; }
    //    public virtual DebtorMonth Month { get; set; }
    //    public virtual decimal? LastBudget { get; set; }
    //    public virtual decimal? CurrentBudget { get; set; }
    //    public virtual decimal? NextBudget { get; set; }
    //}

    public partial class DebtorContactName
    {
        public virtual bool? DefaultContact { get; set; }
        public virtual bool? DebtorContact { get; set; }
        public virtual bool? CreditorContact { get; set; }
        public virtual string ContactNameID { get; set; }
        public virtual string ContactID { get; set; }
        public virtual string AccountNo { get; set; }
        public virtual string Title { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string Surname { get; set; }
        public virtual string PrimaryPositionID { get; set; }
        public virtual string PrimaryPositionName { get; set; }
        public virtual string SecondaryPositionID { get; set; }
        public virtual string SecondaryPositionName { get; set; }
        public virtual string TertiaryPositionID { get; set; }
        public virtual string TertiaryPositionName { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Mobile { get; set; }
        public virtual string Fax { get; set; }
        public virtual string EmailAddress { get; set; }
        //public virtual string ProspectID { get; set; }
        //public virtual string LogonCode { get; set; }
        //public virtual string LogonPassword { get; set; }
        public virtual string ExternalAppRecID { get; set; }
        public virtual DateTimeOffset? LastSavedDateTime { get; set; }
        //public virtual bool? LogonCodeChangedByUser { get; set; }
        public virtual string CustomerWebPortalPassword { get; set; }
        public virtual List<Tags.Tag> TagMemberships { get; set; }
        //public virtual List<CustomFieldValue> CustomFieldValues { get; set; }
    }

    public partial class DebtorContactNameTag
        : Tag
    {
    }

    public partial class DebtorDeliveryAddress
    {
        public virtual bool? IsDefault { get; set; }
        public virtual string DeliveryAddressID { get; set; }
        public virtual string DeliveryAddressName { get; set; }
        public virtual string DeliveryAddressCode { get; set; }
        public virtual string Address1 { get; set; }
        public virtual string Address2 { get; set; }
        public virtual string Address3 { get; set; }
        public virtual string Address4 { get; set; }
        public virtual string Postcode { get; set; }
        public virtual string Country { get; set; }
        public virtual string Notes { get; set; }
        public virtual string CourierDetails { get; set; }
        public virtual string EDIStoreLocationCode { get; set; }
        public virtual decimal? Latitude { get; set; }
        public virtual decimal? Longitude { get; set; }
        public virtual string EmailAddress { get; set; }
        public virtual string Phone { get; set; }
        //public virtual List<CustomFieldValue> CustomFieldValues { get; set; }
    }

    //public partial class DebtorDirector
    //{
    //    public virtual string DirectorID { get; set; }
    //    public virtual string Name { get; set; }
    //    public virtual string Address { get; set; }
    //    public virtual string OfficeHeld { get; set; }
    //}

    //public partial class DebtorFreightForwarderAddress
    //{
    //    public virtual bool? IsDefault { get; set; }
    //    public virtual string FreightForwarderAddressID { get; set; }
    //    public virtual string Address1 { get; set; }
    //    public virtual string Address2 { get; set; }
    //    public virtual string Address3 { get; set; }
    //    public virtual string Address4 { get; set; }
    //    public virtual string Country { get; set; }
    //    public virtual string Notes { get; set; }
    //    public virtual string Postcode { get; set; }
    //    public virtual string EmailAddress { get; set; }
    //    public virtual string Phone { get; set; }
    //    public virtual string CourierDetails { get; set; }
    //    public virtual decimal? Latitude { get; set; }
    //    public virtual decimal? Longitude { get; set; }
    //}

    //public partial class DebtorPartNumber
    //{
    //    public virtual string PartNumberID { get; set; }
    //    public virtual string InventoryID { get; set; }
    //    public virtual string PartNo { get; set; }
    //    public virtual string DebtorPartNo { get; set; }
    //    public virtual string DebtorBarcode { get; set; }
    //}

    public partial class DebtorBackOrder
    {
        public virtual string InvoiceID { get; set; }
        public virtual string InvoiceNo { get; set; }
        public virtual string CustomerOrderNo { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual decimal Quantity { get; set; }
        public virtual short QuantityDecimalPlaces { get; set; }
        public virtual string Warehouse { get; set; }
        public virtual string InventoryID { get; set; }
        public virtual string PartNo { get; set; }
        public virtual string Description { get; set; }
        public virtual string CreditorID { get; set; }
        public virtual string SupplierName { get; set; }
        public virtual string OrderNo { get; set; }
        public virtual DateTime ExpectedDeliveryDate { get; set; }
        public virtual string HistoryTextComment { get; set; }
        public virtual decimal QuantityConsumed { get; set; }
    }
    #endregion
}
#endregion

#region "Inventory"
namespace JiwaFinancials.Jiwa.JiwaServiceModel.Inventory
{
    public partial class InventoryPriceGETResponse
    {
        public virtual decimal? Price { get; set; }
        public virtual bool? IncludesTax { get; set; }
    }

    public partial class InventoryImage
    {
        public InventoryImage()
        {
            FileBinary = new byte[] { };
            RowHash = new byte[] { };
        }

        public virtual string RecID { get; set; }
        public virtual byte[] FileBinary { get; set; }
        public virtual string AltText { get; set; }
        public virtual string Title { get; set; }
        public virtual string Caption { get; set; }
        public virtual string Description { get; set; }
        public virtual string PhysicalFileName { get; set; }
        public virtual DateTimeOffset? LastSavedDateTime { get; set; }
        public virtual int? ItemNo { get; set; }
        public virtual byte[] RowHash { get; set; }
        public virtual string WebStore_Image_id { get; set; }
        public virtual string WebStore_Image_src { get; set; }
        public virtual string WebStore_Image_name { get; set; }
        public virtual string WebStore_Image_altText { get; set; }
    }

    public partial class InventoryUnitOfMeasure
    {
        public virtual string RecID { get; set; }
        public virtual InventoryUnitOfMeasure InnerUnitOfMeasure { get; set; }
        public virtual decimal? QuantityInnersPerUnitOfMeasure { get; set; }
        public virtual bool? IsSell { get; set; }
        public virtual bool? IsDefaultSell { get; set; }
        public virtual bool? IsPurchase { get; set; }
        public virtual int? ItemNo { get; set; }
        public virtual DateTimeOffset? LastSavedDateTime { get; set; }
        public virtual string UnitOfMeasureID { get; set; }
        public virtual string Name { get; set; }
        public virtual string PartNo { get; set; }
        public virtual string Barcode { get; set; }
        public virtual decimal? Length { get; set; }
        public virtual decimal? Width { get; set; }
        public virtual decimal? Height { get; set; }
        public virtual decimal? Volume { get; set; }
        public virtual decimal? Weight { get; set; }
        public virtual List<CustomFieldValue> CustomFieldValues { get; set; }
        public virtual bool? IsEnabled { get; set; }
    }
}
#endregion

#region "Notes"
namespace JiwaFinancials.Jiwa.JiwaServiceModel.Notes
{
    public partial class Note
    {
        public virtual string NoteID { get; set; }
        public virtual NoteType NoteType { get; set; }
        public virtual int? LineNo { get; set; }
        public virtual DateTimeOffset? LastSavedDateTime { get; set; }
        public virtual string LastModifiedByStaffID { get; set; }
        public virtual string LastModifiedByStaffUsername { get; set; }
        public virtual string LastModifiedByStaffTitle { get; set; }
        public virtual string LastModifiedByStaffFirstName { get; set; }
        public virtual string LastModifiedByStaffSurname { get; set; }
        public virtual string NoteText { get; set; }
        public virtual byte[] RowHash { get; set; }
    }

    public partial class NoteType
    {
        public virtual string NoteTypeID { get; set; }
        public virtual string Description { get; set; }
        public virtual bool? DefaultType { get; set; }
        public virtual int? ItemNo { get; set; }
        public virtual DateTimeOffset? LastSavedDateTime { get; set; }
        public virtual byte[] RowHash { get; set; }
    }
}

#endregion

#region "Sales Order"
namespace JiwaFinancials.Jiwa.JiwaServiceModel.SalesOrders
{
    public partial class CartageCharge
    {
        public virtual decimal? ExTaxAmount { get; set; }
        public virtual decimal? FXExTaxAmount { get; set; }
        public virtual decimal? TaxAmount { get; set; }
        public virtual decimal? FXTaxAmount { get; set; }
        public virtual TaxRate TaxRate { get; set; }
    }

    public partial class CreditReason
    {
        public virtual string CreditReasonID { get; set; }
        public virtual string CreditReasonDescription { get; set; }
        public virtual bool? CreditIntoStock { get; set; }
        public virtual bool? IsEnabled { get; set; }
        public virtual bool? IsDefault { get; set; }
        public virtual DateTimeOffset? LastSavedDateTime { get; set; }
        public virtual int? ItemNo { get; set; }
    }

    public partial class DeliveryMethod
    {
        public virtual string RecID { get; set; }
        public virtual string Name { get; set; }
        public virtual bool? IsEnabled { get; set; }
        public virtual bool? IsDefault { get; set; }
        public virtual int? ItemNo { get; set; }
        public virtual DateTimeOffset? LastSavedDateTime { get; set; }
        public virtual byte[] RowHash { get; set; }
    }

    public partial class Origin
    {
        public virtual string RecID { get; set; }
        public virtual string Name { get; set; }
        public virtual bool? IsEnabled { get; set; }
        public virtual bool? IsDefault { get; set; }
        public virtual int? ItemNo { get; set; }
        public virtual DateTimeOffset? LastSavedDateTime { get; set; }
        public virtual byte[] RowHash { get; set; }
    }

    public partial class PaymentType
    {
        public virtual string PaymentTypeID { get; set; }
        public virtual string Name { get; set; }
        public virtual string Code { get; set; }
        public virtual int? ItemNo { get; set; }
        public virtual bool? IsEnabled { get; set; }
        public virtual bool? IsDefault { get; set; }
        public virtual bool? IsCreditCard { get; set; }
        public virtual bool? IsPOS { get; set; }
        //public virtual BankAccount BankAccount { get; set; } // Don't need this, so we don't include it
        public virtual DateTimeOffset? LastSavedDateTime { get; set; }
    }

    // ICustomFieldValuesHost interface added to the class declaration for our purposes here in the web portal - this is not populated or used by the Jiwa REST API
    public partial class SalesOrder : ICustomFieldValuesHost
    {
        public virtual string Type { get; set; }
        public virtual SalesOrderSystemSettings SystemSettings { get; set; }
        public virtual DateTimeOffset? LastSavedDateTime { get; set; }
        public virtual DateTime? InitiatedDate { get; set; }
        public virtual DateTime? InvoiceInitDate { get; set; }
        public virtual SalesOrderTypes? SalesOrderType { get; set; }
        public virtual SalesOrderOrderTypes? OrderType { get; set; }
        public virtual SalesOrderStatuses? Status { get; set; }
        public virtual SalesOrderEDIPickStatuses? EDIStatus { get; set; }
        public virtual SalesOrderBillTypes? BillType { get; set; }
        public virtual DateTime? ExpectedDeliveryDate { get; set; }
        public virtual DateTime? DeliveredDate { get; set; }
        public virtual bool? Delivered { get; set; }
        public virtual SalesOrderEDIPickStatuses? EDIPickStatus { get; set; }
        public virtual SalesOrderEDIOrderTypes? EDIOrderType { get; set; }
        public virtual DateTime? EDIDeliverNotBeforeDate { get; set; }
        public virtual DateTime? EDIDeliverNotAfterDate { get; set; }
        public virtual SalesOrderCashSales CashSales { get; set; }
        public virtual bool? DropShipment { get; set; }
        public virtual decimal? Cartage1ExGst { get; set; }
        public virtual decimal? FXCartage1ExGst { get; set; }
        public virtual decimal? Cartage1GstRate { get; set; }
        public virtual decimal? Cartage1Gst { get; set; }
        public virtual decimal? FXCartage1Gst { get; set; }
        public virtual decimal? Cartage2ExGst { get; set; }
        public virtual decimal? FXCartage2ExGst { get; set; }
        public virtual decimal? Cartage2GstRate { get; set; }
        public virtual decimal? Cartage2Gst { get; set; }
        public virtual decimal? FXCartage2Gst { get; set; }
        public virtual decimal? Cartage3ExGst { get; set; }
        public virtual decimal? FXCartage3ExGst { get; set; }
        public virtual decimal? Cartage3GstRate { get; set; }
        public virtual decimal? Cartage3Gst { get; set; }
        public virtual decimal? FXCartage3Gst { get; set; }
        public virtual decimal? RCTIAmount { get; set; }
        public virtual decimal? FXRCTIAmount { get; set; }
        public virtual DateTime? RCTIDate { get; set; }
        public virtual SalesOrderJobCosting JobCosting { get; set; }
        public virtual string InvoiceID { get; set; }
        public virtual string InvoiceNo { get; set; }
        public virtual string LogicalID { get; set; }
        public virtual string LogicalWarehouseDescription { get; set; }
        public virtual string PhysicalWarehouseDescription { get; set; }
        public virtual bool? CreditNote { get; set; }
        public virtual string StaffID { get; set; }
        public virtual string StaffUserName { get; set; }
        public virtual string StaffTitle { get; set; }
        public virtual string StaffFirstName { get; set; }
        public virtual string StaffSurname { get; set; }
        public virtual string BranchID { get; set; }
        public virtual string BranchDescription { get; set; }
        public virtual string BranchName { get; set; }
        public virtual string OrderNo { get; set; }
        public virtual string SOReference { get; set; }
        public virtual string SenderEDIAddress { get; set; }
        public virtual string ReceiverEDIAddress { get; set; }
        public virtual string EDIVendorNumber { get; set; }
        public virtual string EDIBuyerNumber { get; set; }
        public virtual string DebtorID { get; set; }
        public virtual string DebtorAccountNo { get; set; }
        public virtual string DebtorName { get; set; }
        public virtual string DebtorEmailAddress { get; set; }
        public virtual string DebtorContactName { get; set; }
        public virtual string EDIASN { get; set; }
        public virtual string DeliveryAddressee { get; set; }
        public virtual string DeliveryAddressPhone { get; set; }
        public virtual string DeliveryAddress1 { get; set; }
        public virtual string DeliveryAddress2 { get; set; }
        public virtual string DeliveryAddressSuburb { get; set; }
        public virtual string DeliveryAddressState { get; set; }
        public virtual string DeliveryAddressContactName { get; set; }
        public virtual string DeliveryAddressPostcode { get; set; }
        public virtual string DeliveryAddressCountry { get; set; }
        public virtual decimal? DeliveryAddressLatitude { get; set; }
        public virtual decimal? DeliveryAddressLongitude { get; set; }
        public virtual string DeliveryAddressNotes { get; set; }
        public virtual string DeliveryAddressCourierDetails { get; set; }
        public virtual string DeliveryAddressEmailAddress { get; set; }
        public virtual string RCTINo { get; set; }
        public virtual List<CustomFieldValue> CustomFieldValues { get; set; }
        public virtual List<Note> Notes { get; set; }
        public virtual List<Document> Documents { get; set; }
        public virtual List<SalesOrderPayment> Payments { get; set; }
        public virtual List<SalesOrderLine> Lines { get; set; }
        public virtual List<SalesOrderHistory> Histories { get; set; }
        public virtual List<SalesOrderASN> ASNs { get; set; }
        public virtual Origin Origin { get; set; }
        public virtual DeliveryMethod DeliveryMethod { get; set; }
        public virtual CreditReason CreditReason { get; set; }
        public virtual string CreditNoteFromInvoiceHistoryID { get; set; }
        public virtual string CurrencyID { get; set; }
        public virtual string CurrencyShortName { get; set; }
        public virtual decimal? CurrencyRate { get; set; }
        virtual public decimal? LinesOrderedExGSTTotal { get; set; }
        virtual public decimal? LinesOrderedGSTTotal { get; set; }
        virtual public decimal? LinesOrderedIncGSTTotal { get; set; }
        virtual public decimal? LinesFXOrderedExGSTTotal { get; set; }
        virtual public decimal? LinesFXOrderedGSTTotal { get; set; }
        virtual public decimal? LinesFXOrderedIncGSTTotal { get; set; }
        virtual public decimal? OrderedExGSTTotal { get; set; }
        virtual public decimal? OrderedGSTTotal { get; set; }
        virtual public decimal? OrderedIncGSTTotal { get; set; }
        virtual public decimal? FXOrderedExGSTTotal { get; set; }
        virtual public decimal? FXOrderedGSTTotal { get; set; }
        virtual public decimal? FXOrderedIncGSTTotal { get; set; }
        public enum SalesOrderTypes
        {
            e_SalesOrderNormalSalesOrder,
            e_SalesOrderBackToBack,
        }

        public enum SalesOrderOrderTypes
        {
            e_SalesOrderOrderTypeReserveOrder,
            e_SalesOrderOrderTypeInvoiceOrder,
            e_SalesOrderOrderTypeForwardOrder,
            e_SalesOrderOrderTypeActiveOrder,
        }

        public enum SalesOrderStatuses
        {
            e_SalesOrderEntered,
            e_SalesOrderProcessed,
            e_SalesOrderClosed,
            e_SalesOrderUnprocessedPrinted,
        }

        public enum SalesOrderEDIPickStatuses
        {
            e_SalesOrderHistoryEDIPickStatusNone,
            e_SalesOrderHistoryEDIPickStatusPOReceived,
            e_SalesOrderHistoryEDIPickStatusPOAcknowledgementReadyToSend,
            e_SalesOrderHistoryEDIPickStatusPOAcknowledgementSent,
            e_SalesOrderHistoryEDIPickStatusReadyToBePicked,
            e_SalesOrderHistoryEDIPickStatusPicking,
            e_SalesOrderHistoryEDIPickStatusPicked,
            e_SalesOrderHistoryEDIPickStatusASNReadyToSend,
            e_SalesOrderHistoryEDIPickStatusASNSent,
            e_SalesOrderHistoryEDIPickStatusRCTIReceived,
            e_SalesOrderHistoryEDIPickStatusError,
            e_SalesOrderHistoryEDIPickStatusRejectionReadyToSend,
            e_SalesOrderHistoryEDIPickStatusRejectionSent,
        }

        public enum SalesOrderBillTypes
        {
            e_SalesOrderShipAndBill,
            e_SalesOrderBillWhenComplete,
            e_SalesOrderShipWhenComplete,
        }

        public enum SalesOrderEDIOrderTypes
        {
            e_SalesOrderEDIOrderTypeNormal,
            e_SalesOrderEDIOrderTypeConsolidated,
        }

    }

    public partial class SalesOrderASN
    {
        public virtual string ASNNo { get; set; }
        public virtual string PurchaseOrderNo { get; set; }
        public virtual string ReceiptNo { get; set; }
        public virtual decimal? GrossAmount { get; set; }
        public virtual decimal? TotalGSTAmount { get; set; }
        public virtual DateTime? ReceiptDate { get; set; }
    }

    public partial class SalesOrderCarrier
    {
        public virtual string CarrierID { get; set; }
        public virtual string CarrierName { get; set; }
        public virtual string AccountNo { get; set; }
        public virtual SalesOrderCarrierService Service { get; set; }
        public virtual bool? UseLeastCost { get; set; }
        public virtual FreightChargeTos? ChargeTo { get; set; }
        public virtual FreightSystemStatuses? Status { get; set; }
        public virtual List<SalesOrderFreightItem> FreightItemCollection { get; set; }
        public virtual List<SalesOrderConsignmentNote> ConsignmentNoteCollection { get; set; }
        public enum FreightSystemStatuses
        {
            FreightSystemStatusNone,
            FreightSystemStatusReadyToSend,
            FreightSystemStatusSent,
            FreightSystemStatusCompleted,
        }

        public enum FreightChargeTos
        {
            FreightChargeToSender,
            FreightChargeToReceiver,
        }

    }

    public partial class SalesOrderCarrierFreightDescription
    {
        public virtual string CarrierFreightDescriptionID { get; set; }
        public virtual string Description { get; set; }
    }

    public partial class SalesOrderCarrierService
    {
        public virtual string CarrierServiceID { get; set; }
        public virtual string Name { get; set; }
        public virtual decimal? MaximumWeight { get; set; }
    }

    public partial class SalesOrderCashSales
    {
        public virtual string Name { get; set; }
        public virtual string Company { get; set; }
        public virtual string Address1 { get; set; }
        public virtual string Address2 { get; set; }
        public virtual string Address3 { get; set; }
        public virtual string Address4 { get; set; }
        public virtual string PostCode { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Fax { get; set; }
        public virtual string ContactName { get; set; }
        public virtual string Country { get; set; }
        public virtual string EmailAddress { get; set; }
    }

    public partial class SalesOrderConsignmentNote
    {
        public virtual string ConsignmentNoteID { get; set; }
        public virtual DateTime? ConsignmentNoteDate { get; set; }
        public virtual decimal? ExGSTAmount { get; set; }
        public virtual decimal? GSTAmount { get; set; }
        public virtual string ConsignmentNoteNo { get; set; }
        public virtual decimal? IncGSTAmount { get; set; }
    }

    public partial class SalesOrderFreightItem
    {
        public virtual string FreightItemID { get; set; }
        public virtual int? NumberItems { get; set; }
        public virtual decimal? ItemWeight { get; set; }
        public virtual decimal? ItemCubic { get; set; }
        public virtual decimal? ItemLength { get; set; }
        public virtual decimal? ItemWidth { get; set; }
        public virtual decimal? ItemHeight { get; set; }
        public virtual string Reference { get; set; }
        public virtual SalesOrderCarrierFreightDescription FreightDescription { get; set; }
        public virtual SalesOrderConsignmentNote ConsignmentNote { get; set; }
        public virtual List<CustomFieldValue> CustomFieldValues { get; set; }
    }

    // ICustomFieldValuesHost interface added to the class declaration for our purposes here in the web portal - this is not populated or used by the Jiwa REST API
    public partial class SalesOrderHistory : ICustomFieldValuesHost
    {
        public virtual string InvoiceHistoryID { get; set; }
        public virtual int? HistoryNo { get; set; }
        public virtual SalesOrderHistoryStatuses? Status { get; set; }
        public virtual SalesOrderHistoryEDIPickStatuses? EDIPickStatus { get; set; }
        public virtual string DBTransID { get; set; }
        public virtual string Ref { get; set; }
        public virtual string LastModifiedBy { get; set; }
        public virtual decimal? HistoryTotal { get; set; }
        public virtual decimal? AmountPaid { get; set; }
        public virtual decimal? FXAmountPaid { get; set; }
        public virtual decimal? TotalQuantityDelivered { get; set; }
        public virtual string RunNo { get; set; }
        public virtual bool? Delivered { get; set; }
        public virtual DateTime? DeliveredDate { get; set; }
        public virtual DateTime? RecordDate { get; set; }
        public virtual DateTime? DateCreated { get; set; }
        public virtual DateTime? DateLastSaved { get; set; }
        public virtual DateTime? DatePosted { get; set; }
        public virtual DateTime? DateProcessed { get; set; }
        public virtual bool? InvoicePrinted { get; set; }
        public virtual bool? DocketPrinted { get; set; }
        public virtual bool? PackSlipPrinted { get; set; }
        public virtual bool? PickSheetPrinted { get; set; }
        public virtual bool? OtherPrinted { get; set; }
        public virtual bool? InvoiceEmailed { get; set; }
        public virtual bool? DocketEmailed { get; set; }
        public virtual bool? PackSlipEmailed { get; set; }
        public virtual bool? PickSheetEmailed { get; set; }
        public virtual bool? OtherEmailed { get; set; }
        public virtual string DeliveryAddressContactName { get; set; }
        public virtual string DeliveryAddressee { get; set; }
        public virtual string DeliveryAddressPhone { get; set; }
        public virtual string DeliveryAddress1 { get; set; }
        public virtual string DeliveryAddress2 { get; set; }
        public virtual string DeliveryAddress3 { get; set; }
        public virtual string DeliveryAddress4 { get; set; }
        public virtual string DeliveryAddressPostcode { get; set; }
        public virtual string DeliveryAddressCountry { get; set; }
        public virtual decimal? DeliveryAddressLatitude { get; set; }
        public virtual decimal? DeliveryAddressLongitude { get; set; }
        public virtual string Notes { get; set; }
        public virtual string CourierDetails { get; set; }
        public virtual string DeliveryAddressEmailAddress { get; set; }
        public virtual string FreightForwardAddressPhone { get; set; }
        public virtual string FreightForwardAddress1 { get; set; }
        public virtual string FreightForwardAddress2 { get; set; }
        public virtual string FreightForwardAddress3 { get; set; }
        public virtual string FreightForwardAddress4 { get; set; }
        public virtual string FreightForwardAddressPostcode { get; set; }
        public virtual string FreightForwardAddressCountry { get; set; }
        public virtual decimal? FreightForwardAddressLatitude { get; set; }
        public virtual decimal? FreightForwardAddressLongitude { get; set; }
        public virtual string FreightForwardAddressNotes { get; set; }
        public virtual string FreightForwardAddressCourierDetails { get; set; }
        public virtual string FreightForwardAddressEmailAddress { get; set; }
        public virtual string ConsignmentNote { get; set; }
        public virtual string EDIASNNumber { get; set; }
        public virtual bool? DropShipment { get; set; }
        public virtual CartageCharge CartageCharge1 { get; set; }
        public virtual CartageCharge CartageCharge2 { get; set; }
        public virtual CartageCharge CartageCharge3 { get; set; }
        public virtual SalesOrderCarrier Carrier { get; set; }
        public virtual List<CustomFieldValue> CustomFieldValues { get; set; }
        public virtual StaffMember ProcessedBy { get; set; }
        public virtual List<SalesOrderLine> Lines { get; set; }
        virtual public decimal? LinesExGSTTotal { get; set; }
        virtual public decimal? LinesGSTTotal { get; set; }
        virtual public decimal? LinesIncGSTTotal { get; set; }
        virtual public decimal? LinesFXExGSTTotal { get; set; }
        virtual public decimal? LinesFXGSTTotal { get; set; }
        virtual public decimal? LinesFXIncGSTTotal { get; set; }
        virtual public decimal? ExGSTTotal { get; set; }
        virtual public decimal? GSTTotal { get; set; }
        virtual public decimal? IncGSTTotal { get; set; }
        virtual public decimal? FXExGSTTotal { get; set; }
        virtual public decimal? FXGSTTotal { get; set; }
        virtual public decimal? FXIncGSTTotal { get; set; }
        public enum SalesOrderHistoryStatuses
        {
            e_SalesOrderHistoryStatusEntering,
            e_SalesOrderHistoryStatusEntered,
            e_SalesOrderHistoryStatusReadyForPicking,
            e_SalesOrderHistoryStatusPicking,
            e_SalesOrderHistoryStatusPicked,
            e_SalesOrderHistoryStatusDelivery,
            e_SalesOrderHistoryStatusDelivered,
            e_SalesOrderHistoryStatusInvoicing,
            e_SalesOrderHistoryStatusInvoiced,
        }

        public enum SalesOrderHistoryEDIPickStatuses
        {
            e_SalesOrderHistoryEDIPickStatusNone,
            e_SalesOrderHistoryStatuse_SalesOrderHistoryEDIPickStatusPOReceivedEntered,
            e_SalesOrderHistoryEDIPickStatusPOAcknowledgementReadyToSend,
            e_SalesOrderHistoryEDIPickStatusPOAcknowledgementSent,
            e_SalesOrderHistoryEDIPickStatusReadyToBePicked,
            e_SalesOrderHistoryEDIPickStatusPicking,
            e_SalesOrderHistoryEDIPickStatusPicked,
            e_SalesOrderHistoryEDIPickStatusASNReadyToSend,
            e_SalesOrderHistoryEDIPickStatusASNSent,
            e_SalesOrderHistoryEDIPickStatusRCTIReceived,
            e_SalesOrderHistoryEDIPickStatusError,
            e_SalesOrderHistoryEDIPickStatusRejectionReadyToSend,
            e_SalesOrderHistoryEDIPickStatusRejectionSent,
        }

    }

    public partial class SalesOrderJobCosting
    {
        public virtual bool? GSTApplicable { get; set; }
        public virtual string JobCostID { get; set; }
        public virtual string JobCostNo { get; set; }
        public virtual string Description { get; set; }
    }

    // ICustomFieldValuesHost interface added to the class declaration for our purposes here in the web portal - this is not populated or used by the Jiwa REST API
    public partial class SalesOrderLine : ICustomFieldValuesHost
    {
        public virtual int? ItemNo { get; set; }
        public virtual bool? CommentLine { get; set; }
        public virtual decimal? QuantityOrdered { get; set; }
        virtual public decimal? QuantityPreviousDemand { get; set; }
        public virtual decimal? QuantityDemand { get; set; }
        virtual public decimal? QuantityPreviousDelivery { get; set; }
        public virtual decimal? QuantityThisDel { get; set; }
        public virtual decimal? QuantityBackOrd { get; set; }
        public virtual bool? Picked { get; set; }
        public virtual decimal? PriceExGst { get; set; }
        public virtual decimal? FXPriceExGst { get; set; }
        public virtual decimal? PriceIncGst { get; set; }
        public virtual decimal? FXPriceIncGst { get; set; }
        public virtual decimal? DiscountedPrice { get; set; }
        public virtual decimal? FXDiscountedPrice { get; set; }
        public virtual decimal? TaxToCharge { get; set; }
        public virtual decimal? FXTaxToCharge { get; set; }
        public virtual TaxRate TaxRate { get; set; }
        public virtual decimal? UnitCost { get; set; }
        public virtual bool? FixSellPrice { get; set; }
        public virtual bool? FixPrice { get; set; }
        public virtual decimal? UserDefinedFloat1 { get; set; }
        public virtual decimal? UserDefinedFloat2 { get; set; }
        public virtual decimal? UserDefinedFloat3 { get; set; }
        public virtual DateTime? ForwardOrderDate { get; set; }
        public virtual DateTime? ScheduledDate { get; set; }
        public virtual decimal? LineTotal { get; set; }
        public virtual decimal? FXLineTotal { get; set; }
        public virtual decimal? Weight { get; set; }
        public virtual decimal? Cubic { get; set; }
        public virtual decimal? QuotedDiscountedPrice { get; set; }
        public virtual decimal? FXQuotedDiscountedPrice { get; set; }
        public virtual decimal? QuotedDiscountPercentage { get; set; }
        public virtual decimal? DiscountedPercentage { get; set; }
        public virtual decimal? DiscountGiven { get; set; }
        public virtual decimal? FXDiscountGiven { get; set; }
        public virtual short? QuantityDecimalPlaces { get; set; }
        public virtual decimal? QuantityOriginalOrdered { get; set; }
        public virtual SalesOrderSerialStockSelectionTypesEnum? SalesOrderSerialStockSelectionTypes { get; set; }
        public virtual bool? NonInventory { get; set; }
        virtual public string PreviousSnapInvoiceLineID { get; set; }
        public virtual string InvoiceLineID { get; set; }
        public virtual string InventoryID { get; set; }
        public virtual string PartNo { get; set; }
        public virtual string Description { get; set; }
        public virtual string CommentText { get; set; }
        public virtual string Aux2 { get; set; }
        public virtual string LineLinkID { get; set; }
        public virtual string EDIStoreLocationCode { get; set; }
        public virtual string EDIDCLocationCode { get; set; }
        public virtual string CostCenter { get; set; }
        public virtual string Stage { get; set; }
        public virtual List<CustomFieldValue> CustomFieldValues { get; set; }
        public virtual List<SalesOrderLineDetail> LineDetails { get; set; }
        public virtual List<SalesOrderShippingLabel> ShippingLabels { get; set; }
        public virtual JiwaFinancials.Jiwa.JiwaServiceModel.Inventory.InventoryUnitOfMeasure UnitOfMeasure { get; set; }
        public virtual SalesOrderKitLineTypesEnum? KitLineType { get; set; }
        public virtual decimal? KitUnits { get; set; }
        public virtual string KitHeaderLineID { get; set; }
        public virtual string SKUUnitName { get; set; }
        virtual public decimal? OrderedExGSTTotal { get; set; }
        virtual public decimal? OrderedGSTTotal { get; set; }
        virtual public decimal? OrderedIncGSTTotal { get; set; }
        virtual public decimal? FXOrderedExGSTTotal { get; set; }
        virtual public decimal? FXOrderedGSTTotal { get; set; }
        virtual public decimal? FXOrderedIncGSTTotal { get; set; }

        public enum SalesOrderSerialStockSelectionTypesEnum
        {
            e_SalesOrderSerialStockSelectionPrompted,
            e_SalesOrderSerialStockSelectionFIFO,
        }

        public enum SalesOrderKitLineTypesEnum
        {
            e_SalesOrderNormalLine,
            e_SalesOrderKitHeader,
            e_SalesOrderKitComponent,
        }

    }

    public partial class SalesOrderLineDetail
    {
        public virtual decimal? Cost { get; set; }
        public virtual DateTime? DateIn { get; set; }
        public virtual DateTime? ExpiryDate { get; set; }
        public virtual decimal? SpecialPrice { get; set; }
        public virtual decimal? Quantity { get; set; }
        public virtual string LineDetailID { get; set; }
        public virtual string BinLocationID { get; set; }
        public virtual string BinLocation { get; set; }
        public virtual string BinLocationShortName { get; set; }
        public virtual string SerialNo { get; set; }
        public virtual string SOHID { get; set; }
        public virtual string IN_LogicalID { get; set; }
    }

    public partial class SalesOrderPayment
    {
        public virtual int? HistoryNo { get; set; }
        public virtual PaymentType PaymentType { get; set; }
        public virtual decimal? AmountPaid { get; set; }
        public virtual decimal? FXAmountPaid { get; set; }
        public virtual DateTime? PaymentDate { get; set; }
        public virtual bool? ProcessPayment { get; set; }
        public virtual PaymentAuthStatuses? AuthorisationStatus { get; set; }
        public virtual int? PaymentGatewayReturnCode { get; set; }
        public virtual bool? Processed { get; set; }
        public virtual DateTime? CardExpiry { get; set; }
        public virtual string PaymentID { get; set; }
        public virtual string PaymentRef { get; set; }
        public virtual string AuthorisationNumber { get; set; }
        public virtual string PaymentGatewayReturnMessage { get; set; }
        public virtual string CardNumber { get; set; }
        public virtual string CardHolder { get; set; }
        public virtual string BankName { get; set; }
        public virtual string BSBN { get; set; }
        public virtual string BankAcc { get; set; }
        public virtual string AccountName { get; set; }
        public enum PaymentAuthStatuses
        {
            NoAuthorisationNeeded,
            AuthorisationRequired,
            Authorised,
            Declined,
            Error,
        }

    }

    public partial class SalesOrderShippingLabel
    {
        public virtual decimal? Quantity { get; set; }
        public virtual DateTime? UseByDate { get; set; }
        public virtual int? LabelNumber { get; set; }
        public virtual decimal? SpareNumeric1 { get; set; }
        public virtual decimal? SpareNumeric2 { get; set; }
        public virtual decimal? SpareNumeric3 { get; set; }
        public virtual DateTime? SpareDate1 { get; set; }
        public virtual DateTime? SpareDate2 { get; set; }
        public virtual DateTime? SpareDate3 { get; set; }
        public virtual string ShippingLabelID { get; set; }
        public virtual string SSCCNumber { get; set; }
        public virtual string BatchNo { get; set; }
        public virtual string Reference { get; set; }
        public virtual string SpareString1 { get; set; }
        public virtual string SpareString2 { get; set; }
        public virtual string SpareString3 { get; set; }
        public virtual DateTimeOffset? LastSavedDateTime { get; set; }
    }

    public partial class SalesOrderSystemSettings
    {
        public virtual bool? ForceInventorySelection { get; set; }
        public virtual bool? SuppressLineRetotalling { get; set; }
        public virtual bool? IgnoreDebtorOnHold { get; set; }
        public virtual bool? CompensateTaxRounding { get; set; }
    }

}
#endregion

#region Sales Quote"
namespace JiwaFinancials.Jiwa.JiwaServiceModel.SalesQuotes
{
    public partial class CartageCharge
    {
        public virtual decimal? ExTaxAmount { get; set; }
        public virtual decimal? FXExTaxAmount { get; set; }
        public virtual decimal? TaxAmount { get; set; }
        public virtual decimal? FXTaxAmount { get; set; }
        public virtual TaxRate TaxRate { get; set; }
    }

    public partial class OpportunityStatusReason
    {
        public virtual string OpportunityStatusReasonID { get; set; }
        public virtual string Description { get; set; }
        public virtual string Note { get; set; }
        public virtual OpportunityStatusReasonTypes? StatusType { get; set; }
        public enum OpportunityStatusReasonTypes
        {
            OnGoing = 0,
            Won = 1,
            Lost = 1,
        }

    }

    // ICustomFieldValuesHost interface added to the class declaration for our purposes here in the web portal - this is not populated or used by the Jiwa REST API
    public partial class SalesQuote : ICustomFieldValuesHost
    {
        public virtual string Type { get; set; }
        public virtual SalesQuoteSettings SystemSettings { get; set; }
        public virtual DateTimeOffset? LastSavedDateTime { get; set; }
        public virtual string QuoteID { get; set; }
        public virtual string QuoteNo { get; set; }
        public virtual string LogicalID { get; set; }
        public virtual string LogicalWarehouseDescription { get; set; }
        public virtual string PhysicalWarehouseDescription { get; set; }
        public virtual string StaffID { get; set; }
        public virtual string StaffUserName { get; set; }
        public virtual string StaffTitle { get; set; }
        public virtual string StaffFirstName { get; set; }
        public virtual string StaffSurname { get; set; }
        public virtual string BranchID { get; set; }
        public virtual string BranchName { get; set; }
        public virtual string BranchDescription { get; set; }
        public virtual DateTime? InitiatedDate { get; set; }
        public virtual DateTime? InvoiceInitDate { get; set; }
        public virtual string OrderNo { get; set; }
        public virtual string TaxExemptionNo { get; set; }
        public virtual string SOReference { get; set; }
        public virtual e_SalesQuoteTypes? SalesQuoteType { get; set; }
        public virtual e_SalesQuoteOrderTypes? OrderType { get; set; }
        public virtual e_SalesQuoteStatuses? Status { get; set; }
        public virtual e_SalesQuoteBillTypes? BillType { get; set; }
        public virtual DateTime? ExpectedDeliveryDate { get; set; }
        public virtual string DebtorID { get; set; }
        public virtual string DebtorAccountNo { get; set; }
        public virtual string DebtorName { get; set; }
        public virtual string DebtorEmailAddress { get; set; }
        public virtual string DebtorContactName { get; set; }
        public virtual string DeliveryAddressee { get; set; }
        public virtual string DeliveryAddressPhone { get; set; }
        public virtual string DeliveryAddress1 { get; set; }
        public virtual string DeliveryAddress2 { get; set; }
        public virtual string DeliveryAddressSuburb { get; set; }
        public virtual string DeliveryAddressState { get; set; }
        public virtual string DeliveryAddressContactName { get; set; }
        public virtual string DeliveryAddressPostcode { get; set; }
        public virtual string DeliveryAddressCountry { get; set; }
        public virtual decimal? DeliveryAddressLatitude { get; set; }
        public virtual decimal? DeliveryAddressLongitude { get; set; }
        public virtual string DeliveryAddressNotes { get; set; }
        public virtual string DeliveryAddressCourierDetails { get; set; }
        public virtual string DeliveryAddressEmailAddress { get; set; }
        public virtual bool? DropShipment { get; set; }
        public virtual decimal? Cartage1ExGst { get; set; }
        public virtual decimal? FXCartage1ExGst { get; set; }
        public virtual decimal? Cartage1GstRate { get; set; }
        public virtual decimal? Cartage1Gst { get; set; }
        public virtual decimal? FXCartage1Gst { get; set; }
        public virtual decimal? Cartage2ExGst { get; set; }
        public virtual decimal? FXCartage2ExGst { get; set; }
        public virtual decimal? Cartage2GstRate { get; set; }
        public virtual decimal? Cartage2Gst { get; set; }
        public virtual decimal? FXCartage2Gst { get; set; }
        public virtual decimal? Cartage3ExGst { get; set; }
        public virtual decimal? FXCartage3ExGst { get; set; }
        public virtual decimal? Cartage3GstRate { get; set; }
        public virtual decimal? Cartage3Gst { get; set; }
        public virtual decimal? FXCartage3Gst { get; set; }
        public virtual List<CustomFieldValue> CustomFieldValues { get; set; }
        public virtual List<Note> Notes { get; set; }
        public virtual List<Document> Documents { get; set; }
        public virtual List<SalesQuoteLine> Lines { get; set; }
        public virtual List<SalesQuoteHistory> Histories { get; set; }
        public virtual SalesQuoteCashSales CashSales { get; set; }
        public virtual SalesQuoteJobCosting JobCosting { get; set; }
        public virtual Origin Origin { get; set; }
        public virtual DeliveryMethod DeliveryMethod { get; set; }
        public virtual string CurrencyID { get; set; }
        public virtual string CurrencyShortName { get; set; }
        public virtual decimal? CurrencyRate { get; set; }
        public enum e_SalesQuoteTypes
        {
            e_SalesQuoteNormalSalesOrder,
            e_SalesQuoteBackToBack,
        }

        public enum e_SalesQuoteBillTypes
        {
            e_SalesQuoteShipAndBill,
            e_SalesQuoteBillWhenComplete,
            e_SalesQuoteShipWhenComplete,
        }

        public enum e_SalesQuoteStatuses
        {
            e_SalesQuoteEntered,
            e_SalesQuoteClosed,
        }

        public enum e_SalesQuoteOrderTypes
        {
            e_SalesQuoteOrderTypeReserveOrder,
            e_SalesQuoteOrderTypeInvoiceOrder,
            e_SalesQuoteOrderTypeForwardOrder,
            e_SalesQuoteOrderTypeActiveOrder,
        }

    }

    public partial class SalesQuoteCashSales
    {
        public virtual string Name { get; set; }
        public virtual string Company { get; set; }
        public virtual string Address1 { get; set; }
        public virtual string Address2 { get; set; }
        public virtual string Address3 { get; set; }
        public virtual string Address4 { get; set; }
        public virtual string PostCode { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Fax { get; set; }
        public virtual string ContactName { get; set; }
        public virtual string Country { get; set; }
        public virtual string EmailAddress { get; set; }
    }

    public partial class SalesQuoteHistory
    {
        public virtual string QuoteHistoryID { get; set; }
        public virtual int? HistoryNo { get; set; }
        public virtual SalesStage SalesStage { get; set; }
        public virtual CartageCharge CartageCharge1 { get; set; }
        public virtual CartageCharge CartageCharge2 { get; set; }
        public virtual CartageCharge CartageCharge3 { get; set; }
        public virtual OpportunityStatusReason OpportunityStatusReason { get; set; }
        public virtual string Ref { get; set; }
        public virtual string LastModifiedBy { get; set; }
        public virtual decimal? HistoryTotal { get; set; }
        public virtual decimal? FXHistoryTotal { get; set; }
        public virtual DateTime? RecordDate { get; set; }
        public virtual DateTime? ExpiryDate { get; set; }
        public virtual int? ExpiryDays { get; set; }
        public virtual bool? DocketPrinted { get; set; }
        public virtual string DeliveryAddressPhone { get; set; }
        public virtual string DelAddress1 { get; set; }
        public virtual string DelAddress2 { get; set; }
        public virtual string DelAddress3 { get; set; }
        public virtual string DelAddress4 { get; set; }
        public virtual string PostCode { get; set; }
        public virtual string DeliveryAddressCountry { get; set; }
        public virtual decimal? DeliveryAddressLatitude { get; set; }
        public virtual decimal? DeliveryAddressLongitude { get; set; }
        public virtual string Notes { get; set; }
        public virtual string CourierDetails { get; set; }
        public virtual string DeliveryAddressEmailAddress { get; set; }
        public virtual string FreightForwardAddressPhone { get; set; }
        public virtual string FreightForwardAddress1 { get; set; }
        public virtual string FreightForwardAddress2 { get; set; }
        public virtual string FreightForwardAddress3 { get; set; }
        public virtual string FreightForwardAddress4 { get; set; }
        public virtual string FreightForwardAddressPostcode { get; set; }
        public virtual string FreightForwardAddressCountry { get; set; }
        public virtual decimal? FreightForwardAddressLatitude { get; set; }
        public virtual decimal? FreightForwardAddressLongitude { get; set; }
        public virtual string FreightForwardNotes { get; set; }
        public virtual string FreightForwardCourierDetails { get; set; }
        public virtual string FreightForwardAddressEmailAddress { get; set; }
        public virtual bool? InvoicePrinted { get; set; }
        public virtual string DelContactName { get; set; }
        public virtual DateTime? ExpectedCloseDate { get; set; }
        public virtual StatusTypes? OpportunityStatus { get; set; }
        public virtual string OpportunityNote { get; set; }
        public virtual bool? InvoiceEmailed { get; set; }
        public virtual bool? DropShipment { get; set; }
        public virtual string DeliveryAddressee { get; set; }
        public virtual List<SalesQuoteLine> Lines { get; set; }
        public virtual decimal? CurrencyRate { get; set; }
        public enum StatusTypes
        {
            OnGoing = 0,
            Won = 1,
            Lost = 1,
        }

    }

    public partial class SalesQuoteJobCosting
    {
        public virtual bool? GSTApplicable { get; set; }
        public virtual string JobCostID { get; set; }
        public virtual string JobCostNo { get; set; }
        public virtual string Description { get; set; }
    }

    public partial class SalesQuoteLine : ICustomFieldValuesHost
    {
        public virtual int? ItemNo { get; set; }
        public virtual string QuoteLineID { get; set; }
        public virtual string InventoryID { get; set; }
        public virtual string PartNo { get; set; }
        public virtual string Description { get; set; }
        public virtual bool? CommentLine { get; set; }
        public virtual string CommentText { get; set; }
        public virtual decimal? QuantityOrdered { get; set; }
        public virtual decimal? PriceExGst { get; set; }
        public virtual decimal? FXPriceExGst { get; set; }
        public virtual decimal? PriceIncGst { get; set; }
        public virtual decimal? FXPriceIncGst { get; set; }
        public virtual decimal? DiscountedPrice { get; set; }
        public virtual decimal? FXDiscountedPrice { get; set; }
        public virtual decimal? TaxToCharge { get; set; }
        public virtual decimal? FXTaxToCharge { get; set; }
        public virtual TaxRate TaxRate { get; set; }
        public virtual decimal? UnitCost { get; set; }
        public virtual string LineLinkID { get; set; }
        public virtual bool? FixSellPrice { get; set; }
        public virtual decimal? UserDefinedFloat1 { get; set; }
        public virtual decimal? UserDefinedFloat2 { get; set; }
        public virtual decimal? UserDefinedFloat3 { get; set; }
        public virtual decimal? LineTotal { get; set; }
        public virtual decimal? FXLineTotal { get; set; }
        public virtual decimal? Weight { get; set; }
        public virtual decimal? Cubic { get; set; }
        public virtual decimal? DiscountedPercentage { get; set; }
        public virtual decimal? DiscountGiven { get; set; }
        public virtual decimal? FXDiscountGiven { get; set; }
        public virtual short? QuantityDecimalPlaces { get; set; }
        public virtual decimal? QuantityOriginalOrdered { get; set; }
        public virtual bool? NonInventory { get; set; }
        public virtual List<CustomFieldValue> CustomFieldValues { get; set; }
        public virtual SalesQuoteKitLineTypesEnum? KitLineType { get; set; }
        public virtual decimal? KitUnits { get; set; }
        public virtual string KitHeaderLineID { get; set; }
        public virtual JiwaFinancials.Jiwa.JiwaServiceModel.Inventory.InventoryUnitOfMeasure UnitOfMeasure { get; set; }
        public virtual string SKUUnitName { get; set; }

        public enum SalesQuoteKitLineTypesEnum
        {
            e_SalesQuoteNormalLine,
            e_SalesQuoteKitHeader,
            e_SalesQuoteKitComponent,
        }

    }

    public partial class SalesQuoteSettings
    {
        public virtual string Cat1Description { get; set; }
        public virtual string Cat2Description { get; set; }
        public virtual string Cat3Description { get; set; }
        public virtual string Cat4Description { get; set; }
        public virtual string Cat5Description { get; set; }
        public virtual bool? DisplayProductUpSellPopUp { get; set; }
        public virtual short? MoneyDecimalPlaces { get; set; }
        public virtual short? SalesOrdersMoneyDecimalPlaces { get; set; }
        public virtual string SalesOrdersMoneyFormatStr { get; set; }
        public virtual string MoneyFormatStr { get; set; }
        public virtual int? ComponentsForeColour { get; set; }
        public virtual int? KitRoundingForeColour { get; set; }
        public virtual int? KitForeColour { get; set; }
        public virtual int? NonInventoryForeColour { get; set; }
        public virtual decimal? DefaultQuantity { get; set; }
        public virtual string GroupedCaption2 { get; set; }
        public virtual bool? AllowInvoiceNumberOverride { get; set; }
        public virtual bool? AllowPriceOverride { get; set; }
        public virtual bool? AllowOtherOverrides { get; set; }
        public virtual bool? AllowManualPartNoEntry { get; set; }
        public virtual bool? AllowNonInventoryItems { get; set; }
        public virtual bool? AllowTaxRateOverrides { get; set; }
        public virtual bool? IncludeValueOfBackOrdersInCreditLimitCheck { get; set; }
        public virtual bool? PrintInvoicesWithZeroQuantityDel { get; set; }
        public virtual short? InvoicePrinterCopies { get; set; }
        public virtual bool? PrintToScreen { get; set; }
        public virtual bool? AllowModificationOfPrintedUnprocessedInvoices { get; set; }
        public virtual bool? CheckForDuplicateOrderNos { get; set; }
        public virtual bool? UseDefaultSalesPerson { get; set; }
        public virtual bool? DefaultInvoiceTypeIsWholesale { get; set; }
        public virtual string LinkSELECT { get; set; }
        public virtual string LinkTITLE { get; set; }
        public virtual string LinkDESC { get; set; }
        public virtual string LinkKEY { get; set; }
        public virtual string LinkID { get; set; }
        public virtual bool? AllowKitComponentOverride { get; set; }
        public virtual bool? PostTendered { get; set; }
        public virtual bool? AllowInitDateEdit { get; set; }
        public virtual bool? UseZeroCreditLimit { get; set; }
        public virtual bool? CollectJobCostCode { get; set; }
        public virtual bool? IgnoreBackOrderAllocations { get; set; }
        public virtual bool? UsePicking { get; set; }
        public virtual bool? ShowOnlyUsersDebtors { get; set; }
        public virtual bool? AllowSaveToOrderWhenDebtorOnHold { get; set; }
        public virtual bool? AddFreightToAllSnapshots { get; set; }
        public virtual bool? DontChangePickPrices { get; set; }
        public virtual bool? AutoKitPricing { get; set; }
        public virtual bool? InvoicesFromQuotesUseActivateDate { get; set; }
        public virtual bool? GrabSOHFromDefaultBin { get; set; }
        public virtual bool? UseTaxExemption { get; set; }
        public virtual bool? ValidateABN { get; set; }
        public virtual bool? CompensateTaxRounding { get; set; }
        public virtual bool? UseBranching { get; set; }
        public virtual string InvoiceTypeDescription1 { get; set; }
        public virtual string InvoiceTypeDescription2 { get; set; }
        public virtual bool? AllowInvoiceTypeChange { get; set; }
        public virtual bool? AllowForwardOrders { get; set; }
        public virtual bool? AllowActiveOrders { get; set; }
        public virtual string DocketNumHeader { get; set; }
        public virtual string CreditNoteHeader { get; set; }
        public virtual bool? BuildPaymentReferenceFromDebtor { get; set; }
        public virtual string QuoteNoDescription { get; set; }
        public virtual string ShortDateFormat { get; set; }
        public virtual bool? ManualPrintSelection { get; set; }
        public virtual bool? UseDirectTaxIfSellPriceIncTax { get; set; }
        public virtual string JobCodeSeparator { get; set; }
        public virtual bool? UseKitRoundingPart { get; set; }
        public virtual string KitRoundingPartID { get; set; }
        public virtual short? DiscountPercentDecimalPlaces { get; set; }
        public virtual short? DefaultQuoteExpiryDays { get; set; }
        public virtual bool? AllowDocumentManipulationOnClosedQuote { get; set; }
        public virtual string DefaultDocumentTypeKey { get; set; }
        public virtual string DefaultNoteTypeKey { get; set; }
        public virtual bool? CopySalesQuoteIncludesNotes { get; set; }
        public virtual bool? CopySalesQuoteIncludesDocuments { get; set; }
        public virtual bool? DontApplyDebtorDiscounts { get; set; }
        public virtual bool? UserTodoOnly { get; set; }
        public virtual bool? DoNotReadSOHLevelsForQuotes { get; set; }
    }

    public partial class SalesStage
    {
        public virtual string SalesStageID { get; set; }
        public virtual DateTimeOffset? LastSavedDateTime { get; set; }
        public virtual int? ItemNo { get; set; }
        public virtual string Description { get; set; }
        public virtual short? PercentComplete { get; set; }
        public virtual bool? IsDefault { get; set; }
        public virtual bool? IsEnabled { get; set; }
    }
}
#endregion

#region "Staff"
namespace JiwaFinancials.Jiwa.JiwaServiceModel.Staff
{
    public partial class StaffMember
    {
        public virtual string StaffID { get; set; }
        public virtual string Title { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string Surname { get; set; }
        public virtual string Username { get; set; }
        public virtual bool? IsActive { get; set; }
        public virtual bool? IsEnabled { get; set; }
    }

}
#endregion

#region "Tags"
namespace JiwaFinancials.Jiwa.JiwaServiceModel.Tags
{
    public partial class Tag
    {
        public virtual string RecID { get; set; }
        public virtual string Text { get; set; }
        public virtual int? Colour { get; set; }
        public virtual DateTimeOffset? LastSavedDateTime { get; set; }
        public virtual byte[] RowHash { get; set; }
        public virtual int? ItemNo { get; set; }
    }
}
#endregion

#region "Tax"
namespace JiwaFinancials.Jiwa.JiwaServiceModel.Tax
{
    public partial class TaxRate
    {
        public virtual string RecID { get; set; }
        public virtual string TaxID { get; set; }
        public virtual string Description { get; set; }
        public virtual TaxRateTypes? GSTTaxGroup { get; set; }
        public virtual decimal? Rate { get; set; }
        public virtual bool? IsDefaultRate { get; set; }
        public virtual decimal? BASCode { get; set; }
        public virtual bool? IsDefaultRateInGroup { get; set; }
        public virtual bool? IsEnabled { get; set; }
        public virtual Account LedgerAccount { get; set; }
        public virtual DateTimeOffset? LastSavedDateTime { get; set; }
        public virtual List<CustomFieldValue> CustomFieldValues { get; set; }
    }

    public enum TaxRateTypes
    {
        WST,
        GSTIn,
        GSTOut,
        GSTAdjustmentsIn,
        GSTAdjustmentsOut,
    }

}
#endregion
#endregion

#region "AutoQueries and Tables"
namespace JiwaFinancials.Jiwa.JiwaServiceModel.Tables.Or
{
    #region "Inventory"    
    [Route("/Queries/OR/InventoryItemListImmutableWarehouse", "GET")]
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    public partial class v_Jiwa_Inventory_Item_List_OR_ImmutableWarehouseQuery
        : v_Jiwa_Inventory_Item_ListORQuery, IReturn<QueryResponse<v_Jiwa_Inventory_Item_ListOR>>
    {
        public virtual string Immutable_IN_LogicalID { get; set; }
        public virtual bool? Immutable_WebEnabled { get; set; }
    }

    public partial class v_Jiwa_Inventory_Item_ListOR
    {
        public v_Jiwa_Inventory_Item_ListOR()
        {
            Picture = new byte[] { };
        }

        [Required]
        public virtual string InventoryID { get; set; }

        [Required]
        public virtual string PartNo { get; set; }

        public virtual string Description { get; set; }
        public virtual byte[] Picture { get; set; }
        [Required]
        public virtual string InventoryClassificationID { get; set; }

        public virtual string ClassificationDescription { get; set; }
        [Required]
        public virtual string Category1ID { get; set; }

        public virtual string Category1Description { get; set; }
        [Required]
        public virtual string Category2ID { get; set; }

        public virtual string Category2Description { get; set; }
        [Required]
        public virtual string Category3ID { get; set; }

        public virtual string Category3Description { get; set; }
        [Required]
        public virtual string Category4ID { get; set; }

        public virtual string Category4Description { get; set; }
        [Required]
        public virtual string Category5ID { get; set; }

        public virtual string Category5Description { get; set; }
        [Required]
        public virtual string IN_LogicalID { get; set; }

        public virtual string LogicalWarehouseDescription { get; set; }
        [Required]
        public virtual string IN_PhysicalID { get; set; }

        [Required]
        public virtual string PhysicalWarehouseDescription { get; set; }

        public virtual decimal? AvailableStock { get; set; }
        public virtual decimal? SellPrice { get; set; }
        public virtual decimal? RRPPrice { get; set; }
        [Required]
        public virtual DateTimeOffset LastSavedDateTime { get; set; }

        public virtual decimal? InStock { get; set; }
        public virtual short? QuantityDecimalPlaces { get; set; }
    }

    //[Route("/Queries/OR/InventoryItemList", "GET")] // Need to comment this out otherwise our v_Jiwa_Inventory_Item_List_OR_ImmutableWarehouseQuery requests go here instead of to /Queries/OR/InventoryItemListImmutableWarehouse
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    public partial class v_Jiwa_Inventory_Item_ListORQuery : QueryDb<v_Jiwa_Inventory_Item_ListOR>
    {
        public string InventoryID { get; set; }

        public string InventoryIDStartsWith { get; set; }
        public string InventoryIDEndsWith { get; set; }
        public string InventoryIDContains { get; set; }
        public string InventoryIDLike { get; set; }
        public string[] InventoryIDBetween { get; set; }
        public string[] InventoryIDIn { get; set; }

        public string PartNo { get; set; }

        public string PartNoStartsWith { get; set; }
        public string PartNoEndsWith { get; set; }
        public string PartNoContains { get; set; }
        public string PartNoLike { get; set; }
        public string[] PartNoBetween { get; set; }
        public string[] PartNoIn { get; set; }

        public string Description { get; set; }

        public string DescriptionStartsWith { get; set; }
        public string DescriptionEndsWith { get; set; }
        public string DescriptionContains { get; set; }
        public string DescriptionLike { get; set; }
        public string[] DescriptionBetween { get; set; }
        public string[] DescriptionIn { get; set; }

        public byte[] Picture { get; set; }

        public string InventoryClassificationID { get; set; }

        public string InventoryClassificationIDStartsWith { get; set; }
        public string InventoryClassificationIDEndsWith { get; set; }
        public string InventoryClassificationIDContains { get; set; }
        public string InventoryClassificationIDLike { get; set; }
        public string[] InventoryClassificationIDBetween { get; set; }
        public string[] InventoryClassificationIDIn { get; set; }

        public string ClassificationDescription { get; set; }

        public string ClassificationDescriptionStartsWith { get; set; }
        public string ClassificationDescriptionEndsWith { get; set; }
        public string ClassificationDescriptionContains { get; set; }
        public string ClassificationDescriptionLike { get; set; }
        public string[] ClassificationDescriptionBetween { get; set; }
        public string[] ClassificationDescriptionIn { get; set; }

        public string Category1ID { get; set; }

        public string Category1IDStartsWith { get; set; }
        public string Category1IDEndsWith { get; set; }
        public string Category1IDContains { get; set; }
        public string Category1IDLike { get; set; }
        public string[] Category1IDBetween { get; set; }
        public string[] Category1IDIn { get; set; }

        public string Category1Description { get; set; }

        public string Category1DescriptionStartsWith { get; set; }
        public string Category1DescriptionEndsWith { get; set; }
        public string Category1DescriptionContains { get; set; }
        public string Category1DescriptionLike { get; set; }
        public string[] Category1DescriptionBetween { get; set; }
        public string[] Category1DescriptionIn { get; set; }

        public string Category2ID { get; set; }

        public string Category2IDStartsWith { get; set; }
        public string Category2IDEndsWith { get; set; }
        public string Category2IDContains { get; set; }
        public string Category2IDLike { get; set; }
        public string[] Category2IDBetween { get; set; }
        public string[] Category2IDIn { get; set; }

        public string Category2Description { get; set; }

        public string Category2DescriptionStartsWith { get; set; }
        public string Category2DescriptionEndsWith { get; set; }
        public string Category2DescriptionContains { get; set; }
        public string Category2DescriptionLike { get; set; }
        public string[] Category2DescriptionBetween { get; set; }
        public string[] Category2DescriptionIn { get; set; }

        public string Category3ID { get; set; }

        public string Category3IDStartsWith { get; set; }
        public string Category3IDEndsWith { get; set; }
        public string Category3IDContains { get; set; }
        public string Category3IDLike { get; set; }
        public string[] Category3IDBetween { get; set; }
        public string[] Category3IDIn { get; set; }

        public string Category3Description { get; set; }

        public string Category3DescriptionStartsWith { get; set; }
        public string Category3DescriptionEndsWith { get; set; }
        public string Category3DescriptionContains { get; set; }
        public string Category3DescriptionLike { get; set; }
        public string[] Category3DescriptionBetween { get; set; }
        public string[] Category3DescriptionIn { get; set; }

        public string Category4ID { get; set; }

        public string Category4IDStartsWith { get; set; }
        public string Category4IDEndsWith { get; set; }
        public string Category4IDContains { get; set; }
        public string Category4IDLike { get; set; }
        public string[] Category4IDBetween { get; set; }
        public string[] Category4IDIn { get; set; }

        public string Category4Description { get; set; }

        public string Category4DescriptionStartsWith { get; set; }
        public string Category4DescriptionEndsWith { get; set; }
        public string Category4DescriptionContains { get; set; }
        public string Category4DescriptionLike { get; set; }
        public string[] Category4DescriptionBetween { get; set; }
        public string[] Category4DescriptionIn { get; set; }

        public string Category5ID { get; set; }

        public string Category5IDStartsWith { get; set; }
        public string Category5IDEndsWith { get; set; }
        public string Category5IDContains { get; set; }
        public string Category5IDLike { get; set; }
        public string[] Category5IDBetween { get; set; }
        public string[] Category5IDIn { get; set; }

        public string Category5Description { get; set; }

        public string Category5DescriptionStartsWith { get; set; }
        public string Category5DescriptionEndsWith { get; set; }
        public string Category5DescriptionContains { get; set; }
        public string Category5DescriptionLike { get; set; }
        public string[] Category5DescriptionBetween { get; set; }
        public string[] Category5DescriptionIn { get; set; }

        public string IN_LogicalID { get; set; }

        public string IN_LogicalIDStartsWith { get; set; }
        public string IN_LogicalIDEndsWith { get; set; }
        public string IN_LogicalIDContains { get; set; }
        public string IN_LogicalIDLike { get; set; }
        public string[] IN_LogicalIDBetween { get; set; }
        public string[] IN_LogicalIDIn { get; set; }

        public string LogicalWarehouseDescription { get; set; }

        public string LogicalWarehouseDescriptionStartsWith { get; set; }
        public string LogicalWarehouseDescriptionEndsWith { get; set; }
        public string LogicalWarehouseDescriptionContains { get; set; }
        public string LogicalWarehouseDescriptionLike { get; set; }
        public string[] LogicalWarehouseDescriptionBetween { get; set; }
        public string[] LogicalWarehouseDescriptionIn { get; set; }

        public string IN_PhysicalID { get; set; }

        public string IN_PhysicalIDStartsWith { get; set; }
        public string IN_PhysicalIDEndsWith { get; set; }
        public string IN_PhysicalIDContains { get; set; }
        public string IN_PhysicalIDLike { get; set; }
        public string[] IN_PhysicalIDBetween { get; set; }
        public string[] IN_PhysicalIDIn { get; set; }

        public string PhysicalWarehouseDescription { get; set; }

        public string PhysicalWarehouseDescriptionStartsWith { get; set; }
        public string PhysicalWarehouseDescriptionEndsWith { get; set; }
        public string PhysicalWarehouseDescriptionContains { get; set; }
        public string PhysicalWarehouseDescriptionLike { get; set; }
        public string[] PhysicalWarehouseDescriptionBetween { get; set; }
        public string[] PhysicalWarehouseDescriptionIn { get; set; }

        public decimal? AvailableStock { get; set; }

        public decimal? AvailableStockGreaterThanOrEqualTo { get; set; }
        public decimal? AvailableStockGreaterThan { get; set; }
        public decimal? AvailableStockLessThan { get; set; }
        public decimal? AvailableStockLessThanOrEqualTo { get; set; }
        public decimal? AvailableStockNotEqualTo { get; set; }
        public decimal?[] AvailableStockBetween { get; set; }
        public decimal?[] AvailableStockIn { get; set; }

        public decimal? SellPrice { get; set; }

        public decimal? SellPriceGreaterThanOrEqualTo { get; set; }
        public decimal? SellPriceGreaterThan { get; set; }
        public decimal? SellPriceLessThan { get; set; }
        public decimal? SellPriceLessThanOrEqualTo { get; set; }
        public decimal? SellPriceNotEqualTo { get; set; }
        public decimal?[] SellPriceBetween { get; set; }
        public decimal?[] SellPriceIn { get; set; }

        public decimal? RRPPrice { get; set; }

        public decimal? RRPPriceGreaterThanOrEqualTo { get; set; }
        public decimal? RRPPriceGreaterThan { get; set; }
        public decimal? RRPPriceLessThan { get; set; }
        public decimal? RRPPriceLessThanOrEqualTo { get; set; }
        public decimal? RRPPriceNotEqualTo { get; set; }
        public decimal?[] RRPPriceBetween { get; set; }
        public decimal?[] RRPPriceIn { get; set; }

        public DateTimeOffset? LastSavedDateTime { get; set; }

        public DateTimeOffset? LastSavedDateTimeGreaterThanOrEqualTo { get; set; }
        public DateTimeOffset? LastSavedDateTimeGreaterThan { get; set; }
        public DateTimeOffset? LastSavedDateTimeLessThan { get; set; }
        public DateTimeOffset? LastSavedDateTimeLessThanOrEqualTo { get; set; }
        public DateTimeOffset? LastSavedDateTimeNotEqualTo { get; set; }
        public DateTimeOffset[] LastSavedDateTimeBetween { get; set; }
        public DateTimeOffset[] LastSavedDateTimeIn { get; set; }

        public decimal? InStock { get; set; }

        public decimal? InStockGreaterThanOrEqualTo { get; set; }
        public decimal? InStockGreaterThan { get; set; }
        public decimal? InStockLessThan { get; set; }
        public decimal? InStockLessThanOrEqualTo { get; set; }
        public decimal? InStockNotEqualTo { get; set; }
        public decimal?[] InStockBetween { get; set; }
        public decimal?[] InStockIn { get; set; }

        public short? QuantityDecimalPlaces { get; set; }

        public short? QuantityDecimalPlacesGreaterThanOrEqualTo { get; set; }
        public short? QuantityDecimalPlacesGreaterThan { get; set; }
        public short? QuantityDecimalPlacesLessThan { get; set; }
        public short? QuantityDecimalPlacesLessThanOrEqualTo { get; set; }
        public short? QuantityDecimalPlacesNotEqualTo { get; set; }
        public short?[] QuantityDecimalPlacesBetween { get; set; }
        public short?[] QuantityDecimalPlacesIn { get; set; }

        public short? Status { get; set; }

        public short? StatusGreaterThanOrEqualTo { get; set; }
        public short? StatusGreaterThan { get; set; }
        public short? StatusLessThan { get; set; }
        public short? StatusLessThanOrEqualTo { get; set; }
        public short? StatusNotEqualTo { get; set; }
        public short[] StatusBetween { get; set; }
        public short[] StatusIn { get; set; }

        public bool? WebEnabled { get; set; }

    }
    #endregion

    #region "Debtors"
    public partial class v_Jiwa_Debtor_ListOR
    {
        [Required]
        public virtual string DebtorID { get; set; }

        [Required]
        public virtual string AccountNo { get; set; }

        public virtual string Name { get; set; }
        public virtual string AltAccountNo { get; set; }
        public virtual string Address1 { get; set; }
        public virtual string Address2 { get; set; }
        public virtual string Address3 { get; set; }
        public virtual string Address4 { get; set; }
        public virtual string PostCode { get; set; }
        public virtual string Country { get; set; }
        public virtual string Phone { get; set; }
        public virtual bool? AccountOnHold { get; set; }
        public virtual string EmailAddress { get; set; }
        public virtual decimal? CurrentBalance { get; set; }
        [Required]
        public virtual bool WebAccess { get; set; }

        [Required]
        public virtual DateTimeOffset LastSavedDateTime { get; set; }

        public virtual byte? TradingStatus { get; set; }
        [Required]
        public virtual string DebtorClassificationID { get; set; }

        [Required]
        public virtual string ClassificationDescription { get; set; }

        [Required]
        public virtual string Category1ID { get; set; }

        public virtual string Category1Description { get; set; }
        [Required]
        public virtual string Category2ID { get; set; }

        public virtual string Category2Description { get; set; }
        [Required]
        public virtual string Category3ID { get; set; }

        public virtual string Category3Description { get; set; }
        [Required]
        public virtual string Category4ID { get; set; }

        public virtual string Category4Description { get; set; }
        [Required]
        public virtual string Category5ID { get; set; }

        public virtual string Category5Description { get; set; }
        [Required]
        public virtual string PriceSchemeID { get; set; }

        [Required]
        public virtual string PriceSchemeDescription { get; set; }

        [Required]
        public virtual string PricingGroupDescription { get; set; }
    }

    [Route("/Queries/OR/DebtorList", "GET")]
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    public partial class v_Jiwa_Debtor_ListORQuery
        : QueryDb<v_Jiwa_Debtor_ListOR>, IReturn<QueryResponse<v_Jiwa_Debtor_ListOR>>
    {
        public virtual string DebtorID { get; set; }
        public virtual string DebtorIDStartsWith { get; set; }
        public virtual string DebtorIDEndsWith { get; set; }
        public virtual string DebtorIDContains { get; set; }
        public virtual string DebtorIDLike { get; set; }
        public virtual string[] DebtorIDBetween { get; set; }
        public virtual string[] DebtorIDIn { get; set; }
        public virtual string AccountNo { get; set; }
        public virtual string AccountNoStartsWith { get; set; }
        public virtual string AccountNoEndsWith { get; set; }
        public virtual string AccountNoContains { get; set; }
        public virtual string AccountNoLike { get; set; }
        public virtual string[] AccountNoBetween { get; set; }
        public virtual string[] AccountNoIn { get; set; }
        public virtual string Name { get; set; }
        public virtual string NameStartsWith { get; set; }
        public virtual string NameEndsWith { get; set; }
        public virtual string NameContains { get; set; }
        public virtual string NameLike { get; set; }
        public virtual string[] NameBetween { get; set; }
        public virtual string[] NameIn { get; set; }
        public virtual string AltAccountNo { get; set; }
        public virtual string AltAccountNoStartsWith { get; set; }
        public virtual string AltAccountNoEndsWith { get; set; }
        public virtual string AltAccountNoContains { get; set; }
        public virtual string AltAccountNoLike { get; set; }
        public virtual string[] AltAccountNoBetween { get; set; }
        public virtual string[] AltAccountNoIn { get; set; }
        public virtual string Address1 { get; set; }
        public virtual string Address1StartsWith { get; set; }
        public virtual string Address1EndsWith { get; set; }
        public virtual string Address1Contains { get; set; }
        public virtual string Address1Like { get; set; }
        public virtual string[] Address1Between { get; set; }
        public virtual string[] Address1In { get; set; }
        public virtual string Address2 { get; set; }
        public virtual string Address2StartsWith { get; set; }
        public virtual string Address2EndsWith { get; set; }
        public virtual string Address2Contains { get; set; }
        public virtual string Address2Like { get; set; }
        public virtual string[] Address2Between { get; set; }
        public virtual string[] Address2In { get; set; }
        public virtual string Address3 { get; set; }
        public virtual string Address3StartsWith { get; set; }
        public virtual string Address3EndsWith { get; set; }
        public virtual string Address3Contains { get; set; }
        public virtual string Address3Like { get; set; }
        public virtual string[] Address3Between { get; set; }
        public virtual string[] Address3In { get; set; }
        public virtual string Address4 { get; set; }
        public virtual string Address4StartsWith { get; set; }
        public virtual string Address4EndsWith { get; set; }
        public virtual string Address4Contains { get; set; }
        public virtual string Address4Like { get; set; }
        public virtual string[] Address4Between { get; set; }
        public virtual string[] Address4In { get; set; }
        public virtual string PostCode { get; set; }
        public virtual string PostCodeStartsWith { get; set; }
        public virtual string PostCodeEndsWith { get; set; }
        public virtual string PostCodeContains { get; set; }
        public virtual string PostCodeLike { get; set; }
        public virtual string[] PostCodeBetween { get; set; }
        public virtual string[] PostCodeIn { get; set; }
        public virtual string Country { get; set; }
        public virtual string CountryStartsWith { get; set; }
        public virtual string CountryEndsWith { get; set; }
        public virtual string CountryContains { get; set; }
        public virtual string CountryLike { get; set; }
        public virtual string[] CountryBetween { get; set; }
        public virtual string[] CountryIn { get; set; }
        public virtual string Phone { get; set; }
        public virtual string PhoneStartsWith { get; set; }
        public virtual string PhoneEndsWith { get; set; }
        public virtual string PhoneContains { get; set; }
        public virtual string PhoneLike { get; set; }
        public virtual string[] PhoneBetween { get; set; }
        public virtual string[] PhoneIn { get; set; }
        public virtual bool? AccountOnHold { get; set; }
        public virtual string EmailAddress { get; set; }
        public virtual string EmailAddressStartsWith { get; set; }
        public virtual string EmailAddressEndsWith { get; set; }
        public virtual string EmailAddressContains { get; set; }
        public virtual string EmailAddressLike { get; set; }
        public virtual string[] EmailAddressBetween { get; set; }
        public virtual string[] EmailAddressIn { get; set; }
        public virtual decimal? CurrentBalance { get; set; }
        public virtual decimal? CurrentBalanceGreaterThanOrEqualTo { get; set; }
        public virtual decimal? CurrentBalanceGreaterThan { get; set; }
        public virtual decimal? CurrentBalanceLessThan { get; set; }
        public virtual decimal? CurrentBalanceLessThanOrEqualTo { get; set; }
        public virtual decimal? CurrentBalanceNotEqualTo { get; set; }
        public virtual decimal?[] CurrentBalanceBetween { get; set; }
        public virtual decimal?[] CurrentBalanceIn { get; set; }
        public virtual bool? WebAccess { get; set; }
        public virtual DateTimeOffset? LastSavedDateTime { get; set; }
        public virtual DateTimeOffset? LastSavedDateTimeGreaterThanOrEqualTo { get; set; }
        public virtual DateTimeOffset? LastSavedDateTimeGreaterThan { get; set; }
        public virtual DateTimeOffset? LastSavedDateTimeLessThan { get; set; }
        public virtual DateTimeOffset? LastSavedDateTimeLessThanOrEqualTo { get; set; }
        public virtual DateTimeOffset? LastSavedDateTimeNotEqualTo { get; set; }
        public virtual DateTimeOffset[] LastSavedDateTimeBetween { get; set; }
        public virtual DateTimeOffset[] LastSavedDateTimeIn { get; set; }
        public virtual byte? TradingStatus { get; set; }
        public virtual byte? TradingStatusGreaterThanOrEqualTo { get; set; }
        public virtual byte? TradingStatusGreaterThan { get; set; }
        public virtual byte? TradingStatusLessThan { get; set; }
        public virtual byte? TradingStatusLessThanOrEqualTo { get; set; }
        public virtual byte? TradingStatusNotEqualTo { get; set; }
        public virtual byte?[] TradingStatusBetween { get; set; }
        public virtual byte?[] TradingStatusIn { get; set; }
        public virtual string DebtorClassificationID { get; set; }
        public virtual string DebtorClassificationIDStartsWith { get; set; }
        public virtual string DebtorClassificationIDEndsWith { get; set; }
        public virtual string DebtorClassificationIDContains { get; set; }
        public virtual string DebtorClassificationIDLike { get; set; }
        public virtual string[] DebtorClassificationIDBetween { get; set; }
        public virtual string[] DebtorClassificationIDIn { get; set; }
        public virtual string ClassificationDescription { get; set; }
        public virtual string ClassificationDescriptionStartsWith { get; set; }
        public virtual string ClassificationDescriptionEndsWith { get; set; }
        public virtual string ClassificationDescriptionContains { get; set; }
        public virtual string ClassificationDescriptionLike { get; set; }
        public virtual string[] ClassificationDescriptionBetween { get; set; }
        public virtual string[] ClassificationDescriptionIn { get; set; }
        public virtual string Category1ID { get; set; }
        public virtual string Category1IDStartsWith { get; set; }
        public virtual string Category1IDEndsWith { get; set; }
        public virtual string Category1IDContains { get; set; }
        public virtual string Category1IDLike { get; set; }
        public virtual string[] Category1IDBetween { get; set; }
        public virtual string[] Category1IDIn { get; set; }
        public virtual string Category1Description { get; set; }
        public virtual string Category1DescriptionStartsWith { get; set; }
        public virtual string Category1DescriptionEndsWith { get; set; }
        public virtual string Category1DescriptionContains { get; set; }
        public virtual string Category1DescriptionLike { get; set; }
        public virtual string[] Category1DescriptionBetween { get; set; }
        public virtual string[] Category1DescriptionIn { get; set; }
        public virtual string Category2ID { get; set; }
        public virtual string Category2IDStartsWith { get; set; }
        public virtual string Category2IDEndsWith { get; set; }
        public virtual string Category2IDContains { get; set; }
        public virtual string Category2IDLike { get; set; }
        public virtual string[] Category2IDBetween { get; set; }
        public virtual string[] Category2IDIn { get; set; }
        public virtual string Category2Description { get; set; }
        public virtual string Category2DescriptionStartsWith { get; set; }
        public virtual string Category2DescriptionEndsWith { get; set; }
        public virtual string Category2DescriptionContains { get; set; }
        public virtual string Category2DescriptionLike { get; set; }
        public virtual string[] Category2DescriptionBetween { get; set; }
        public virtual string[] Category2DescriptionIn { get; set; }
        public virtual string Category3ID { get; set; }
        public virtual string Category3IDStartsWith { get; set; }
        public virtual string Category3IDEndsWith { get; set; }
        public virtual string Category3IDContains { get; set; }
        public virtual string Category3IDLike { get; set; }
        public virtual string[] Category3IDBetween { get; set; }
        public virtual string[] Category3IDIn { get; set; }
        public virtual string Category3Description { get; set; }
        public virtual string Category3DescriptionStartsWith { get; set; }
        public virtual string Category3DescriptionEndsWith { get; set; }
        public virtual string Category3DescriptionContains { get; set; }
        public virtual string Category3DescriptionLike { get; set; }
        public virtual string[] Category3DescriptionBetween { get; set; }
        public virtual string[] Category3DescriptionIn { get; set; }
        public virtual string Category4ID { get; set; }
        public virtual string Category4IDStartsWith { get; set; }
        public virtual string Category4IDEndsWith { get; set; }
        public virtual string Category4IDContains { get; set; }
        public virtual string Category4IDLike { get; set; }
        public virtual string[] Category4IDBetween { get; set; }
        public virtual string[] Category4IDIn { get; set; }
        public virtual string Category4Description { get; set; }
        public virtual string Category4DescriptionStartsWith { get; set; }
        public virtual string Category4DescriptionEndsWith { get; set; }
        public virtual string Category4DescriptionContains { get; set; }
        public virtual string Category4DescriptionLike { get; set; }
        public virtual string[] Category4DescriptionBetween { get; set; }
        public virtual string[] Category4DescriptionIn { get; set; }
        public virtual string Category5ID { get; set; }
        public virtual string Category5IDStartsWith { get; set; }
        public virtual string Category5IDEndsWith { get; set; }
        public virtual string Category5IDContains { get; set; }
        public virtual string Category5IDLike { get; set; }
        public virtual string[] Category5IDBetween { get; set; }
        public virtual string[] Category5IDIn { get; set; }
        public virtual string Category5Description { get; set; }
        public virtual string Category5DescriptionStartsWith { get; set; }
        public virtual string Category5DescriptionEndsWith { get; set; }
        public virtual string Category5DescriptionContains { get; set; }
        public virtual string Category5DescriptionLike { get; set; }
        public virtual string[] Category5DescriptionBetween { get; set; }
        public virtual string[] Category5DescriptionIn { get; set; }
        public virtual string PriceSchemeID { get; set; }
        public virtual string PriceSchemeIDStartsWith { get; set; }
        public virtual string PriceSchemeIDEndsWith { get; set; }
        public virtual string PriceSchemeIDContains { get; set; }
        public virtual string PriceSchemeIDLike { get; set; }
        public virtual string[] PriceSchemeIDBetween { get; set; }
        public virtual string[] PriceSchemeIDIn { get; set; }
        public virtual string PriceSchemeDescription { get; set; }
        public virtual string PriceSchemeDescriptionStartsWith { get; set; }
        public virtual string PriceSchemeDescriptionEndsWith { get; set; }
        public virtual string PriceSchemeDescriptionContains { get; set; }
        public virtual string PriceSchemeDescriptionLike { get; set; }
        public virtual string[] PriceSchemeDescriptionBetween { get; set; }
        public virtual string[] PriceSchemeDescriptionIn { get; set; }
        public virtual string PricingGroupDescription { get; set; }
        public virtual string PricingGroupDescriptionStartsWith { get; set; }
        public virtual string PricingGroupDescriptionEndsWith { get; set; }
        public virtual string PricingGroupDescriptionContains { get; set; }
        public virtual string PricingGroupDescriptionLike { get; set; }
        public virtual string[] PricingGroupDescriptionBetween { get; set; }
        public virtual string[] PricingGroupDescriptionIn { get; set; }
    }

    #endregion
}

namespace JiwaFinancials.Jiwa.JiwaServiceModel.Tables
{
    #region "Inventory"
    public partial class IN_Main
    {
        public IN_Main()
        {
            RowHash = new byte[] { };
            Picture = new byte[] { };
        }

        [Required]
        public virtual string InventoryID { get; set; }

        [Required]
        public virtual DateTimeOffset LastSavedDateTime { get; set; }

        [Required]
        public virtual string PartNo { get; set; }

        public virtual string Description { get; set; }
        public virtual string Units { get; set; }
        [Required]
        public virtual bool PhysicalItem { get; set; }

        [Required]
        public virtual bool Discountable { get; set; }

        public virtual decimal? DirectTax { get; set; }
        [Required]
        public virtual string Catagory1ID { get; set; }

        [Required]
        public virtual string Catagory2ID { get; set; }

        [Required]
        public virtual string Catagory3ID { get; set; }

        [Required]
        public virtual string Catagory4ID { get; set; }

        [Required]
        public virtual string Catagory5ID { get; set; }

        [Required]
        public virtual string ClassificationID { get; set; }

        [Required]
        public virtual short Status { get; set; }

        public virtual decimal? DefaultPrice { get; set; }
        public virtual decimal? RRPPrice { get; set; }
        public virtual decimal? LCost { get; set; }
        public virtual decimal? SCost { get; set; }
        public virtual short? DecimalPlaces { get; set; }
        public virtual decimal? MinimumGP { get; set; }
        public virtual decimal? Weight { get; set; }
        public virtual decimal? Cubic { get; set; }
        [Required]
        public virtual bool UseSerialNo { get; set; }

        public virtual string Aux1 { get; set; }
        public virtual string Aux2 { get; set; }
        public virtual string Aux3 { get; set; }
        public virtual string Aux4 { get; set; }
        public virtual string Aux5 { get; set; }
        [Required]
        public virtual bool BackOrderable { get; set; }

        public virtual string LedgerInvValue { get; set; }
        public virtual string LedgerMovement_COG { get; set; }
        public virtual string LedgerExpAsset { get; set; }
        public virtual string LedgerExpLiab { get; set; }
        public virtual string LedgerDelAsset { get; set; }
        public virtual string LedgerDelLiab { get; set; }
        public virtual decimal? SalesManCost { get; set; }
        public virtual string LedgerAssignedValue { get; set; }
        public virtual string LedgerCogVariance { get; set; }
        public virtual string LedgerInvSales { get; set; }
        public virtual string LedgerAccumulator { get; set; }
        public virtual string LedgerPurchases { get; set; }
        public virtual string LedgerShipComplete { get; set; }
        public virtual string LedgerWriteOn { get; set; }
        public virtual string LedgerWriteOff { get; set; }
        public virtual string LedgerCostPriceAdj { get; set; }
        [Required]
        public virtual short BOMObject { get; set; }

        [Required]
        public virtual bool UseExpiryDate { get; set; }

        [Required]
        public virtual bool UseStandardCost { get; set; }

        public virtual decimal? StandardCost { get; set; }
        [Required]
        public virtual bool WebEnabled { get; set; }

        public virtual string GSTInwardsID { get; set; }
        public virtual string GSTOutwardsID { get; set; }
        public virtual string GSTAdjustmentsINID { get; set; }
        public virtual string GSTAdjustmentsOUTID { get; set; }
        [Required]
        public virtual bool SellPriceIncTax { get; set; }

        public virtual string StyleID { get; set; }
        public virtual string ColourID { get; set; }
        public virtual string SizeID { get; set; }
        public virtual short? PartEncodeOrder { get; set; }
        public virtual bool? TypeStyle { get; set; }
        public virtual string MatrixDescription { get; set; }
        public virtual decimal? SecondaryCost { get; set; }
        public virtual string PricingGroupID { get; set; }
        public virtual bool? ShipWithPhysicalItem { get; set; }
        [Required]
        public virtual byte[] RowHash { get; set; }

        public virtual byte[] Picture { get; set; }
        public virtual string WebStoreDescription { get; set; }
        public virtual string WebStoreShortDescription { get; set; }
    }

    [Route("/Queries/IN_Main", "GET")]
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    public partial class IN_MainQuery
        : QueryDb<IN_Main>, IReturn<QueryResponse<IN_Main>>
    {
        public virtual string InventoryID { get; set; }
        public virtual string InventoryIDStartsWith { get; set; }
        public virtual string InventoryIDEndsWith { get; set; }
        public virtual string InventoryIDContains { get; set; }
        public virtual string InventoryIDLike { get; set; }
        public virtual string[] InventoryIDBetween { get; set; }
        public virtual string[] InventoryIDIn { get; set; }
        public virtual DateTimeOffset? LastSavedDateTime { get; set; }
        public virtual DateTimeOffset? LastSavedDateTimeGreaterThanOrEqualTo { get; set; }
        public virtual DateTimeOffset? LastSavedDateTimeGreaterThan { get; set; }
        public virtual DateTimeOffset? LastSavedDateTimeLessThan { get; set; }
        public virtual DateTimeOffset? LastSavedDateTimeLessThanOrEqualTo { get; set; }
        public virtual DateTimeOffset? LastSavedDateTimeNotEqualTo { get; set; }
        public virtual DateTimeOffset[] LastSavedDateTimeBetween { get; set; }
        public virtual DateTimeOffset[] LastSavedDateTimeIn { get; set; }
        public virtual string PartNo { get; set; }
        public virtual string PartNoStartsWith { get; set; }
        public virtual string PartNoEndsWith { get; set; }
        public virtual string PartNoContains { get; set; }
        public virtual string PartNoLike { get; set; }
        public virtual string[] PartNoBetween { get; set; }
        public virtual string[] PartNoIn { get; set; }
        public virtual string Description { get; set; }
        public virtual string DescriptionStartsWith { get; set; }
        public virtual string DescriptionEndsWith { get; set; }
        public virtual string DescriptionContains { get; set; }
        public virtual string DescriptionLike { get; set; }
        public virtual string[] DescriptionBetween { get; set; }
        public virtual string[] DescriptionIn { get; set; }
        public virtual string Units { get; set; }
        public virtual string UnitsStartsWith { get; set; }
        public virtual string UnitsEndsWith { get; set; }
        public virtual string UnitsContains { get; set; }
        public virtual string UnitsLike { get; set; }
        public virtual string[] UnitsBetween { get; set; }
        public virtual string[] UnitsIn { get; set; }
        public virtual bool? PhysicalItem { get; set; }
        public virtual bool? Discountable { get; set; }
        public virtual decimal? DirectTax { get; set; }
        public virtual decimal? DirectTaxGreaterThanOrEqualTo { get; set; }
        public virtual decimal? DirectTaxGreaterThan { get; set; }
        public virtual decimal? DirectTaxLessThan { get; set; }
        public virtual decimal? DirectTaxLessThanOrEqualTo { get; set; }
        public virtual decimal? DirectTaxNotEqualTo { get; set; }
        public virtual decimal?[] DirectTaxBetween { get; set; }
        public virtual decimal?[] DirectTaxIn { get; set; }
        public virtual string Catagory1ID { get; set; }
        public virtual string Catagory1IDStartsWith { get; set; }
        public virtual string Catagory1IDEndsWith { get; set; }
        public virtual string Catagory1IDContains { get; set; }
        public virtual string Catagory1IDLike { get; set; }
        public virtual string[] Catagory1IDBetween { get; set; }
        public virtual string[] Catagory1IDIn { get; set; }
        public virtual string Catagory2ID { get; set; }
        public virtual string Catagory2IDStartsWith { get; set; }
        public virtual string Catagory2IDEndsWith { get; set; }
        public virtual string Catagory2IDContains { get; set; }
        public virtual string Catagory2IDLike { get; set; }
        public virtual string[] Catagory2IDBetween { get; set; }
        public virtual string[] Catagory2IDIn { get; set; }
        public virtual string Catagory3ID { get; set; }
        public virtual string Catagory3IDStartsWith { get; set; }
        public virtual string Catagory3IDEndsWith { get; set; }
        public virtual string Catagory3IDContains { get; set; }
        public virtual string Catagory3IDLike { get; set; }
        public virtual string[] Catagory3IDBetween { get; set; }
        public virtual string[] Catagory3IDIn { get; set; }
        public virtual string Catagory4ID { get; set; }
        public virtual string Catagory4IDStartsWith { get; set; }
        public virtual string Catagory4IDEndsWith { get; set; }
        public virtual string Catagory4IDContains { get; set; }
        public virtual string Catagory4IDLike { get; set; }
        public virtual string[] Catagory4IDBetween { get; set; }
        public virtual string[] Catagory4IDIn { get; set; }
        public virtual string Catagory5ID { get; set; }
        public virtual string Catagory5IDStartsWith { get; set; }
        public virtual string Catagory5IDEndsWith { get; set; }
        public virtual string Catagory5IDContains { get; set; }
        public virtual string Catagory5IDLike { get; set; }
        public virtual string[] Catagory5IDBetween { get; set; }
        public virtual string[] Catagory5IDIn { get; set; }
        public virtual string ClassificationID { get; set; }
        public virtual string ClassificationIDStartsWith { get; set; }
        public virtual string ClassificationIDEndsWith { get; set; }
        public virtual string ClassificationIDContains { get; set; }
        public virtual string ClassificationIDLike { get; set; }
        public virtual string[] ClassificationIDBetween { get; set; }
        public virtual string[] ClassificationIDIn { get; set; }
        public virtual short? Status { get; set; }
        public virtual short? StatusGreaterThanOrEqualTo { get; set; }
        public virtual short? StatusGreaterThan { get; set; }
        public virtual short? StatusLessThan { get; set; }
        public virtual short? StatusLessThanOrEqualTo { get; set; }
        public virtual short? StatusNotEqualTo { get; set; }
        public virtual short[] StatusBetween { get; set; }
        public virtual short[] StatusIn { get; set; }
        public virtual decimal? DefaultPrice { get; set; }
        public virtual decimal? DefaultPriceGreaterThanOrEqualTo { get; set; }
        public virtual decimal? DefaultPriceGreaterThan { get; set; }
        public virtual decimal? DefaultPriceLessThan { get; set; }
        public virtual decimal? DefaultPriceLessThanOrEqualTo { get; set; }
        public virtual decimal? DefaultPriceNotEqualTo { get; set; }
        public virtual decimal?[] DefaultPriceBetween { get; set; }
        public virtual decimal?[] DefaultPriceIn { get; set; }
        public virtual decimal? RRPPrice { get; set; }
        public virtual decimal? RRPPriceGreaterThanOrEqualTo { get; set; }
        public virtual decimal? RRPPriceGreaterThan { get; set; }
        public virtual decimal? RRPPriceLessThan { get; set; }
        public virtual decimal? RRPPriceLessThanOrEqualTo { get; set; }
        public virtual decimal? RRPPriceNotEqualTo { get; set; }
        public virtual decimal?[] RRPPriceBetween { get; set; }
        public virtual decimal?[] RRPPriceIn { get; set; }
        public virtual decimal? LCost { get; set; }
        public virtual decimal? LCostGreaterThanOrEqualTo { get; set; }
        public virtual decimal? LCostGreaterThan { get; set; }
        public virtual decimal? LCostLessThan { get; set; }
        public virtual decimal? LCostLessThanOrEqualTo { get; set; }
        public virtual decimal? LCostNotEqualTo { get; set; }
        public virtual decimal?[] LCostBetween { get; set; }
        public virtual decimal?[] LCostIn { get; set; }
        public virtual decimal? SCost { get; set; }
        public virtual decimal? SCostGreaterThanOrEqualTo { get; set; }
        public virtual decimal? SCostGreaterThan { get; set; }
        public virtual decimal? SCostLessThan { get; set; }
        public virtual decimal? SCostLessThanOrEqualTo { get; set; }
        public virtual decimal? SCostNotEqualTo { get; set; }
        public virtual decimal?[] SCostBetween { get; set; }
        public virtual decimal?[] SCostIn { get; set; }
        public virtual short? DecimalPlaces { get; set; }
        public virtual short? DecimalPlacesGreaterThanOrEqualTo { get; set; }
        public virtual short? DecimalPlacesGreaterThan { get; set; }
        public virtual short? DecimalPlacesLessThan { get; set; }
        public virtual short? DecimalPlacesLessThanOrEqualTo { get; set; }
        public virtual short? DecimalPlacesNotEqualTo { get; set; }
        public virtual short?[] DecimalPlacesBetween { get; set; }
        public virtual short?[] DecimalPlacesIn { get; set; }
        public virtual decimal? MinimumGP { get; set; }
        public virtual decimal? MinimumGPGreaterThanOrEqualTo { get; set; }
        public virtual decimal? MinimumGPGreaterThan { get; set; }
        public virtual decimal? MinimumGPLessThan { get; set; }
        public virtual decimal? MinimumGPLessThanOrEqualTo { get; set; }
        public virtual decimal? MinimumGPNotEqualTo { get; set; }
        public virtual decimal?[] MinimumGPBetween { get; set; }
        public virtual decimal?[] MinimumGPIn { get; set; }
        public virtual decimal? Weight { get; set; }
        public virtual decimal? WeightGreaterThanOrEqualTo { get; set; }
        public virtual decimal? WeightGreaterThan { get; set; }
        public virtual decimal? WeightLessThan { get; set; }
        public virtual decimal? WeightLessThanOrEqualTo { get; set; }
        public virtual decimal? WeightNotEqualTo { get; set; }
        public virtual decimal?[] WeightBetween { get; set; }
        public virtual decimal?[] WeightIn { get; set; }
        public virtual decimal? Cubic { get; set; }
        public virtual decimal? CubicGreaterThanOrEqualTo { get; set; }
        public virtual decimal? CubicGreaterThan { get; set; }
        public virtual decimal? CubicLessThan { get; set; }
        public virtual decimal? CubicLessThanOrEqualTo { get; set; }
        public virtual decimal? CubicNotEqualTo { get; set; }
        public virtual decimal?[] CubicBetween { get; set; }
        public virtual decimal?[] CubicIn { get; set; }
        public virtual bool? UseSerialNo { get; set; }
        public virtual string Aux1 { get; set; }
        public virtual string Aux1StartsWith { get; set; }
        public virtual string Aux1EndsWith { get; set; }
        public virtual string Aux1Contains { get; set; }
        public virtual string Aux1Like { get; set; }
        public virtual string[] Aux1Between { get; set; }
        public virtual string[] Aux1In { get; set; }
        public virtual string Aux2 { get; set; }
        public virtual string Aux2StartsWith { get; set; }
        public virtual string Aux2EndsWith { get; set; }
        public virtual string Aux2Contains { get; set; }
        public virtual string Aux2Like { get; set; }
        public virtual string[] Aux2Between { get; set; }
        public virtual string[] Aux2In { get; set; }
        public virtual string Aux3 { get; set; }
        public virtual string Aux3StartsWith { get; set; }
        public virtual string Aux3EndsWith { get; set; }
        public virtual string Aux3Contains { get; set; }
        public virtual string Aux3Like { get; set; }
        public virtual string[] Aux3Between { get; set; }
        public virtual string[] Aux3In { get; set; }
        public virtual string Aux4 { get; set; }
        public virtual string Aux4StartsWith { get; set; }
        public virtual string Aux4EndsWith { get; set; }
        public virtual string Aux4Contains { get; set; }
        public virtual string Aux4Like { get; set; }
        public virtual string[] Aux4Between { get; set; }
        public virtual string[] Aux4In { get; set; }
        public virtual string Aux5 { get; set; }
        public virtual string Aux5StartsWith { get; set; }
        public virtual string Aux5EndsWith { get; set; }
        public virtual string Aux5Contains { get; set; }
        public virtual string Aux5Like { get; set; }
        public virtual string[] Aux5Between { get; set; }
        public virtual string[] Aux5In { get; set; }
        public virtual bool? BackOrderable { get; set; }
        public virtual string LedgerInvValue { get; set; }
        public virtual string LedgerInvValueStartsWith { get; set; }
        public virtual string LedgerInvValueEndsWith { get; set; }
        public virtual string LedgerInvValueContains { get; set; }
        public virtual string LedgerInvValueLike { get; set; }
        public virtual string[] LedgerInvValueBetween { get; set; }
        public virtual string[] LedgerInvValueIn { get; set; }
        public virtual string LedgerMovement_COG { get; set; }
        public virtual string LedgerMovement_COGStartsWith { get; set; }
        public virtual string LedgerMovement_COGEndsWith { get; set; }
        public virtual string LedgerMovement_COGContains { get; set; }
        public virtual string LedgerMovement_COGLike { get; set; }
        public virtual string[] LedgerMovement_COGBetween { get; set; }
        public virtual string[] LedgerMovement_COGIn { get; set; }
        public virtual string LedgerExpAsset { get; set; }
        public virtual string LedgerExpAssetStartsWith { get; set; }
        public virtual string LedgerExpAssetEndsWith { get; set; }
        public virtual string LedgerExpAssetContains { get; set; }
        public virtual string LedgerExpAssetLike { get; set; }
        public virtual string[] LedgerExpAssetBetween { get; set; }
        public virtual string[] LedgerExpAssetIn { get; set; }
        public virtual string LedgerExpLiab { get; set; }
        public virtual string LedgerExpLiabStartsWith { get; set; }
        public virtual string LedgerExpLiabEndsWith { get; set; }
        public virtual string LedgerExpLiabContains { get; set; }
        public virtual string LedgerExpLiabLike { get; set; }
        public virtual string[] LedgerExpLiabBetween { get; set; }
        public virtual string[] LedgerExpLiabIn { get; set; }
        public virtual string LedgerDelAsset { get; set; }
        public virtual string LedgerDelAssetStartsWith { get; set; }
        public virtual string LedgerDelAssetEndsWith { get; set; }
        public virtual string LedgerDelAssetContains { get; set; }
        public virtual string LedgerDelAssetLike { get; set; }
        public virtual string[] LedgerDelAssetBetween { get; set; }
        public virtual string[] LedgerDelAssetIn { get; set; }
        public virtual string LedgerDelLiab { get; set; }
        public virtual string LedgerDelLiabStartsWith { get; set; }
        public virtual string LedgerDelLiabEndsWith { get; set; }
        public virtual string LedgerDelLiabContains { get; set; }
        public virtual string LedgerDelLiabLike { get; set; }
        public virtual string[] LedgerDelLiabBetween { get; set; }
        public virtual string[] LedgerDelLiabIn { get; set; }
        public virtual decimal? SalesManCost { get; set; }
        public virtual decimal? SalesManCostGreaterThanOrEqualTo { get; set; }
        public virtual decimal? SalesManCostGreaterThan { get; set; }
        public virtual decimal? SalesManCostLessThan { get; set; }
        public virtual decimal? SalesManCostLessThanOrEqualTo { get; set; }
        public virtual decimal? SalesManCostNotEqualTo { get; set; }
        public virtual decimal?[] SalesManCostBetween { get; set; }
        public virtual decimal?[] SalesManCostIn { get; set; }
        public virtual string LedgerAssignedValue { get; set; }
        public virtual string LedgerAssignedValueStartsWith { get; set; }
        public virtual string LedgerAssignedValueEndsWith { get; set; }
        public virtual string LedgerAssignedValueContains { get; set; }
        public virtual string LedgerAssignedValueLike { get; set; }
        public virtual string[] LedgerAssignedValueBetween { get; set; }
        public virtual string[] LedgerAssignedValueIn { get; set; }
        public virtual string LedgerCogVariance { get; set; }
        public virtual string LedgerCogVarianceStartsWith { get; set; }
        public virtual string LedgerCogVarianceEndsWith { get; set; }
        public virtual string LedgerCogVarianceContains { get; set; }
        public virtual string LedgerCogVarianceLike { get; set; }
        public virtual string[] LedgerCogVarianceBetween { get; set; }
        public virtual string[] LedgerCogVarianceIn { get; set; }
        public virtual string LedgerInvSales { get; set; }
        public virtual string LedgerInvSalesStartsWith { get; set; }
        public virtual string LedgerInvSalesEndsWith { get; set; }
        public virtual string LedgerInvSalesContains { get; set; }
        public virtual string LedgerInvSalesLike { get; set; }
        public virtual string[] LedgerInvSalesBetween { get; set; }
        public virtual string[] LedgerInvSalesIn { get; set; }
        public virtual string LedgerAccumulator { get; set; }
        public virtual string LedgerAccumulatorStartsWith { get; set; }
        public virtual string LedgerAccumulatorEndsWith { get; set; }
        public virtual string LedgerAccumulatorContains { get; set; }
        public virtual string LedgerAccumulatorLike { get; set; }
        public virtual string[] LedgerAccumulatorBetween { get; set; }
        public virtual string[] LedgerAccumulatorIn { get; set; }
        public virtual string LedgerPurchases { get; set; }
        public virtual string LedgerPurchasesStartsWith { get; set; }
        public virtual string LedgerPurchasesEndsWith { get; set; }
        public virtual string LedgerPurchasesContains { get; set; }
        public virtual string LedgerPurchasesLike { get; set; }
        public virtual string[] LedgerPurchasesBetween { get; set; }
        public virtual string[] LedgerPurchasesIn { get; set; }
        public virtual string LedgerShipComplete { get; set; }
        public virtual string LedgerShipCompleteStartsWith { get; set; }
        public virtual string LedgerShipCompleteEndsWith { get; set; }
        public virtual string LedgerShipCompleteContains { get; set; }
        public virtual string LedgerShipCompleteLike { get; set; }
        public virtual string[] LedgerShipCompleteBetween { get; set; }
        public virtual string[] LedgerShipCompleteIn { get; set; }
        public virtual string LedgerWriteOn { get; set; }
        public virtual string LedgerWriteOnStartsWith { get; set; }
        public virtual string LedgerWriteOnEndsWith { get; set; }
        public virtual string LedgerWriteOnContains { get; set; }
        public virtual string LedgerWriteOnLike { get; set; }
        public virtual string[] LedgerWriteOnBetween { get; set; }
        public virtual string[] LedgerWriteOnIn { get; set; }
        public virtual string LedgerWriteOff { get; set; }
        public virtual string LedgerWriteOffStartsWith { get; set; }
        public virtual string LedgerWriteOffEndsWith { get; set; }
        public virtual string LedgerWriteOffContains { get; set; }
        public virtual string LedgerWriteOffLike { get; set; }
        public virtual string[] LedgerWriteOffBetween { get; set; }
        public virtual string[] LedgerWriteOffIn { get; set; }
        public virtual string LedgerCostPriceAdj { get; set; }
        public virtual string LedgerCostPriceAdjStartsWith { get; set; }
        public virtual string LedgerCostPriceAdjEndsWith { get; set; }
        public virtual string LedgerCostPriceAdjContains { get; set; }
        public virtual string LedgerCostPriceAdjLike { get; set; }
        public virtual string[] LedgerCostPriceAdjBetween { get; set; }
        public virtual string[] LedgerCostPriceAdjIn { get; set; }
        public virtual short? BOMObject { get; set; }
        public virtual short? BOMObjectGreaterThanOrEqualTo { get; set; }
        public virtual short? BOMObjectGreaterThan { get; set; }
        public virtual short? BOMObjectLessThan { get; set; }
        public virtual short? BOMObjectLessThanOrEqualTo { get; set; }
        public virtual short? BOMObjectNotEqualTo { get; set; }
        public virtual short[] BOMObjectBetween { get; set; }
        public virtual short[] BOMObjectIn { get; set; }
        public virtual bool? UseExpiryDate { get; set; }
        public virtual bool? UseStandardCost { get; set; }
        public virtual decimal? StandardCost { get; set; }
        public virtual decimal? StandardCostGreaterThanOrEqualTo { get; set; }
        public virtual decimal? StandardCostGreaterThan { get; set; }
        public virtual decimal? StandardCostLessThan { get; set; }
        public virtual decimal? StandardCostLessThanOrEqualTo { get; set; }
        public virtual decimal? StandardCostNotEqualTo { get; set; }
        public virtual decimal?[] StandardCostBetween { get; set; }
        public virtual decimal?[] StandardCostIn { get; set; }
        public virtual bool? WebEnabled { get; set; }
        public virtual string GSTInwardsID { get; set; }
        public virtual string GSTInwardsIDStartsWith { get; set; }
        public virtual string GSTInwardsIDEndsWith { get; set; }
        public virtual string GSTInwardsIDContains { get; set; }
        public virtual string GSTInwardsIDLike { get; set; }
        public virtual string[] GSTInwardsIDBetween { get; set; }
        public virtual string[] GSTInwardsIDIn { get; set; }
        public virtual string GSTOutwardsID { get; set; }
        public virtual string GSTOutwardsIDStartsWith { get; set; }
        public virtual string GSTOutwardsIDEndsWith { get; set; }
        public virtual string GSTOutwardsIDContains { get; set; }
        public virtual string GSTOutwardsIDLike { get; set; }
        public virtual string[] GSTOutwardsIDBetween { get; set; }
        public virtual string[] GSTOutwardsIDIn { get; set; }
        public virtual string GSTAdjustmentsINID { get; set; }
        public virtual string GSTAdjustmentsINIDStartsWith { get; set; }
        public virtual string GSTAdjustmentsINIDEndsWith { get; set; }
        public virtual string GSTAdjustmentsINIDContains { get; set; }
        public virtual string GSTAdjustmentsINIDLike { get; set; }
        public virtual string[] GSTAdjustmentsINIDBetween { get; set; }
        public virtual string[] GSTAdjustmentsINIDIn { get; set; }
        public virtual string GSTAdjustmentsOUTID { get; set; }
        public virtual string GSTAdjustmentsOUTIDStartsWith { get; set; }
        public virtual string GSTAdjustmentsOUTIDEndsWith { get; set; }
        public virtual string GSTAdjustmentsOUTIDContains { get; set; }
        public virtual string GSTAdjustmentsOUTIDLike { get; set; }
        public virtual string[] GSTAdjustmentsOUTIDBetween { get; set; }
        public virtual string[] GSTAdjustmentsOUTIDIn { get; set; }
        public virtual bool? SellPriceIncTax { get; set; }
        public virtual string StyleID { get; set; }
        public virtual string StyleIDStartsWith { get; set; }
        public virtual string StyleIDEndsWith { get; set; }
        public virtual string StyleIDContains { get; set; }
        public virtual string StyleIDLike { get; set; }
        public virtual string[] StyleIDBetween { get; set; }
        public virtual string[] StyleIDIn { get; set; }
        public virtual string ColourID { get; set; }
        public virtual string ColourIDStartsWith { get; set; }
        public virtual string ColourIDEndsWith { get; set; }
        public virtual string ColourIDContains { get; set; }
        public virtual string ColourIDLike { get; set; }
        public virtual string[] ColourIDBetween { get; set; }
        public virtual string[] ColourIDIn { get; set; }
        public virtual string SizeID { get; set; }
        public virtual string SizeIDStartsWith { get; set; }
        public virtual string SizeIDEndsWith { get; set; }
        public virtual string SizeIDContains { get; set; }
        public virtual string SizeIDLike { get; set; }
        public virtual string[] SizeIDBetween { get; set; }
        public virtual string[] SizeIDIn { get; set; }
        public virtual short? PartEncodeOrder { get; set; }
        public virtual short? PartEncodeOrderGreaterThanOrEqualTo { get; set; }
        public virtual short? PartEncodeOrderGreaterThan { get; set; }
        public virtual short? PartEncodeOrderLessThan { get; set; }
        public virtual short? PartEncodeOrderLessThanOrEqualTo { get; set; }
        public virtual short? PartEncodeOrderNotEqualTo { get; set; }
        public virtual short?[] PartEncodeOrderBetween { get; set; }
        public virtual short?[] PartEncodeOrderIn { get; set; }
        public virtual bool? TypeStyle { get; set; }
        public virtual string MatrixDescription { get; set; }
        public virtual string MatrixDescriptionStartsWith { get; set; }
        public virtual string MatrixDescriptionEndsWith { get; set; }
        public virtual string MatrixDescriptionContains { get; set; }
        public virtual string MatrixDescriptionLike { get; set; }
        public virtual string[] MatrixDescriptionBetween { get; set; }
        public virtual string[] MatrixDescriptionIn { get; set; }
        public virtual decimal? SecondaryCost { get; set; }
        public virtual decimal? SecondaryCostGreaterThanOrEqualTo { get; set; }
        public virtual decimal? SecondaryCostGreaterThan { get; set; }
        public virtual decimal? SecondaryCostLessThan { get; set; }
        public virtual decimal? SecondaryCostLessThanOrEqualTo { get; set; }
        public virtual decimal? SecondaryCostNotEqualTo { get; set; }
        public virtual decimal?[] SecondaryCostBetween { get; set; }
        public virtual decimal?[] SecondaryCostIn { get; set; }
        public virtual string PricingGroupID { get; set; }
        public virtual string PricingGroupIDStartsWith { get; set; }
        public virtual string PricingGroupIDEndsWith { get; set; }
        public virtual string PricingGroupIDContains { get; set; }
        public virtual string PricingGroupIDLike { get; set; }
        public virtual string[] PricingGroupIDBetween { get; set; }
        public virtual string[] PricingGroupIDIn { get; set; }
        public virtual bool? ShipWithPhysicalItem { get; set; }
        public virtual byte[] RowHash { get; set; }
        public virtual byte[] Picture { get; set; }
        public virtual string WebStoreDescription { get; set; }
        public virtual string WebStoreDescriptionStartsWith { get; set; }
        public virtual string WebStoreDescriptionEndsWith { get; set; }
        public virtual string WebStoreDescriptionContains { get; set; }
        public virtual string WebStoreDescriptionLike { get; set; }
        public virtual string[] WebStoreDescriptionBetween { get; set; }
        public virtual string[] WebStoreDescriptionIn { get; set; }
        public virtual string WebStoreShortDescription { get; set; }
        public virtual string WebStoreShortDescriptionStartsWith { get; set; }
        public virtual string WebStoreShortDescriptionEndsWith { get; set; }
        public virtual string WebStoreShortDescriptionContains { get; set; }
        public virtual string WebStoreShortDescriptionLike { get; set; }
        public virtual string[] WebStoreShortDescriptionBetween { get; set; }
        public virtual string[] WebStoreShortDescriptionIn { get; set; }
    }
    #endregion

    #region "Sales Orders"
    public partial class v_Jiwa_SalesOrder_List
    {
        [Required]
        public virtual string InvoiceID { get; set; }

        [Required]
        public virtual string InvoiceNo { get; set; }

        public virtual string InvoiceNoDashHistoryNo { get; set; }
        public virtual string OrderNo { get; set; }
        public virtual string SOReference { get; set; }
        [Required]
        public virtual DateTime InvoiceInitDate { get; set; }

        public virtual short? Status { get; set; }
        [Required]
        public virtual bool CreditNote { get; set; }

        [Required]
        public virtual DateTimeOffset LastSavedDateTime { get; set; }

        public virtual decimal? LocalInvoiceTotalIncTax { get; set; }
        public virtual decimal? FXInvoiceTotalIncTax { get; set; }
        [Required]
        public virtual string DebtorID { get; set; }

        [Required]
        public virtual string AccountNo { get; set; }

        public virtual string DebtorName { get; set; }
        [Required]
        public virtual string IN_LogicalID { get; set; }

        public virtual string LogicalWarehouseDescription { get; set; }
        [Required]
        public virtual string IN_PhysicalID { get; set; }

        [Required]
        public virtual string PhysicalWarehouseDescription { get; set; }

        [Required]
        public virtual string BranchID { get; set; }

        [Required]
        public virtual string BranchDescription { get; set; }

        public virtual string CashSaleAddress1 { get; set; }
        public virtual string CashSaleAddress2 { get; set; }
        public virtual string CashSaleAddress3 { get; set; }
        public virtual string CashSaleAddress4 { get; set; }
        public virtual string CashSalePostcode { get; set; }
        public virtual string CashSaleCompany { get; set; }
        public virtual string CashSaleName { get; set; }
        public virtual string CashSalePhone { get; set; }
        [Required]
        public virtual string InvoiceHistoryID { get; set; }

        public virtual string DeliveryAddressContactName { get; set; }
        public virtual string DeliveryAddressee { get; set; }
        public virtual string DeliveryAddress1 { get; set; }
        public virtual string DeliveryAddress2 { get; set; }
        public virtual string DeliveryAddress3 { get; set; }
        public virtual string DeliveryAddress4 { get; set; }
        public virtual string DeliveryAddressPostcode { get; set; }
        public virtual string DeliveryAddressCountry { get; set; }
        [Required]
        public virtual bool Delivered { get; set; }

        public virtual DateTime? DeliveredDate { get; set; }
        public virtual string ConsignmentNote { get; set; }
        public virtual decimal? CartageCharge1 { get; set; }
        public virtual decimal? Cartage1TaxAmount { get; set; }
        public virtual decimal? CartageCharge2 { get; set; }
        public virtual decimal? Cartage2TaxAmount { get; set; }
        public virtual decimal? CartageCharge3 { get; set; }
        public virtual decimal? Cartage3TaxAmount { get; set; }
        public virtual decimal? FXCartageCharge1 { get; set; }
        public virtual decimal? FXCartage1TaxAmount { get; set; }
        public virtual decimal? FXCartageCharge2 { get; set; }
        public virtual decimal? FXCartage2TaxAmount { get; set; }
        public virtual decimal? FXCartageCharge3 { get; set; }
        public virtual decimal? FXCartage3TaxAmount { get; set; }
        public virtual string CourierDetails { get; set; }
        public virtual string Notes { get; set; }
        public virtual string EmailAddress { get; set; }
        [Required]
        public virtual string StaffID { get; set; }

        public virtual string StaffTitle { get; set; }
        public virtual string StaffFirstName { get; set; }
        public virtual string StaffSurname { get; set; }
        [Required]
        public virtual string StaffUsername { get; set; }

        public virtual byte? HistoryStatus { get; set; }
        public virtual short? HistoryNo { get; set; }
        [Required]
        public virtual string CurrencyID { get; set; }

        public virtual string CurrencyShortName { get; set; }
        public virtual string CurrencyName { get; set; }
        public virtual short? DecimalPlaces { get; set; }
        public virtual decimal? TotalAllocated { get; set; }
        public virtual DateTime? DueDate { get; set; }
    }

    [Route("/Queries/SalesOrderList", "GET")]
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    public partial class v_Jiwa_SalesOrder_ListQuery
        : QueryDb<v_Jiwa_SalesOrder_List>, IReturn<QueryResponse<v_Jiwa_SalesOrder_List>>
    {
        public virtual string InvoiceID { get; set; }
        public virtual string InvoiceIDStartsWith { get; set; }
        public virtual string InvoiceIDEndsWith { get; set; }
        public virtual string InvoiceIDContains { get; set; }
        public virtual string InvoiceIDLike { get; set; }
        public virtual string[] InvoiceIDBetween { get; set; }
        public virtual string[] InvoiceIDIn { get; set; }
        public virtual string InvoiceNo { get; set; }
        public virtual string InvoiceNoStartsWith { get; set; }
        public virtual string InvoiceNoEndsWith { get; set; }
        public virtual string InvoiceNoContains { get; set; }
        public virtual string InvoiceNoLike { get; set; }
        public virtual string[] InvoiceNoBetween { get; set; }
        public virtual string[] InvoiceNoIn { get; set; }
        public virtual string InvoiceNoDashHistoryNo { get; set; }
        public virtual string InvoiceNoDashHistoryNoStartsWith { get; set; }
        public virtual string InvoiceNoDashHistoryNoEndsWith { get; set; }
        public virtual string InvoiceNoDashHistoryNoContains { get; set; }
        public virtual string InvoiceNoDashHistoryNoLike { get; set; }
        public virtual string[] InvoiceNoDashHistoryNoBetween { get; set; }
        public virtual string[] InvoiceNoDashHistoryNoIn { get; set; }
        public virtual string OrderNo { get; set; }
        public virtual string OrderNoStartsWith { get; set; }
        public virtual string OrderNoEndsWith { get; set; }
        public virtual string OrderNoContains { get; set; }
        public virtual string OrderNoLike { get; set; }
        public virtual string[] OrderNoBetween { get; set; }
        public virtual string[] OrderNoIn { get; set; }
        public virtual string SOReference { get; set; }
        public virtual string SOReferenceStartsWith { get; set; }
        public virtual string SOReferenceEndsWith { get; set; }
        public virtual string SOReferenceContains { get; set; }
        public virtual string SOReferenceLike { get; set; }
        public virtual string[] SOReferenceBetween { get; set; }
        public virtual string[] SOReferenceIn { get; set; }
        public virtual DateTime? InvoiceInitDate { get; set; }
        public virtual DateTime? InvoiceInitDateGreaterThanOrEqualTo { get; set; }
        public virtual DateTime? InvoiceInitDateGreaterThan { get; set; }
        public virtual DateTime? InvoiceInitDateLessThan { get; set; }
        public virtual DateTime? InvoiceInitDateLessThanOrEqualTo { get; set; }
        public virtual DateTime? InvoiceInitDateNotEqualTo { get; set; }
        public virtual DateTime[] InvoiceInitDateBetween { get; set; }
        public virtual DateTime[] InvoiceInitDateIn { get; set; }
        public virtual short? Status { get; set; }
        public virtual short? StatusGreaterThanOrEqualTo { get; set; }
        public virtual short? StatusGreaterThan { get; set; }
        public virtual short? StatusLessThan { get; set; }
        public virtual short? StatusLessThanOrEqualTo { get; set; }
        public virtual short? StatusNotEqualTo { get; set; }
        public virtual short?[] StatusBetween { get; set; }
        public virtual short?[] StatusIn { get; set; }
        public virtual bool? CreditNote { get; set; }
        public virtual DateTimeOffset? LastSavedDateTime { get; set; }
        public virtual DateTimeOffset? LastSavedDateTimeGreaterThanOrEqualTo { get; set; }
        public virtual DateTimeOffset? LastSavedDateTimeGreaterThan { get; set; }
        public virtual DateTimeOffset? LastSavedDateTimeLessThan { get; set; }
        public virtual DateTimeOffset? LastSavedDateTimeLessThanOrEqualTo { get; set; }
        public virtual DateTimeOffset? LastSavedDateTimeNotEqualTo { get; set; }
        public virtual DateTimeOffset[] LastSavedDateTimeBetween { get; set; }
        public virtual DateTimeOffset[] LastSavedDateTimeIn { get; set; }
        public virtual decimal? LocalInvoiceTotalIncTax { get; set; }
        public virtual decimal? LocalInvoiceTotalIncTaxGreaterThanOrEqualTo { get; set; }
        public virtual decimal? LocalInvoiceTotalIncTaxGreaterThan { get; set; }
        public virtual decimal? LocalInvoiceTotalIncTaxLessThan { get; set; }
        public virtual decimal? LocalInvoiceTotalIncTaxLessThanOrEqualTo { get; set; }
        public virtual decimal? LocalInvoiceTotalIncTaxNotEqualTo { get; set; }
        public virtual decimal?[] LocalInvoiceTotalIncTaxBetween { get; set; }
        public virtual decimal?[] LocalInvoiceTotalIncTaxIn { get; set; }
        public virtual decimal? FXInvoiceTotalIncTax { get; set; }
        public virtual decimal? FXInvoiceTotalIncTaxGreaterThanOrEqualTo { get; set; }
        public virtual decimal? FXInvoiceTotalIncTaxGreaterThan { get; set; }
        public virtual decimal? FXInvoiceTotalIncTaxLessThan { get; set; }
        public virtual decimal? FXInvoiceTotalIncTaxLessThanOrEqualTo { get; set; }
        public virtual decimal? FXInvoiceTotalIncTaxNotEqualTo { get; set; }
        public virtual decimal?[] FXInvoiceTotalIncTaxBetween { get; set; }
        public virtual decimal?[] FXInvoiceTotalIncTaxIn { get; set; }
        public virtual string DebtorID { get; set; }
        public virtual string DebtorIDStartsWith { get; set; }
        public virtual string DebtorIDEndsWith { get; set; }
        public virtual string DebtorIDContains { get; set; }
        public virtual string DebtorIDLike { get; set; }
        public virtual string[] DebtorIDBetween { get; set; }
        public virtual string[] DebtorIDIn { get; set; }
        public virtual string AccountNo { get; set; }
        public virtual string AccountNoStartsWith { get; set; }
        public virtual string AccountNoEndsWith { get; set; }
        public virtual string AccountNoContains { get; set; }
        public virtual string AccountNoLike { get; set; }
        public virtual string[] AccountNoBetween { get; set; }
        public virtual string[] AccountNoIn { get; set; }
        public virtual string DebtorName { get; set; }
        public virtual string DebtorNameStartsWith { get; set; }
        public virtual string DebtorNameEndsWith { get; set; }
        public virtual string DebtorNameContains { get; set; }
        public virtual string DebtorNameLike { get; set; }
        public virtual string[] DebtorNameBetween { get; set; }
        public virtual string[] DebtorNameIn { get; set; }
        public virtual string IN_LogicalID { get; set; }
        public virtual string IN_LogicalIDStartsWith { get; set; }
        public virtual string IN_LogicalIDEndsWith { get; set; }
        public virtual string IN_LogicalIDContains { get; set; }
        public virtual string IN_LogicalIDLike { get; set; }
        public virtual string[] IN_LogicalIDBetween { get; set; }
        public virtual string[] IN_LogicalIDIn { get; set; }
        public virtual string LogicalWarehouseDescription { get; set; }
        public virtual string LogicalWarehouseDescriptionStartsWith { get; set; }
        public virtual string LogicalWarehouseDescriptionEndsWith { get; set; }
        public virtual string LogicalWarehouseDescriptionContains { get; set; }
        public virtual string LogicalWarehouseDescriptionLike { get; set; }
        public virtual string[] LogicalWarehouseDescriptionBetween { get; set; }
        public virtual string[] LogicalWarehouseDescriptionIn { get; set; }
        public virtual string IN_PhysicalID { get; set; }
        public virtual string IN_PhysicalIDStartsWith { get; set; }
        public virtual string IN_PhysicalIDEndsWith { get; set; }
        public virtual string IN_PhysicalIDContains { get; set; }
        public virtual string IN_PhysicalIDLike { get; set; }
        public virtual string[] IN_PhysicalIDBetween { get; set; }
        public virtual string[] IN_PhysicalIDIn { get; set; }
        public virtual string PhysicalWarehouseDescription { get; set; }
        public virtual string PhysicalWarehouseDescriptionStartsWith { get; set; }
        public virtual string PhysicalWarehouseDescriptionEndsWith { get; set; }
        public virtual string PhysicalWarehouseDescriptionContains { get; set; }
        public virtual string PhysicalWarehouseDescriptionLike { get; set; }
        public virtual string[] PhysicalWarehouseDescriptionBetween { get; set; }
        public virtual string[] PhysicalWarehouseDescriptionIn { get; set; }
        public virtual string BranchID { get; set; }
        public virtual string BranchIDStartsWith { get; set; }
        public virtual string BranchIDEndsWith { get; set; }
        public virtual string BranchIDContains { get; set; }
        public virtual string BranchIDLike { get; set; }
        public virtual string[] BranchIDBetween { get; set; }
        public virtual string[] BranchIDIn { get; set; }
        public virtual string BranchDescription { get; set; }
        public virtual string BranchDescriptionStartsWith { get; set; }
        public virtual string BranchDescriptionEndsWith { get; set; }
        public virtual string BranchDescriptionContains { get; set; }
        public virtual string BranchDescriptionLike { get; set; }
        public virtual string[] BranchDescriptionBetween { get; set; }
        public virtual string[] BranchDescriptionIn { get; set; }
        public virtual string CashSaleAddress1 { get; set; }
        public virtual string CashSaleAddress1StartsWith { get; set; }
        public virtual string CashSaleAddress1EndsWith { get; set; }
        public virtual string CashSaleAddress1Contains { get; set; }
        public virtual string CashSaleAddress1Like { get; set; }
        public virtual string[] CashSaleAddress1Between { get; set; }
        public virtual string[] CashSaleAddress1In { get; set; }
        public virtual string CashSaleAddress2 { get; set; }
        public virtual string CashSaleAddress2StartsWith { get; set; }
        public virtual string CashSaleAddress2EndsWith { get; set; }
        public virtual string CashSaleAddress2Contains { get; set; }
        public virtual string CashSaleAddress2Like { get; set; }
        public virtual string[] CashSaleAddress2Between { get; set; }
        public virtual string[] CashSaleAddress2In { get; set; }
        public virtual string CashSaleAddress3 { get; set; }
        public virtual string CashSaleAddress3StartsWith { get; set; }
        public virtual string CashSaleAddress3EndsWith { get; set; }
        public virtual string CashSaleAddress3Contains { get; set; }
        public virtual string CashSaleAddress3Like { get; set; }
        public virtual string[] CashSaleAddress3Between { get; set; }
        public virtual string[] CashSaleAddress3In { get; set; }
        public virtual string CashSaleAddress4 { get; set; }
        public virtual string CashSaleAddress4StartsWith { get; set; }
        public virtual string CashSaleAddress4EndsWith { get; set; }
        public virtual string CashSaleAddress4Contains { get; set; }
        public virtual string CashSaleAddress4Like { get; set; }
        public virtual string[] CashSaleAddress4Between { get; set; }
        public virtual string[] CashSaleAddress4In { get; set; }
        public virtual string CashSalePostcode { get; set; }
        public virtual string CashSalePostcodeStartsWith { get; set; }
        public virtual string CashSalePostcodeEndsWith { get; set; }
        public virtual string CashSalePostcodeContains { get; set; }
        public virtual string CashSalePostcodeLike { get; set; }
        public virtual string[] CashSalePostcodeBetween { get; set; }
        public virtual string[] CashSalePostcodeIn { get; set; }
        public virtual string CashSaleCompany { get; set; }
        public virtual string CashSaleCompanyStartsWith { get; set; }
        public virtual string CashSaleCompanyEndsWith { get; set; }
        public virtual string CashSaleCompanyContains { get; set; }
        public virtual string CashSaleCompanyLike { get; set; }
        public virtual string[] CashSaleCompanyBetween { get; set; }
        public virtual string[] CashSaleCompanyIn { get; set; }
        public virtual string CashSaleName { get; set; }
        public virtual string CashSaleNameStartsWith { get; set; }
        public virtual string CashSaleNameEndsWith { get; set; }
        public virtual string CashSaleNameContains { get; set; }
        public virtual string CashSaleNameLike { get; set; }
        public virtual string[] CashSaleNameBetween { get; set; }
        public virtual string[] CashSaleNameIn { get; set; }
        public virtual string CashSalePhone { get; set; }
        public virtual string CashSalePhoneStartsWith { get; set; }
        public virtual string CashSalePhoneEndsWith { get; set; }
        public virtual string CashSalePhoneContains { get; set; }
        public virtual string CashSalePhoneLike { get; set; }
        public virtual string[] CashSalePhoneBetween { get; set; }
        public virtual string[] CashSalePhoneIn { get; set; }
        public virtual string InvoiceHistoryID { get; set; }
        public virtual string InvoiceHistoryIDStartsWith { get; set; }
        public virtual string InvoiceHistoryIDEndsWith { get; set; }
        public virtual string InvoiceHistoryIDContains { get; set; }
        public virtual string InvoiceHistoryIDLike { get; set; }
        public virtual string[] InvoiceHistoryIDBetween { get; set; }
        public virtual string[] InvoiceHistoryIDIn { get; set; }
        public virtual string DeliveryAddressContactName { get; set; }
        public virtual string DeliveryAddressContactNameStartsWith { get; set; }
        public virtual string DeliveryAddressContactNameEndsWith { get; set; }
        public virtual string DeliveryAddressContactNameContains { get; set; }
        public virtual string DeliveryAddressContactNameLike { get; set; }
        public virtual string[] DeliveryAddressContactNameBetween { get; set; }
        public virtual string[] DeliveryAddressContactNameIn { get; set; }
        public virtual string DeliveryAddressee { get; set; }
        public virtual string DeliveryAddresseeStartsWith { get; set; }
        public virtual string DeliveryAddresseeEndsWith { get; set; }
        public virtual string DeliveryAddresseeContains { get; set; }
        public virtual string DeliveryAddresseeLike { get; set; }
        public virtual string[] DeliveryAddresseeBetween { get; set; }
        public virtual string[] DeliveryAddresseeIn { get; set; }
        public virtual string DeliveryAddress1 { get; set; }
        public virtual string DeliveryAddress1StartsWith { get; set; }
        public virtual string DeliveryAddress1EndsWith { get; set; }
        public virtual string DeliveryAddress1Contains { get; set; }
        public virtual string DeliveryAddress1Like { get; set; }
        public virtual string[] DeliveryAddress1Between { get; set; }
        public virtual string[] DeliveryAddress1In { get; set; }
        public virtual string DeliveryAddress2 { get; set; }
        public virtual string DeliveryAddress2StartsWith { get; set; }
        public virtual string DeliveryAddress2EndsWith { get; set; }
        public virtual string DeliveryAddress2Contains { get; set; }
        public virtual string DeliveryAddress2Like { get; set; }
        public virtual string[] DeliveryAddress2Between { get; set; }
        public virtual string[] DeliveryAddress2In { get; set; }
        public virtual string DeliveryAddress3 { get; set; }
        public virtual string DeliveryAddress3StartsWith { get; set; }
        public virtual string DeliveryAddress3EndsWith { get; set; }
        public virtual string DeliveryAddress3Contains { get; set; }
        public virtual string DeliveryAddress3Like { get; set; }
        public virtual string[] DeliveryAddress3Between { get; set; }
        public virtual string[] DeliveryAddress3In { get; set; }
        public virtual string DeliveryAddress4 { get; set; }
        public virtual string DeliveryAddress4StartsWith { get; set; }
        public virtual string DeliveryAddress4EndsWith { get; set; }
        public virtual string DeliveryAddress4Contains { get; set; }
        public virtual string DeliveryAddress4Like { get; set; }
        public virtual string[] DeliveryAddress4Between { get; set; }
        public virtual string[] DeliveryAddress4In { get; set; }
        public virtual string DeliveryAddressPostcode { get; set; }
        public virtual string DeliveryAddressPostcodeStartsWith { get; set; }
        public virtual string DeliveryAddressPostcodeEndsWith { get; set; }
        public virtual string DeliveryAddressPostcodeContains { get; set; }
        public virtual string DeliveryAddressPostcodeLike { get; set; }
        public virtual string[] DeliveryAddressPostcodeBetween { get; set; }
        public virtual string[] DeliveryAddressPostcodeIn { get; set; }
        public virtual string DeliveryAddressCountry { get; set; }
        public virtual string DeliveryAddressCountryStartsWith { get; set; }
        public virtual string DeliveryAddressCountryEndsWith { get; set; }
        public virtual string DeliveryAddressCountryContains { get; set; }
        public virtual string DeliveryAddressCountryLike { get; set; }
        public virtual string[] DeliveryAddressCountryBetween { get; set; }
        public virtual string[] DeliveryAddressCountryIn { get; set; }
        public virtual bool? Delivered { get; set; }
        public virtual DateTime? DeliveredDate { get; set; }
        public virtual DateTime? DeliveredDateGreaterThanOrEqualTo { get; set; }
        public virtual DateTime? DeliveredDateGreaterThan { get; set; }
        public virtual DateTime? DeliveredDateLessThan { get; set; }
        public virtual DateTime? DeliveredDateLessThanOrEqualTo { get; set; }
        public virtual DateTime? DeliveredDateNotEqualTo { get; set; }
        public virtual DateTime?[] DeliveredDateBetween { get; set; }
        public virtual DateTime?[] DeliveredDateIn { get; set; }
        public virtual string ConsignmentNote { get; set; }
        public virtual string ConsignmentNoteStartsWith { get; set; }
        public virtual string ConsignmentNoteEndsWith { get; set; }
        public virtual string ConsignmentNoteContains { get; set; }
        public virtual string ConsignmentNoteLike { get; set; }
        public virtual string[] ConsignmentNoteBetween { get; set; }
        public virtual string[] ConsignmentNoteIn { get; set; }
        public virtual decimal? CartageCharge1 { get; set; }
        public virtual decimal? CartageCharge1GreaterThanOrEqualTo { get; set; }
        public virtual decimal? CartageCharge1GreaterThan { get; set; }
        public virtual decimal? CartageCharge1LessThan { get; set; }
        public virtual decimal? CartageCharge1LessThanOrEqualTo { get; set; }
        public virtual decimal? CartageCharge1NotEqualTo { get; set; }
        public virtual decimal?[] CartageCharge1Between { get; set; }
        public virtual decimal?[] CartageCharge1In { get; set; }
        public virtual decimal? Cartage1TaxAmount { get; set; }
        public virtual decimal? Cartage1TaxAmountGreaterThanOrEqualTo { get; set; }
        public virtual decimal? Cartage1TaxAmountGreaterThan { get; set; }
        public virtual decimal? Cartage1TaxAmountLessThan { get; set; }
        public virtual decimal? Cartage1TaxAmountLessThanOrEqualTo { get; set; }
        public virtual decimal? Cartage1TaxAmountNotEqualTo { get; set; }
        public virtual decimal?[] Cartage1TaxAmountBetween { get; set; }
        public virtual decimal?[] Cartage1TaxAmountIn { get; set; }
        public virtual decimal? CartageCharge2 { get; set; }
        public virtual decimal? CartageCharge2GreaterThanOrEqualTo { get; set; }
        public virtual decimal? CartageCharge2GreaterThan { get; set; }
        public virtual decimal? CartageCharge2LessThan { get; set; }
        public virtual decimal? CartageCharge2LessThanOrEqualTo { get; set; }
        public virtual decimal? CartageCharge2NotEqualTo { get; set; }
        public virtual decimal?[] CartageCharge2Between { get; set; }
        public virtual decimal?[] CartageCharge2In { get; set; }
        public virtual decimal? Cartage2TaxAmount { get; set; }
        public virtual decimal? Cartage2TaxAmountGreaterThanOrEqualTo { get; set; }
        public virtual decimal? Cartage2TaxAmountGreaterThan { get; set; }
        public virtual decimal? Cartage2TaxAmountLessThan { get; set; }
        public virtual decimal? Cartage2TaxAmountLessThanOrEqualTo { get; set; }
        public virtual decimal? Cartage2TaxAmountNotEqualTo { get; set; }
        public virtual decimal?[] Cartage2TaxAmountBetween { get; set; }
        public virtual decimal?[] Cartage2TaxAmountIn { get; set; }
        public virtual decimal? CartageCharge3 { get; set; }
        public virtual decimal? CartageCharge3GreaterThanOrEqualTo { get; set; }
        public virtual decimal? CartageCharge3GreaterThan { get; set; }
        public virtual decimal? CartageCharge3LessThan { get; set; }
        public virtual decimal? CartageCharge3LessThanOrEqualTo { get; set; }
        public virtual decimal? CartageCharge3NotEqualTo { get; set; }
        public virtual decimal?[] CartageCharge3Between { get; set; }
        public virtual decimal?[] CartageCharge3In { get; set; }
        public virtual decimal? Cartage3TaxAmount { get; set; }
        public virtual decimal? Cartage3TaxAmountGreaterThanOrEqualTo { get; set; }
        public virtual decimal? Cartage3TaxAmountGreaterThan { get; set; }
        public virtual decimal? Cartage3TaxAmountLessThan { get; set; }
        public virtual decimal? Cartage3TaxAmountLessThanOrEqualTo { get; set; }
        public virtual decimal? Cartage3TaxAmountNotEqualTo { get; set; }
        public virtual decimal?[] Cartage3TaxAmountBetween { get; set; }
        public virtual decimal?[] Cartage3TaxAmountIn { get; set; }
        public virtual decimal? FXCartageCharge1 { get; set; }
        public virtual decimal? FXCartageCharge1GreaterThanOrEqualTo { get; set; }
        public virtual decimal? FXCartageCharge1GreaterThan { get; set; }
        public virtual decimal? FXCartageCharge1LessThan { get; set; }
        public virtual decimal? FXCartageCharge1LessThanOrEqualTo { get; set; }
        public virtual decimal? FXCartageCharge1NotEqualTo { get; set; }
        public virtual decimal?[] FXCartageCharge1Between { get; set; }
        public virtual decimal?[] FXCartageCharge1In { get; set; }
        public virtual decimal? FXCartage1TaxAmount { get; set; }
        public virtual decimal? FXCartage1TaxAmountGreaterThanOrEqualTo { get; set; }
        public virtual decimal? FXCartage1TaxAmountGreaterThan { get; set; }
        public virtual decimal? FXCartage1TaxAmountLessThan { get; set; }
        public virtual decimal? FXCartage1TaxAmountLessThanOrEqualTo { get; set; }
        public virtual decimal? FXCartage1TaxAmountNotEqualTo { get; set; }
        public virtual decimal?[] FXCartage1TaxAmountBetween { get; set; }
        public virtual decimal?[] FXCartage1TaxAmountIn { get; set; }
        public virtual decimal? FXCartageCharge2 { get; set; }
        public virtual decimal? FXCartageCharge2GreaterThanOrEqualTo { get; set; }
        public virtual decimal? FXCartageCharge2GreaterThan { get; set; }
        public virtual decimal? FXCartageCharge2LessThan { get; set; }
        public virtual decimal? FXCartageCharge2LessThanOrEqualTo { get; set; }
        public virtual decimal? FXCartageCharge2NotEqualTo { get; set; }
        public virtual decimal?[] FXCartageCharge2Between { get; set; }
        public virtual decimal?[] FXCartageCharge2In { get; set; }
        public virtual decimal? FXCartage2TaxAmount { get; set; }
        public virtual decimal? FXCartage2TaxAmountGreaterThanOrEqualTo { get; set; }
        public virtual decimal? FXCartage2TaxAmountGreaterThan { get; set; }
        public virtual decimal? FXCartage2TaxAmountLessThan { get; set; }
        public virtual decimal? FXCartage2TaxAmountLessThanOrEqualTo { get; set; }
        public virtual decimal? FXCartage2TaxAmountNotEqualTo { get; set; }
        public virtual decimal?[] FXCartage2TaxAmountBetween { get; set; }
        public virtual decimal?[] FXCartage2TaxAmountIn { get; set; }
        public virtual decimal? FXCartageCharge3 { get; set; }
        public virtual decimal? FXCartageCharge3GreaterThanOrEqualTo { get; set; }
        public virtual decimal? FXCartageCharge3GreaterThan { get; set; }
        public virtual decimal? FXCartageCharge3LessThan { get; set; }
        public virtual decimal? FXCartageCharge3LessThanOrEqualTo { get; set; }
        public virtual decimal? FXCartageCharge3NotEqualTo { get; set; }
        public virtual decimal?[] FXCartageCharge3Between { get; set; }
        public virtual decimal?[] FXCartageCharge3In { get; set; }
        public virtual decimal? FXCartage3TaxAmount { get; set; }
        public virtual decimal? FXCartage3TaxAmountGreaterThanOrEqualTo { get; set; }
        public virtual decimal? FXCartage3TaxAmountGreaterThan { get; set; }
        public virtual decimal? FXCartage3TaxAmountLessThan { get; set; }
        public virtual decimal? FXCartage3TaxAmountLessThanOrEqualTo { get; set; }
        public virtual decimal? FXCartage3TaxAmountNotEqualTo { get; set; }
        public virtual decimal?[] FXCartage3TaxAmountBetween { get; set; }
        public virtual decimal?[] FXCartage3TaxAmountIn { get; set; }
        public virtual string CourierDetails { get; set; }
        public virtual string CourierDetailsStartsWith { get; set; }
        public virtual string CourierDetailsEndsWith { get; set; }
        public virtual string CourierDetailsContains { get; set; }
        public virtual string CourierDetailsLike { get; set; }
        public virtual string[] CourierDetailsBetween { get; set; }
        public virtual string[] CourierDetailsIn { get; set; }
        public virtual string Notes { get; set; }
        public virtual string NotesStartsWith { get; set; }
        public virtual string NotesEndsWith { get; set; }
        public virtual string NotesContains { get; set; }
        public virtual string NotesLike { get; set; }
        public virtual string[] NotesBetween { get; set; }
        public virtual string[] NotesIn { get; set; }
        public virtual string EmailAddress { get; set; }
        public virtual string EmailAddressStartsWith { get; set; }
        public virtual string EmailAddressEndsWith { get; set; }
        public virtual string EmailAddressContains { get; set; }
        public virtual string EmailAddressLike { get; set; }
        public virtual string[] EmailAddressBetween { get; set; }
        public virtual string[] EmailAddressIn { get; set; }
        public virtual string StaffID { get; set; }
        public virtual string StaffIDStartsWith { get; set; }
        public virtual string StaffIDEndsWith { get; set; }
        public virtual string StaffIDContains { get; set; }
        public virtual string StaffIDLike { get; set; }
        public virtual string[] StaffIDBetween { get; set; }
        public virtual string[] StaffIDIn { get; set; }
        public virtual string StaffTitle { get; set; }
        public virtual string StaffTitleStartsWith { get; set; }
        public virtual string StaffTitleEndsWith { get; set; }
        public virtual string StaffTitleContains { get; set; }
        public virtual string StaffTitleLike { get; set; }
        public virtual string[] StaffTitleBetween { get; set; }
        public virtual string[] StaffTitleIn { get; set; }
        public virtual string StaffFirstName { get; set; }
        public virtual string StaffFirstNameStartsWith { get; set; }
        public virtual string StaffFirstNameEndsWith { get; set; }
        public virtual string StaffFirstNameContains { get; set; }
        public virtual string StaffFirstNameLike { get; set; }
        public virtual string[] StaffFirstNameBetween { get; set; }
        public virtual string[] StaffFirstNameIn { get; set; }
        public virtual string StaffSurname { get; set; }
        public virtual string StaffSurnameStartsWith { get; set; }
        public virtual string StaffSurnameEndsWith { get; set; }
        public virtual string StaffSurnameContains { get; set; }
        public virtual string StaffSurnameLike { get; set; }
        public virtual string[] StaffSurnameBetween { get; set; }
        public virtual string[] StaffSurnameIn { get; set; }
        public virtual string StaffUsername { get; set; }
        public virtual string StaffUsernameStartsWith { get; set; }
        public virtual string StaffUsernameEndsWith { get; set; }
        public virtual string StaffUsernameContains { get; set; }
        public virtual string StaffUsernameLike { get; set; }
        public virtual string[] StaffUsernameBetween { get; set; }
        public virtual string[] StaffUsernameIn { get; set; }
        public virtual byte? HistoryStatus { get; set; }
        public virtual byte? HistoryStatusGreaterThanOrEqualTo { get; set; }
        public virtual byte? HistoryStatusGreaterThan { get; set; }
        public virtual byte? HistoryStatusLessThan { get; set; }
        public virtual byte? HistoryStatusLessThanOrEqualTo { get; set; }
        public virtual byte? HistoryStatusNotEqualTo { get; set; }
        public virtual byte?[] HistoryStatusBetween { get; set; }
        public virtual byte?[] HistoryStatusIn { get; set; }
        public virtual short? HistoryNo { get; set; }
        public virtual short? HistoryNoGreaterThanOrEqualTo { get; set; }
        public virtual short? HistoryNoGreaterThan { get; set; }
        public virtual short? HistoryNoLessThan { get; set; }
        public virtual short? HistoryNoLessThanOrEqualTo { get; set; }
        public virtual short? HistoryNoNotEqualTo { get; set; }
        public virtual short?[] HistoryNoBetween { get; set; }
        public virtual short?[] HistoryNoIn { get; set; }
        public virtual string CurrencyID { get; set; }
        public virtual string CurrencyIDStartsWith { get; set; }
        public virtual string CurrencyIDEndsWith { get; set; }
        public virtual string CurrencyIDContains { get; set; }
        public virtual string CurrencyIDLike { get; set; }
        public virtual string[] CurrencyIDBetween { get; set; }
        public virtual string[] CurrencyIDIn { get; set; }
        public virtual string CurrencyShortName { get; set; }
        public virtual string CurrencyShortNameStartsWith { get; set; }
        public virtual string CurrencyShortNameEndsWith { get; set; }
        public virtual string CurrencyShortNameContains { get; set; }
        public virtual string CurrencyShortNameLike { get; set; }
        public virtual string[] CurrencyShortNameBetween { get; set; }
        public virtual string[] CurrencyShortNameIn { get; set; }
        public virtual string CurrencyName { get; set; }
        public virtual string CurrencyNameStartsWith { get; set; }
        public virtual string CurrencyNameEndsWith { get; set; }
        public virtual string CurrencyNameContains { get; set; }
        public virtual string CurrencyNameLike { get; set; }
        public virtual string[] CurrencyNameBetween { get; set; }
        public virtual string[] CurrencyNameIn { get; set; }
        public virtual short? DecimalPlaces { get; set; }
        public virtual short? DecimalPlacesGreaterThanOrEqualTo { get; set; }
        public virtual short? DecimalPlacesGreaterThan { get; set; }
        public virtual short? DecimalPlacesLessThan { get; set; }
        public virtual short? DecimalPlacesLessThanOrEqualTo { get; set; }
        public virtual short? DecimalPlacesNotEqualTo { get; set; }
        public virtual short?[] DecimalPlacesBetween { get; set; }
        public virtual short?[] DecimalPlacesIn { get; set; }
        public virtual decimal? TotalAllocated { get; set; }
        public virtual decimal? TotalAllocatedGreaterThanOrEqualTo { get; set; }
        public virtual decimal? TotalAllocatedGreaterThan { get; set; }
        public virtual decimal? TotalAllocatedLessThan { get; set; }
        public virtual decimal? TotalAllocatedLessThanOrEqualTo { get; set; }
        public virtual decimal? TotalAllocatedNotEqualTo { get; set; }
        public virtual decimal?[] TotalAllocatedBetween { get; set; }
        public virtual decimal?[] TotalAllocatedIn { get; set; }
        public virtual DateTime? DueDate { get; set; }
        public virtual DateTime? DueDateGreaterThanOrEqualTo { get; set; }
        public virtual DateTime? DueDateGreaterThan { get; set; }
        public virtual DateTime? DueDateLessThan { get; set; }
        public virtual DateTime? DueDateLessThanOrEqualTo { get; set; }
        public virtual DateTime? DueDateNotEqualTo { get; set; }
        public virtual DateTime?[] DueDateBetween { get; set; }
        public virtual DateTime?[] DueDateIn { get; set; }
    }
    #endregion

    #region "Sales Quotes"
    public partial class v_Jiwa_SalesQuote_List
    {
        [Required]
        public virtual string InvoiceID { get; set; }

        [Required]
        public virtual string InvoiceNo { get; set; }

        public virtual string InvoiceNoDashHistoryNo { get; set; }
        public virtual string OrderNo { get; set; }
        public virtual string QOReference { get; set; }
        [Required]
        public virtual DateTime InvoiceInitDate { get; set; }

        public virtual short? Status { get; set; }
        [Required]
        public virtual DateTimeOffset LastSavedDateTime { get; set; }

        public virtual decimal? LocalInvoiceTotalIncTax { get; set; }
        public virtual decimal? FXInvoiceTotalIncTax { get; set; }
        [Required]
        public virtual string DebtorID { get; set; }

        [Required]
        public virtual string AccountNo { get; set; }

        public virtual string DebtorName { get; set; }
        [Required]
        public virtual string IN_LogicalID { get; set; }

        public virtual string LogicalWarehouseDescription { get; set; }
        [Required]
        public virtual string IN_PhysicalID { get; set; }

        [Required]
        public virtual string PhysicalWarehouseDescription { get; set; }

        [Required]
        public virtual string BranchID { get; set; }

        [Required]
        public virtual string BranchDescription { get; set; }

        public virtual string CashSaleAddress1 { get; set; }
        public virtual string CashSaleAddress2 { get; set; }
        public virtual string CashSaleAddress3 { get; set; }
        public virtual string CashSaleAddress4 { get; set; }
        public virtual string CashSalePostcode { get; set; }
        public virtual string CashSaleCompany { get; set; }
        public virtual string CashSaleName { get; set; }
        public virtual string CashSalePhone { get; set; }
        [Required]
        public virtual string InvoiceHistoryID { get; set; }

        public virtual string DeliveryAddressContactName { get; set; }
        public virtual string DeliveryAddressee { get; set; }
        public virtual string DeliveryAddress1 { get; set; }
        public virtual string DeliveryAddress2 { get; set; }
        public virtual string DeliveryAddress3 { get; set; }
        public virtual string DeliveryAddress4 { get; set; }
        public virtual string DeliveryAddressPostcode { get; set; }
        public virtual string DeliveryAddressCountry { get; set; }
        public virtual decimal? CartageCharge1 { get; set; }
        public virtual decimal? Cartage1TaxAmount { get; set; }
        public virtual decimal? CartageCharge2 { get; set; }
        public virtual decimal? Cartage2TaxAmount { get; set; }
        public virtual decimal? CartageCharge3 { get; set; }
        public virtual decimal? Cartage3TaxAmount { get; set; }
        public virtual decimal? FXCartageCharge1 { get; set; }
        public virtual decimal? FXCartage1TaxAmount { get; set; }
        public virtual decimal? FXCartageCharge2 { get; set; }
        public virtual decimal? FXCartage2TaxAmount { get; set; }
        public virtual decimal? FXCartageCharge3 { get; set; }
        public virtual decimal? FXCartage3TaxAmount { get; set; }
        public virtual string CourierDetails { get; set; }
        public virtual string Notes { get; set; }
        public virtual string EmailAddress { get; set; }
        [Required]
        public virtual string StaffID { get; set; }

        public virtual string StaffTitle { get; set; }
        public virtual string StaffFirstName { get; set; }
        public virtual string StaffSurname { get; set; }
        [Required]
        public virtual string StaffUsername { get; set; }

        public virtual short? HistoryNo { get; set; }
        [Required]
        public virtual string CurrencyID { get; set; }

        public virtual string CurrencyShortName { get; set; }
        public virtual string CurrencyName { get; set; }
        public virtual short? DecimalPlaces { get; set; }
    }

    [Route("/Queries/SalesQuoteList", "GET")]
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    public partial class v_Jiwa_SalesQuote_ListQuery
        : QueryDb<v_Jiwa_SalesQuote_List>, IReturn<QueryResponse<v_Jiwa_SalesQuote_List>>
    {
        public virtual string InvoiceID { get; set; }
        public virtual string InvoiceIDStartsWith { get; set; }
        public virtual string InvoiceIDEndsWith { get; set; }
        public virtual string InvoiceIDContains { get; set; }
        public virtual string InvoiceIDLike { get; set; }
        public virtual string[] InvoiceIDBetween { get; set; }
        public virtual string[] InvoiceIDIn { get; set; }
        public virtual string InvoiceNo { get; set; }
        public virtual string InvoiceNoStartsWith { get; set; }
        public virtual string InvoiceNoEndsWith { get; set; }
        public virtual string InvoiceNoContains { get; set; }
        public virtual string InvoiceNoLike { get; set; }
        public virtual string[] InvoiceNoBetween { get; set; }
        public virtual string[] InvoiceNoIn { get; set; }
        public virtual string InvoiceNoDashHistoryNo { get; set; }
        public virtual string InvoiceNoDashHistoryNoStartsWith { get; set; }
        public virtual string InvoiceNoDashHistoryNoEndsWith { get; set; }
        public virtual string InvoiceNoDashHistoryNoContains { get; set; }
        public virtual string InvoiceNoDashHistoryNoLike { get; set; }
        public virtual string[] InvoiceNoDashHistoryNoBetween { get; set; }
        public virtual string[] InvoiceNoDashHistoryNoIn { get; set; }
        public virtual string OrderNo { get; set; }
        public virtual string OrderNoStartsWith { get; set; }
        public virtual string OrderNoEndsWith { get; set; }
        public virtual string OrderNoContains { get; set; }
        public virtual string OrderNoLike { get; set; }
        public virtual string[] OrderNoBetween { get; set; }
        public virtual string[] OrderNoIn { get; set; }
        public virtual string QOReference { get; set; }
        public virtual string QOReferenceStartsWith { get; set; }
        public virtual string QOReferenceEndsWith { get; set; }
        public virtual string QOReferenceContains { get; set; }
        public virtual string QOReferenceLike { get; set; }
        public virtual string[] QOReferenceBetween { get; set; }
        public virtual string[] QOReferenceIn { get; set; }
        public virtual DateTime? InvoiceInitDate { get; set; }
        public virtual DateTime? InvoiceInitDateGreaterThanOrEqualTo { get; set; }
        public virtual DateTime? InvoiceInitDateGreaterThan { get; set; }
        public virtual DateTime? InvoiceInitDateLessThan { get; set; }
        public virtual DateTime? InvoiceInitDateLessThanOrEqualTo { get; set; }
        public virtual DateTime? InvoiceInitDateNotEqualTo { get; set; }
        public virtual DateTime[] InvoiceInitDateBetween { get; set; }
        public virtual DateTime[] InvoiceInitDateIn { get; set; }
        public virtual short? Status { get; set; }
        public virtual short? StatusGreaterThanOrEqualTo { get; set; }
        public virtual short? StatusGreaterThan { get; set; }
        public virtual short? StatusLessThan { get; set; }
        public virtual short? StatusLessThanOrEqualTo { get; set; }
        public virtual short? StatusNotEqualTo { get; set; }
        public virtual short?[] StatusBetween { get; set; }
        public virtual short?[] StatusIn { get; set; }
        public virtual DateTimeOffset? LastSavedDateTime { get; set; }
        public virtual DateTimeOffset? LastSavedDateTimeGreaterThanOrEqualTo { get; set; }
        public virtual DateTimeOffset? LastSavedDateTimeGreaterThan { get; set; }
        public virtual DateTimeOffset? LastSavedDateTimeLessThan { get; set; }
        public virtual DateTimeOffset? LastSavedDateTimeLessThanOrEqualTo { get; set; }
        public virtual DateTimeOffset? LastSavedDateTimeNotEqualTo { get; set; }
        public virtual DateTimeOffset[] LastSavedDateTimeBetween { get; set; }
        public virtual DateTimeOffset[] LastSavedDateTimeIn { get; set; }
        public virtual decimal? LocalInvoiceTotalIncTax { get; set; }
        public virtual decimal? LocalInvoiceTotalIncTaxGreaterThanOrEqualTo { get; set; }
        public virtual decimal? LocalInvoiceTotalIncTaxGreaterThan { get; set; }
        public virtual decimal? LocalInvoiceTotalIncTaxLessThan { get; set; }
        public virtual decimal? LocalInvoiceTotalIncTaxLessThanOrEqualTo { get; set; }
        public virtual decimal? LocalInvoiceTotalIncTaxNotEqualTo { get; set; }
        public virtual decimal?[] LocalInvoiceTotalIncTaxBetween { get; set; }
        public virtual decimal?[] LocalInvoiceTotalIncTaxIn { get; set; }
        public virtual decimal? FXInvoiceTotalIncTax { get; set; }
        public virtual decimal? FXInvoiceTotalIncTaxGreaterThanOrEqualTo { get; set; }
        public virtual decimal? FXInvoiceTotalIncTaxGreaterThan { get; set; }
        public virtual decimal? FXInvoiceTotalIncTaxLessThan { get; set; }
        public virtual decimal? FXInvoiceTotalIncTaxLessThanOrEqualTo { get; set; }
        public virtual decimal? FXInvoiceTotalIncTaxNotEqualTo { get; set; }
        public virtual decimal?[] FXInvoiceTotalIncTaxBetween { get; set; }
        public virtual decimal?[] FXInvoiceTotalIncTaxIn { get; set; }
        public virtual string DebtorID { get; set; }
        public virtual string DebtorIDStartsWith { get; set; }
        public virtual string DebtorIDEndsWith { get; set; }
        public virtual string DebtorIDContains { get; set; }
        public virtual string DebtorIDLike { get; set; }
        public virtual string[] DebtorIDBetween { get; set; }
        public virtual string[] DebtorIDIn { get; set; }
        public virtual string AccountNo { get; set; }
        public virtual string AccountNoStartsWith { get; set; }
        public virtual string AccountNoEndsWith { get; set; }
        public virtual string AccountNoContains { get; set; }
        public virtual string AccountNoLike { get; set; }
        public virtual string[] AccountNoBetween { get; set; }
        public virtual string[] AccountNoIn { get; set; }
        public virtual string DebtorName { get; set; }
        public virtual string DebtorNameStartsWith { get; set; }
        public virtual string DebtorNameEndsWith { get; set; }
        public virtual string DebtorNameContains { get; set; }
        public virtual string DebtorNameLike { get; set; }
        public virtual string[] DebtorNameBetween { get; set; }
        public virtual string[] DebtorNameIn { get; set; }
        public virtual string IN_LogicalID { get; set; }
        public virtual string IN_LogicalIDStartsWith { get; set; }
        public virtual string IN_LogicalIDEndsWith { get; set; }
        public virtual string IN_LogicalIDContains { get; set; }
        public virtual string IN_LogicalIDLike { get; set; }
        public virtual string[] IN_LogicalIDBetween { get; set; }
        public virtual string[] IN_LogicalIDIn { get; set; }
        public virtual string LogicalWarehouseDescription { get; set; }
        public virtual string LogicalWarehouseDescriptionStartsWith { get; set; }
        public virtual string LogicalWarehouseDescriptionEndsWith { get; set; }
        public virtual string LogicalWarehouseDescriptionContains { get; set; }
        public virtual string LogicalWarehouseDescriptionLike { get; set; }
        public virtual string[] LogicalWarehouseDescriptionBetween { get; set; }
        public virtual string[] LogicalWarehouseDescriptionIn { get; set; }
        public virtual string IN_PhysicalID { get; set; }
        public virtual string IN_PhysicalIDStartsWith { get; set; }
        public virtual string IN_PhysicalIDEndsWith { get; set; }
        public virtual string IN_PhysicalIDContains { get; set; }
        public virtual string IN_PhysicalIDLike { get; set; }
        public virtual string[] IN_PhysicalIDBetween { get; set; }
        public virtual string[] IN_PhysicalIDIn { get; set; }
        public virtual string PhysicalWarehouseDescription { get; set; }
        public virtual string PhysicalWarehouseDescriptionStartsWith { get; set; }
        public virtual string PhysicalWarehouseDescriptionEndsWith { get; set; }
        public virtual string PhysicalWarehouseDescriptionContains { get; set; }
        public virtual string PhysicalWarehouseDescriptionLike { get; set; }
        public virtual string[] PhysicalWarehouseDescriptionBetween { get; set; }
        public virtual string[] PhysicalWarehouseDescriptionIn { get; set; }
        public virtual string BranchID { get; set; }
        public virtual string BranchIDStartsWith { get; set; }
        public virtual string BranchIDEndsWith { get; set; }
        public virtual string BranchIDContains { get; set; }
        public virtual string BranchIDLike { get; set; }
        public virtual string[] BranchIDBetween { get; set; }
        public virtual string[] BranchIDIn { get; set; }
        public virtual string BranchDescription { get; set; }
        public virtual string BranchDescriptionStartsWith { get; set; }
        public virtual string BranchDescriptionEndsWith { get; set; }
        public virtual string BranchDescriptionContains { get; set; }
        public virtual string BranchDescriptionLike { get; set; }
        public virtual string[] BranchDescriptionBetween { get; set; }
        public virtual string[] BranchDescriptionIn { get; set; }
        public virtual string CashSaleAddress1 { get; set; }
        public virtual string CashSaleAddress1StartsWith { get; set; }
        public virtual string CashSaleAddress1EndsWith { get; set; }
        public virtual string CashSaleAddress1Contains { get; set; }
        public virtual string CashSaleAddress1Like { get; set; }
        public virtual string[] CashSaleAddress1Between { get; set; }
        public virtual string[] CashSaleAddress1In { get; set; }
        public virtual string CashSaleAddress2 { get; set; }
        public virtual string CashSaleAddress2StartsWith { get; set; }
        public virtual string CashSaleAddress2EndsWith { get; set; }
        public virtual string CashSaleAddress2Contains { get; set; }
        public virtual string CashSaleAddress2Like { get; set; }
        public virtual string[] CashSaleAddress2Between { get; set; }
        public virtual string[] CashSaleAddress2In { get; set; }
        public virtual string CashSaleAddress3 { get; set; }
        public virtual string CashSaleAddress3StartsWith { get; set; }
        public virtual string CashSaleAddress3EndsWith { get; set; }
        public virtual string CashSaleAddress3Contains { get; set; }
        public virtual string CashSaleAddress3Like { get; set; }
        public virtual string[] CashSaleAddress3Between { get; set; }
        public virtual string[] CashSaleAddress3In { get; set; }
        public virtual string CashSaleAddress4 { get; set; }
        public virtual string CashSaleAddress4StartsWith { get; set; }
        public virtual string CashSaleAddress4EndsWith { get; set; }
        public virtual string CashSaleAddress4Contains { get; set; }
        public virtual string CashSaleAddress4Like { get; set; }
        public virtual string[] CashSaleAddress4Between { get; set; }
        public virtual string[] CashSaleAddress4In { get; set; }
        public virtual string CashSalePostcode { get; set; }
        public virtual string CashSalePostcodeStartsWith { get; set; }
        public virtual string CashSalePostcodeEndsWith { get; set; }
        public virtual string CashSalePostcodeContains { get; set; }
        public virtual string CashSalePostcodeLike { get; set; }
        public virtual string[] CashSalePostcodeBetween { get; set; }
        public virtual string[] CashSalePostcodeIn { get; set; }
        public virtual string CashSaleCompany { get; set; }
        public virtual string CashSaleCompanyStartsWith { get; set; }
        public virtual string CashSaleCompanyEndsWith { get; set; }
        public virtual string CashSaleCompanyContains { get; set; }
        public virtual string CashSaleCompanyLike { get; set; }
        public virtual string[] CashSaleCompanyBetween { get; set; }
        public virtual string[] CashSaleCompanyIn { get; set; }
        public virtual string CashSaleName { get; set; }
        public virtual string CashSaleNameStartsWith { get; set; }
        public virtual string CashSaleNameEndsWith { get; set; }
        public virtual string CashSaleNameContains { get; set; }
        public virtual string CashSaleNameLike { get; set; }
        public virtual string[] CashSaleNameBetween { get; set; }
        public virtual string[] CashSaleNameIn { get; set; }
        public virtual string CashSalePhone { get; set; }
        public virtual string CashSalePhoneStartsWith { get; set; }
        public virtual string CashSalePhoneEndsWith { get; set; }
        public virtual string CashSalePhoneContains { get; set; }
        public virtual string CashSalePhoneLike { get; set; }
        public virtual string[] CashSalePhoneBetween { get; set; }
        public virtual string[] CashSalePhoneIn { get; set; }
        public virtual string InvoiceHistoryID { get; set; }
        public virtual string InvoiceHistoryIDStartsWith { get; set; }
        public virtual string InvoiceHistoryIDEndsWith { get; set; }
        public virtual string InvoiceHistoryIDContains { get; set; }
        public virtual string InvoiceHistoryIDLike { get; set; }
        public virtual string[] InvoiceHistoryIDBetween { get; set; }
        public virtual string[] InvoiceHistoryIDIn { get; set; }
        public virtual string DeliveryAddressContactName { get; set; }
        public virtual string DeliveryAddressContactNameStartsWith { get; set; }
        public virtual string DeliveryAddressContactNameEndsWith { get; set; }
        public virtual string DeliveryAddressContactNameContains { get; set; }
        public virtual string DeliveryAddressContactNameLike { get; set; }
        public virtual string[] DeliveryAddressContactNameBetween { get; set; }
        public virtual string[] DeliveryAddressContactNameIn { get; set; }
        public virtual string DeliveryAddressee { get; set; }
        public virtual string DeliveryAddresseeStartsWith { get; set; }
        public virtual string DeliveryAddresseeEndsWith { get; set; }
        public virtual string DeliveryAddresseeContains { get; set; }
        public virtual string DeliveryAddresseeLike { get; set; }
        public virtual string[] DeliveryAddresseeBetween { get; set; }
        public virtual string[] DeliveryAddresseeIn { get; set; }
        public virtual string DeliveryAddress1 { get; set; }
        public virtual string DeliveryAddress1StartsWith { get; set; }
        public virtual string DeliveryAddress1EndsWith { get; set; }
        public virtual string DeliveryAddress1Contains { get; set; }
        public virtual string DeliveryAddress1Like { get; set; }
        public virtual string[] DeliveryAddress1Between { get; set; }
        public virtual string[] DeliveryAddress1In { get; set; }
        public virtual string DeliveryAddress2 { get; set; }
        public virtual string DeliveryAddress2StartsWith { get; set; }
        public virtual string DeliveryAddress2EndsWith { get; set; }
        public virtual string DeliveryAddress2Contains { get; set; }
        public virtual string DeliveryAddress2Like { get; set; }
        public virtual string[] DeliveryAddress2Between { get; set; }
        public virtual string[] DeliveryAddress2In { get; set; }
        public virtual string DeliveryAddress3 { get; set; }
        public virtual string DeliveryAddress3StartsWith { get; set; }
        public virtual string DeliveryAddress3EndsWith { get; set; }
        public virtual string DeliveryAddress3Contains { get; set; }
        public virtual string DeliveryAddress3Like { get; set; }
        public virtual string[] DeliveryAddress3Between { get; set; }
        public virtual string[] DeliveryAddress3In { get; set; }
        public virtual string DeliveryAddress4 { get; set; }
        public virtual string DeliveryAddress4StartsWith { get; set; }
        public virtual string DeliveryAddress4EndsWith { get; set; }
        public virtual string DeliveryAddress4Contains { get; set; }
        public virtual string DeliveryAddress4Like { get; set; }
        public virtual string[] DeliveryAddress4Between { get; set; }
        public virtual string[] DeliveryAddress4In { get; set; }
        public virtual string DeliveryAddressPostcode { get; set; }
        public virtual string DeliveryAddressPostcodeStartsWith { get; set; }
        public virtual string DeliveryAddressPostcodeEndsWith { get; set; }
        public virtual string DeliveryAddressPostcodeContains { get; set; }
        public virtual string DeliveryAddressPostcodeLike { get; set; }
        public virtual string[] DeliveryAddressPostcodeBetween { get; set; }
        public virtual string[] DeliveryAddressPostcodeIn { get; set; }
        public virtual string DeliveryAddressCountry { get; set; }
        public virtual string DeliveryAddressCountryStartsWith { get; set; }
        public virtual string DeliveryAddressCountryEndsWith { get; set; }
        public virtual string DeliveryAddressCountryContains { get; set; }
        public virtual string DeliveryAddressCountryLike { get; set; }
        public virtual string[] DeliveryAddressCountryBetween { get; set; }
        public virtual string[] DeliveryAddressCountryIn { get; set; }
        public virtual decimal? CartageCharge1 { get; set; }
        public virtual decimal? CartageCharge1GreaterThanOrEqualTo { get; set; }
        public virtual decimal? CartageCharge1GreaterThan { get; set; }
        public virtual decimal? CartageCharge1LessThan { get; set; }
        public virtual decimal? CartageCharge1LessThanOrEqualTo { get; set; }
        public virtual decimal? CartageCharge1NotEqualTo { get; set; }
        public virtual decimal?[] CartageCharge1Between { get; set; }
        public virtual decimal?[] CartageCharge1In { get; set; }
        public virtual decimal? Cartage1TaxAmount { get; set; }
        public virtual decimal? Cartage1TaxAmountGreaterThanOrEqualTo { get; set; }
        public virtual decimal? Cartage1TaxAmountGreaterThan { get; set; }
        public virtual decimal? Cartage1TaxAmountLessThan { get; set; }
        public virtual decimal? Cartage1TaxAmountLessThanOrEqualTo { get; set; }
        public virtual decimal? Cartage1TaxAmountNotEqualTo { get; set; }
        public virtual decimal?[] Cartage1TaxAmountBetween { get; set; }
        public virtual decimal?[] Cartage1TaxAmountIn { get; set; }
        public virtual decimal? CartageCharge2 { get; set; }
        public virtual decimal? CartageCharge2GreaterThanOrEqualTo { get; set; }
        public virtual decimal? CartageCharge2GreaterThan { get; set; }
        public virtual decimal? CartageCharge2LessThan { get; set; }
        public virtual decimal? CartageCharge2LessThanOrEqualTo { get; set; }
        public virtual decimal? CartageCharge2NotEqualTo { get; set; }
        public virtual decimal?[] CartageCharge2Between { get; set; }
        public virtual decimal?[] CartageCharge2In { get; set; }
        public virtual decimal? Cartage2TaxAmount { get; set; }
        public virtual decimal? Cartage2TaxAmountGreaterThanOrEqualTo { get; set; }
        public virtual decimal? Cartage2TaxAmountGreaterThan { get; set; }
        public virtual decimal? Cartage2TaxAmountLessThan { get; set; }
        public virtual decimal? Cartage2TaxAmountLessThanOrEqualTo { get; set; }
        public virtual decimal? Cartage2TaxAmountNotEqualTo { get; set; }
        public virtual decimal?[] Cartage2TaxAmountBetween { get; set; }
        public virtual decimal?[] Cartage2TaxAmountIn { get; set; }
        public virtual decimal? CartageCharge3 { get; set; }
        public virtual decimal? CartageCharge3GreaterThanOrEqualTo { get; set; }
        public virtual decimal? CartageCharge3GreaterThan { get; set; }
        public virtual decimal? CartageCharge3LessThan { get; set; }
        public virtual decimal? CartageCharge3LessThanOrEqualTo { get; set; }
        public virtual decimal? CartageCharge3NotEqualTo { get; set; }
        public virtual decimal?[] CartageCharge3Between { get; set; }
        public virtual decimal?[] CartageCharge3In { get; set; }
        public virtual decimal? Cartage3TaxAmount { get; set; }
        public virtual decimal? Cartage3TaxAmountGreaterThanOrEqualTo { get; set; }
        public virtual decimal? Cartage3TaxAmountGreaterThan { get; set; }
        public virtual decimal? Cartage3TaxAmountLessThan { get; set; }
        public virtual decimal? Cartage3TaxAmountLessThanOrEqualTo { get; set; }
        public virtual decimal? Cartage3TaxAmountNotEqualTo { get; set; }
        public virtual decimal?[] Cartage3TaxAmountBetween { get; set; }
        public virtual decimal?[] Cartage3TaxAmountIn { get; set; }
        public virtual decimal? FXCartageCharge1 { get; set; }
        public virtual decimal? FXCartageCharge1GreaterThanOrEqualTo { get; set; }
        public virtual decimal? FXCartageCharge1GreaterThan { get; set; }
        public virtual decimal? FXCartageCharge1LessThan { get; set; }
        public virtual decimal? FXCartageCharge1LessThanOrEqualTo { get; set; }
        public virtual decimal? FXCartageCharge1NotEqualTo { get; set; }
        public virtual decimal?[] FXCartageCharge1Between { get; set; }
        public virtual decimal?[] FXCartageCharge1In { get; set; }
        public virtual decimal? FXCartage1TaxAmount { get; set; }
        public virtual decimal? FXCartage1TaxAmountGreaterThanOrEqualTo { get; set; }
        public virtual decimal? FXCartage1TaxAmountGreaterThan { get; set; }
        public virtual decimal? FXCartage1TaxAmountLessThan { get; set; }
        public virtual decimal? FXCartage1TaxAmountLessThanOrEqualTo { get; set; }
        public virtual decimal? FXCartage1TaxAmountNotEqualTo { get; set; }
        public virtual decimal?[] FXCartage1TaxAmountBetween { get; set; }
        public virtual decimal?[] FXCartage1TaxAmountIn { get; set; }
        public virtual decimal? FXCartageCharge2 { get; set; }
        public virtual decimal? FXCartageCharge2GreaterThanOrEqualTo { get; set; }
        public virtual decimal? FXCartageCharge2GreaterThan { get; set; }
        public virtual decimal? FXCartageCharge2LessThan { get; set; }
        public virtual decimal? FXCartageCharge2LessThanOrEqualTo { get; set; }
        public virtual decimal? FXCartageCharge2NotEqualTo { get; set; }
        public virtual decimal?[] FXCartageCharge2Between { get; set; }
        public virtual decimal?[] FXCartageCharge2In { get; set; }
        public virtual decimal? FXCartage2TaxAmount { get; set; }
        public virtual decimal? FXCartage2TaxAmountGreaterThanOrEqualTo { get; set; }
        public virtual decimal? FXCartage2TaxAmountGreaterThan { get; set; }
        public virtual decimal? FXCartage2TaxAmountLessThan { get; set; }
        public virtual decimal? FXCartage2TaxAmountLessThanOrEqualTo { get; set; }
        public virtual decimal? FXCartage2TaxAmountNotEqualTo { get; set; }
        public virtual decimal?[] FXCartage2TaxAmountBetween { get; set; }
        public virtual decimal?[] FXCartage2TaxAmountIn { get; set; }
        public virtual decimal? FXCartageCharge3 { get; set; }
        public virtual decimal? FXCartageCharge3GreaterThanOrEqualTo { get; set; }
        public virtual decimal? FXCartageCharge3GreaterThan { get; set; }
        public virtual decimal? FXCartageCharge3LessThan { get; set; }
        public virtual decimal? FXCartageCharge3LessThanOrEqualTo { get; set; }
        public virtual decimal? FXCartageCharge3NotEqualTo { get; set; }
        public virtual decimal?[] FXCartageCharge3Between { get; set; }
        public virtual decimal?[] FXCartageCharge3In { get; set; }
        public virtual decimal? FXCartage3TaxAmount { get; set; }
        public virtual decimal? FXCartage3TaxAmountGreaterThanOrEqualTo { get; set; }
        public virtual decimal? FXCartage3TaxAmountGreaterThan { get; set; }
        public virtual decimal? FXCartage3TaxAmountLessThan { get; set; }
        public virtual decimal? FXCartage3TaxAmountLessThanOrEqualTo { get; set; }
        public virtual decimal? FXCartage3TaxAmountNotEqualTo { get; set; }
        public virtual decimal?[] FXCartage3TaxAmountBetween { get; set; }
        public virtual decimal?[] FXCartage3TaxAmountIn { get; set; }
        public virtual string CourierDetails { get; set; }
        public virtual string CourierDetailsStartsWith { get; set; }
        public virtual string CourierDetailsEndsWith { get; set; }
        public virtual string CourierDetailsContains { get; set; }
        public virtual string CourierDetailsLike { get; set; }
        public virtual string[] CourierDetailsBetween { get; set; }
        public virtual string[] CourierDetailsIn { get; set; }
        public virtual string Notes { get; set; }
        public virtual string NotesStartsWith { get; set; }
        public virtual string NotesEndsWith { get; set; }
        public virtual string NotesContains { get; set; }
        public virtual string NotesLike { get; set; }
        public virtual string[] NotesBetween { get; set; }
        public virtual string[] NotesIn { get; set; }
        public virtual string EmailAddress { get; set; }
        public virtual string EmailAddressStartsWith { get; set; }
        public virtual string EmailAddressEndsWith { get; set; }
        public virtual string EmailAddressContains { get; set; }
        public virtual string EmailAddressLike { get; set; }
        public virtual string[] EmailAddressBetween { get; set; }
        public virtual string[] EmailAddressIn { get; set; }
        public virtual string StaffID { get; set; }
        public virtual string StaffIDStartsWith { get; set; }
        public virtual string StaffIDEndsWith { get; set; }
        public virtual string StaffIDContains { get; set; }
        public virtual string StaffIDLike { get; set; }
        public virtual string[] StaffIDBetween { get; set; }
        public virtual string[] StaffIDIn { get; set; }
        public virtual string StaffTitle { get; set; }
        public virtual string StaffTitleStartsWith { get; set; }
        public virtual string StaffTitleEndsWith { get; set; }
        public virtual string StaffTitleContains { get; set; }
        public virtual string StaffTitleLike { get; set; }
        public virtual string[] StaffTitleBetween { get; set; }
        public virtual string[] StaffTitleIn { get; set; }
        public virtual string StaffFirstName { get; set; }
        public virtual string StaffFirstNameStartsWith { get; set; }
        public virtual string StaffFirstNameEndsWith { get; set; }
        public virtual string StaffFirstNameContains { get; set; }
        public virtual string StaffFirstNameLike { get; set; }
        public virtual string[] StaffFirstNameBetween { get; set; }
        public virtual string[] StaffFirstNameIn { get; set; }
        public virtual string StaffSurname { get; set; }
        public virtual string StaffSurnameStartsWith { get; set; }
        public virtual string StaffSurnameEndsWith { get; set; }
        public virtual string StaffSurnameContains { get; set; }
        public virtual string StaffSurnameLike { get; set; }
        public virtual string[] StaffSurnameBetween { get; set; }
        public virtual string[] StaffSurnameIn { get; set; }
        public virtual string StaffUsername { get; set; }
        public virtual string StaffUsernameStartsWith { get; set; }
        public virtual string StaffUsernameEndsWith { get; set; }
        public virtual string StaffUsernameContains { get; set; }
        public virtual string StaffUsernameLike { get; set; }
        public virtual string[] StaffUsernameBetween { get; set; }
        public virtual string[] StaffUsernameIn { get; set; }
        public virtual short? HistoryNo { get; set; }
        public virtual short? HistoryNoGreaterThanOrEqualTo { get; set; }
        public virtual short? HistoryNoGreaterThan { get; set; }
        public virtual short? HistoryNoLessThan { get; set; }
        public virtual short? HistoryNoLessThanOrEqualTo { get; set; }
        public virtual short? HistoryNoNotEqualTo { get; set; }
        public virtual short?[] HistoryNoBetween { get; set; }
        public virtual short?[] HistoryNoIn { get; set; }
        public virtual string CurrencyID { get; set; }
        public virtual string CurrencyIDStartsWith { get; set; }
        public virtual string CurrencyIDEndsWith { get; set; }
        public virtual string CurrencyIDContains { get; set; }
        public virtual string CurrencyIDLike { get; set; }
        public virtual string[] CurrencyIDBetween { get; set; }
        public virtual string[] CurrencyIDIn { get; set; }
        public virtual string CurrencyShortName { get; set; }
        public virtual string CurrencyShortNameStartsWith { get; set; }
        public virtual string CurrencyShortNameEndsWith { get; set; }
        public virtual string CurrencyShortNameContains { get; set; }
        public virtual string CurrencyShortNameLike { get; set; }
        public virtual string[] CurrencyShortNameBetween { get; set; }
        public virtual string[] CurrencyShortNameIn { get; set; }
        public virtual string CurrencyName { get; set; }
        public virtual string CurrencyNameStartsWith { get; set; }
        public virtual string CurrencyNameEndsWith { get; set; }
        public virtual string CurrencyNameContains { get; set; }
        public virtual string CurrencyNameLike { get; set; }
        public virtual string[] CurrencyNameBetween { get; set; }
        public virtual string[] CurrencyNameIn { get; set; }
        public virtual short? DecimalPlaces { get; set; }
        public virtual short? DecimalPlacesGreaterThanOrEqualTo { get; set; }
        public virtual short? DecimalPlacesGreaterThan { get; set; }
        public virtual short? DecimalPlacesLessThan { get; set; }
        public virtual short? DecimalPlacesLessThanOrEqualTo { get; set; }
        public virtual short? DecimalPlacesNotEqualTo { get; set; }
        public virtual short?[] DecimalPlacesBetween { get; set; }
        public virtual short?[] DecimalPlacesIn { get; set; }
    }
    #endregion

    #region "Debtors"
    public partial class v_Jiwa_DB_ContactNameMultiple
    {
        [Required]
        public virtual string RecID { get; set; }

        [Required]
        public virtual string AccountNo { get; set; }

        public virtual string Name { get; set; }
        public virtual string Address1 { get; set; }
        public virtual string Address2 { get; set; }
        public virtual string Address3 { get; set; }
        public virtual string Address4 { get; set; }
        public virtual string PostCode { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string Surname { get; set; }
        public virtual string EmailAddress { get; set; }
        [Required]
        public virtual string DebtorID { get; set; }
    }

    [Route("/Queries/ContactNameMultiples", "GET")]
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    public partial class v_Jiwa_DB_ContactNameMultipleQuery
        : QueryDb<v_Jiwa_DB_ContactNameMultiple>, IReturn<QueryResponse<v_Jiwa_DB_ContactNameMultiple>>
    {
        public virtual string RecID { get; set; }
        public virtual string RecIDStartsWith { get; set; }
        public virtual string RecIDEndsWith { get; set; }
        public virtual string RecIDContains { get; set; }
        public virtual string RecIDLike { get; set; }
        public virtual string[] RecIDBetween { get; set; }
        public virtual string[] RecIDIn { get; set; }
        public virtual string AccountNo { get; set; }
        public virtual string AccountNoStartsWith { get; set; }
        public virtual string AccountNoEndsWith { get; set; }
        public virtual string AccountNoContains { get; set; }
        public virtual string AccountNoLike { get; set; }
        public virtual string[] AccountNoBetween { get; set; }
        public virtual string[] AccountNoIn { get; set; }
        public virtual string Name { get; set; }
        public virtual string NameStartsWith { get; set; }
        public virtual string NameEndsWith { get; set; }
        public virtual string NameContains { get; set; }
        public virtual string NameLike { get; set; }
        public virtual string[] NameBetween { get; set; }
        public virtual string[] NameIn { get; set; }
        public virtual string Address1 { get; set; }
        public virtual string Address1StartsWith { get; set; }
        public virtual string Address1EndsWith { get; set; }
        public virtual string Address1Contains { get; set; }
        public virtual string Address1Like { get; set; }
        public virtual string[] Address1Between { get; set; }
        public virtual string[] Address1In { get; set; }
        public virtual string Address2 { get; set; }
        public virtual string Address2StartsWith { get; set; }
        public virtual string Address2EndsWith { get; set; }
        public virtual string Address2Contains { get; set; }
        public virtual string Address2Like { get; set; }
        public virtual string[] Address2Between { get; set; }
        public virtual string[] Address2In { get; set; }
        public virtual string Address3 { get; set; }
        public virtual string Address3StartsWith { get; set; }
        public virtual string Address3EndsWith { get; set; }
        public virtual string Address3Contains { get; set; }
        public virtual string Address3Like { get; set; }
        public virtual string[] Address3Between { get; set; }
        public virtual string[] Address3In { get; set; }
        public virtual string Address4 { get; set; }
        public virtual string Address4StartsWith { get; set; }
        public virtual string Address4EndsWith { get; set; }
        public virtual string Address4Contains { get; set; }
        public virtual string Address4Like { get; set; }
        public virtual string[] Address4Between { get; set; }
        public virtual string[] Address4In { get; set; }
        public virtual string PostCode { get; set; }
        public virtual string PostCodeStartsWith { get; set; }
        public virtual string PostCodeEndsWith { get; set; }
        public virtual string PostCodeContains { get; set; }
        public virtual string PostCodeLike { get; set; }
        public virtual string[] PostCodeBetween { get; set; }
        public virtual string[] PostCodeIn { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string FirstNameStartsWith { get; set; }
        public virtual string FirstNameEndsWith { get; set; }
        public virtual string FirstNameContains { get; set; }
        public virtual string FirstNameLike { get; set; }
        public virtual string[] FirstNameBetween { get; set; }
        public virtual string[] FirstNameIn { get; set; }
        public virtual string Surname { get; set; }
        public virtual string SurnameStartsWith { get; set; }
        public virtual string SurnameEndsWith { get; set; }
        public virtual string SurnameContains { get; set; }
        public virtual string SurnameLike { get; set; }
        public virtual string[] SurnameBetween { get; set; }
        public virtual string[] SurnameIn { get; set; }
        public virtual string EmailAddress { get; set; }
        public virtual string EmailAddressStartsWith { get; set; }
        public virtual string EmailAddressEndsWith { get; set; }
        public virtual string EmailAddressContains { get; set; }
        public virtual string EmailAddressLike { get; set; }
        public virtual string[] EmailAddressBetween { get; set; }
        public virtual string[] EmailAddressIn { get; set; }
        public virtual string DebtorID { get; set; }
        public virtual string DebtorIDStartsWith { get; set; }
        public virtual string DebtorIDEndsWith { get; set; }
        public virtual string DebtorIDContains { get; set; }
        public virtual string DebtorIDLike { get; set; }
        public virtual string[] DebtorIDBetween { get; set; }
        public virtual string[] DebtorIDIn { get; set; }
    }

    public partial class DB_ContactNamePasswordResetRequest
    {
        [Required]
        public virtual Guid RecID { get; set; }

        [Required]
        public virtual string Token { get; set; }

        [Required]
        public virtual DateTimeOffset ExpiryDate { get; set; }

        [Required]
        public virtual DateTimeOffset LastSavedDateTime { get; set; }

        [Required]
        public virtual string DB_ContactName_RecID { get; set; }

        public virtual byte[] RowHash { get; set; }
    }

    public partial class v_Jiwa_Debtor_Transactions_List
    {
        [Required]
        public virtual string TransID { get; set; }

        [Required]
        public virtual string DebtorID { get; set; }

        [Required]
        public virtual string AccountNo { get; set; }

        public virtual string Name { get; set; }
        public virtual DateTime? TranDate { get; set; }
        public virtual DateTime? DueDate { get; set; }
        public virtual string InvRemitNo { get; set; }
        [Required]
        public virtual bool DebitCredit { get; set; }

        [Required]
        public virtual decimal Amount { get; set; }

        public virtual decimal? AllocatedAmount { get; set; }
        [Required]
        public virtual decimal GSTAmount { get; set; }

        public virtual decimal? OutstandingAmount { get; set; }
        [Required]
        public virtual decimal DebitAmountExTax { get; set; }

        [Required]
        public virtual decimal CreditAmountExTax { get; set; }

        public virtual decimal? DebitAmountIncTax { get; set; }
        public virtual decimal? CreditAmountIncTax { get; set; }
        public virtual string Description { get; set; }
        public virtual string SourceID { get; set; }
        public virtual string Ref { get; set; }
        public virtual string Remark { get; set; }
        public virtual string Note { get; set; }
        [Required]
        public virtual bool AgedOut { get; set; }

        [Required]
        public virtual string CurrencyID { get; set; }

        [Required]
        public virtual string CurrencyShortName { get; set; }

        [Required]
        public virtual string CurrencyName { get; set; }

        [Required]
        public virtual short DecimalPlaces { get; set; }
    }

    [Route("/Queries/DebtorTransactionList", "GET")]
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    public partial class v_Jiwa_Debtor_Transactions_ListQuery
        : QueryDb<v_Jiwa_Debtor_Transactions_List>, IReturn<QueryResponse<v_Jiwa_Debtor_Transactions_List>>
    {
        public virtual string TransID { get; set; }
        public virtual string TransIDStartsWith { get; set; }
        public virtual string TransIDEndsWith { get; set; }
        public virtual string TransIDContains { get; set; }
        public virtual string TransIDLike { get; set; }
        public virtual string[] TransIDBetween { get; set; }
        public virtual string[] TransIDIn { get; set; }
        public virtual string DebtorID { get; set; }
        public virtual string DebtorIDStartsWith { get; set; }
        public virtual string DebtorIDEndsWith { get; set; }
        public virtual string DebtorIDContains { get; set; }
        public virtual string DebtorIDLike { get; set; }
        public virtual string[] DebtorIDBetween { get; set; }
        public virtual string[] DebtorIDIn { get; set; }
        public virtual string AccountNo { get; set; }
        public virtual string AccountNoStartsWith { get; set; }
        public virtual string AccountNoEndsWith { get; set; }
        public virtual string AccountNoContains { get; set; }
        public virtual string AccountNoLike { get; set; }
        public virtual string[] AccountNoBetween { get; set; }
        public virtual string[] AccountNoIn { get; set; }
        public virtual string Name { get; set; }
        public virtual string NameStartsWith { get; set; }
        public virtual string NameEndsWith { get; set; }
        public virtual string NameContains { get; set; }
        public virtual string NameLike { get; set; }
        public virtual string[] NameBetween { get; set; }
        public virtual string[] NameIn { get; set; }
        public virtual DateTime? TranDate { get; set; }
        public virtual DateTime? TranDateGreaterThanOrEqualTo { get; set; }
        public virtual DateTime? TranDateGreaterThan { get; set; }
        public virtual DateTime? TranDateLessThan { get; set; }
        public virtual DateTime? TranDateLessThanOrEqualTo { get; set; }
        public virtual DateTime? TranDateNotEqualTo { get; set; }
        public virtual DateTime?[] TranDateBetween { get; set; }
        public virtual DateTime?[] TranDateIn { get; set; }
        public virtual DateTime? DueDate { get; set; }
        public virtual DateTime? DueDateGreaterThanOrEqualTo { get; set; }
        public virtual DateTime? DueDateGreaterThan { get; set; }
        public virtual DateTime? DueDateLessThan { get; set; }
        public virtual DateTime? DueDateLessThanOrEqualTo { get; set; }
        public virtual DateTime? DueDateNotEqualTo { get; set; }
        public virtual DateTime?[] DueDateBetween { get; set; }
        public virtual DateTime?[] DueDateIn { get; set; }
        public virtual string InvRemitNo { get; set; }
        public virtual string InvRemitNoStartsWith { get; set; }
        public virtual string InvRemitNoEndsWith { get; set; }
        public virtual string InvRemitNoContains { get; set; }
        public virtual string InvRemitNoLike { get; set; }
        public virtual string[] InvRemitNoBetween { get; set; }
        public virtual string[] InvRemitNoIn { get; set; }
        public virtual bool? DebitCredit { get; set; }
        public virtual decimal? Amount { get; set; }
        public virtual decimal? AmountGreaterThanOrEqualTo { get; set; }
        public virtual decimal? AmountGreaterThan { get; set; }
        public virtual decimal? AmountLessThan { get; set; }
        public virtual decimal? AmountLessThanOrEqualTo { get; set; }
        public virtual decimal? AmountNotEqualTo { get; set; }
        public virtual decimal[] AmountBetween { get; set; }
        public virtual decimal[] AmountIn { get; set; }
        public virtual decimal? AllocatedAmount { get; set; }
        public virtual decimal? AllocatedAmountGreaterThanOrEqualTo { get; set; }
        public virtual decimal? AllocatedAmountGreaterThan { get; set; }
        public virtual decimal? AllocatedAmountLessThan { get; set; }
        public virtual decimal? AllocatedAmountLessThanOrEqualTo { get; set; }
        public virtual decimal? AllocatedAmountNotEqualTo { get; set; }
        public virtual decimal?[] AllocatedAmountBetween { get; set; }
        public virtual decimal?[] AllocatedAmountIn { get; set; }
        public virtual decimal? GSTAmount { get; set; }
        public virtual decimal? GSTAmountGreaterThanOrEqualTo { get; set; }
        public virtual decimal? GSTAmountGreaterThan { get; set; }
        public virtual decimal? GSTAmountLessThan { get; set; }
        public virtual decimal? GSTAmountLessThanOrEqualTo { get; set; }
        public virtual decimal? GSTAmountNotEqualTo { get; set; }
        public virtual decimal[] GSTAmountBetween { get; set; }
        public virtual decimal[] GSTAmountIn { get; set; }
        public virtual decimal? OutstandingAmount { get; set; }
        public virtual decimal? OutstandingAmountGreaterThanOrEqualTo { get; set; }
        public virtual decimal? OutstandingAmountGreaterThan { get; set; }
        public virtual decimal? OutstandingAmountLessThan { get; set; }
        public virtual decimal? OutstandingAmountLessThanOrEqualTo { get; set; }
        public virtual decimal? OutstandingAmountNotEqualTo { get; set; }
        public virtual decimal?[] OutstandingAmountBetween { get; set; }
        public virtual decimal?[] OutstandingAmountIn { get; set; }
        public virtual decimal? DebitAmountExTax { get; set; }
        public virtual decimal? DebitAmountExTaxGreaterThanOrEqualTo { get; set; }
        public virtual decimal? DebitAmountExTaxGreaterThan { get; set; }
        public virtual decimal? DebitAmountExTaxLessThan { get; set; }
        public virtual decimal? DebitAmountExTaxLessThanOrEqualTo { get; set; }
        public virtual decimal? DebitAmountExTaxNotEqualTo { get; set; }
        public virtual decimal[] DebitAmountExTaxBetween { get; set; }
        public virtual decimal[] DebitAmountExTaxIn { get; set; }
        public virtual decimal? CreditAmountExTax { get; set; }
        public virtual decimal? CreditAmountExTaxGreaterThanOrEqualTo { get; set; }
        public virtual decimal? CreditAmountExTaxGreaterThan { get; set; }
        public virtual decimal? CreditAmountExTaxLessThan { get; set; }
        public virtual decimal? CreditAmountExTaxLessThanOrEqualTo { get; set; }
        public virtual decimal? CreditAmountExTaxNotEqualTo { get; set; }
        public virtual decimal[] CreditAmountExTaxBetween { get; set; }
        public virtual decimal[] CreditAmountExTaxIn { get; set; }
        public virtual decimal? DebitAmountIncTax { get; set; }
        public virtual decimal? DebitAmountIncTaxGreaterThanOrEqualTo { get; set; }
        public virtual decimal? DebitAmountIncTaxGreaterThan { get; set; }
        public virtual decimal? DebitAmountIncTaxLessThan { get; set; }
        public virtual decimal? DebitAmountIncTaxLessThanOrEqualTo { get; set; }
        public virtual decimal? DebitAmountIncTaxNotEqualTo { get; set; }
        public virtual decimal?[] DebitAmountIncTaxBetween { get; set; }
        public virtual decimal?[] DebitAmountIncTaxIn { get; set; }
        public virtual decimal? CreditAmountIncTax { get; set; }
        public virtual decimal? CreditAmountIncTaxGreaterThanOrEqualTo { get; set; }
        public virtual decimal? CreditAmountIncTaxGreaterThan { get; set; }
        public virtual decimal? CreditAmountIncTaxLessThan { get; set; }
        public virtual decimal? CreditAmountIncTaxLessThanOrEqualTo { get; set; }
        public virtual decimal? CreditAmountIncTaxNotEqualTo { get; set; }
        public virtual decimal?[] CreditAmountIncTaxBetween { get; set; }
        public virtual decimal?[] CreditAmountIncTaxIn { get; set; }
        public virtual string Description { get; set; }
        public virtual string DescriptionStartsWith { get; set; }
        public virtual string DescriptionEndsWith { get; set; }
        public virtual string DescriptionContains { get; set; }
        public virtual string DescriptionLike { get; set; }
        public virtual string[] DescriptionBetween { get; set; }
        public virtual string[] DescriptionIn { get; set; }
        public virtual string SourceID { get; set; }
        public virtual string SourceIDStartsWith { get; set; }
        public virtual string SourceIDEndsWith { get; set; }
        public virtual string SourceIDContains { get; set; }
        public virtual string SourceIDLike { get; set; }
        public virtual string[] SourceIDBetween { get; set; }
        public virtual string[] SourceIDIn { get; set; }
        public virtual string Ref { get; set; }
        public virtual string RefStartsWith { get; set; }
        public virtual string RefEndsWith { get; set; }
        public virtual string RefContains { get; set; }
        public virtual string RefLike { get; set; }
        public virtual string[] RefBetween { get; set; }
        public virtual string[] RefIn { get; set; }
        public virtual string Remark { get; set; }
        public virtual string RemarkStartsWith { get; set; }
        public virtual string RemarkEndsWith { get; set; }
        public virtual string RemarkContains { get; set; }
        public virtual string RemarkLike { get; set; }
        public virtual string[] RemarkBetween { get; set; }
        public virtual string[] RemarkIn { get; set; }
        public virtual string Note { get; set; }
        public virtual string NoteStartsWith { get; set; }
        public virtual string NoteEndsWith { get; set; }
        public virtual string NoteContains { get; set; }
        public virtual string NoteLike { get; set; }
        public virtual string[] NoteBetween { get; set; }
        public virtual string[] NoteIn { get; set; }
        public virtual bool? AgedOut { get; set; }
        public virtual string CurrencyID { get; set; }
        public virtual string CurrencyIDStartsWith { get; set; }
        public virtual string CurrencyIDEndsWith { get; set; }
        public virtual string CurrencyIDContains { get; set; }
        public virtual string CurrencyIDLike { get; set; }
        public virtual string[] CurrencyIDBetween { get; set; }
        public virtual string[] CurrencyIDIn { get; set; }
        public virtual string CurrencyShortName { get; set; }
        public virtual string CurrencyShortNameStartsWith { get; set; }
        public virtual string CurrencyShortNameEndsWith { get; set; }
        public virtual string CurrencyShortNameContains { get; set; }
        public virtual string CurrencyShortNameLike { get; set; }
        public virtual string[] CurrencyShortNameBetween { get; set; }
        public virtual string[] CurrencyShortNameIn { get; set; }
        public virtual string CurrencyName { get; set; }
        public virtual string CurrencyNameStartsWith { get; set; }
        public virtual string CurrencyNameEndsWith { get; set; }
        public virtual string CurrencyNameContains { get; set; }
        public virtual string CurrencyNameLike { get; set; }
        public virtual string[] CurrencyNameBetween { get; set; }
        public virtual string[] CurrencyNameIn { get; set; }
        public virtual short? DecimalPlaces { get; set; }
        public virtual short? DecimalPlacesGreaterThanOrEqualTo { get; set; }
        public virtual short? DecimalPlacesGreaterThan { get; set; }
        public virtual short? DecimalPlacesLessThan { get; set; }
        public virtual short? DecimalPlacesLessThanOrEqualTo { get; set; }
        public virtual short? DecimalPlacesNotEqualTo { get; set; }
        public virtual short[] DecimalPlacesBetween { get; set; }
        public virtual short[] DecimalPlacesIn { get; set; }
    }

    [Route("/Debtors/{DebtorID}/Backorders", "GET")]
    [ApiResponse(Description = "Rread OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    [ApiResponse(Description = "No debtor with the DebtorID provided was found", StatusCode = 404)]
    public partial class DebtorBackordersGETRequest
        : IReturn<List<DebtorBackOrder>>
    {
        public virtual string DebtorID { get; set; }
    }

    public partial class v_Jiwa_Debtor_List
    {
        [Required]
        public virtual string DebtorID { get; set; }

        [Required]
        public virtual string AccountNo { get; set; }

        public virtual string Name { get; set; }
        public virtual string AltAccountNo { get; set; }
        public virtual string Address1 { get; set; }
        public virtual string Address2 { get; set; }
        public virtual string Address3 { get; set; }
        public virtual string Address4 { get; set; }
        public virtual string PostCode { get; set; }
        public virtual string Country { get; set; }
        public virtual string Phone { get; set; }
        public virtual bool? AccountOnHold { get; set; }
        public virtual string EmailAddress { get; set; }
        public virtual decimal? CurrentBalance { get; set; }
        [Required]
        public virtual bool WebAccess { get; set; }

        [Required]
        public virtual DateTimeOffset LastSavedDateTime { get; set; }

        public virtual byte? TradingStatus { get; set; }
        [Required]
        public virtual string DebtorClassificationID { get; set; }

        [Required]
        public virtual string ClassificationDescription { get; set; }

        [Required]
        public virtual string Category1ID { get; set; }

        public virtual string Category1Description { get; set; }
        [Required]
        public virtual string Category2ID { get; set; }

        public virtual string Category2Description { get; set; }
        [Required]
        public virtual string Category3ID { get; set; }

        public virtual string Category3Description { get; set; }
        [Required]
        public virtual string Category4ID { get; set; }

        public virtual string Category4Description { get; set; }
        [Required]
        public virtual string Category5ID { get; set; }

        public virtual string Category5Description { get; set; }
        [Required]
        public virtual string PriceSchemeID { get; set; }

        [Required]
        public virtual string PriceSchemeDescription { get; set; }

        [Required]
        public virtual string PricingGroupDescription { get; set; }
    }

    [Route("/Queries/DebtorList", "GET")]
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    public partial class v_Jiwa_Debtor_ListQuery
        : QueryDb<v_Jiwa_Debtor_List>, IReturn<QueryResponse<v_Jiwa_Debtor_List>>
    {
        public virtual string DebtorID { get; set; }
        public virtual string DebtorIDStartsWith { get; set; }
        public virtual string DebtorIDEndsWith { get; set; }
        public virtual string DebtorIDContains { get; set; }
        public virtual string DebtorIDLike { get; set; }
        public virtual string[] DebtorIDBetween { get; set; }
        public virtual string[] DebtorIDIn { get; set; }
        public virtual string AccountNo { get; set; }
        public virtual string AccountNoStartsWith { get; set; }
        public virtual string AccountNoEndsWith { get; set; }
        public virtual string AccountNoContains { get; set; }
        public virtual string AccountNoLike { get; set; }
        public virtual string[] AccountNoBetween { get; set; }
        public virtual string[] AccountNoIn { get; set; }
        public virtual string Name { get; set; }
        public virtual string NameStartsWith { get; set; }
        public virtual string NameEndsWith { get; set; }
        public virtual string NameContains { get; set; }
        public virtual string NameLike { get; set; }
        public virtual string[] NameBetween { get; set; }
        public virtual string[] NameIn { get; set; }
        public virtual string AltAccountNo { get; set; }
        public virtual string AltAccountNoStartsWith { get; set; }
        public virtual string AltAccountNoEndsWith { get; set; }
        public virtual string AltAccountNoContains { get; set; }
        public virtual string AltAccountNoLike { get; set; }
        public virtual string[] AltAccountNoBetween { get; set; }
        public virtual string[] AltAccountNoIn { get; set; }
        public virtual string Address1 { get; set; }
        public virtual string Address1StartsWith { get; set; }
        public virtual string Address1EndsWith { get; set; }
        public virtual string Address1Contains { get; set; }
        public virtual string Address1Like { get; set; }
        public virtual string[] Address1Between { get; set; }
        public virtual string[] Address1In { get; set; }
        public virtual string Address2 { get; set; }
        public virtual string Address2StartsWith { get; set; }
        public virtual string Address2EndsWith { get; set; }
        public virtual string Address2Contains { get; set; }
        public virtual string Address2Like { get; set; }
        public virtual string[] Address2Between { get; set; }
        public virtual string[] Address2In { get; set; }
        public virtual string Address3 { get; set; }
        public virtual string Address3StartsWith { get; set; }
        public virtual string Address3EndsWith { get; set; }
        public virtual string Address3Contains { get; set; }
        public virtual string Address3Like { get; set; }
        public virtual string[] Address3Between { get; set; }
        public virtual string[] Address3In { get; set; }
        public virtual string Address4 { get; set; }
        public virtual string Address4StartsWith { get; set; }
        public virtual string Address4EndsWith { get; set; }
        public virtual string Address4Contains { get; set; }
        public virtual string Address4Like { get; set; }
        public virtual string[] Address4Between { get; set; }
        public virtual string[] Address4In { get; set; }
        public virtual string PostCode { get; set; }
        public virtual string PostCodeStartsWith { get; set; }
        public virtual string PostCodeEndsWith { get; set; }
        public virtual string PostCodeContains { get; set; }
        public virtual string PostCodeLike { get; set; }
        public virtual string[] PostCodeBetween { get; set; }
        public virtual string[] PostCodeIn { get; set; }
        public virtual string Country { get; set; }
        public virtual string CountryStartsWith { get; set; }
        public virtual string CountryEndsWith { get; set; }
        public virtual string CountryContains { get; set; }
        public virtual string CountryLike { get; set; }
        public virtual string[] CountryBetween { get; set; }
        public virtual string[] CountryIn { get; set; }
        public virtual string Phone { get; set; }
        public virtual string PhoneStartsWith { get; set; }
        public virtual string PhoneEndsWith { get; set; }
        public virtual string PhoneContains { get; set; }
        public virtual string PhoneLike { get; set; }
        public virtual string[] PhoneBetween { get; set; }
        public virtual string[] PhoneIn { get; set; }
        public virtual bool? AccountOnHold { get; set; }
        public virtual string EmailAddress { get; set; }
        public virtual string EmailAddressStartsWith { get; set; }
        public virtual string EmailAddressEndsWith { get; set; }
        public virtual string EmailAddressContains { get; set; }
        public virtual string EmailAddressLike { get; set; }
        public virtual string[] EmailAddressBetween { get; set; }
        public virtual string[] EmailAddressIn { get; set; }
        public virtual decimal? CurrentBalance { get; set; }
        public virtual decimal? CurrentBalanceGreaterThanOrEqualTo { get; set; }
        public virtual decimal? CurrentBalanceGreaterThan { get; set; }
        public virtual decimal? CurrentBalanceLessThan { get; set; }
        public virtual decimal? CurrentBalanceLessThanOrEqualTo { get; set; }
        public virtual decimal? CurrentBalanceNotEqualTo { get; set; }
        public virtual decimal?[] CurrentBalanceBetween { get; set; }
        public virtual decimal?[] CurrentBalanceIn { get; set; }
        public virtual bool? WebAccess { get; set; }
        public virtual DateTimeOffset? LastSavedDateTime { get; set; }
        public virtual DateTimeOffset? LastSavedDateTimeGreaterThanOrEqualTo { get; set; }
        public virtual DateTimeOffset? LastSavedDateTimeGreaterThan { get; set; }
        public virtual DateTimeOffset? LastSavedDateTimeLessThan { get; set; }
        public virtual DateTimeOffset? LastSavedDateTimeLessThanOrEqualTo { get; set; }
        public virtual DateTimeOffset? LastSavedDateTimeNotEqualTo { get; set; }
        public virtual DateTimeOffset[] LastSavedDateTimeBetween { get; set; }
        public virtual DateTimeOffset[] LastSavedDateTimeIn { get; set; }
        public virtual byte? TradingStatus { get; set; }
        public virtual byte? TradingStatusGreaterThanOrEqualTo { get; set; }
        public virtual byte? TradingStatusGreaterThan { get; set; }
        public virtual byte? TradingStatusLessThan { get; set; }
        public virtual byte? TradingStatusLessThanOrEqualTo { get; set; }
        public virtual byte? TradingStatusNotEqualTo { get; set; }
        public virtual byte?[] TradingStatusBetween { get; set; }
        public virtual byte?[] TradingStatusIn { get; set; }
        public virtual string DebtorClassificationID { get; set; }
        public virtual string DebtorClassificationIDStartsWith { get; set; }
        public virtual string DebtorClassificationIDEndsWith { get; set; }
        public virtual string DebtorClassificationIDContains { get; set; }
        public virtual string DebtorClassificationIDLike { get; set; }
        public virtual string[] DebtorClassificationIDBetween { get; set; }
        public virtual string[] DebtorClassificationIDIn { get; set; }
        public virtual string ClassificationDescription { get; set; }
        public virtual string ClassificationDescriptionStartsWith { get; set; }
        public virtual string ClassificationDescriptionEndsWith { get; set; }
        public virtual string ClassificationDescriptionContains { get; set; }
        public virtual string ClassificationDescriptionLike { get; set; }
        public virtual string[] ClassificationDescriptionBetween { get; set; }
        public virtual string[] ClassificationDescriptionIn { get; set; }
        public virtual string Category1ID { get; set; }
        public virtual string Category1IDStartsWith { get; set; }
        public virtual string Category1IDEndsWith { get; set; }
        public virtual string Category1IDContains { get; set; }
        public virtual string Category1IDLike { get; set; }
        public virtual string[] Category1IDBetween { get; set; }
        public virtual string[] Category1IDIn { get; set; }
        public virtual string Category1Description { get; set; }
        public virtual string Category1DescriptionStartsWith { get; set; }
        public virtual string Category1DescriptionEndsWith { get; set; }
        public virtual string Category1DescriptionContains { get; set; }
        public virtual string Category1DescriptionLike { get; set; }
        public virtual string[] Category1DescriptionBetween { get; set; }
        public virtual string[] Category1DescriptionIn { get; set; }
        public virtual string Category2ID { get; set; }
        public virtual string Category2IDStartsWith { get; set; }
        public virtual string Category2IDEndsWith { get; set; }
        public virtual string Category2IDContains { get; set; }
        public virtual string Category2IDLike { get; set; }
        public virtual string[] Category2IDBetween { get; set; }
        public virtual string[] Category2IDIn { get; set; }
        public virtual string Category2Description { get; set; }
        public virtual string Category2DescriptionStartsWith { get; set; }
        public virtual string Category2DescriptionEndsWith { get; set; }
        public virtual string Category2DescriptionContains { get; set; }
        public virtual string Category2DescriptionLike { get; set; }
        public virtual string[] Category2DescriptionBetween { get; set; }
        public virtual string[] Category2DescriptionIn { get; set; }
        public virtual string Category3ID { get; set; }
        public virtual string Category3IDStartsWith { get; set; }
        public virtual string Category3IDEndsWith { get; set; }
        public virtual string Category3IDContains { get; set; }
        public virtual string Category3IDLike { get; set; }
        public virtual string[] Category3IDBetween { get; set; }
        public virtual string[] Category3IDIn { get; set; }
        public virtual string Category3Description { get; set; }
        public virtual string Category3DescriptionStartsWith { get; set; }
        public virtual string Category3DescriptionEndsWith { get; set; }
        public virtual string Category3DescriptionContains { get; set; }
        public virtual string Category3DescriptionLike { get; set; }
        public virtual string[] Category3DescriptionBetween { get; set; }
        public virtual string[] Category3DescriptionIn { get; set; }
        public virtual string Category4ID { get; set; }
        public virtual string Category4IDStartsWith { get; set; }
        public virtual string Category4IDEndsWith { get; set; }
        public virtual string Category4IDContains { get; set; }
        public virtual string Category4IDLike { get; set; }
        public virtual string[] Category4IDBetween { get; set; }
        public virtual string[] Category4IDIn { get; set; }
        public virtual string Category4Description { get; set; }
        public virtual string Category4DescriptionStartsWith { get; set; }
        public virtual string Category4DescriptionEndsWith { get; set; }
        public virtual string Category4DescriptionContains { get; set; }
        public virtual string Category4DescriptionLike { get; set; }
        public virtual string[] Category4DescriptionBetween { get; set; }
        public virtual string[] Category4DescriptionIn { get; set; }
        public virtual string Category5ID { get; set; }
        public virtual string Category5IDStartsWith { get; set; }
        public virtual string Category5IDEndsWith { get; set; }
        public virtual string Category5IDContains { get; set; }
        public virtual string Category5IDLike { get; set; }
        public virtual string[] Category5IDBetween { get; set; }
        public virtual string[] Category5IDIn { get; set; }
        public virtual string Category5Description { get; set; }
        public virtual string Category5DescriptionStartsWith { get; set; }
        public virtual string Category5DescriptionEndsWith { get; set; }
        public virtual string Category5DescriptionContains { get; set; }
        public virtual string Category5DescriptionLike { get; set; }
        public virtual string[] Category5DescriptionBetween { get; set; }
        public virtual string[] Category5DescriptionIn { get; set; }
        public virtual string PriceSchemeID { get; set; }
        public virtual string PriceSchemeIDStartsWith { get; set; }
        public virtual string PriceSchemeIDEndsWith { get; set; }
        public virtual string PriceSchemeIDContains { get; set; }
        public virtual string PriceSchemeIDLike { get; set; }
        public virtual string[] PriceSchemeIDBetween { get; set; }
        public virtual string[] PriceSchemeIDIn { get; set; }
        public virtual string PriceSchemeDescription { get; set; }
        public virtual string PriceSchemeDescriptionStartsWith { get; set; }
        public virtual string PriceSchemeDescriptionEndsWith { get; set; }
        public virtual string PriceSchemeDescriptionContains { get; set; }
        public virtual string PriceSchemeDescriptionLike { get; set; }
        public virtual string[] PriceSchemeDescriptionBetween { get; set; }
        public virtual string[] PriceSchemeDescriptionIn { get; set; }
        public virtual string PricingGroupDescription { get; set; }
        public virtual string PricingGroupDescriptionStartsWith { get; set; }
        public virtual string PricingGroupDescriptionEndsWith { get; set; }
        public virtual string PricingGroupDescriptionContains { get; set; }
        public virtual string PricingGroupDescriptionLike { get; set; }
        public virtual string[] PricingGroupDescriptionBetween { get; set; }
        public virtual string[] PricingGroupDescriptionIn { get; set; }
    }
    #endregion

    #region "FX Currency"
    public partial class FX_Currency
    {
        [Required]
        public virtual string RecID { get; set; }

        [Required]
        public virtual DateTimeOffset LastSavedDateTime { get; set; }

        [Required]
        public virtual string LastSavedByStaffID { get; set; }

        public virtual string Name { get; set; }
        public virtual string ShortName { get; set; }
        public virtual short? DecimalPlaces { get; set; }
        [Required]
        public virtual bool IsLocal { get; set; }

        [Required]
        public virtual bool IsEnabled { get; set; }

        public virtual byte[] Picture { get; set; }
        public virtual string Symbol { get; set; }
        public virtual Guid? Default_BA_BankAccount_RecID { get; set; }
    }

    [Route("/Queries/FX_Currency", "GET")]
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    public partial class FX_CurrencyQuery
        : QueryDb<FX_Currency>, IReturn<QueryResponse<FX_Currency>>
    {
        public virtual string RecID { get; set; }
        public virtual string RecIDStartsWith { get; set; }
        public virtual string RecIDEndsWith { get; set; }
        public virtual string RecIDContains { get; set; }
        public virtual string RecIDLike { get; set; }
        public virtual string[] RecIDBetween { get; set; }
        public virtual string[] RecIDIn { get; set; }
        public virtual DateTimeOffset? LastSavedDateTime { get; set; }
        public virtual DateTimeOffset? LastSavedDateTimeGreaterThanOrEqualTo { get; set; }
        public virtual DateTimeOffset? LastSavedDateTimeGreaterThan { get; set; }
        public virtual DateTimeOffset? LastSavedDateTimeLessThan { get; set; }
        public virtual DateTimeOffset? LastSavedDateTimeLessThanOrEqualTo { get; set; }
        public virtual DateTimeOffset? LastSavedDateTimeNotEqualTo { get; set; }
        public virtual DateTimeOffset[] LastSavedDateTimeBetween { get; set; }
        public virtual DateTimeOffset[] LastSavedDateTimeIn { get; set; }
        public virtual string LastSavedByStaffID { get; set; }
        public virtual string LastSavedByStaffIDStartsWith { get; set; }
        public virtual string LastSavedByStaffIDEndsWith { get; set; }
        public virtual string LastSavedByStaffIDContains { get; set; }
        public virtual string LastSavedByStaffIDLike { get; set; }
        public virtual string[] LastSavedByStaffIDBetween { get; set; }
        public virtual string[] LastSavedByStaffIDIn { get; set; }
        public virtual string Name { get; set; }
        public virtual string NameStartsWith { get; set; }
        public virtual string NameEndsWith { get; set; }
        public virtual string NameContains { get; set; }
        public virtual string NameLike { get; set; }
        public virtual string[] NameBetween { get; set; }
        public virtual string[] NameIn { get; set; }
        public virtual string ShortName { get; set; }
        public virtual string ShortNameStartsWith { get; set; }
        public virtual string ShortNameEndsWith { get; set; }
        public virtual string ShortNameContains { get; set; }
        public virtual string ShortNameLike { get; set; }
        public virtual string[] ShortNameBetween { get; set; }
        public virtual string[] ShortNameIn { get; set; }
        public virtual short? DecimalPlaces { get; set; }
        public virtual short? DecimalPlacesGreaterThanOrEqualTo { get; set; }
        public virtual short? DecimalPlacesGreaterThan { get; set; }
        public virtual short? DecimalPlacesLessThan { get; set; }
        public virtual short? DecimalPlacesLessThanOrEqualTo { get; set; }
        public virtual short? DecimalPlacesNotEqualTo { get; set; }
        public virtual short?[] DecimalPlacesBetween { get; set; }
        public virtual short?[] DecimalPlacesIn { get; set; }
        public virtual bool? IsLocal { get; set; }
        public virtual bool? IsEnabled { get; set; }
        public virtual byte[] Picture { get; set; }
        public virtual string Symbol { get; set; }
        public virtual string SymbolStartsWith { get; set; }
        public virtual string SymbolEndsWith { get; set; }
        public virtual string SymbolContains { get; set; }
        public virtual string SymbolLike { get; set; }
        public virtual string[] SymbolBetween { get; set; }
        public virtual string[] SymbolIn { get; set; }
        public virtual Guid? Default_BA_BankAccount_RecID { get; set; }
        public virtual Guid?[] Default_BA_BankAccount_RecIDIn { get; set; }
    }
    #endregion

    #region "Diagnostics"
    [Route("/Queries/StartupLog", "GET")]
    public partial class StartupLogEntryQuery
        : QueryData<StartupLogEntry>, IReturn<QueryResponse<StartupLogEntry>>
    {
    }

    [Route("/Queries/PluginExceptions", "GET")]
    public partial class PluginExceptionQuery
       : QueryData<PluginException>, IReturn<QueryResponse<PluginException>>
    {
    }
    #endregion
}
#endregion 

namespace JiwaFinancials.Jiwa.JiwaServiceModel.Startup.Diagnostics
{
    public enum ExceptionPolicies
    {
        Report,
        Abort,
        Ignore,
    }

    public partial class PluginException
    {
        public virtual string RecID { get; set; }
        public virtual string Name { get; set; }
        public virtual Exception Exception { get; set; }
        public virtual ExceptionPolicies ExceptionPolicy { get; set; }
    }

    public partial class StartupLogEntry
    {
        public virtual string Description { get; set; }
        public virtual DateTime StartDateTime { get; set; }
        public virtual DateTime EndDateTime { get; set; }
        public virtual long ElapsedMilliseconds { get; set; }
        public virtual int Depth { get; set; }
    }

}

#endregion

