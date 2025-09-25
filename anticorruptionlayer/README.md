# Capa Anti-Corrupción (Anti Corruption Layer)

Este ejemplo sencillo en **.NET 8** muestra cómo una capa anti-corrupción (ACL) protege
al dominio de los detalles de un sistema legado. El dominio depende de un contrato
propio (`ICustomerGateway`) y una implementación (`LegacyCustomerAdapter`) traduce
los datos provenientes del sistema legado.

## Estructura

```text
anticorruptionlayer/
├── Acl/
│   └── LegacyCustomerAdapter.cs
├── Domain/
│   ├── CustomerOnboardingService.cs
│   ├── CustomerProfile.cs
│   ├── CustomerSnapshot.cs
│   └── ICustomerGateway.cs
├── Legacy/
│   ├── LegacyCrmService.cs
│   └── Models/
│       └── LegacyCustomerRecord.cs
├── Program.cs
└── README.md
```

## Cómo ejecutar

Dentro de la carpeta raíz del repositorio ejecuta:

```powershell
Set-Location c:/Users/jc_qu/Repos/patronesdearquitectura
dotnet run --project anticorruptionlayer/anticorruptionlayer.csproj
```

## ¿Qué observar?

- El dominio (`CustomerOnboardingService`) trabaja con objetos propios (`CustomerProfile`)
  sin conocer el formato legado.
- La ACL (`LegacyCustomerAdapter`) traduce el `LegacyCustomerRecord` al `CustomerSnapshot`
  que espera el dominio, encapsulando reglas de mapeo.
- Al ejecutar verás clientes traducidos y un ejemplo de cómo se maneja un identificador
  inexistente sin propagar detalles del sistema legacy.
