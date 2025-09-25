# Circuit Breaker con Retry en estado Semi-Abierto

Ejemplo en **.NET 8** que ilustra cómo un *Circuit Breaker* pasa por los estados
*Cerrado → Abierto → Semi-Abierto* y, durante el estado semi-abierto, aplica un
patrón de *Retry* para comprobar si el servicio externo ya se recuperó.

## Estructura

```text
retry/
├── Program.cs
├── Resilience/
│   ├── CircuitBreaker.cs
│   ├── CircuitBreakerOptions.cs
│   ├── CircuitBreakerState.cs
│   ├── CircuitOpenException.cs
│   └── RetryPolicy.cs
└── Services/
    └── UnreliableExternalService.cs
```

## Cómo ejecutar

```powershell
Set-Location c:/Users/jc_qu/Repos/patronesdearquitectura
dotnet run --project retry/retry.csproj
```

## Qué observar

- Tras varios fallos consecutivos el circuito se abre y deja de llamar al
  servicio, informando cuánto esperar antes de reintentar.
- Cuando el temporizador expira, el circuito entra a *Semi-Abierto* y utiliza la
  política de reintentos con backoff lineal para «tantear» el servicio.
- Si alguno de los intentos tiene éxito, el circuito vuelve a *Cerrado* y se
  restablece el contador de fallos.
- Si todos los intentos fallan, se vuelve a abrir, extendiendo la protección.
