using InterviewTaskMastercard.Models;

namespace InterviewTaskMastercard.Logic
{
    /// <summary>
    /// Interface for the service responsible for providing answers to the questions related to the king's statistics.
    /// </summary>
    public interface IKingsLogic
    {
        /// <summary>
        /// Initializes the service with data related to kings.
        /// </summary>
        /// <param name="kings">The collection that should be allocated in the current object.</param>
        /// <returns>Itself with the defined collection from parameter.</returns>
        IKingsLogic WithData(IEnumerable<King> kings);

        /// <summary>
        /// Provides the answer to the question related to the number of monarchs in the list.
        /// </summary>
        /// <returns><see cref="QuestionAnswer"/> object containing the question and the answer.</returns>
        Task<QuestionAnswer> KingsCount();

        /// <summary>
        /// Provides the answer to the question related to the longest ruling monarch.
        /// </summary>
        /// <returns><see cref="QuestionAnswer"/> object containing the question and the answer.</returns>
        Task<QuestionAnswer> LongestRulingMonarch();

        /// <summary>
        /// Provides the answer to the question related to the longest ruling house.
        /// </summary>
        /// <returns><see cref="QuestionAnswer"/> object containing the question and the answer.</returns>
        Task<QuestionAnswer> LongestRulingHouse();

        /// <summary>
        /// Provides the answer to the question related to the most common first name.
        /// </summary>
        /// <returns><see cref="QuestionAnswer"/> object containing the question and the answer.</returns>
        Task<QuestionAnswer> MostUsedFirstName();
    }
}
