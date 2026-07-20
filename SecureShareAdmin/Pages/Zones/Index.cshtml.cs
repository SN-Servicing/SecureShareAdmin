using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Snsc.SecureShareAdmin.Domain;
using Snsc.SecureShareAdmin.Security;
using Snsc.SecureShareAdmin.Zones;

namespace Snsc.SecureShareAdmin.Pages.Zones;

public sealed class IndexModel : PageModel
{
    private readonly AmsUserContext _amsUser;
    private readonly ZoneCatalog _zoneCatalog;

    public IndexModel(AmsUserContext amsUser, ZoneCatalog zoneCatalog)
    {
        _amsUser = amsUser;
        _zoneCatalog = zoneCatalog;
    }

    [BindProperty(SupportsGet = true)]
    public int ZoneTypeId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    public IReadOnlyList<Zone> Zones { get; private set; } = Array.Empty<Zone>();
    public List<SelectListItem> ZoneTypeOptions { get; private set; } = new();
    public string EmptyMessage { get; private set; } = "Enter search terms to find zones. Separate multiple terms with a space.";
    public string? ErrorMessage { get; private set; }

    public void OnGet()
    {
        LoadPage();
    }

    public void OnPostSearch()
    {
        LoadPage();
    }

    public IActionResult OnPostToggleNotify(int zoneId, bool currentlyOptedIn)
    {
        int amsUserId = _amsUser.RequireAmsUserId();
        if (!_zoneCatalog.CanUserAdministerZone(amsUserId, zoneId))
        {
            return RedirectToPage(new { ZoneTypeId, Search });
        }

        _zoneCatalog.SetAmsEmailNotification(zoneId, !currentlyOptedIn, amsUserId);
        return RedirectToPage(new { ZoneTypeId, Search });
    }

    private void LoadPage()
    {
        int amsUserId = _amsUser.RequireAmsUserId();
        IReadOnlyList<Zone> allZones = _zoneCatalog.GetZonesForAmsUser(amsUserId);
        IReadOnlyList<ZoneType> types = _zoneCatalog.GetDistinctZoneTypes(allZones);

        ZoneTypeOptions = new List<SelectListItem>
        {
            new("*** ALL ***", "0")
        };
        ZoneTypeOptions.AddRange(types.Select(t => new SelectListItem(t.Name, t.ZoneTypeId.ToString())));

        Zones = ZoneSearch.FilterAndRank(allZones, ZoneTypeId, Search);
        if (!string.IsNullOrWhiteSpace(Search) && Zones.Count == 0)
        {
            EmptyMessage = "No zones found";
        }
    }
}
