namespace Snsc.SecureShareAdmin.Security;

public sealed class AmsUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AmsUserLookup _amsUserLookup;
    private int? _cachedAmsUserId;
    private bool _resolved;

    public AmsUserContext(IHttpContextAccessor httpContextAccessor, AmsUserLookup amsUserLookup)
    {
        _httpContextAccessor = httpContextAccessor;
        _amsUserLookup = amsUserLookup;
    }

    public string WindowsUserName
    {
        get
        {
            System.Security.Claims.ClaimsPrincipal? user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true || string.IsNullOrWhiteSpace(user.Identity.Name))
            {
                throw new InvalidOperationException(
                    "Windows authentication is required. The current request is not authenticated.");
            }

            return user.Identity.Name;
        }
    }

    public int RequireAmsUserId()
    {
        if (_resolved)
        {
            return _cachedAmsUserId
                ?? throw new InvalidOperationException("AMS user id resolution failed previously for this request.");
        }

        _resolved = true;
        string windowsUserName = WindowsUserName;
        _cachedAmsUserId = _amsUserLookup.FindAmsUserIdByWindowsLogin(windowsUserName);

        if (!_cachedAmsUserId.HasValue)
        {
            throw new InvalidOperationException(
                $"No AMS user was found for Windows login '{windowsUserName}'. " +
                "Ensure the account exists in LMS ([User].NTLoginName) and SNConfig can reach db:lmssystem.");
        }

        return _cachedAmsUserId.Value;
    }
}
