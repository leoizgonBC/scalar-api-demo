using Microsoft.AspNetCore.Mvc;
using PruebaApi.Models;
using Scalar.AspNetCore;

namespace PruebaApi.Controllers;

/// <summary>
/// Controlador para gestión del catálogo de productos
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Products")]
public class ProductsController : ControllerBase
{
    private static readonly List<Product> _products = new()
    {
        new Product { Id = 1, Name = "Laptop Gaming Pro", Description = "Laptop de alto rendimiento con RTX 4080", Price = 1299.99m, Category = "Electronics", Sku = "LAPTOP-GP-001", Stock = 50, Rating = 4.8, ReviewCount = 245, IsFeatured = true, Tags = new List<string> { "gaming", "laptop", "rtx" } },
        new Product { Id = 2, Name = "Wireless Mouse", Description = "Mouse inalámbrico ergonómico", Price = 49.99m, Category = "Accessories", Sku = "MOUSE-WL-002", Stock = 200, Rating = 4.5, ReviewCount = 1024, Tags = new List<string> { "mouse", "wireless", "ergonomic" } },
        new Product { Id = 3, Name = "Mechanical Keyboard", Description = "Teclado mecánico RGB con switches Cherry MX", Price = 129.99m, SalePrice = 99.99m, Category = "Accessories", Sku = "KB-MECH-003", Stock = 75, Rating = 4.7, ReviewCount = 512, IsFeatured = true, Tags = new List<string> { "keyboard", "mechanical", "rgb" } },
        new Product { Id = 4, Name = "4K Monitor 27\"", Description = "Monitor 4K UHD con panel IPS", Price = 399.99m, Category = "Electronics", Sku = "MON-4K-004", Stock = 30, Rating = 4.6, ReviewCount = 189, Tags = new List<string> { "monitor", "4k", "ips" } },
        new Product { Id = 5, Name = "USB-C Hub", Description = "Hub USB-C 7 en 1", Price = 39.99m, Category = "Accessories", Sku = "HUB-UC-005", Stock = 150, Rating = 4.3, ReviewCount = 87, Tags = new List<string> { "usb-c", "hub", "adapter" } },
        new Product { Id = 6, Name = "Gaming Headset", Description = "Auriculares gaming con sonido 7.1 surround", Price = 89.99m, Category = "Audio", Sku = "HEAD-GM-006", Stock = 0, Rating = 4.4, ReviewCount = 332, IsActive = true, Tags = new List<string> { "headset", "gaming", "7.1" } },
        new Product { Id = 7, Name = "Webcam 1080p", Description = "Webcam Full HD con micrófono integrado", Price = 59.99m, Category = "Electronics", Sku = "CAM-FHD-007", Stock = 80, Rating = 4.2, ReviewCount = 156, Tags = new List<string> { "webcam", "1080p", "streaming" } },
        new Product { Id = 8, Name = "SSD 1TB NVMe", Description = "Disco SSD NVMe de alta velocidad", Price = 89.99m, SalePrice = 79.99m, Category = "Storage", Sku = "SSD-NV-008", Stock = 120, Rating = 4.9, ReviewCount = 567, IsFeatured = true, Tags = new List<string> { "ssd", "nvme", "storage" } }
    };

    private static long _nextId = 9;

