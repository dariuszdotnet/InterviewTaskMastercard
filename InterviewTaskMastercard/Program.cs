using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

/// <summary>
/// Type representing a King.
/// </summary>
/// <param name="Id">King's identifier.</param>
/// <param name="Name">King's full name.</param>
/// <param name="Country">King's country.</param>
/// <param name="House">King's housse.</param>
/// <param name="Years">Years when the King has been ruling.</param>
public readonly record struct King(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("nm")] string Name,
    [property: JsonPropertyName("cty")] string Country,
    [property: JsonPropertyName("hse")] string House,
    [property: JsonPropertyName("yrs")] string Years)
{
    // regex having a number group called start, then optional dash group, then optional number group called end
    public static readonly Regex regex = new Regex(@"^(?<start>\d{3,4})(?<dash>-)?(?<end>\d{3,4})?$");
    public int StartYear => ParseStartYear(Years);
    public int EndYear => ParseEndYear(Years);
    public int Duration => EndYear - StartYear + 1;
    public string FirstName => GetFirstName(Name);

    private static int ParseStartYear(string years)
    {
        var match = regex.Match(years);

        if (match.Groups["start"].Success)
            return int.TryParse(match.Groups["start"].Value, out var start) ? start : 0;
        else
            return 0;
    }

    private static int ParseEndYear(string years)
    {
        var match = regex.Match(years);

        // gets end date
        if (match.Groups["end"].Success)
            return int.TryParse(match.Groups["end"].Value, out var end) ? end : 0;

        // or current year, if there is only dash at the end
        else if (match.Groups["dash"].Success)
            return DateTime.Now.Year;

        // or start year, if there is only one number in string
        else if (match.Groups["start"].Success)
            return int.TryParse(match.Groups["start"].Value, out var start) ? start : 0;

        // or return default value, if there is no match
        else
            return default;
    }

    private static string GetFirstName(string name)
    {
        var parts = name.Split(' ');
        return parts.Length > 0 ? parts.First() : string.Empty;
    }
}

/// <summary>
/// Service responsible for fetching, deserialization and validation of data related to the Kings.
/// </summary>
public class KingsDataProvider
{
    /// Fetches and deserializes data related to kings, from the external source.
    /// </summary>
    /// <param name="url">HTTP address pointing to the source of the data related to kings.</param>
    /// <returns><see cref="IEnumerable{King}"/> collection containing elements representing kings.</returns>
    public async Task<IEnumerable<King>> GetKingsAsync(string url)
    {
        using HttpClient client = new();
        var fetchedData = await client.GetStringAsync(url);
        var kings = JsonSerializer.Deserialize<IEnumerable<King>>(fetchedData);
        return kings == null ? new List<King>() : kings;
    }

    /// <summary>
    /// Validates data related to kings for basic correctness.
    /// </summary>
    /// <param name="kings"><see cref="IEnumerable{King}"/> collection containing elements representing kings.</param>
    /// <exception cref="ArgumentNullException">In case collection is null.</exception>
    /// <exception cref="ArgumentException">In case there is at least one wrong element in collection.</exception>
    public void ValidateKings(IEnumerable<King> kings)
    {
        if (kings == null)
            throw new ArgumentNullException("Failed to fetch kings data. Empty kings collection.");

        ValidateAssumption(kings, (IEnumerable<King> x) => x.Where(k => k.Id < 1), "Invalid IDs");
        ValidateAssumption(kings, kings => kings.GroupBy(x => x.Id).Where(x => x.Count() > 1).Select(x => x.First()), "Duplicated IDs");

        ValidateAssumption(kings, (IEnumerable<King> x) => x.Where(k => string.IsNullOrWhiteSpace(k.Name) || string.IsNullOrWhiteSpace(k.FirstName)), "Empty name");

        ValidateAssumption(kings, (IEnumerable<King> x) => x.Where(k => string.IsNullOrWhiteSpace(k.House)), "Empty house");

        ValidateAssumption(kings, (IEnumerable<King> x) => x.Where(k => k.StartYear > k.EndYear), "Wrong years");
        ValidateAssumption(kings, (IEnumerable<King> x) => x.Where(k => k.Duration < 1), "Invalid ruling duration");

        void ValidateAssumption(IEnumerable<King> kings, Func<IEnumerable<King>, IEnumerable<King>> check, string message)
        {
            var incorrectKings = check.Invoke(kings);
            if (incorrectKings.Any())
                throw new ArgumentException($"Wrong kings data. {message} for IDs: {string.Join(", ", incorrectKings.Select(x => x.Id))}");
        }
    }
}