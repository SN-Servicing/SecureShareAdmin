namespace Snsc.SecureShareAdmin.Domain;

[Flags]
public enum ExternalUserAccess
{
    None = 0,
    LoanAccess = 1,
    SdiUpload = 2,
    SdiDownload = 4,
    SecureRequests = 8,
    ExpressAccess = 16,
    LoanDocuments = 32
}
