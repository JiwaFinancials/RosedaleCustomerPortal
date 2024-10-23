using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ServiceStack;
using ServiceStack.DataAnnotations;

using JiwaFinancials.Jiwa.JiwaServiceModel.Tables;
using ServiceStack.Web;
using System.Reflection.Metadata;
using JiwaFinancials.Jiwa.JiwaServiceModel.Debtors;
using JiwaFinancials.Jiwa.JiwaServiceModel.Tags;

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
}
#endregion

#region "Standard Jiwa API DTOs"
#region "Request DTOs"
namespace JiwaFinancials.Jiwa.JiwaServiceModel
{    
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

    [Route("/SalesOrders/{InvoiceID}/InvoiceReport/{ReportID}", "GET")]
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    [ApiResponse(Description = "No Sales Order with the InvoiceID, or Report with the ReportID provided was found", StatusCode = 404)]
    public partial class InvoiceReportGETRequest
        : IReturn<IHttpResult>
    {
        public virtual string InvoiceID { get; set; }
        public virtual string ReportID { get; set; }
        public virtual bool AsAttachment { get; set; }
    }

    [Route("/SalesQuotes/{QuoteID}/QuoteReport/{ReportID}", "GET")]
    [ApiResponse(Description = "Read OK", StatusCode = 200)]
    [ApiResponse(Description = "Not authenticated", StatusCode = 401)]
    [ApiResponse(Description = "Not authorised", StatusCode = 403)]
    [ApiResponse(Description = "No Sales Quote with the QuoteID, or Report with the ReportID provided was found", StatusCode = 404)]
    public partial class SalesQuoteReportGETRequest
        : IReturn<IHttpResult>
    {
        public virtual string QuoteID { get; set; }
        public virtual string ReportID { get; set; }
        public virtual bool AsAttachment { get; set; }
    }

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
}
#endregion

#region "Models"
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
        public virtual int? FXDecimalPlaces { get; set; }
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
        public virtual int? DefaultCurrencyDecimalPlaces { get; set; }
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
        public virtual int? CurrencyDecimalPlaces { get; set; }
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
        public virtual string CurrentCustomerWebPortalPassword { get; set; }
        public virtual string NewCustomerWebPortalPassword { get; set; }
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
        public virtual int QuantityDecimalPlaces { get; set; }
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
#endregion

#region "AutoQueries and Tables"
namespace JiwaFinancials.Jiwa.JiwaServiceModel.Tables
{
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

        public virtual decimal? InvoiceTotal { get; set; }
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
        [Required]
        public virtual string DeliveryAddressee { get; set; }

        public virtual string DeliveryAddress1 { get; set; }
        public virtual string DeliveryAddress2 { get; set; }
        public virtual string DeliveryAddress3 { get; set; }
        public virtual string DeliveryAddress4 { get; set; }
        public virtual string DeliveryAddressPostcode { get; set; }
        [Required]
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
        public virtual decimal? InvoiceTotal { get; set; }
        public virtual decimal? InvoiceTotalGreaterThanOrEqualTo { get; set; }
        public virtual decimal? InvoiceTotalGreaterThan { get; set; }
        public virtual decimal? InvoiceTotalLessThan { get; set; }
        public virtual decimal? InvoiceTotalLessThanOrEqualTo { get; set; }
        public virtual decimal? InvoiceTotalNotEqualTo { get; set; }
        public virtual decimal?[] InvoiceTotalBetween { get; set; }
        public virtual decimal?[] InvoiceTotalIn { get; set; }
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

        public virtual string OrderNo { get; set; }
        public virtual string QOReference { get; set; }
        [Required]
        public virtual DateTime InvoiceInitDate { get; set; }

        public virtual short? Status { get; set; }
        [Required]
        public virtual DateTimeOffset LastSavedDateTime { get; set; }

        public virtual decimal? InvoiceTotal { get; set; }
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
        public virtual string DeliveryAddressContactName { get; set; }
        [Required]
        public virtual string DeliveryAddressee { get; set; }

        public virtual string DeliveryAddress1 { get; set; }
        public virtual string DeliveryAddress2 { get; set; }
        public virtual string DeliveryAddress3 { get; set; }
        public virtual string DeliveryAddress4 { get; set; }
        public virtual string DeliveryAddressPostcode { get; set; }
        [Required]
        public virtual string DeliveryAddressCountry { get; set; }

        public virtual decimal? CartageCharge1 { get; set; }
        public virtual decimal? Cartage1TaxAmount { get; set; }
        public virtual decimal? CartageCharge2 { get; set; }
        public virtual decimal? Cartage2TaxAmount { get; set; }
        public virtual decimal? CartageCharge3 { get; set; }
        public virtual decimal? Cartage3TaxAmount { get; set; }
        public virtual string CourierDetails { get; set; }
        public virtual string Notes { get; set; }
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
        public virtual decimal? InvoiceTotal { get; set; }
        public virtual decimal? InvoiceTotalGreaterThanOrEqualTo { get; set; }
        public virtual decimal? InvoiceTotalGreaterThan { get; set; }
        public virtual decimal? InvoiceTotalLessThan { get; set; }
        public virtual decimal? InvoiceTotalLessThanOrEqualTo { get; set; }
        public virtual decimal? InvoiceTotalNotEqualTo { get; set; }
        public virtual decimal?[] InvoiceTotalBetween { get; set; }
        public virtual decimal?[] InvoiceTotalIn { get; set; }
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
}
#endregion 
#endregion
