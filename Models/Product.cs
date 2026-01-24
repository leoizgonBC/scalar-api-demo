using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PruebaApi.Models;

/// <summary>
/// Representa un producto del catálogo
/// </summary>
public class Product
{
    /// <summary>
    /// Identificador único del producto
    /// </summary>
    /// <example>1</example>
    public long Id { get; set; }

    /// <summary>
    /// Nombre del producto
    /// </summary>
    /// <example>Laptop Gaming Pro</example>
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 200 caracteres")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada del producto
    /// </summary>
    /// <example>Laptop de alto rendimiento para gaming con RTX 4080</example>
    [StringLength(2000, ErrorMessage = "La descripción no puede exceder 2000 caracteres")]
    public string? Description { get; set; }

    /// <summary>
    /// Precio del producto en USD
    /// </summary>
    /// <example>1299.99</example>
    [Required]
    [Range(0.01, 999999.99, ErrorMessage = "El precio debe estar entre 0.01 y 999999.99")]
    public decimal Price { get; set; }

    /// <summary>
    /// Precio con descuento (si aplica)
    /// </summary>
    /// <example>999.99</example>
    [Range(0, 999999.99)]
    public decimal? SalePrice { get; set; }

    /// <summary>
    /// Categoría del producto
    /// </summary>
    /// <example>Electronics</example>
    [Required]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// SKU único del producto
    /// </summary>
    /// <example>LAPTOP-GP-001</example>
    [Required]
    [StringLength(50)]
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// Cantidad en stock
    /// </summary>
    /// <example>50</example>
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    /// <summary>
    /// URL de la imagen principal
    /// </summary>
    /// <example>https://example.com/images/laptop.jpg</example>
    [Url]
    public string? ImageUrl { get; set; }

    /// <summary>
    /// URLs de imágenes adicionales
    /// </summary>
    public List<string>? ImageGallery { get; set; }

    /// <summary>
    /// Tags/etiquetas del producto
    /// </summary>
    /// <example>["gaming", "laptop", "rtx"]</example>
    public List<string>? Tags { get; set; }

    /// <summary>
    /// Indica si el producto está activo
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Indica si el producto está destacado
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool IsFeatured { get; set; }

    /// <summary>
    /// Calificación promedio (1-5)
    /// </summary>
    /// <example>4.5</example>
    [Range(0, 5)]
    public double Rating { get; set; }

    /// <summary>
    /// Número de reseñas
    /// </summary>
    /// <example>128</example>
    public int ReviewCount { get; set; }

    /// <summary>
    /// Fecha de creación
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha de última actualización
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO para crear/actualizar un producto
/// </summary>
public class ProductRequest
{
    /// <summary>
    /// Nombre del producto
    /// </summary>
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descripción del producto
    /// </summary>
    [StringLength(2000)]
    public string? Description { get; set; }

    /// <summary>
    /// Precio en USD
    /// </summary>
    [Required]
    [Range(0.01, 999999.99)]
    public decimal Price { get; set; }

    /// <summary>
    /// Categoría
    /// </summary>
    [Required]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// SKU único
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// Stock inicial
    /// </summary>
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    /// <summary>
    /// Tags del producto
    /// </summary>
    public List<string>? Tags { get; set; }
}

/// <summary>
/// Filtros para búsqueda de productos
/// </summary>
public class ProductFilter
{
    /// <summary>
    /// Búsqueda por nombre
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Filtrar por categoría
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Precio mínimo
    /// </summary>
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// Precio máximo
    /// </summary>
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// Solo productos en stock
    /// </summary>
    public bool? InStock { get; set; }

    /// <summary>
    /// Solo productos destacados
    /// </summary>
    public bool? Featured { get; set; }

    /// <summary>
    /// Ordenar por (name, price, rating, created)
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Orden descendente
    /// </summary>
    public bool Descending { get; set; }

    /// <summary>
    /// Página actual (1-based)
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    /// <summary>
    /// Elementos por página
    /// </summary>
    [Range(1, 100)]
    public int PageSize { get; set; } = 10;
}

/// <summary>
/// Respuesta paginada de productos
/// </summary>
public class ProductListResponse
{
    /// <summary>
    /// Lista de productos
    /// </summary>
    public List<Product> Items { get; set; } = new();

    /// <summary>
    /// Total de productos
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Página actual
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Elementos por página
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total de páginas
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Hay página siguiente
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Hay página anterior
    /// </summary>
    public bool HasPreviousPage => Page > 1;
}
