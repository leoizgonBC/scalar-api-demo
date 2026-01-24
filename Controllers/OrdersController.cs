using Microsoft.AspNetCore.Mvc;
using PruebaApi.Models;
using Scalar.AspNetCore;

namespace PruebaApi.Controllers;

/// <summary>
/// Controlador para gestión de pedidos
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Orders")]
public class OrdersController : ControllerBase
{
    private static readonly List<Order> _orders = new()
    {
        new Order
        {
            Id = 1,
            OrderNumber = "ORD-2024-0001",
            UserId = 2,
            Status = OrderStatus.Delivered,
            Items = new List<OrderItem>
            {
                new() { ProductId = 1, ProductName = "Laptop Gaming Pro", Sku = "LAPTOP-GP-001", Quantity = 1, UnitPrice = 1299.99m }
            },
            Subtotal = 1299.99m,
            Tax = 104.00m,
            ShippingCost = 0m,
            Total = 1403.99m,
            PaymentMethod = PaymentMethod.CreditCard,
            ShippingAddress = new ShippingAddress { RecipientName = "John Doe", AddressLine1 = "123 Main St", City = "New York", State = "NY", PostalCode = "10001", Country = "USA" },
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            DeliveredAt = DateTime.UtcNow.AddDays(-7)
        },
        new Order
        {
            Id = 2,
            OrderNumber = "ORD-2024-0002",
            UserId = 2,
            Status = OrderStatus.Shipped,
            Items = new List<OrderItem>
            {
                new() { ProductId = 2, ProductName = "Wireless Mouse", Sku = "MOUSE-WL-002", Quantity = 2, UnitPrice = 49.99m },
                new() { ProductId = 3, ProductName = "Mechanical Keyboard", Sku = "KB-MECH-003", Quantity = 1, UnitPrice = 99.99m }
            },
            Subtotal = 199.97m,
            Tax = 16.00m,
            ShippingCost = 9.99m,
            Total = 225.96m,
            PaymentMethod = PaymentMethod.PayPal,
            ShippingAddress = new ShippingAddress { RecipientName = "John Doe", AddressLine1 = "123 Main St", City = "New York", State = "NY", PostalCode = "10001", Country = "USA" },
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            ShippedAt = DateTime.UtcNow.AddDays(-1)
        },
        new Order
        {
            Id = 3,
            OrderNumber = "ORD-2024-0003",
            UserId = 3,
            Status = OrderStatus.Processing,
            Items = new List<OrderItem>
            {
                new() { ProductId = 8, ProductName = "SSD 1TB NVMe", Sku = "SSD-NV-008", Quantity = 2, UnitPrice = 79.99m }
            },
            Subtotal = 159.98m,
            Tax = 12.80m,
            ShippingCost = 5.99m,
            Total = 178.77m,
            PaymentMethod = PaymentMethod.CreditCard,
            ShippingAddress = new ShippingAddress { RecipientName = "Jane Smith", AddressLine1 = "456 Oak Ave", City = "Los Angeles", State = "CA", PostalCode = "90001", Country = "USA" },
            CreatedAt = DateTime.UtcNow.AddHours(-12)
        }
    };

    private static long _nextId = 4;
    private static int _orderCounter = 3;

    /// <summary>
    /// Lista todos los pedidos
    /// </summary>
    /// <remarks>
    /// Retorna la lista de pedidos con filtros opcionales.
    /// 
    /// ### Filtros disponibles
    /// - `userId`: Filtrar por usuario
    /// - `status`: Filtrar por estado
    /// - `fromDate`: Fecha desde
    /// - `toDate`: Fecha hasta
    /// </remarks>
    [HttpGet]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Order>> GetAll(
        [FromQuery] long? userId,
        [FromQuery] OrderStatus? status,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        var query = _orders.AsEnumerable();

        if (userId.HasValue)
            query = query.Where(o => o.UserId == userId.Value);

        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);

        if (fromDate.HasValue)
            query = query.Where(o => o.CreatedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(o => o.CreatedAt <= toDate.Value);

        return Ok(query.OrderByDescending(o => o.CreatedAt).ToList());
    }

