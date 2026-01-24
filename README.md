# Scalar API Demo

Una API RESTful de demostración construida con **.NET 10** y documentada con **Scalar**, mostrando una aplicación completa de comercio electrónico con documentación interactiva moderna.

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat&logo=dotnet)
![License](https://img.shields.io/badge/License-MIT-green.svg)
![Scalar](https://img.shields.io/badge/Docs-Scalar-blue)

## Descripcion

Esta API de ejemplo demuestra cómo crear documentación de API profesional e interactiva usando [Scalar](https://scalar.com/) como alternativa moderna a Swagger UI. Incluye módulos completos para gestión de tareas, usuarios, productos, pedidos y notificaciones.

## Caracteristicas

- **CRUD completo** para múltiples entidades
- **Documentación interactiva** con Scalar (tema DeepSpace)
- **Filtros y paginación** en endpoints de listado
- **Validación de datos** con Data Annotations
- **Endpoints organizados** por tags/categorías
- **OpenAPI 3.1** con documentación XML detallada

## Tecnologias

| Tecnología | Versión | Descripción |
|------------|---------|-------------|
| .NET | 10.0 | Framework principal |
| ASP.NET Core | 10.0 | Framework web |
| Scalar.AspNetCore | 2.12.15 | Documentación de API |
| OpenAPI | 3.1 | Especificación de API |

## Estructura del Proyecto

```
scalar-api-demo/
|-- Controllers/
|   |-- TodoController.cs          # Gestion de tareas
|   |-- UsersController.cs         # Gestion de usuarios
|   |-- ProductsController.cs      # Catalogo de productos
|   |-- OrdersController.cs        # Gestion de pedidos
|   +-- NotificationsController.cs # Sistema de notificaciones
|-- Models/
|   |-- TodoItem.cs
|   |-- User.cs
|   |-- Product.cs
|   |-- Order.cs
|   +-- Notification.cs
|-- Program.cs                     # Configuracion de la aplicacion
+-- PruebaApi.csproj
```

## Inicio Rapido

### Prerrequisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### Instalacion

1. Clona el repositorio:
   ```bash
   git clone https://github.com/leoizgonBC/scalar-api-demo.git
   cd scalar-api-demo
   ```

2. Restaura las dependencias:
   ```bash
   dotnet restore
   ```

3. Ejecuta la aplicación:
   ```bash
   dotnet run
   ```

4. Abre la documentación en tu navegador:
   - **Documentación Scalar**: https://localhost:7219/scalar/
   - **OpenAPI JSON**: https://localhost:7219/openapi/PruebaApi.json

## Modulos de la API

| Módulo | Ruta Base | Descripción |
|--------|-----------|-------------|
| **Todo** | `/api/Todo` | Gestión de tareas y recordatorios |
| **Users** | `/api/Users` | Autenticación y gestión de usuarios |
| **Products** | `/api/Products` | Catálogo con búsqueda y filtros |
| **Orders** | `/api/Orders` | Gestión de pedidos y checkout |
| **Notifications** | `/api/Notifications` | Sistema de notificaciones |

## Configuracion de Scalar

Este proyecto usa Scalar con las siguientes configuraciones personalizadas:

```csharp
app.MapScalarApiReference(options =>
{
    options
        .WithTitle("PruebaApi - Documentación")
        .WithTheme(ScalarTheme.DeepSpace)
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.RestSharp)
        .ForceDarkMode()
        .WithClassicLayout()
        .SortOperationsByMethod()
        .ExpandAllTags()
        .WithYamlDocumentDownload();
});
```

## Codigos de Respuesta HTTP

| Código | Descripción |
|--------|-------------|
| `200` | OK - Solicitud exitosa |
| `201` | Created - Recurso creado |
| `204` | No Content - Operación exitosa sin contenido |
| `400` | Bad Request - Error en la solicitud |
| `404` | Not Found - Recurso no encontrado |
| `409` | Conflict - Conflicto (ej: email duplicado) |

## Contribuir

Las contribuciones son bienvenidas. Por favor, abre un issue primero para discutir los cambios que te gustaría realizar.

## Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para más detalles.

---

<p align="center">
  Hecho con Scalar y .NET 10
</p>
