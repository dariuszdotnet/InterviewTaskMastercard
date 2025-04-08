namespace InterviewTaskMastercard.Models
{
    /// <summary>
    /// Type representing a question and its answer.
    /// </summary>
    /// <param name="Question">Predefined question.</param>
    public record QuestionAnswer(string Question)
    {
        public string? Answer { get; set; }
    }

    //public interface IQuestion<T>
    //{
    //    string Question { get; }
    //    Func<IEnumerable<King>, T> Logic { get; }
    //    T Result { get; set; }
    //    string? Answer { get; set; }
    //}

    //public class QuestionAnswer<T> : IQuestion<T>
    //{
    //    public string Question { get; }
    //    public Func<IEnumerable<King>, T> Logic { get; }
    //    public T Result { get; set; }
    //    public string? Answer { get; set; }
    //
    //    public QuestionAnswer(string question, Func<IEnumerable<King>, T> logic)
    //    {
    //        Question = question;
    //        Logic = logic;
    //    }
    //}
}
