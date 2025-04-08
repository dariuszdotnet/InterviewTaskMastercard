using InterviewTaskMastercard.Logic;
using InterviewTaskMastercard.Models;

namespace InterviewTaskMastercard.Tests
{
    public class KingsLogicTests
    {
        private static List<King> GetMockKings() => new()
            {
            new King(1, "Richard the Braveheart", "England", "Plantagenet", "1010-1050"),
            new King(2, "Phill the Wise", "Wales", "Capet", "1050-1060"),
            new King(3, "William the Conqueror", "England", "Normandy", "1060-1080"),
            new King(4, "Henry the Strong", "England", "Plantagenet", "1080-1100"),
            new King(5, "Richard the Gallant", "Scotland", "Stuart", "1100-1130"),
            new King(6, "Henry the Great", "Wales", "Capet", "1130-1150"),
            new King(7, "Edward the Mighty", "England", "Tudor", "1150-1175")
        };

        [Fact]
        public async Task KingsCount_ReturnsCorrectCount()
        {
            var logic = new KingsLogic().WithData(GetMockKings());

            var result = await logic.KingsCount();

            Assert.Contains("7", result.Answer);
        }

        [Fact]
        public async Task MostUsedFirstName_ReturnsMostFrequentName()
        {
            var logic = new KingsLogic().WithData(GetMockKings());

            var result = await logic.MostUsedFirstName();

            Assert.Contains("Richard.", result.Answer);
        }

        [Fact]
        public async Task LongestRulingMonarch_ReturnsCorrectKing()
        {
            var logic = new KingsLogic().WithData(GetMockKings());

            var result = await logic.LongestRulingMonarch();

            Assert.Contains("Richard the Braveheart", result.Answer);
            Assert.Contains("41 years", result.Answer);
        }

        [Fact]
        public async Task LongestRulingHouse_ReturnsCorrectHouse()
        {
            var logic = new KingsLogic().WithData(GetMockKings());

            var result = await logic.LongestRulingHouse();

            Assert.Contains("Plantagenet", result.Answer);
            Assert.Contains("60", result.Answer);
        }
    }
}