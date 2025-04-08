using InterviewTaskMastercard.Models;

namespace InterviewTaskMastercard.Logic
{
    /// <summary>
    /// Service responsible for providing answers to the questions related to the king's statistics.
    /// </summary>
    public class KingsLogic : IKingsLogic
    {
        private IEnumerable<King>? _kings;

        /// <inheritdoc/>
        public IKingsLogic WithData(IEnumerable<King> kings)
        {
            _kings = kings;
            return this;
        }

        /// <inheritdoc/>
        public async Task<QuestionAnswer> KingsCount()
        {
            await Task.Delay(500); // simulation

            var question = new QuestionAnswer("How many monarchs are there in the list?");
            var count = _kings!.Distinct().Count();
            question.Answer = $"There are {count} kings in the list.";
            return question;
        }

        /// <inheritdoc/>
        public async Task<QuestionAnswer> LongestRulingMonarch()
        {
            //await Task.Delay(1000); // simulation

            var question = new QuestionAnswer("Which monarch ruled the longest (and for how long)?");
            var longest = _kings!.OrderByDescending(k => k.Duration).First();
            question.Answer = $"The monarch who rules the longest period was {longest.Name}, who ruled {longest.Duration} years.";
            return question;
        }

        /// <inheritdoc/>
        public async Task<QuestionAnswer> LongestRulingHouse()
        {
            var question = new QuestionAnswer("Which house ruled the longest (and for how long)?");

            //var house = _kings!
            //    .GroupBy(k => k.House)
            //    .Select(g => new { House = g.Key, Duration = g.Sum(k => k.Duration) - g.Count() }) // substract g.Count() in order to not overlap the same year in continous ruling of different kings from the same house
            //    .OrderByDescending(g => g.Duration)
            //    .First();

            var house = _kings!
                .GroupBy(k => k.House)
                .Aggregate( // avoids sorting, just keeps track of the maximum group directly — O(n) instead of O(n log n)
                    new { House = "", Duration = 0 },
                    (max, group) =>
                    {
                        var duration = group.Sum(k => k.Duration) - group.Count(); // substract g.Count() in order to not overlap the same year in continous ruling of different kings from the same house
                        return duration > max.Duration
                            ? new { House = group.Key, Duration = duration }
                            : max;
                    });

            question.Answer = $"House that rules the longest was {house.House}, who ruled {house.Duration} years.";
            return question;
        }

        /// <inheritdoc/>
        public async Task<QuestionAnswer> MostUsedFirstName()
        {
            var question = new QuestionAnswer("What was the most common first name?");
            var name = _kings!
                .GroupBy(k => k.FirstName)
                .OrderByDescending(g => g.Count())
                .First()
                .Key;
            question.Answer = $"The most common first name on the list was {name}.";
            return question;
        }
    }
}
