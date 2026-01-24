using Microsoft.AspNetCore.Mvc;
using PruebaApi.Models;
using Scalar.AspNetCore;

namespace PruebaApi.Controllers;

/// <summary>
/// Controlador para gestionar elementos Todo.
/// Proporciona operaciones CRUD completas para tareas.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Todo")]
public class TodoController : ControllerBase
{
    // Almacenamiento en memoria para propósitos de demostración
    private static readonly List<TodoItem> _todos = new()
    {
        new TodoItem { Id = 1, Name = "Aprender Scalar", Description = "Estudiar la documentación de Scalar para APIs modernas", IsComplete = false },
        new TodoItem { Id = 2, Name = "Crear API", Description = "Crear una API de ejemplo con documentación profesional", IsComplete = true },
        new TodoItem { Id = 3, Name = "Configurar CI/CD", Description = "Implementar pipeline de integración continua", IsComplete = false }
    };

    private static long _nextId = 4;

    /// <summary>
    /// Obtiene todos los elementos Todo
    /// </summary>
    /// <remarks>
    /// Este endpoint retorna una lista completa de todas las tareas almacenadas.
    /// 
    /// ### Ejemplo de respuesta exitosa
    /// 
    /// ```json
    /// [
    ///   {
    ///     "id": 1,
    ///     "name": "Mi tarea",
    ///     "description": "Descripción de la tarea",
    ///     "isComplete": false,
    ///     "createdAt": "2024-01-15T10:30:00Z"
    ///   }
    /// ]
    /// ```
    /// </remarks>
    /// <returns>Lista de todos los elementos Todo</returns>
    /// <response code="200">Retorna la lista de elementos Todo</response>
    [HttpGet]
    [Badge("New")]
    [Badge("V2", BadgePosition.After, "#007bff")]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(IEnumerable<TodoItem>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<TodoItem>> GetAll()
    {
        return Ok(_todos);
    }

    /// <summary>
    /// Obtiene un elemento Todo por su ID
    /// </summary>
    /// <remarks>
    /// Busca y retorna un elemento específico basado en su identificador único.
    /// 
    /// ### Ejemplo de uso
    /// 
    /// ```
    /// GET /api/Todo/1
    /// ```
    /// </remarks>
    /// <param name="id">ID único del elemento Todo (número entero positivo)</param>
    /// <returns>El elemento Todo solicitado</returns>
    /// <response code="200">Retorna el elemento Todo encontrado</response>
    /// <response code="404">Si el elemento con el ID especificado no existe</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(TodoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<TodoItem> GetById(long id)
    {
        var todo = _todos.FirstOrDefault(t => t.Id == id);
        
        if (todo == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Recurso no encontrado",
                Detail = $"No se encontró ningún Todo con el ID {id}",
                Status = StatusCodes.Status404NotFound,
                Instance = $"/api/Todo/{id}"
            });
        }

