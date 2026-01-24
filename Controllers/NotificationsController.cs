using Microsoft.AspNetCore.Mvc;
using PruebaApi.Models;
using Scalar.AspNetCore;

namespace PruebaApi.Controllers;

/// <summary>
/// Controlador para gestión de notificaciones
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Notifications")]
public class NotificationsController : ControllerBase
{
    private static readonly List<Notification> _notifications = new()
    {
        new Notification { Id = 1, UserId = 2, Title = "Pedido confirmado", Message = "Tu pedido ORD-2024-0001 ha sido confirmado", Type = NotificationType.Order, Priority = NotificationPriority.Normal, IsRead = true, ActionUrl = "/orders/1", Icon = "check-circle", ReadAt = DateTime.UtcNow.AddDays(-9) },
        new Notification { Id = 2, UserId = 2, Title = "Pedido enviado", Message = "Tu pedido ORD-2024-0001 ha sido enviado. Número de seguimiento: TRK123456", Type = NotificationType.Order, Priority = NotificationPriority.High, IsRead = true, ActionUrl = "/orders/1", Icon = "truck", ReadAt = DateTime.UtcNow.AddDays(-8) },
        new Notification { Id = 3, UserId = 2, Title = "Pedido entregado", Message = "Tu pedido ORD-2024-0001 ha sido entregado", Type = NotificationType.Success, Priority = NotificationPriority.Normal, IsRead = true, ActionUrl = "/orders/1", Icon = "package", ReadAt = DateTime.UtcNow.AddDays(-7) },
        new Notification { Id = 4, UserId = 2, Title = "Nuevo pedido en camino", Message = "Tu pedido ORD-2024-0002 ha sido enviado", Type = NotificationType.Order, Priority = NotificationPriority.Normal, IsRead = false, ActionUrl = "/orders/2", Icon = "truck" },
        new Notification { Id = 5, UserId = 2, Title = "Oferta especial", Message = "¡20% de descuento en accesorios! Usa el código SAVE20", Type = NotificationType.Promotion, Priority = NotificationPriority.Low, IsRead = false, ActionUrl = "/products?category=Accessories", Icon = "tag", ExpiresAt = DateTime.UtcNow.AddDays(7) },
        new Notification { Id = 6, UserId = 3, Title = "Pedido en proceso", Message = "Tu pedido ORD-2024-0003 está siendo preparado", Type = NotificationType.Order, Priority = NotificationPriority.Normal, IsRead = false, ActionUrl = "/orders/3", Icon = "box" },
        new Notification { Id = 7, UserId = 1, Title = "Alerta de sistema", Message = "Se detectó un intento de acceso inusual a tu cuenta", Type = NotificationType.Warning, Priority = NotificationPriority.Urgent, IsRead = false, Icon = "alert-triangle" }
    };

    private static long _nextId = 8;

    /// <summary>
    /// Obtiene las notificaciones de un usuario
    /// </summary>
    /// <remarks>
    /// Lista las notificaciones del usuario con filtros opcionales.
    /// 
    /// ### Filtros
    /// - `unreadOnly`: Solo mostrar no leídas
    /// - `type`: Filtrar por tipo
    /// - `limit`: Número máximo de resultados
    /// </remarks>
    /// <param name="userId">ID del usuario</param>
    /// <param name="unreadOnly">Solo no leídas</param>
    /// <param name="type">Tipo de notificación</param>
    /// <param name="limit">Límite de resultados</param>
    /// <returns>Lista de notificaciones</returns>
    [HttpGet("user/{userId:long}")]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Notification>> GetByUser(
        long userId,
        [FromQuery] bool? unreadOnly,
        [FromQuery] NotificationType? type,
        [FromQuery] int limit = 50)
    {
        var query = _notifications.Where(n => n.UserId == userId && (n.ExpiresAt == null || n.ExpiresAt > DateTime.UtcNow));

        if (unreadOnly == true)
            query = query.Where(n => !n.IsRead);

        if (type.HasValue)
            query = query.Where(n => n.Type == type.Value);

        return Ok(query.OrderByDescending(n => n.CreatedAt).Take(limit).ToList());
    }

