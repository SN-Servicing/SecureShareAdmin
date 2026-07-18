using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Snsc.SecureShareAdmin.Domain;
using Snsc.SecureShareAdmin.Security;
using Snsc.SecureShareAdmin.Zones;

namespace Snsc.SecureShareAdmin.Pages.Options;

public sealed class IndexModel : PageModel
{
    private readonly AmsUserContext _amsUser;
    private readonly ZoneCatalog _zoneCatalog;

    public IndexModel(AmsUserContext amsUser, ZoneCatalog zoneCatalog)
    {
        _amsUser = amsUser;
        _zoneCatalog = zoneCatalog;
    }

    public IReadOnlyList<Zone> Zones { get; private set; } = Array.Empty<Zone>();
    public string? ErrorMessage { get; private set; }
    public string? Message { get; private set; }

    public void OnGet()
    {
        LoadZones();
    }

    public IActionResult OnPostToggleNotify(int zoneId, bool currentlyOptedIn)
    {
        int amsUserId = _amsUser.RequireAmsUserId();
        _zoneCatalog.SetAmsEmailNotification(zoneId, !currentlyOptedIn, amsUserId);
        Message = "Notification preference updated.";
        LoadZones();
        return Page();
    }

    private void LoadZones()
    {
        int amsUserId = _amsUser.RequireAmsUserId();
        Zones = _zoneCatalog.GetZonesForAmsUser(amsUserId)
            .OrderBy(z => z.Description)
            .ToList();
    }
}
