using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Snsc.SecureShareAdmin.Domain;
using Snsc.SecureShareAdmin.Security;
using Snsc.SecureShareAdmin.Zones;

namespace Snsc.SecureShareAdmin.Pages.Zones;

public sealed class FilesModel : PageModel
{
    private readonly AmsUserContext _amsUser;
    private readonly ZoneCatalog _zoneCatalog;
    private readonly ZoneFileCatalog _fileCatalog;

    public FilesModel(AmsUserContext amsUser, ZoneCatalog zoneCatalog, ZoneFileCatalog fileCatalog)
    {
        _amsUser = amsUser;
        _zoneCatalog = zoneCatalog;
        _fileCatalog = fileCatalog;
    }

    [BindProperty(SupportsGet = true)]
    public int ZoneId { get; set; }

    public string ZoneDescription { get; private set; } = string.Empty;
    public IReadOnlyList<SharedFile> Files { get; private set; } = Array.Empty<SharedFile>();

    public IActionResult OnGet()
    {
        if (!Load())
        {
            return RedirectToPage("/Zones/Index");
        }

        return Page();
    }

    public IActionResult OnPostDelete(int sharedFileId)
    {
        _fileCatalog.DeleteFile(sharedFileId);
        return RedirectToPage(new { ZoneId });
    }

    private bool Load()
    {
        int amsUserId = _amsUser.RequireAmsUserId();
        Zone? zone = _zoneCatalog.GetZone(amsUserId, ZoneId);
        if (zone == null)
        {
            return false;
        }

        ZoneDescription = zone.Description;
        Files = _fileCatalog.GetFilesByZoneId(ZoneId);
        return true;
    }
}