    /// <summary>
    /// Lista productos con filtros y paginación
    /// </summary>
    /// <remarks>
    /// Endpoint principal para listar productos del catálogo con múltiples opciones de filtrado.
    /// 
    /// ### Ejemplos de uso
    /// 
    /// ```
    /// GET /api/Products?category=Electronics
    /// GET /api/Products?minPrice=50&amp;maxPrice=200
    /// GET /api/Products?search=laptop&amp;sortBy=price&amp;descending=true
    /// GET /api/Products?inStock=true&amp;featured=true
    /// ```
    /// </remarks>
    /// <param name="filter">Filtros de búsqueda</param>
    /// <returns>Lista paginada de productos</returns>
    [HttpGet]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(ProductListResponse), StatusCodes.Status200OK)]
    public ActionResult<ProductListResponse> GetAll([FromQuery] ProductFilter filter)
    {
        var query = _products.Where(p => p.IsActive).AsEnumerable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(p => p.Name.Contains(filter.Search, StringComparison.OrdinalIgnoreCase) ||
                                     (p.Description?.Contains(filter.Search, StringComparison.OrdinalIgnoreCase) ?? false));

        if (!string.IsNullOrWhiteSpace(filter.Category))
            query = query.Where(p => p.Category.Equals(filter.Category, StringComparison.OrdinalIgnoreCase));

        if (filter.MinPrice.HasValue)
            query = query.Where(p => (p.SalePrice ?? p.Price) >= filter.MinPrice.Value);

        if (filter.MaxPrice.HasValue)
            query = query.Where(p => (p.SalePrice ?? p.Price) <= filter.MaxPrice.Value);

        if (filter.InStock == true)
            query = query.Where(p => p.Stock > 0);

        if (filter.Featured == true)
            query = query.Where(p => p.IsFeatured);

        var totalCount = query.Count();

        query = filter.SortBy?.ToLower() switch
        {
            "name" => filter.Descending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            "price" => filter.Descending ? query.OrderByDescending(p => p.SalePrice ?? p.Price) : query.OrderBy(p => p.SalePrice ?? p.Price),
            "rating" => filter.Descending ? query.OrderByDescending(p => p.Rating) : query.OrderBy(p => p.Rating),
            "created" => filter.Descending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
            _ => query.OrderByDescending(p => p.IsFeatured).ThenBy(p => p.Name)
        };

        var items = query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToList();

        return Ok(new ProductListResponse
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        });
    }

    /// <summary>
    /// Obtiene un producto por ID
    /// </summary>
    /// <param name="id">ID del producto</param>
    /// <returns>Detalles del producto</returns>
    [HttpGet("{id:long}")]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<Product> GetById(long id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null)
            return NotFound(new ProblemDetails { Title = "Producto no encontrado", Detail = $"No existe producto con ID {id}" });

        return Ok(product);
    }

    /// <summary>
    /// Obtiene un producto por SKU
    /// </summary>
    /// <param name="sku">SKU del producto</param>
    /// <returns>Detalles del producto</returns>
    [HttpGet("sku/{sku}")]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<Product> GetBySku(string sku)
    {
        var product = _products.FirstOrDefault(p => p.Sku.Equals(sku, StringComparison.OrdinalIgnoreCase));
        if (product == null)
            return NotFound(new ProblemDetails { Title = "Producto no encontrado", Detail = $"No existe producto con SKU {sku}" });

        return Ok(product);
    }

    /// <summary>
    /// Crea un nuevo producto
    /// </summary>
    /// <param name="request">Datos del producto</param>
    /// <returns>Producto creado</returns>
    [HttpPost]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public ActionResult<Product> Create([FromBody] ProductRequest request)
    {
        if (_products.Any(p => p.Sku.Equals(request.Sku, StringComparison.OrdinalIgnoreCase)))
            return Conflict(new ProblemDetails { Title = "Conflicto", Detail = "Ya existe un producto con ese SKU" });

        var product = new Product
        {
            Id = _nextId++,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Category = request.Category,
            Sku = request.Sku,
            Stock = request.Stock,
            Tags = request.Tags,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _products.Add(product);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    /// <summary>
    /// Actualiza un producto
    /// </summary>
    /// <param name="id">ID del producto</param>
    /// <param name="request">Datos actualizados</param>
    /// <returns>Producto actualizado</returns>
    [HttpPut("{id:long}")]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<Product> Update(long id, [FromBody] ProductRequest request)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null)
            return NotFound(new ProblemDetails { Title = "Producto no encontrado" });

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.Category = request.Category;
        product.Stock = request.Stock;
        product.Tags = request.Tags;
        product.UpdatedAt = DateTime.UtcNow;

        return Ok(product);
    }

    /// <summary>
    /// Actualiza el stock de un producto
    /// </summary>
    /// <param name="id">ID del producto</param>
    /// <param name="quantity">Nueva cantidad (puede ser negativa para reducir)</param>
    /// <returns>Stock actualizado</returns>
    [HttpPatch("{id:long}/stock")]
    [Badge("Inventory")]
    [Stability(Stability.Stable)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult UpdateStock(long id, [FromQuery] int quantity)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null)
            return NotFound(new ProblemDetails { Title = "Producto no encontrado" });

        var newStock = product.Stock + quantity;
        if (newStock < 0)
            return BadRequest(new ProblemDetails { Title = "Stock insuficiente", Detail = $"Stock actual: {product.Stock}, solicitado: {Math.Abs(quantity)}" });

        product.Stock = newStock;
        return Ok(new { ProductId = id, PreviousStock = product.Stock - quantity, NewStock = product.Stock });
    }

    /// <summary>
    /// Elimina un producto
    /// </summary>
    /// <param name="id">ID del producto</param>
    /// <returns>Sin contenido</returns>
    [HttpDelete("{id:long}")]
    [Stability(Stability.Stable)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Delete(long id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null)
            return NotFound(new ProblemDetails { Title = "Producto no encontrado" });

        product.IsActive = false;
        return NoContent();
    }

    /// <summary>
    /// Obtiene las categorías disponibles
    /// </summary>
    /// <returns>Lista de categorías con conteo</returns>
    [HttpGet("categories")]
    [Stability(Stability.Stable)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult GetCategories()
    {
        var categories = _products
            .Where(p => p.IsActive)
            .GroupBy(p => p.Category)
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .OrderBy(c => c.Category)
            .ToList();

        return Ok(categories);
    }

    /// <summary>
    /// Obtiene productos destacados
    /// </summary>
    /// <param name="limit">Número máximo de productos</param>
    /// <returns>Lista de productos destacados</returns>
    [HttpGet("featured")]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Product>> GetFeatured([FromQuery] int limit = 5)
    {
        var featured = _products
            .Where(p => p.IsActive && p.IsFeatured)
            .OrderByDescending(p => p.Rating)
            .Take(limit)
            .ToList();

        return Ok(featured);
    }

    /// <summary>
    /// Busca productos por tags
    /// </summary>
    /// <param name="tags">Tags separados por coma</param>
    /// <returns>Productos que coinciden</returns>
    [HttpGet("by-tags")]
    [Stability(Stability.Experimental)]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Product>> GetByTags([FromQuery] string tags)
    {
        var tagList = tags.Split(',').Select(t => t.Trim().ToLower()).ToList();

        var products = _products
            .Where(p => p.IsActive && p.Tags != null && p.Tags.Any(t => tagList.Contains(t.ToLower())))
            .ToList();

        return Ok(products);
    }
}
