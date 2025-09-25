using asynchronousRequestReply.Messaging;
using asynchronousRequestReply.Services;

Console.WriteLine("=== Patrón Asynchronous Request-Reply (Ejemplo .NET 8) ===\n");

await using var bus = new AsyncRequestReplyBus();

var responder = new UppercaseResponder(bus);
var responderTask = Task.Run(() => responder.RunAsync(), CancellationToken.None);

string[] payloads =
[
	"solicitud 1: reporte diario",
	"solicitud 2: métricas",
	"solicitud 3: auditoría"
];

var clientTasks = payloads.Select(async payload =>
{
	Console.WriteLine($"Cliente -> Enviando: {payload}");
	var response = await bus.SendRequestAsync(payload);
	var status = response.IsSuccessful ? "OK" : "Error";
	Console.WriteLine($"Cliente <- ({status}) CorrelationId {response.CorrelationId}: {response.Payload}");
});

await Task.WhenAll(clientTasks);

bus.StopAcceptingRequests();
await responderTask.ConfigureAwait(false);

Console.WriteLine("\nTodas las solicitudes se procesaron de manera asíncrona.");
