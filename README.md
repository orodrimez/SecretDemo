# Azure Function segura para consumir APIs externas (Key Vault + Managed Identity)

Este proyecto implementa una **Azure Function en .NET 8 (isolated worker)** que consume una **API externa** de forma segura, almacenando credenciales en **Azure Key Vault** y usando **Managed Identity** para autenticación sin secretos.

El objetivo es demostrar una **arquitectura correcta en Azure**, aplicable a escenarios reales de producción, evitando los errores comunes de manejo de secretos y seguridad.

---

## 🎯 Objetivo del proyecto

- Consumir una API externa protegida por API Key / Token
- No exponer credenciales al frontend
- No hardcodear secretos
- Usar identidad administrada en lugar de client secrets
- Mantener el mismo código para local y cloud

---

## 🧱 Arquitectura

Cliente
|
v
Azure Function (HTTP Trigger)
|
|-- Managed Identity
v
Azure Key Vault
|
v
API Externa


### Componentes

- **Azure Function (.NET 8 – Isolated)**
  - Backend serverless
  - Punto de entrada HTTP
  - Orquestador de llamadas externas

- **Azure Key Vault**
  - Almacenamiento seguro de secretos
  - Control de acceso vía RBAC

- **Managed Identity**
  - Autenticación automática sin credenciales
  - Eliminación de client secrets y certificados

- **API Externa**
  - Servicio de terceros (Weather, Payments, Maps, etc.)

Todo corre sobre **:contentReference[oaicite:0]{index=0}**.

---

## 🔐 Seguridad (puntos clave)

- ✅ No hay secretos en el código
- ✅ No hay secretos en `appsettings.json`
- ✅ El cliente nunca ve la API Key
- ✅ Acceso a Key Vault mediante RBAC
- ✅ Compatible con rotación de secretos sin redeploy
- ❌ No se usan App Registrations innecesarias
- ❌ No se usan client secrets

---

## 🔁 Flujo de ejecución

1. El cliente llama a la Azure Function.
2. La Function se autentica usando Managed Identity.
3. La Function lee la API Key desde Azure Key Vault.
4. La Function llama a la API externa usando esa API Key.
5. La respuesta se valida / transforma.
6. El resultado se devuelve al cliente.

La Function actúa como:
- Proxy seguro
- Adaptador de API
- Orquestador (si se conectan múltiples APIs)

---

## 🛠️ Tecnologías usadas

- .NET 8 (Isolated Worker)
- Azure Functions v4
- Azure Key Vault
- Azure Managed Identity
- Azure RBAC
- Azure CLI
- Azure Functions Core Tools

---

## 📁 Estructura del proyecto

SecretDemo/
│
├── GetSecret.cs # Azure Function HTTP
├── Program.cs # Bootstrap del worker
├── host.json
├── local.settings.json # Solo desarrollo local
├── SecretDemo.csproj


---

## ⚙️ Configuración local

### Requisitos

- .NET 8 SDK
- Azure Functions Core Tools v4
- Azure CLI
- Acceso al Key Vault en Azure

---

### Autenticación local

```bash
az login --use-device-code
El proyecto usa DefaultAzureCredential, que en local se autentica usando Azure CLI y en Azure usa Managed Identity automáticamente.

local.settings.json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "KeyVaultUrl": "https://<key-vault-name>.vault.azure.net/"
  }
}
⚠️ La URL del Key Vault no es un secreto.

▶️ Ejecutar en local
func start
Endpoint:

GET http://localhost:7071/api/GetSecret
Respuesta esperada (ejemplo):

Hola desde Key Vault
☁️ Despliegue en Azure
func azure functionapp publish <function-app-name>
En Azure:

La Function usa Managed Identity

No requiere variables sensibles

No requiere login manual

🧠 Decisiones de diseño
Azure Functions: backend serverless, escalable y de bajo mantenimiento

Key Vault: separación clara entre código y secretos

RBAC: principio de mínimo privilegio

DefaultAzureCredential: mismo código para local y producción

Isolated Worker: separación del runtime ASP.NET Core

❌ Antipatrones evitados
API Keys en frontend

Secrets en repositorio

App Registrations innecesarias

Client secrets en CI/CD

Dependencia de variables sensibles en pipelines

📈 Posibles extensiones
Cachear secretos en memoria

Implementar retries y circuit breaker

Proteger la Function con Entra ID

Versionar la API

Integrar con Cosmos DB o SQL

Centralizar logging con Application Insights

