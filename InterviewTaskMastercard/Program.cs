using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using InterviewTaskMastercard.Models;
using InterviewTaskMastercard.Logic;
using InterviewTaskMastercard.Data;

string kingsSourceUrl = "https://gist.githubusercontent.com/christianpanton/10d65ccef9f29de3acd49d97ed423736/raw/b09563bc0c4b318132c7a738e679d4f984ef0048/kings";

IHost host = DISetup(args);

try
{
    IEnumerable<King> kings = await GetData(host, kingsSourceUrl);

    var logic = host.Services.GetRequiredService<IKingsLogic>()
        .WithData(kings);

    await ExecuteQuestionLogic(logic);

    WaitToExit();
}
catch (Exception ex)
{
    Console.WriteLine($"An error was caught by global handler. Message: {ex.Message}");
}

static IHost DISetup(string[] args)
{
    return Host.CreateDefaultBuilder(args)
        .ConfigureServices((_, services) =>
        {
            services.AddSingleton<IKingsDataProvider, KingsDataProvider>();
            services.AddSingleton<IKingsLogic, KingsLogic>();
        })
        .Build();
}

static async Task<IEnumerable<King>> GetData(IHost host, string kingsSourceUrl)
{
    var data = host.Services.GetRequiredService<IKingsDataProvider>();
    var kings = await data.GetKingsAsync(kingsSourceUrl);
    data.ValidateKings(kings);
    return kings;
}

static async Task ExecuteQuestionLogic(IKingsLogic logic)
{
    var tasks = new List<Task<QuestionAnswer>>
    {
        logic.KingsCount(),
        logic.LongestRulingMonarch(),
        logic.LongestRulingHouse(),
        logic.MostUsedFirstName()
    };

    while (tasks.Any())
    {
        var finishedTask = await Task.WhenAny(tasks);
        tasks.Remove(finishedTask);
        var result = await finishedTask;

        Console.WriteLine(result.Question);
        Console.WriteLine(result.Answer);
        Console.WriteLine();
    }
}

static void WaitToExit()
{
    Console.WriteLine("Press ENTER to exit.");
    Console.ReadLine();
}