﻿using System.Text.Json;
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

/// <summary>
/// Service responsible for providing answers to the questions related to the king's statistics.
/// </summary>
public class KingsLogic
{
    private IEnumerable<King> kings;

    /// <summary>
    /// Constructor for the <see cref="KingsLogic"/> class.
    /// </summary>
    /// <param name="kings"><see cref="IEnumerable{King}"/> collection containing elements representing kings.</param>
    public KingsLogic(IEnumerable<King> kings)
    {
        this.kings = kings;
    }

    /// <summary>
    /// Provides the answer to the question related to the number of monarchs in the list.
    /// Prints question, calculates and print the answer.
    /// </summary>
    public void KingsCount()
    {
        Console.WriteLine("Question 1: How many monarchs are there in the list?");
        Console.WriteLine($"Answer 1: There are {kings.Distinct().Count()} kings in the list.");
    }

    /// <summary>
    /// Provides the answer to the question related to the longest ruling monarch.
    /// Prints question, calculates and print the answer.
    /// </summary>
    public void LongestRulingMonarch()
    {
        Console.WriteLine("Question 2: Which monarch ruled the longest (and for how long)?");
        var longestRulingKing = kings.OrderByDescending(k => k.Duration).First();
        Console.WriteLine($"Answer 2: The monarch that rules the longest period was {longestRulingKing.Name}, who rules {longestRulingKing.Duration} years.");
    }

    /// <summary>
    /// Provides the answer to the question related to the longest ruling house.
    /// Prints question, calculates and print the answer.
    /// </summary>
    public void LongestRulingHouse()
    {
        Console.WriteLine("Question 3: Which house ruled the longest (and for how long)?");
        var longestRulingHouse = kings
            .GroupBy(k => k.House)
            .Select(g => new { House = g.Key, Duration = g.Sum(k => k.Duration) - g.Count() }) // substract g.Count() in order to not overlap the same year in continous ruling of different kings from the same house
            .OrderByDescending(g => g.Duration)
            .First();
        Console.WriteLine($"Answer 3: House that rules the longest was {longestRulingHouse.House}, who ruled {longestRulingHouse.Duration} years.");
    }

    /// <summary>
    /// Provides the answer to the question related to the most common first name.
    /// Prints question, calculates and print the answer.
    /// </summary>
    public void MostUsedFirstName()
    {
        Console.WriteLine("Question 4: What was the most common first name?");
        var mostCommonFirstName = kings
            .GroupBy(k => k.FirstName)
            .OrderByDescending(g => g.Count())
            .First()
            .Key;
        Console.WriteLine($"Answer 4: The most common first name on the list was {mostCommonFirstName}.");
    }
}