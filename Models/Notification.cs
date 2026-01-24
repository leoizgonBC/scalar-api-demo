using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PruebaApi.Models;

/// <summary>
/// Representa una notificación del sistema
/// </summary>
public class Notification
{
    /// <summary>
    /// Identificador único
    /// </summary>
    /// <example>1</example>
    public long Id { get; set; }

    /// <summary>
    /// ID del usuario destinatario
    /// </summary>
    /// <example>1</example>
    public long UserId { get; set; }

    /// <summary>
    /// Título de la notificación
    /// </summary>
    /// <example>Nuevo pedido recibido</example>
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Mensaje de la notificación
    /// </summary>
    /// <example>Tu pedido ORD-2024-0001 ha sido confirmado</example>
    [Required]
    [StringLength(1000)]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de notificación
    /// </summary>
    /// <example>Order</example>
    public NotificationType Type { get; set; }

    /// <summary>
    /// Prioridad de la notificación
    /// </summary>
    /// <example>Normal</example>
    [DefaultValue(NotificationPriority.Normal)]
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

    /// <summary>
    /// Indica si ha sido leída
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool IsRead { get; set; }

    /// <summary>
    /// URL de acción (clic en la notificación)
    /// </summary>
    /// <example>/orders/1</example>
    public string? ActionUrl { get; set; }

    /// <summary>
    /// Icono a mostrar
    /// </summary>
    /// <example>shopping-cart</example>
    public string? Icon { get; set; }

    /// <summary>
    /// Datos adicionales en formato JSON
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// Fecha de creación
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha en que fue leída
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// Fecha de expiración
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
}

/// <summary>
/// Tipos de notificación
/// </summary>
public enum NotificationType
{
    /// <summary>Información general</summary>
    Info,
    /// <summary>Éxito/Confirmación</summary>
    Success,
    /// <summary>Advertencia</summary>
    Warning,
    /// <summary>Error</summary>
    Error,
    /// <summary>Relacionada con pedidos</summary>
    Order,
    /// <summary>Relacionada con pagos</summary>
    Payment,
    /// <summary>Promoción/Marketing</summary>
    Promotion,
    /// <summary>Sistema</summary>
    System
}

/// <summary>
/// Prioridad de la notificación
/// </summary>
public enum NotificationPriority
{
    /// <summary>Baja prioridad</summary>
    Low,
    /// <summary>Prioridad normal</summary>
    Normal,
    /// <summary>Alta prioridad</summary>
    High,
    /// <summary>Urgente</summary>
    Urgent
}

/// <summary>
/// DTO para crear una notificación
/// </summary>
public class CreateNotificationRequest
{
    /// <summary>
    /// ID del usuario destinatario (null para broadcast)
    /// </summary>
    public long? UserId { get; set; }

    /// <summary>
    /// Título
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Mensaje
    /// </summary>
    [Required]
    [StringLength(1000)]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Tipo
    /// </summary>
    public NotificationType Type { get; set; } = NotificationType.Info;

    /// <summary>
    /// Prioridad
    /// </summary>
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

    /// <summary>
    /// URL de acción
    /// </summary>
    public string? ActionUrl { get; set; }

    /// <summary>
    /// Horas hasta expiración (null = no expira)
    /// </summary>
    public int? ExpiresInHours { get; set; }
}

/// <summary>
/// Resumen de notificaciones
/// </summary>
public class NotificationSummary
{
    /// <summary>
    /// Total de notificaciones
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// No leídas
    /// </summary>
    public int Unread { get; set; }

    /// <summary>
    /// Alta prioridad sin leer
    /// </summary>
    public int HighPriorityUnread { get; set; }

    /// <summary>
    /// Últimas notificaciones
    /// </summary>
    public List<Notification> Recent { get; set; } = new();
}