    /// <summary>
    /// Obtiene el resumen de notificaciones de un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <returns>Resumen con conteos y últimas notificaciones</returns>
    [HttpGet("user/{userId:long}/summary")]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(NotificationSummary), StatusCodes.Status200OK)]
    public ActionResult<NotificationSummary> GetSummary(long userId)
    {
        var userNotifications = _notifications.Where(n => n.UserId == userId && (n.ExpiresAt == null || n.ExpiresAt > DateTime.UtcNow)).ToList();

        return Ok(new NotificationSummary
        {
            Total = userNotifications.Count,
            Unread = userNotifications.Count(n => !n.IsRead),
            HighPriorityUnread = userNotifications.Count(n => !n.IsRead && (n.Priority == NotificationPriority.High || n.Priority == NotificationPriority.Urgent)),
            Recent = userNotifications.OrderByDescending(n => n.CreatedAt).Take(5).ToList()
        });
    }

    /// <summary>
    /// Obtiene una notificación por ID
    /// </summary>
    /// <param name="id">ID de la notificación</param>
    /// <returns>Detalles de la notificación</returns>
    [HttpGet("{id:long}")]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(Notification), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<Notification> GetById(long id)
    {
        var notification = _notifications.FirstOrDefault(n => n.Id == id);
        if (notification == null)
            return NotFound(new ProblemDetails { Title = "Notificación no encontrada" });

        return Ok(notification);
    }

    /// <summary>
    /// Crea una nueva notificación
    /// </summary>
    /// <remarks>
    /// Crea una notificación para un usuario específico o broadcast.
    /// 
    /// ### Ejemplo
    /// ```json
    /// {
    ///   "userId": 2,
    ///   "title": "Nuevo mensaje",
    ///   "message": "Tienes un nuevo mensaje de soporte",
    ///   "type": "Info",
    ///   "priority": "Normal",
    ///   "actionUrl": "/messages/123"
    /// }
    /// ```
    /// </remarks>
    /// <param name="request">Datos de la notificación</param>
    /// <returns>Notificación creada</returns>
    [HttpPost]
    [Badge("Admin")]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(Notification), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<Notification> Create([FromBody] CreateNotificationRequest request)
    {
        var notification = new Notification
        {
            Id = _nextId++,
            UserId = request.UserId ?? 0,
            Title = request.Title,
            Message = request.Message,
            Type = request.Type,
            Priority = request.Priority,
            ActionUrl = request.ActionUrl,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = request.ExpiresInHours.HasValue ? DateTime.UtcNow.AddHours(request.ExpiresInHours.Value) : null
        };

        _notifications.Add(notification);
        return CreatedAtAction(nameof(GetById), new { id = notification.Id }, notification);
    }

    /// <summary>
    /// Marca una notificación como leída
    /// </summary>
    /// <param name="id">ID de la notificación</param>
    /// <returns>Notificación actualizada</returns>
    [HttpPatch("{id:long}/read")]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(Notification), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<Notification> MarkAsRead(long id)
    {
        var notification = _notifications.FirstOrDefault(n => n.Id == id);
        if (notification == null)
            return NotFound(new ProblemDetails { Title = "Notificación no encontrada" });

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;

        return Ok(notification);
    }

    /// <summary>
    /// Marca todas las notificaciones de un usuario como leídas
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <returns>Número de notificaciones actualizadas</returns>
    [HttpPatch("user/{userId:long}/read-all")]
    [Stability(Stability.Stable)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult MarkAllAsRead(long userId)
    {
        var count = 0;
        foreach (var notification in _notifications.Where(n => n.UserId == userId && !n.IsRead))
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            count++;
        }

        return Ok(new { MarkedAsRead = count });
    }

    /// <summary>
    /// Elimina una notificación
    /// </summary>
    /// <param name="id">ID de la notificación</param>
    /// <returns>Sin contenido</returns>
    [HttpDelete("{id:long}")]
    [Stability(Stability.Stable)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Delete(long id)
    {
        var notification = _notifications.FirstOrDefault(n => n.Id == id);
        if (notification == null)
            return NotFound(new ProblemDetails { Title = "Notificación no encontrada" });

        _notifications.Remove(notification);
        return NoContent();
    }

    /// <summary>
    /// Elimina todas las notificaciones leídas de un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <returns>Número de notificaciones eliminadas</returns>
    [HttpDelete("user/{userId:long}/read")]
    [Stability(Stability.Stable)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult DeleteAllRead(long userId)
    {
        var toRemove = _notifications.Where(n => n.UserId == userId && n.IsRead).ToList();
        foreach (var notification in toRemove)
            _notifications.Remove(notification);

        return Ok(new { Deleted = toRemove.Count });
    }

    /// <summary>
    /// Envía una notificación de prueba
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="type">Tipo de notificación</param>
    /// <returns>Notificación de prueba creada</returns>
    [HttpPost("test/{userId:long}")]
    [Badge("Dev")]
    [Stability(Stability.Experimental)]
    [ProducesResponseType(typeof(Notification), StatusCodes.Status201Created)]
    public ActionResult<Notification> SendTestNotification(long userId, [FromQuery] NotificationType type = NotificationType.Info)
    {
        var notification = new Notification
        {
            Id = _nextId++,
            UserId = userId,
            Title = $"Notificación de prueba ({type})",
            Message = $"Esta es una notificación de prueba de tipo {type} generada el {DateTime.UtcNow:G}",
            Type = type,
            Priority = NotificationPriority.Normal,
            Icon = "bell",
            CreatedAt = DateTime.UtcNow
        };

        _notifications.Add(notification);
        return CreatedAtAction(nameof(GetById), new { id = notification.Id }, notification);
    }
}
