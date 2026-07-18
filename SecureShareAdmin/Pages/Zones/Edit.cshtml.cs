using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Snsc.SecureShareAdmin.Domain;
using Snsc.SecureShareAdmin.Security;
using Snsc.SecureShareAdmin.Users;
using Snsc.SecureShareAdmin.Zones;

namespace Snsc.SecureShareAdmin.Pages.Zones;

public sealed class EditModel : PageModel
{
    private readonly AmsUserContext _amsUser;
    private readonly ZoneCatalog _zoneCatalog;
    private readonly ExternalUserDirectory _users;

    public EditModel(AmsUserContext amsUser, ZoneCatalog zoneCatalog, ExternalUserDirectory users)
    {
        _amsUser = amsUser;
        _zoneCatalog = zoneCatalog;
        _users = users;
    }

    [BindProperty(SupportsGet = true)]
    public int ZoneId { get; set; }

    [BindProperty]
    public int ZoneTypeId { get; set; }

    [BindProperty]
    public string Description { get; set; } = string.Empty;

    [BindProperty]
    public string PrimaryValue { get; set; } = string.Empty;

    [BindProperty]
    public string? SecondaryValue { get; set; }

    [BindProperty]
    public string? UserSearch { get; set; }

    [BindProperty]
    public List<string> SelectedUserNames { get; set; } = new();

    public List<SelectListItem> ZoneTypeOptions { get; private set; } = new();
    public IReadOnlyList<ZoneUser> ZoneUsers { get; private set; } = Array.Empty<ZoneUser>();
    public IReadOnlyList<ExternalUser> FoundUsers { get; private set; } = Array.Empty<ExternalUser>();
    public ZoneType? SelectedType { get; private set; }
    public string? Message { get; private set; }
    public string? ErrorMessage { get; private set; }

    public IActionResult OnGet(int? saved)
    {
        if (!LoadZone())
        {
            return RedirectToPage("/Zones/Index");
        }

        if (saved == 1)
        {
            Message = "Zone saved";
        }

        return Page();
    }

    public IActionResult OnPostSearchUsers()
    {
        if (!LoadZone(bindForm: true))
        {
            return RedirectToPage("/Zones/Index");
        }

        if (!string.IsNullOrWhiteSpace(UserSearch))
        {
            FoundUsers = _users.FindByUserName("%" + UserSearch.Trim() + "%");
        }

        return Page();
    }

    public IActionResult OnPostSave()
    {
        if (!LoadZone(bindForm: true))
        {
            return RedirectToPage("/Zones/Index");
        }

        int updateResult = _zoneCatalog.UpdateZone(
            ZoneId,
            ZoneTypeId,
            Description.Trim(),
            PrimaryValue.Trim(),
            SecondaryValue?.Trim() ?? string.Empty);

        if (updateResult == -1)
        {
            ErrorMessage = "Update failed due to a unique constraint conflict.";
            return Page();
        }

        HashSet<string> existing = ZoneUsers.Select(u => u.UserName).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (string userName in SelectedUserNames.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (existing.Contains(userName))
            {
                continue;
            }

            ExternalUser? user = _users.GetByUserName(userName);
            if (user != null && user.UserId != Guid.Empty)
            {
                _zoneCatalog.AddUserPermission(ZoneId, user.UserId);
            }
        }

        LoadZoneUsers();
        SelectedUserNames.Clear();
        FoundUsers = Array.Empty<ExternalUser>();
        Message = "Zone saved";
        return Page();
    }

    public IActionResult OnPostRemoveUser(int zonePermissionId)
    {
        _zoneCatalog.RemoveUserPermission(zonePermissionId);
        return RedirectToPage(new { ZoneId });
    }

    public IActionResult OnPostDeleteZone()
    {
        _zoneCatalog.DeleteZone(ZoneId);
        return RedirectToPage("/Zones/Index");
    }

    private bool LoadZone(bool bindForm = false)
    {
        int amsUserId = _amsUser.RequireAmsUserId();
        IReadOnlyList<Zone> allZones = _zoneCatalog.GetZonesForAmsUser(amsUserId);
        Zone? zone = allZones.FirstOrDefault(z => z.ZoneId == ZoneId);
        if (zone == null)
        {
            return false;
        }

        IReadOnlyList<ZoneType> types = _zoneCatalog.GetDistinctZoneTypes(allZones);
        ZoneTypeOptions = types.Select(t => new SelectListItem(t.Name, t.ZoneTypeId.ToString())).ToList();

        if (!bindForm)
        {
            ZoneTypeId = zone.ZoneTypeId;
            Description = zone.Description;
            PrimaryValue = zone.PrimaryIdValue;
            SecondaryValue = zone.SecondaryIdValue;
        }

        SelectedType = types.FirstOrDefault(t => t.ZoneTypeId == ZoneTypeId) ?? types.FirstOrDefault();
        LoadZoneUsers();
        return true;
    }

    private void LoadZoneUsers()
    {
        ZoneUsers = _zoneCatalog.GetUsersForZone(ZoneId);
    }
}
