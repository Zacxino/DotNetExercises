using System.ClientModel;
using Microsoft.Extensions.AI;

OllamaChatClient chatClient = new OllamaChatClient("http://localhost:11434", "deepseek-r1:1.5b");
OpenAIChatClient openAIChatClient = new OpenAIChatClient(
    new OpenAI.OpenAIClient(new ApiKeyCredential("sk-xx"),
new OpenAI.OpenAIClientOptions() { Endpoint = new Uri("https://api.deepseek.com") }), "deepseek-chat");


var chatMessages = new List<ChatMessage>
{
    new ChatMessage(ChatRole.User, "Hello, Ollama!"),
    new ChatMessage(ChatRole.User, "1 + 1 = ?")
};

var reply = await openAIChatClient.CompleteAsync(chatMessages, new ChatOptions() { });
var reply1 = await chatClient.CompleteAsync(chatMessages, new ChatOptions() { });

System.Console.WriteLine(reply.Message);