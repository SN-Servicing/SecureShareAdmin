using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Snsc.SecureShareAdmin.Domain;
using Snsc.SecureShareAdmin.Email;
using Snsc.SecureShareAdmin.Security;
using Snsc.SecureShareAdmin.Users;
using Snsc.SecureShareAdmin.Zones;

namespace Snsc.SecureShareAdmin.Pages.Users;

public sealed class EditModel : PageModel
{
    private readonly ExternalUserDirectory _users;
    private readonly ZoneCatalog _zoneCatalog;
    private readonly AmsUserContext _amsUser;
    private readonly NewAccountEmailSender _emailSender;

    public EditModel(
        ExternalUserDirectory users,
        ZoneCatalog zoneCatalog,
        AmsUserContext amsUser,
        NewAccountEmailSender emailSender)
    {
        _users = users;
        _zoneCatalog = zoneCatalog;
        _amsUser = amsUser;
        _emailSender = emailSender;
    }

    [BindProperty(SupportsGet = true)]
    public Guid ExternalUserId { get; set; }

    [BindProperty]
    public string FirstName { get; set; } = string.Empty;

    [BindProperty]
    public string LastName { get; set; } = string.Empty;

    [BindProperty]
    public bool LoanAccess { get; set; }

    [BindProperty]
    public bool SharedFilesAccess { get; set; }

    [BindProperty]
    public int DisabledReasonId { get; set; }

    [BindProperty]
    public string? ZoneSearchText { get; set; }

    [BindProperty]
    public List<int> SelectedZoneIds { get; set; } = new();

    public ExternalUser? ExternalUser { get; private set; }
    public IReadOnlyList<ExternalUserDisabledReason> DisabledReasons { get; private set; } =
        Array.Empty<ExternalUserDisabledReason>();
    public LoanLevelAccessValidation LoanLevelAccess { get; private set; } = new();
    public IReadOnlyList<Zone> CurrentZones { get; private set; } = Array.Empty<Zone>();
    public IReadOnlyList<Zone> FoundZones { get; private set; } = Array.Empty<Zone>();
    public string? Message { get; private set; }
    public string? ErrorMessage { get; private set; }

    public IActionResult OnGet()
    {
        if (!LoadUser())
        {
            return RedirectToPage("/Users/Index");
        }

        if (TempData["Message"] is string tempMessage)
        {
            Message = tempMessage;
        }

        return Page();
    }

    public IActionResult OnPostSave()
    {
        if (!LoadUser(bindForm: true))
        {
            return RedirectToPage("/Users/Index");
        }

        _users.Update(
            ExternalUser!.UserName,
            FirstName.Trim(),
            LastName.Trim(),
            ExternalUserAccessMask.ToPersistedValue(LoanAccess, SharedFilesAccess),
            DisabledReasonId);

        foreach (int zoneId in SelectedZoneIds.Distinct())
        {
            if (CurrentZones.Any(z => z.ZoneId == zoneId))
            {
                continue;
            }

            _zoneCatalog.AddUserPermission(zoneId, ExternalUser.UserId);
        }

        LoadZones();
        SelectedZoneIds.Clear();
        FoundZones = Array.Empty<Zone>();
        Message = "Update Successful.";
        return Page();
    }

    public IActionResult OnPostSearchZones()
    {
        if (!LoadUser())
        {
            return RedirectToPage("/Users/Index");
        }

        int amsUserId = _amsUser.RequireAmsUserId();
        IReadOnlyList<Zone> allZones = _zoneCatalog.GetZonesForAmsUser(amsUserId);
        FoundZones = ZoneSearch.FilterAndRank(allZones, null, ZoneSearchText);
        return Page();
    }

    public IActionResult OnPostRemoveZone(int zonePermissionId)
    {
        _zoneCatalog.RemoveUserPermission(zonePermissionId);
        return RedirectToPage(new { ExternalUserId });
    }

    public IActionResult OnPostSendPasswordReset()
    {
        if (!LoadUser())
        {
            return RedirectToPage("/Users/Index");
        }

        NewAccountEmailResult emailResult = _emailSender.SendPasswordResetEmail(ExternalUser!.UserName);
        if (emailResult.SkippedForEnvironment)
        {
            Message = "Password reset email was not sent for this environment.";
        }
        else if (!string.IsNullOrEmpty(emailResult.ErrorMessage))
        {
            ErrorMessage = "Password reset email could not be sent: " + emailResult.ErrorMessage;
        }
        else
        {
            Message = "Password reset email sent to " + ExternalUser.UserName + ".";
        }

        return Page();
    }

    private bool LoadUser(bool bindForm = false)
    {
        if (ExternalUserId == Guid.Empty)
        {
            return false;
        }

        ExternalUserAdminDetails? details = _users.GetDetailsForAdmin(ExternalUserId);
        if (details == null || details.User.UserId == Guid.Empty)
        {
            return false;
        }

        ExternalUser = details.User;
        DisabledReasons = details.DisabledReasons;

        if (!bindForm)
        {
            FirstName = ExternalUser.FirstName;
            LastName = ExternalUser.LastName;
            LoanAccess = ExternalUserAccessMask.HasLoanAccess(ExternalUser.AccessMask);
            SharedFilesAccess = ExternalUserAccessMask.HasSharedFilesAccess(ExternalUser.AccessMask);
            DisabledReasonId = ExternalUser.DisabledReasonId;
        }

        LoanLevelAccess = _users.ValidateAccountForLoanLevelAccess(ExternalUserId);
        LoadZones();
        return true;
    }

    private void LoadZones()
    {
        if (ExternalUser == null)
        {
            return;
        }

        CurrentZones = _zoneCatalog.GetZonesForExternalUser(ExternalUser.UserId);
    }
}
