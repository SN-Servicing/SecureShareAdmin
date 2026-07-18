namespace Snsc.Foundation.Enumerations
{
    public enum SdiLogSource
    {
        Unknown = 0,
        SdiWebSite = 1,
        SdiInternalAdmin = 2,
        AmsDocumentumExport = 3,
        AmsGridExport = 4,
        DocumentExtractor = 5
    }

    public enum SdiLogSubType
    {
        UserCreation = 1,
        UserEdit = 2,
        UserDelete = 3,
        FileUpload = 4,
        FileDownload = 5,
        FileDelete = 6,
        LoginSuccess = 7,
        LoginFailure = 8,
        ZoneAccessAdded = 9,
        ZoneAccessRemoved = 10,
        PasswordReset = 11,
        ZoneAdded = 12,
        ZoneDeleted = 13,
        NotificationOptIn = 14,
        NotificationOptOut = 15,
        CreateUserForExport = 16,
        GridExportedToSDI = 17,
        DocumentExportedToSDI = 18,
        AMSUserZoneAdded = 19
    }
}
