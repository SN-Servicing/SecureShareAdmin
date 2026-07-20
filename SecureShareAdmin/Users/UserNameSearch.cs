using Snsc.SecureShareAdmin.Domain;

namespace Snsc.SecureShareAdmin.Users;

public static class UserNameSearch
{
    public static IReadOnlyList<ExternalUser> FilterAndRank(
        IEnumerable<ExternalUser> users,
        string? searchText)
    {
        string[] tokens = ParseTokens(searchText);
        if (tokens.Length == 0)
        {
            return users.ToList();
        }

        return users
            .Select(user => new { User = user, Hits = CountHits(user, tokens) })
            .Where(ranked => ranked.Hits > 0)
            .OrderByDescending(ranked => ranked.Hits)
            .ThenBy(ranked => ranked.User.Name)
            .Select(ranked => ranked.User)
            .ToList();
    }

    private static string[] ParseTokens(string? searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return Array.Empty<string>();
        }

        return searchText.ToLowerInvariant()
            .Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
    }

    private static int CountHits(ExternalUser user, string[] tokens)
    {
        int hits = 0;
        foreach (string token in tokens)
        {
            if (FieldContains(user.FirstName, token) ||
                FieldContains(user.LastName, token) ||
                FieldContains(user.Name, token))
            {
                hits++;
            }
        }

        return hits;
    }

    private static bool FieldContains(string? fieldValue, string token)
    {
        return !string.IsNullOrEmpty(fieldValue) &&
               fieldValue.Contains(token, StringComparison.OrdinalIgnoreCase);
    }
}
