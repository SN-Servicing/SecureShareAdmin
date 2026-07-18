using Snsc.SecureShareAdmin.Domain;

namespace Snsc.SecureShareAdmin.Zones;

public static class ZoneSearch
{
    public static IReadOnlyList<Zone> FilterAndRank(
        IEnumerable<Zone> zones,
        int? zoneTypeId,
        string? searchText)
    {
        string[] tokens = ParseTokens(searchText);
        if (tokens.Length == 0)
        {
            return Array.Empty<Zone>();
        }

        IEnumerable<Zone> filtered = zones;
        if (zoneTypeId.HasValue && zoneTypeId.Value > 0)
        {
            filtered = filtered.Where(zone => zone.ZoneTypeId == zoneTypeId.Value);
        }

        return filtered
            .Select(zone => new { Zone = zone, Hits = CountHits(zone, tokens) })
            .Where(ranked => ranked.Hits > 0)
            .OrderByDescending(ranked => ranked.Hits)
            .ThenBy(ranked => ranked.Zone.Description)
            .Select(ranked => ranked.Zone)
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

    private static int CountHits(Zone zone, string[] tokens)
    {
        int hits = 0;
        foreach (string token in tokens)
        {
            if (FieldContains(zone.Description, token) ||
                FieldContains(zone.PrimaryIdValue, token) ||
                FieldContains(zone.SecondaryIdValue, token))
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
