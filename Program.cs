using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios
builder.Services.AddControllers();

//// Configurar CORS para permitir llamadas desde el frontend
//builder.Services.AddCors(options =>
//{
//    //options.AddPolicy("AllowAll", policy =>
//    //{
//    //    policy.AllowAnyOrigin()
//    //          .AllowAnyMethod()
//    //          .AllowAnyHeader();
//    //});

//    //options.AddPolicy("AllowFrontend", policy =>
//    //{
//    //    policy.WithOrigins(
//    //            "http://localhost:3000",      // React/Next.js default
//    //            "http://localhost:5173",      // Vite default
//    //            "http://localhost:4200",      // Angular default
//    //            "http://localhost:8080",      // Vue default
//    //            "http://127.0.0.1:5500",      // Live Server
//    //            "https://localhost:7219",     // Misma API (HTTPS)
//    //            "http://localhost:5056"       // Misma API (HTTP)
//    //          )
//    //          .AllowAnyMethod()
//    //          .AllowAnyHeader()
//    //          .AllowCredentials();
//    //});
//});

// Configurar OpenAPI con información detallada
builder.Services.AddOpenApi("PruebaApi", options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new()
        {
            Title = "PruebaApi",
            Version = "1.0.0",
            Description = """
                # Introducción

                Bienvenido a la documentación de **PruebaApi**. Esta API RESTful proporciona 
                endpoints completos para la gestión de una aplicación de comercio electrónico.

                ## Módulos disponibles

                | Módulo | Descripción |
                |--------|-------------|
                | **Todo** | Gestión de tareas y recordatorios |
                | **Users** | Autenticación y gestión de usuarios |
                | **Products** | Catálogo de productos con búsqueda y filtros |
                | **Orders** | Gestión de pedidos y checkout |
                | **Notifications** | Sistema de notificaciones |

                ## Autenticación

                Esta API actualmente no requiere autenticación para el entorno de desarrollo.
                En producción, se implementará autenticación mediante API Keys o JWT.
           

                ## Códigos de respuesta

                | Código | Descripción |
                |--------|-------------|
                | `200` | OK - Solicitud exitosa |
                | `201` | Created - Recurso creado |
                | `204` | No Content - Operación exitosa sin contenido |
                | `400` | Bad Request - Error en la solicitud |
                | `401` | Unauthorized - No autenticado |
                | `404` | Not Found - Recurso no encontrado |
                | `409` | Conflict - Conflicto (ej: email duplicado) |
                | `500` | Internal Server Error - Error del servidor |

                ---

                > **Tip**: Usa el botón **"Test Request"** en cada endpoint para probarlo directamente.
                """,
            Contact = new()
            {
                Name = "Equipo de Desarrollo",
                Email = "dev@pruebaapi.com",
                Url = new Uri("https://github.com/pruebaapi")
            },
            License = new()
            {
                Name = "MIT License",
                Url = new Uri("https://opensource.org/licenses/MIT")
            },
            TermsOfService = new Uri("https://pruebaapi.com/terms"),
            Summary = "API completa para aplicaciones de comercio electrónico con usuarios, productos, pedidos y notificaciones."
        };

        // Configurar servidores
        document.Servers = new List<Microsoft.OpenApi.OpenApiServer>
        {
            new() { Url = "https://localhost:7219", Description = "Desarrollo (HTTPS)" },
            new() { Url = "http://localhost:5056", Description = "Desarrollo (HTTP)" },
            new() { Url = "https://localhost:44300", Description = "IIS Express (HTTPS)" },
            new() { Url = "http://localhost:52359", Description = "IIS Express (HTTP)" }
        };

        // Tags con descripciones detalladas
        document.Tags = new HashSet<Microsoft.OpenApi.OpenApiTag>
        {
            new() { Name = "Todo", Description = "Gestión de tareas y recordatorios. Incluye CRUD completo, búsqueda y estadísticas." },
            new() { Name = "Users", Description = "Autenticación y gestión de usuarios. Registro, login, perfiles y roles." },
            new() { Name = "Products", Description = "Catálogo de productos. Búsqueda, filtros, categorías y gestión de inventario." },
            new() { Name = "Orders", Description = "Gestión de pedidos. Crear pedidos, seguimiento, estados y estadísticas de ventas." },
            new() { Name = "Notifications", Description = "Sistema de notificaciones. Envío, lectura y gestión de alertas para usuarios." }
        };

        return Task.CompletedTask;
    });
    options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_1;
});

var app = builder.Build();

//// Habilitar CORS (IMPORTANTE: debe ir antes de otros middlewares)
//app.UseCors("AllowAll");

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Configurar Scalar - Portal de documentación interactivo
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("PruebaApi - Documentación")
            .WithTheme(ScalarTheme.DeepSpace)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.RestSharp)
            .ForceDarkMode()
            .HideSearch()
            .HideDeveloperTools()
            .WithClassicLayout()
            .SortOperationsByMethod()           
            .HideClientButton()  
            .ExpandAllTags()
            .WithYamlDocumentDownload()
            ;             
        
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
