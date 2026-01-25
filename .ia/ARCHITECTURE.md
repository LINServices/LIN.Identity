# Documentación de Arquitectura y Convenciones (AI Guide)

Este documento sirve como referencia para que cualquier IA (incluyendo esta misma) entienda cómo debe trabajar en este proyecto, asegurando consistencia en la infraestructura, nomenclatura y estilo de programación.

## 🏗️ Infraestructura del Proyecto

El proyecto sigue una arquitectura multicapa en .NET, organizada de la siguiente manera:

- **LIN.Cloud.Identity**: Capa de API Web (Controladores, Middleware, Servicios de Identidad).
- **LIN.Cloud.Identity.Persistence**: Capa de Persistencia (Contextos de DB, Repositorios, Modelos de datos, Migraciones).
- **LIN.Cloud.Identity.Services**: Lógica de negocio y servicios auxiliares.
- **LIN.Types**: (Externa) Define los modelos y enumeraciones compartidos.

### Patrones Clave
- **Repository Pattern**: Todo acceso a datos debe pasar por una interfaz en la capa de persistencia.
- **Dependency Injection**: Se utilizan constructores primarios (Primary Constructors) de C# 12 para la inyección de dependencias.
- **Response Pattern**: Todas las acciones deben retornar objetos estandarizados como `ReadOneResponse<T>`, `ReadAllResponse<T>` o `ResponseBase`.

## 🏷️ Nomenclatura y Convenciones

### Nombres (Naming)
- **Clases e Interfaces**: `PascalCase`. Las interfaces siempre empiezan con `I` (ej. `IAccountRepository`).
- **Métodos**: `PascalCase`. Deben ser descriptivos y en **Inglés** (ej. `Create`, `ReadByIdentity`, `UpdatePassword`).
- **Variables y Parámetros**: `camelCase`. **Todo el código debe estar en Inglés**, incluyendo nombres de variables locales, parámetros y propiedades (ej. `model`, `organization`, `filters`, `isDefault`).
- **Namespaces**: Deben seguir la ruta física del archivo (ej. `LIN.Cloud.Identity.Persistence.Repositories.EntityFramework`).

### Estilo de Programación
- **Constructores Primarios**: Utilizar la sintaxis `public class MiServicio(IDependencia dependencia)`. **Importante**: Los constructores y métodos deben escribirse en una sola línea, evitando saltos de línea entre parámetros.
- **Asincronismo**: Todas las operaciones de E/S o base de datos deben ser `async` y retornar `Task<T>`. Los nombres de los métodos **no** necesitan el sufijo `Async` si están dentro del patrón de repositorio, pero es aceptable en otros contextos.
- **LINQ**: Usar sintaxis de extensión (`.Where()`, `.Select()`) o sintaxis de consulta según la legibilidad.
- **Manejo de Errores**: Usar bloques `try-catch` para retornar respuestas de error controladas (`new(Responses.Error)`) en lugar de lanzar excepciones hacia arriba.

## 💬 Comentarios y Documentación

- **Idioma**: Todos los comentarios deben estar en **Español natural**.
- **XML Documentation**: Se deben documentar todos los métodos públicos utilizando etiquetas `<summary>`, `<param>` y `<returns>`.
- **Comentarios Internos**: Usar comentarios breves para explicar bloques lógicos dentro de los métodos.

```csharp
/// <summary>
/// Descripción de la función.
/// </summary>
/// <param name="parametro">Descripción del parámetro.</param>
public async Task<ResponseBase> MiFuncion(int parametro)
{
    // Lógica interna.
    return new(Responses.Success);
}
```

## � Respuestas Estándares (`Responses`)

El sistema utiliza un enum global llamado `Responses` para estandarizar los resultados de todas las operaciones. La IA debe utilizar estos valores al retornar cualquier `ResponseBase`.

| Valor | Descripción |
| :--- | :--- |
| `Success` | Operación exitosa. |
| `InvalidParam` | Uno o varios parámetros son inválidos. |
| `NotExistAccount` | La cuenta solicitada no existe. |
| `ExistAccount` | La cuenta ya existe en el sistema. |
| `Unauthorized` | El usuario no tiene permisos para la acción. |
| `NotRows` | No hay datos que coincidan con la búsqueda. |
| `InvalidPassword` | La contraseña proporcionada es incorrecta. |
| `NotFoundDirectory` | El directorio u organización no fue encontrado. |
| `UnavailableService` | El servicio no está disponible temporalmente. |

> [!TIP]
> Puedes encontrar la lista completa de respuestas en la definición del enum `Responses` dentro del proyecto de tipos compartidos. Usa siempre estos valores en lugar de strings mágicos.

## �🚀 Cómo Realizar Tareas

Cuando se solicite una tarea a la IA:

1. **Analizar Repositorios**: Antes de crear lógica, verificar si la interfaz de repositorio ya existe en `Persistence.Repositories`.
2. **Seguir el Patrón de Respuesta**: Asegurar que los controladores retornen `HttpCreateResponse`, `HttpReadOneResponse`, etc.
3. **Internacionalización**: El código debe escribirse íntegramente en **Inglés** (clases, métodos, variables). Sin embargo, los comentarios y la documentación XML deben escribirse estrictamente en **Español**.
4. **Validaciones**: Las validaciones se realizan comúnmente en los controladores o servicios de formato (ej. `Services.Formats.Account.Validate`).

---
*Este documento es auto-contenido y debe ser leído por la IA antes de cualquier modificación estructural.*