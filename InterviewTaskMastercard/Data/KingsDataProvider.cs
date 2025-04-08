using System.Text.Json;
using InterviewTaskMastercard.Models;

namespace InterviewTaskMastercard.Data
{
    /// <summary>
    /// Service responsible for fetching, deserialization and validation of data related to the Kings.
    /// </summary>
    public class KingsDataProvider : IKingsDataProvider
    {
        /// <inheritdoc/>
        public async Task<IEnumerable<King>> GetKingsAsync(string url)
        {
            using var httpClient = new HttpClient();

            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            //content = @"[
            //  {
            //    ""id"": 1,
            //    ""nm"": ""Edward the Elder"",
            //    ""cty"": ""United Kingdom"",
            //    ""hse"": ""House of Wessex"",
            //    ""yrs"": ""899-925""
            //  },
            //  {
            //    ""id"": 2,
            //    ""nm"": ""Athelstan"",
            //    ""Cty"": ""United Kingdom"",
            //    ""hse"": ""House of Wessex"",
            //    ""yrs"": ""925-940""
            //  },
            //]";

            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
            };
            var kings = JsonSerializer.Deserialize<IEnumerable<King>>(content, options)!;
            return kings == null ? new List<King>() : kings;
        }

        /// <inheritdoc/>
        public void ValidateKings(IEnumerable<King> kings)
        {
            if (kings == null)
                throw new ArgumentNullException("Failed to fetch kings data. Empty kings collection.");

            ValidateAssumption(kings, (x) => x.Where(k => k.Id < 1), "Invalid IDs");
            ValidateAssumption(kings, kings => kings.GroupBy(x => x.Id).Where(x => x.Count() > 1).Select(x => x.First()), "Duplicated IDs");

            ValidateAssumption(kings, (x) => x.Where(k => string.IsNullOrWhiteSpace(k.Name) || string.IsNullOrWhiteSpace(k.FirstName)), "Empty name");

            ValidateAssumption(kings, (x) => x.Where(k => string.IsNullOrWhiteSpace(k.House)), "Empty house");

            ValidateAssumption(kings, (x) => x.Where(k => k.StartYear > k.EndYear), "Wrong years");
            ValidateAssumption(kings, (x) => x.Where(k => k.Duration < 1), "Invalid ruling duration");

            void ValidateAssumption(IEnumerable<King> kings, Func<IEnumerable<King>, IEnumerable<King>> check, string message)
            {
                var incorrectKings = check.Invoke(kings);
                if (incorrectKings.Any())
                    throw new ArgumentException($"Wrong kings data. {message} for IDs: {string.Join(", ", incorrectKings.Select(x => x.Id))}");
            }
        }
    }
}
