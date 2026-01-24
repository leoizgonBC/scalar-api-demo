using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PruebaApi.Models;

/// <summary>
/// Representa un usuario del sistema
/// </summary>
public class User
{
    /// <summary>
    /// Identificador único del usuario
    /// </summary>
    /// <example>1</example>
    public long Id { get; set; }

    /// <summary>
    /// Nombre de usuario único
    /// </summary>
    /// <example>john_doe</example>
    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 50 caracteres")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Correo electrónico del usuario
    /// </summary>
    /// <example>john.doe@example.com</example>
    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Nombre completo del usuario
    /// </summary>
    /// <example>John Doe</example>
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string? FullName { get; set; }

    /// <summary>
    /// URL del avatar del usuario
    /// </summary>
    /// <example>https://example.com/avatars/john.png</example>
    [Url(ErrorMessage = "La URL del avatar no es válida")]
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// Rol del usuario en el sistema
    /// </summary>
    /// <example>User</example>
    [DefaultValue(UserRole.User)]
    public UserRole Role { get; set; } = UserRole.User;

    /// <summary>
    /// Indica si el usuario está activo
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Fecha de registro del usuario
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Última fecha de acceso
    /// </summary>
    public DateTime? LastLoginAt { get; set; }
}

/// <summary>
/// Roles disponibles en el sistema
/// </summary>
public enum UserRole
{
    /// <summary>Usuario estándar</summary>
    User,
    /// <summary>Moderador</summary>
    Moderator,
    /// <summary>Administrador</summary>
    Admin
}

/// <summary>
/// DTO para crear un nuevo usuario
/// </summary>
public class CreateUserRequest
{
    /// <summary>
    /// Nombre de usuario único
    /// </summary>
    /// <example>john_doe</example>
    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Correo electrónico
    /// </summary>
    /// <example>john.doe@example.com</example>
    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña del usuario
    /// </summary>
    /// <example>SecureP@ss123</example>
    [Required(ErrorMessage = "La contraseña es requerida")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Nombre completo
    /// </summary>
    /// <example>John Doe</example>
    public string? FullName { get; set; }
}

/// <summary>
/// DTO para login
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Email o nombre de usuario
    /// </summary>
    /// <example>john.doe@example.com</example>
    [Required]
    public string EmailOrUsername { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña
    /// </summary>
    /// <example>SecureP@ss123</example>
    [Required]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Respuesta de autenticación
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// Token de acceso JWT
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de token
    /// </summary>
    /// <example>Bearer</example>
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    /// Tiempo de expiración en segundos
    /// </summary>
    /// <example>3600</example>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Datos del usuario autenticado
    /// </summary>
    public User? User { get; set; }
}
