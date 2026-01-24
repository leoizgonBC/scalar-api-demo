using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PruebaApi.Models;

/// <summary>
/// Representa un elemento de tarea (Todo)
/// </summary>
public class TodoItem
{
    /// <summary>
    /// Identificador único del elemento
    /// </summary>
    /// <example>1</example>
    public long Id { get; set; }

    /// <summary>
    /// Nombre o título de la tarea
    /// </summary>
    /// <example>Comprar leche</example>
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "El nombre debe tener entre 1 y 100 caracteres")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada de la tarea
    /// </summary>
    /// <example>Ir al supermercado y comprar leche descremada</example>
    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string? Description { get; set; }

    /// <summary>
    /// Indica si la tarea está completada
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool IsComplete { get; set; }

    /// <summary>
    /// Fecha de creación de la tarea
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
