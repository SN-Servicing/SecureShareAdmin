namespace Snsc.SecureShareAdmin.Domain;

public static class ExternalUserAccessMask
{
    public const ExternalUserAccess LoanGroup =
        ExternalUserAccess.LoanAccess | ExternalUserAccess.LoanDocuments;

    public const ExternalUserAccess SharedFilesGroup =
        ExternalUserAccess.SdiUpload | ExternalUserAccess.SdiDownload;

    public static int ToPersistedValue(bool loanAccess, bool sharedFilesAccess)
    {
        ExternalUserAccess mask = ExternalUserAccess.None;
        if (loanAccess)
        {
            mask |= LoanGroup;
        }

        if (sharedFilesAccess)
        {
            mask |= SharedFilesGroup;
        }

        return (int)mask;
    }

    public static bool HasLoanAccess(int mask)
    {
        return ((ExternalUserAccess)mask & ExternalUserAccess.LoanAccess) == ExternalUserAccess.LoanAccess;
    }

    public static bool HasSharedFilesAccess(int mask)
    {
        return ((ExternalUserAccess)mask & ExternalUserAccess.SdiUpload) == ExternalUserAccess.SdiUpload;
    }
}
