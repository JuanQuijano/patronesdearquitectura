# Asynchronous Request-Reply

Este proyecto demuestra un ejemplo mínimo del patrón **Asynchronous Request-Reply** implementado con .NET 8.

## Ejecución

```pwsh
cd asynchronousRequestReply
dotnet run
```

## ¿Qué muestra el ejemplo?

- Un `AsyncRequestReplyBus` que actúa como canal desacoplado entre el cliente y el servicio.
- Solicitudes que se envían de manera asíncrona y producen una respuesta correlacionada mediante un `Guid`.
- Un servicio de ejemplo (`UppercaseResponder`) que procesa las peticiones con latencia variable para simular trabajo en segundo plano.
