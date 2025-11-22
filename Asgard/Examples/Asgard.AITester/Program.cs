using System.ClientModel;

using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

using OpenAI;


var apiAddress = Environment.GetEnvironmentVariable("OPENAIAPI");
var aiKey = Environment.GetEnvironmentVariable("OPENAIKEY");

OpenAIClient client = new OpenAIClient(new ApiKeyCredential(aiKey), new OpenAIClientOptions { Endpoint = new Uri(apiAddress) });


var chatCompletionClient = client.GetChatClient("qwen-max");
AIAgent writer = chatCompletionClient.CreateAIAgent("你是写作Agent", "Writer");
AIAgent reviewer = chatCompletionClient.CreateAIAgent("你是一个审阅者,提出改进的意见", "Reviewer");


var workflow = AgentWorkflowBuilder
    .CreateGroupChatBuilderWith(agents => new RoundRobinGroupChatManager(agents) { MaximumIterationCount = 5 })
    .AddParticipants([writer, reviewer])
    .Build();
// Start the group chat
//var messages = new List<ChatMessage> {
//    new(ChatRole.User, "我的孩子叫做刘沐阳,今年6岁,给我写一个1000字的文章,讲述她从小成长成国防军开飞机的历程,最后要输出这个1000字的文章")
//};

//StreamingRun run = await InProcessExecution.StreamAsync(workflow, messages);
//await run.TrySendMessageAsync(new TurnToken(emitEvents: true));

//await foreach (WorkflowEvent evt in run.WatchStreamAsync().ConfigureAwait(false))
//{
//    if (evt is SuperStepStartedEvent superStep)
//    {
//        Console.WriteLine($"\n--- 新一轮协作开始（SuperStep）: {superStep.StepNumber} ---");
//    }
//    else if (evt is AgentRunUpdateEvent update)
//    {
//        AgentRunResponse response = update.AsResponse();
//        foreach (ChatMessage message in response.Messages)
//        {
//            Console.Write($"{message.Text}");
//        }
//    }
//    else if (evt is WorkflowOutputEvent output)
//    {
//        var conversationHistory = output.As<List<ChatMessage>>();
//        Console.WriteLine("\n=== Final Conversation ===");
//        foreach (var message in conversationHistory)
//        {
//            Console.Write($"{message.Text}");
//        }
//        break;
//    }
//}

// Start the group chat
var messages = new List<ChatMessage> {
    new(ChatRole.User, "我的孩子叫做刘沐阳,今年6岁,给我写一个1000字的文章,讲述她从小成长成国防军开飞机的历程,最后要输出这个1000字的文章")//Create a slogan for an eco-friendly electric vehicle.")
};

StreamingRun run = await InProcessExecution.StreamAsync(workflow, messages);
await run.TrySendMessageAsync(new TurnToken(emitEvents: true));

await foreach (WorkflowEvent evt in run.WatchStreamAsync().ConfigureAwait(false))
{
    if (evt is AgentRunUpdateEvent update)
    {
        // Process streaming agent responses
        AgentRunResponse response = update.AsResponse();
        foreach (ChatMessage message in response.Messages)
        {
            Console.WriteLine($"[{update.ExecutorId}]: {message.Text}");
        }
    }
    else if (evt is WorkflowOutputEvent output)
    {
        // Workflow completed
        var conversationHistory = output.As<List<ChatMessage>>();
        Console.WriteLine("\n=== Final Conversation ===");
        foreach (var message in conversationHistory)
        {
            Console.WriteLine($"{message.AuthorName}: {message.Text}");
        }
        break;
    }
}
/*
 除了 AgentRunUpdateEvent 和 WorkflowOutputEvent，MAF 工作流还可能包含其他事件类型，常见的有：

SuperStepStartedEvent：标记一轮协作（超级步骤）开始。
AgentRunStartedEvent：某个 agent 开始执行任务。
AgentRunCompletedEvent：某个 agent 完成任务。
WorkflowErrorEvent：工作流执行过程中发生错误。
WorkflowCanceledEvent：工作流被取消。
其他自定义事件（如工具调用、外部资源交互等）。
这些事件用于细粒度追踪和管理多 agent 协作流程，便于开发者实现更复杂的业务逻辑和监控。

具体事件类型可参考 Microsoft.Agents.AI.Workflows 事件体系 或相关 API 文档。
 */
