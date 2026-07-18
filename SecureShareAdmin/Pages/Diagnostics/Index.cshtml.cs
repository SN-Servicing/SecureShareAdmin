using Microsoft.AspNetCore.Mvc.RazorPages;
using Snsc.SecureShareAdmin.Security;
using Snsc.SecureShareAdmin.Zones;

namespace Snsc.SecureShareAdmin.Pages.Diagnostics;

public sealed class IndexModel : PageModel
{
    private readonly AmsUserContext _amsUser;
    private readonly ZoneCatalog _zoneCatalog;

    public IndexModel(AmsUserContext amsUser, ZoneCatalog zoneCatalog)
    {
        _amsUser = amsUser;
        _zoneCatalog = zoneCatalog;
    }

    public string WindowsUserName { get; private set; } = string.Empty;
    public int AmsUserId { get; private set; }
    public int ZoneCount { get; private set; }
    public int ZoneTypeCount { get; private set; }

    public void OnGet()
    {
        WindowsUserName = _amsUser.WindowsUserName;
        AmsUserId = _amsUser.RequireAmsUserId();
        var zones = _zoneCatalog.GetZonesForAmsUser(AmsUserId);
        ZoneCount = zones.Count;
        ZoneTypeCount = _zoneCatalog.GetDistinctZoneTypes(zones).Count;
    }
}
