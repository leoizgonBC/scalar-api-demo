using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PruebaApi.Models;

/// <summary>
/// Representa un pedido en el sistema
/// </summary>
public class Order
{
    /// <summary>
    /// Identificador único del pedido
    /// </summary>
    /// <example>1</example>
    public long Id { get; set; }

    /// <summary>
    /// Número de orden único
    /// </summary>
    /// <example>ORD-2024-0001</example>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// ID del usuario que realizó el pedido
    /// </summary>
    /// <example>1</example>
    [Required]
    public long UserId { get; set; }

    /// <summary>
    /// Estado actual del pedido
    /// </summary>
    /// <example>Pending</example>
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    /// <summary>
    /// Items del pedido
    /// </summary>
    public List<OrderItem> Items { get; set; } = new();

    /// <summary>
    /// Subtotal (sin impuestos ni envío)
    /// </summary>
    /// <example>299.98</example>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// Impuestos
    /// </summary>
    /// <example>24.00</example>
    public decimal Tax { get; set; }

    /// <summary>
    /// Costo de envío
    /// </summary>
    /// <example>15.00</example>
    public decimal ShippingCost { get; set; }

    /// <summary>
    /// Descuento aplicado
    /// </summary>
    /// <example>10.00</example>
    public decimal Discount { get; set; }

    /// <summary>
    /// Total del pedido
    /// </summary>
    /// <example>328.98</example>
    public decimal Total { get; set; }

    /// <summary>
    /// Código de cupón aplicado
    /// </summary>
    /// <example>SAVE10</example>
    public string? CouponCode { get; set; }

    /// <summary>
    /// Dirección de envío
    /// </summary>
    public ShippingAddress? ShippingAddress { get; set; }

    /// <summary>
    /// Método de pago
    /// </summary>
    /// <example>CreditCard</example>
    public PaymentMethod PaymentMethod { get; set; }

    /// <summary>
    /// ID de la transacción de pago
    /// </summary>
    /// <example>txn_1234567890</example>
    public string? PaymentTransactionId { get; set; }

    /// <summary>
    /// Notas del cliente
    /// </summary>
    /// <example>Por favor, dejar en recepción</example>
    [StringLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// Fecha de creación
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha de última actualización
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Fecha de envío
    /// </summary>
    public DateTime? ShippedAt { get; set; }

    /// <summary>
    /// Fecha de entrega
    /// </summary>
    public DateTime? DeliveredAt { get; set; }
}

/// <summary>
/// Item de un pedido
/// </summary>
public class OrderItem
{
    /// <summary>
    /// ID del producto
    /// </summary>
    /// <example>1</example>
    public long ProductId { get; set; }

    /// <summary>
    /// Nombre del producto (snapshot al momento de la compra)
    /// </summary>
    /// <example>Laptop Gaming Pro</example>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// SKU del producto
    /// </summary>
    /// <example>LAPTOP-GP-001</example>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// Cantidad
    /// </summary>
    /// <example>2</example>
    [Range(1, 999)]
    public int Quantity { get; set; }

    /// <summary>
    /// Precio unitario al momento de la compra
    /// </summary>
    /// <example>149.99</example>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Total del item (cantidad * precio)
    /// </summary>
    /// <example>299.98</example>
    public decimal Total => Quantity * UnitPrice;
}

/// <summary>
/// Dirección de envío
/// </summary>
public class ShippingAddress
{
    /// <summary>
    /// Nombre del destinatario
    /// </summary>
    /// <example>John Doe</example>
    [Required]
    [StringLength(100)]
    public string RecipientName { get; set; } = string.Empty;

    /// <summary>
    /// Línea de dirección 1
    /// </summary>
    /// <example>123 Main Street</example>
    [Required]
    [StringLength(200)]
    public string AddressLine1 { get; set; } = string.Empty;

    /// <summary>
    /// Línea de dirección 2 (apartamento, suite, etc.)
    /// </summary>
    /// <example>Apt 4B</example>
    [StringLength(200)]
    public string? AddressLine2 { get; set; }

    /// <summary>
    /// Ciudad
    /// </summary>
    /// <example>New York</example>
    [Required]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Estado/Provincia
    /// </summary>
    /// <example>NY</example>
    [Required]
    [StringLength(100)]
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// Código postal
    /// </summary>
    /// <example>10001</example>
    [Required]
    [StringLength(20)]
    public string PostalCode { get; set; } = string.Empty;

    /// <summary>
    /// País
    /// </summary>
    /// <example>United States</example>
    [Required]
    [StringLength(100)]
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Teléfono de contacto
    /// </summary>
    /// <example>+1-555-123-4567</example>
    [Phone]
    public string? Phone { get; set; }
}

/// <summary>
/// Estados posibles de un pedido
/// </summary>
public enum OrderStatus
{
    /// <summary>Pendiente de pago</summary>
    Pending,
    /// <summary>Pago confirmado</summary>
    Confirmed,
    /// <summary>En proceso de preparación</summary>
    Processing,
    /// <summary>Enviado</summary>
    Shipped,
    /// <summary>Entregado</summary>
    Delivered,
    /// <summary>Cancelado</summary>
    Cancelled,
    /// <summary>Reembolsado</summary>
    Refunded
}

/// <summary>
/// Métodos de pago disponibles
/// </summary>
public enum PaymentMethod
{
    /// <summary>Tarjeta de crédito</summary>
    CreditCard,
    /// <summary>Tarjeta de débito</summary>
    DebitCard,
    /// <summary>PayPal</summary>
    PayPal,
    /// <summary>Transferencia bancaria</summary>
    BankTransfer,
    /// <summary>Pago contra entrega</summary>
    CashOnDelivery
}

/// <summary>
/// DTO para crear un pedido
/// </summary>
public class CreateOrderRequest
{
    /// <summary>
    /// Items del pedido
    /// </summary>
    [Required]
    [MinLength(1, ErrorMessage = "El pedido debe tener al menos un item")]
    public List<CreateOrderItem> Items { get; set; } = new();

    /// <summary>
    /// Dirección de envío
    /// </summary>
    [Required]
    public ShippingAddress ShippingAddress { get; set; } = new();

    /// <summary>
    /// Método de pago
    /// </summary>
    [Required]
    public PaymentMethod PaymentMethod { get; set; }

    /// <summary>
    /// Código de cupón (opcional)
    /// </summary>
    public string? CouponCode { get; set; }

    /// <summary>
    /// Notas adicionales
    /// </summary>
    [StringLength(500)]
    public string? Notes { get; set; }
}

/// <summary>
/// Item para crear pedido
/// </summary>
public class CreateOrderItem
{
    /// <summary>
    /// ID del producto
    /// </summary>
    [Required]
    public long ProductId { get; set; }

    /// <summary>
    /// Cantidad
    /// </summary>
    [Required]
    [Range(1, 999)]
    public int Quantity { get; set; }
}

/// <summary>
/// DTO para actualizar estado del pedido
/// </summary>
public class UpdateOrderStatusRequest
{
    /// <summary>
    /// Nuevo estado
    /// </summary>
    [Required]
    public OrderStatus Status { get; set; }

    /// <summary>
    /// Número de seguimiento (para envíos)
    /// </summary>
    public string? TrackingNumber { get; set; }

    /// <summary>
    /// Notas internas
    /// </summary>
    public string? InternalNotes { get; set; }
}