    /// <summary>
    /// Obtiene un pedido por ID
    /// </summary>
    /// <param name="id">ID del pedido</param>
    /// <returns>Detalles del pedido</returns>
    [HttpGet("{id:long}")]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<Order> GetById(long id)
    {
        var order = _orders.FirstOrDefault(o => o.Id == id);
        if (order == null)
            return NotFound(new ProblemDetails { Title = "Pedido no encontrado", Detail = $"No existe pedido con ID {id}" });

        return Ok(order);
    }

    /// <summary>
    /// Obtiene un pedido por número de orden
    /// </summary>
    /// <param name="orderNumber">Número de orden</param>
    /// <returns>Detalles del pedido</returns>
    [HttpGet("by-number/{orderNumber}")]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<Order> GetByOrderNumber(string orderNumber)
    {
        var order = _orders.FirstOrDefault(o => o.OrderNumber.Equals(orderNumber, StringComparison.OrdinalIgnoreCase));
        if (order == null)
            return NotFound(new ProblemDetails { Title = "Pedido no encontrado", Detail = $"No existe pedido con número {orderNumber}" });

        return Ok(order);
    }

    /// <summary>
    /// Crea un nuevo pedido
    /// </summary>
    /// <remarks>
    /// Crea un nuevo pedido con los productos especificados.
    /// 
    /// ### Proceso
    /// 1. Valida que los productos existan y tengan stock
    /// 2. Calcula subtotal, impuestos y total
    /// 3. Genera número de orden único
    /// 4. Crea el pedido en estado "Pending"
    /// 
    /// ### Ejemplo de solicitud
    /// ```json
    /// {
    ///   "items": [
    ///     { "productId": 1, "quantity": 1 },
    ///     { "productId": 2, "quantity": 2 }
    ///   ],
    ///   "shippingAddress": {
    ///     "recipientName": "John Doe",
    ///     "addressLine1": "123 Main St",
    ///     "city": "New York",
    ///     "state": "NY",
    ///     "postalCode": "10001",
    ///     "country": "USA"
    ///   },
    ///   "paymentMethod": "CreditCard"
    /// }
    /// ```
    /// </remarks>
    /// <param name="request">Datos del pedido</param>
    /// <returns>Pedido creado</returns>
    [HttpPost]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<Order> Create([FromBody] CreateOrderRequest request)
    {
        // Simular productos (en realidad vendría de la BD)
        var orderItems = request.Items.Select(i => new OrderItem
        {
            ProductId = i.ProductId,
            ProductName = $"Product {i.ProductId}",
            Sku = $"SKU-{i.ProductId:D3}",
            Quantity = i.Quantity,
            UnitPrice = 99.99m // Precio simulado
        }).ToList();

        var subtotal = orderItems.Sum(i => i.Total);
        var tax = subtotal * 0.08m; // 8% impuesto
        var shipping = subtotal > 100 ? 0m : 9.99m; // Envío gratis +$100
        var discount = string.IsNullOrEmpty(request.CouponCode) ? 0m : subtotal * 0.1m; // 10% descuento con cupón

        var order = new Order
        {
            Id = _nextId++,
            OrderNumber = $"ORD-{DateTime.UtcNow:yyyy}-{++_orderCounter:D4}",
            UserId = 1, // Simulado
            Status = OrderStatus.Pending,
            Items = orderItems,
            Subtotal = subtotal,
            Tax = Math.Round(tax, 2),
            ShippingCost = shipping,
            Discount = Math.Round(discount, 2),
            Total = Math.Round(subtotal + tax + shipping - discount, 2),
            CouponCode = request.CouponCode,
            PaymentMethod = request.PaymentMethod,
            ShippingAddress = request.ShippingAddress,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        _orders.Add(order);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    /// <summary>
    /// Actualiza el estado de un pedido
    /// </summary>
    /// <param name="id">ID del pedido</param>
    /// <param name="request">Nuevo estado</param>
    /// <returns>Pedido actualizado</returns>
    [HttpPatch("{id:long}/status")]
    [Badge("Admin")]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<Order> UpdateStatus(long id, [FromBody] UpdateOrderStatusRequest request)
    {
        var order = _orders.FirstOrDefault(o => o.Id == id);
        if (order == null)
            return NotFound(new ProblemDetails { Title = "Pedido no encontrado" });

        // Validar transiciones de estado
        var validTransitions = new Dictionary<OrderStatus, OrderStatus[]>
        {
            { OrderStatus.Pending, new[] { OrderStatus.Confirmed, OrderStatus.Cancelled } },
            { OrderStatus.Confirmed, new[] { OrderStatus.Processing, OrderStatus.Cancelled } },
            { OrderStatus.Processing, new[] { OrderStatus.Shipped, OrderStatus.Cancelled } },
            { OrderStatus.Shipped, new[] { OrderStatus.Delivered } },
            { OrderStatus.Delivered, new[] { OrderStatus.Refunded } }
        };

        if (validTransitions.TryGetValue(order.Status, out var allowed) && !allowed.Contains(request.Status))
            return BadRequest(new ProblemDetails
            {
                Title = "Transición de estado inválida",
                Detail = $"No se puede cambiar de {order.Status} a {request.Status}"
            });

        order.Status = request.Status;
        order.UpdatedAt = DateTime.UtcNow;

        if (request.Status == OrderStatus.Shipped)
            order.ShippedAt = DateTime.UtcNow;
        else if (request.Status == OrderStatus.Delivered)
            order.DeliveredAt = DateTime.UtcNow;

        return Ok(order);
    }

    /// <summary>
    /// Cancela un pedido
    /// </summary>
    /// <param name="id">ID del pedido</param>
    /// <param name="reason">Motivo de cancelación</param>
    /// <returns>Pedido cancelado</returns>
    [HttpPost("{id:long}/cancel")]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<Order> Cancel(long id, [FromQuery] string? reason)
    {
        var order = _orders.FirstOrDefault(o => o.Id == id);
        if (order == null)
            return NotFound(new ProblemDetails { Title = "Pedido no encontrado" });

        if (order.Status == OrderStatus.Shipped || order.Status == OrderStatus.Delivered)
            return BadRequest(new ProblemDetails { Title = "No se puede cancelar", Detail = "El pedido ya fue enviado o entregado" });

        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = DateTime.UtcNow;
        order.Notes = string.IsNullOrEmpty(reason) ? order.Notes : $"{order.Notes}\nCancelado: {reason}";

        return Ok(order);
    }

    /// <summary>
    /// Obtiene estadísticas de pedidos
    /// </summary>
    /// <returns>Estadísticas de ventas</returns>
    [HttpGet("stats")]
    [Badge("Admin")]
    [Stability(Stability.Experimental)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult GetStats()
    {
        var stats = new
        {
            TotalOrders = _orders.Count,
            TotalRevenue = _orders.Where(o => o.Status != OrderStatus.Cancelled && o.Status != OrderStatus.Refunded).Sum(o => o.Total),
            AverageOrderValue = _orders.Any() ? _orders.Average(o => o.Total) : 0,
            ByStatus = _orders.GroupBy(o => o.Status).ToDictionary(g => g.Key.ToString(), g => g.Count()),
            ByPaymentMethod = _orders.GroupBy(o => o.PaymentMethod).ToDictionary(g => g.Key.ToString(), g => g.Count()),
            RecentOrders = _orders.OrderByDescending(o => o.CreatedAt).Take(5).Select(o => new { o.OrderNumber, o.Total, o.Status, o.CreatedAt })
        };

        return Ok(stats);
    }

    /// <summary>
    /// Obtiene pedidos de un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <returns>Lista de pedidos</returns>
    [HttpGet("user/{userId:long}")]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Order>> GetByUser(long userId)
    {
        var orders = _orders.Where(o => o.UserId == userId).OrderByDescending(o => o.CreatedAt).ToList();
        return Ok(orders);
    }
}
