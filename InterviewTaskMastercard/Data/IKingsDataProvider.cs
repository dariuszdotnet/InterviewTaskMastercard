using InterviewTaskMastercard.Models;

namespace InterviewTaskMastercard.Data
{
    /// <summary>
    /// Interface for the service responsible for fetching, deserialization and validation of data related to the Kings.
    /// </summary>
    public interface IKingsDataProvider
    {
        /// Fetches and deserializes data related to kings, from the external source.
        /// </summary>
        /// <param name="url">HTTP address pointing to the source of the data related to kings.</param>
        /// <returns><see cref="IEnumerable{King}"/> collection containing elements representing kings.</returns>
        Task<IEnumerable<King>> GetKingsAsync(string url);

        /// <summary>
        /// Validates data related to kings for basic correctness.
        /// </summary>
        /// <param name="kings"><see cref="IEnumerable{King}"/> collection containing elements representing kings.</param>
        /// <exception cref="ArgumentNullException">In case collection is null.</exception>
        /// <exception cref="ArgumentException">In case there is at least one wrong element in collection.</exception>
        void ValidateKings(IEnumerable<King> kings);
    }
}