        return Ok(todo);
    }

    /// <summary>
    /// Crea un nuevo elemento Todo
    /// </summary>
    /// <remarks>
    /// Crea una nueva tarea en el sistema. El ID y la fecha de creación se asignan automáticamente.
    /// 
    /// ### Campos requeridos
    /// - **name**: Nombre de la tarea (1-100 caracteres)
    /// 
    /// ### Campos opcionales
    /// - **description**: Descripción detallada (máx. 500 caracteres)
    /// - **isComplete**: Estado de completitud (default: false)
    /// 
    /// ### Ejemplo de solicitud
    /// 
    /// ```json
    /// {
    ///   "name": "Nueva tarea importante",
    ///   "description": "Esta es una descripción detallada de la tarea",
    ///   "isComplete": false
    /// }
    /// ```
    /// </remarks>
    /// <param name="item">Datos del nuevo elemento Todo</param>
    /// <returns>El elemento Todo creado con su ID asignado</returns>
    /// <response code="201">Retorna el elemento Todo recién creado</response>
    /// <response code="400">Si los datos proporcionados son inválidos</response>
    [HttpPost]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(TodoItem), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<TodoItem> Create([FromBody] TodoItem item)
    {
        if (item == null)
        {
            return BadRequest(new ValidationProblemDetails
            {
                Title = "Datos inválidos",
                Detail = "El cuerpo de la solicitud no puede estar vacío"
            });
        }

        item.Id = _nextId++;
        item.CreatedAt = DateTime.UtcNow;
        _todos.Add(item);

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    /// <summary>
    /// Actualiza un elemento Todo existente
    /// </summary>
    /// <remarks>
    /// Reemplaza completamente los datos de un elemento Todo existente.
    /// 
    /// ### Importante
    /// - El ID en la URL debe coincidir con el ID en el cuerpo de la solicitud
    /// - Todos los campos serán actualizados (operación PUT completa)
    /// 
    /// ### Ejemplo de solicitud
    /// 
    /// ```json
    /// {
    ///   "id": 1,
    ///   "name": "Tarea actualizada",
    ///   "description": "Nueva descripción",
    ///   "isComplete": true
    /// }
    /// ```
    /// </remarks>
    /// <param name="id">ID del elemento Todo a actualizar</param>
    /// <param name="item">Datos actualizados del elemento Todo</param>
    /// <returns>Sin contenido si la actualización es exitosa</returns>
    /// <response code="204">Actualización exitosa</response>
    /// <response code="400">Si los IDs no coinciden o los datos son inválidos</response>
    /// <response code="404">Si el elemento no existe</response>
    [HttpPut("{id:long}")]
    [Stability(Stability.Experimental)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Update(long id, [FromBody] TodoItem item)
    {
        if (item == null || id != item.Id)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Solicitud inválida",
                Detail = "El ID del elemento en la URL debe coincidir con el ID en el cuerpo de la solicitud",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var existingTodo = _todos.FirstOrDefault(t => t.Id == id);
        
        if (existingTodo == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Recurso no encontrado",
                Detail = $"No se encontró ningún Todo con el ID {id}",
                Status = StatusCodes.Status404NotFound
            });
        }

        existingTodo.Name = item.Name;
        existingTodo.Description = item.Description;
        existingTodo.IsComplete = item.IsComplete;

        return NoContent();
    }

    /// <summary>
    /// Elimina un elemento Todo
    /// </summary>
    /// <remarks>
    /// Elimina permanentemente un elemento Todo del sistema.
    /// 
    /// ### Advertencia
    /// Esta operación es **irreversible**. Una vez eliminado, el elemento no puede ser recuperado.
    /// </remarks>
    /// <param name="id">ID del elemento Todo a eliminar</param>
    /// <returns>Sin contenido si la eliminación es exitosa</returns>
    /// <response code="204">Eliminación exitosa</response>
    /// <response code="404">Si el elemento no existe</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Delete(long id)
    {
        var todo = _todos.FirstOrDefault(t => t.Id == id);
        
        if (todo == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Recurso no encontrado",
                Detail = $"No se encontró ningún Todo con el ID {id}",
                Status = StatusCodes.Status404NotFound
            });
        }

        _todos.Remove(todo);

        return NoContent();
    }

    /// <summary>
    /// Marca un elemento Todo como completado
    /// </summary>
    /// <remarks>
    /// Actualiza únicamente el estado de completitud de una tarea a `true`.
    /// 
    /// ### Caso de uso
    /// Útil cuando solo necesitas marcar una tarea como completada sin modificar otros campos.
    /// 
    /// ### Ejemplo de respuesta
    /// 
    /// ```json
    /// {
    ///   "id": 1,
    ///   "name": "Mi tarea",
    ///   "description": "Descripción",
    ///   "isComplete": true,
    ///   "createdAt": "2024-01-15T10:30:00Z"
    /// }
    /// ```
    /// </remarks>
    /// <param name="id">ID del elemento Todo</param>
    /// <returns>El elemento Todo actualizado</returns>
    /// <response code="200">Retorna el elemento Todo con isComplete = true</response>
    /// <response code="404">Si el elemento no existe</response>
    [HttpPatch("{id:long}/complete")]
    [Stability(Stability.Deprecated)]
    [ProducesResponseType(typeof(TodoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<TodoItem> MarkAsComplete(long id)
    {
        var todo = _todos.FirstOrDefault(t => t.Id == id);
        
        if (todo == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Recurso no encontrado",
                Detail = $"No se encontró ningún Todo con el ID {id}",
                Status = StatusCodes.Status404NotFound
            });
        }

        todo.IsComplete = true;

        return Ok(todo);
    }

    /// <summary>
    /// Busca elementos Todo por estado de completitud
    /// </summary>
    /// <remarks>
    /// Filtra los elementos Todo según si están completados o pendientes.
    /// 
    /// ### Ejemplos
    /// - `GET /api/Todo/search?completed=true` - Obtiene tareas completadas
    /// - `GET /api/Todo/search?completed=false` - Obtiene tareas pendientes
    /// </remarks>
    /// <param name="completed">Estado de completitud a filtrar</param>
    /// <returns>Lista de elementos Todo filtrados</returns>
    /// <response code="200">Retorna la lista filtrada de elementos Todo</response>
    [HttpGet("search")]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(IEnumerable<TodoItem>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<TodoItem>> Search([FromQuery] bool? completed)
    {
        var result = _todos.AsEnumerable();
        
        if (completed.HasValue)
        {
            result = result.Where(t => t.IsComplete == completed.Value);
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtiene estadísticas de los elementos Todo
    /// </summary>
    /// <remarks>
    /// Retorna un resumen con el total de tareas, completadas y pendientes.
    /// 
    /// ### Ejemplo de respuesta
    /// 
    /// ```json
    /// {
    ///   "total": 10,
    ///   "completed": 4,
    ///   "pending": 6,
    ///   "completionRate": 40.0
    /// }
    /// ```
    /// </remarks>
    /// <returns>Estadísticas de los elementos Todo</returns>
    /// <response code="200">Retorna las estadísticas</response>
    [HttpGet("stats")]
    [Stability(Stability.Experimental)]
    [ProducesResponseType(typeof(TodoStats), StatusCodes.Status200OK)]
    public ActionResult<TodoStats> GetStats()
    {
        var total = _todos.Count;
        var completed = _todos.Count(t => t.IsComplete);
        var pending = total - completed;
        var completionRate = total > 0 ? Math.Round((double)completed / total * 100, 2) : 0;

        return Ok(new TodoStats
        {
            Total = total,
            Completed = completed,
            Pending = pending,
            CompletionRate = completionRate
        });
    }
}

/// <summary>
/// Estadísticas de los elementos Todo
/// </summary>
public class TodoStats
{
    /// <summary>
    /// Número total de tareas
    /// </summary>
    /// <example>10</example>
    public int Total { get; set; }

    /// <summary>
    /// Número de tareas completadas
    /// </summary>
    /// <example>4</example>
    public int Completed { get; set; }

    /// <summary>
    /// Número de tareas pendientes
    /// </summary>
    /// <example>6</example>
    public int Pending { get; set; }

    /// <summary>
    /// Porcentaje de tareas completadas
    /// </summary>
    /// <example>40.0</example>
    public double CompletionRate { get; set; }
}
