using Microsoft.AspNetCore.Mvc;
using PruebaApi.Models;
using Scalar.AspNetCore;

namespace PruebaApi.Controllers;

/// <summary>
/// Controlador para gestión de usuarios y autenticación
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Users")]
public class UsersController : ControllerBase
{
    private static readonly List<User> _users = new()
    {
        new User { Id = 1, Username = "admin", Email = "admin@pruebaapi.com", FullName = "Administrador", Role = UserRole.Admin, IsActive = true },
        new User { Id = 2, Username = "john_doe", Email = "john@example.com", FullName = "John Doe", Role = UserRole.User, IsActive = true },
        new User { Id = 3, Username = "jane_smith", Email = "jane@example.com", FullName = "Jane Smith", Role = UserRole.Moderator, IsActive = true },
        new User { Id = 4, Username = "inactive_user", Email = "inactive@example.com", FullName = "Usuario Inactivo", Role = UserRole.User, IsActive = false }
    };

    private static long _nextId = 5;

    /// <summary>
    /// Obtiene todos los usuarios
    /// </summary>
    /// <remarks>
    /// Retorna la lista completa de usuarios registrados en el sistema.
    /// 
    /// ### Filtros opcionales
    /// - `active`: Filtrar por estado activo/inactivo
    /// - `role`: Filtrar por rol
    /// </remarks>
    /// <param name="active">Filtrar por estado activo</param>
    /// <param name="role">Filtrar por rol</param>
    /// <returns>Lista de usuarios</returns>
    [HttpGet]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<User>> GetAll([FromQuery] bool? active, [FromQuery] UserRole? role)
    {
        var result = _users.AsEnumerable();

        if (active.HasValue)
            result = result.Where(u => u.IsActive == active.Value);

        if (role.HasValue)
            result = result.Where(u => u.Role == role.Value);

        return Ok(result);
    }

    /// <summary>
    /// Obtiene un usuario por ID
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <returns>Datos del usuario</returns>
    [HttpGet("{id:long}")]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<User> GetById(long id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
            return NotFound(new ProblemDetails { Title = "Usuario no encontrado", Detail = $"No existe usuario con ID {id}" });

        return Ok(user);
    }

    /// <summary>
    /// Registra un nuevo usuario
    /// </summary>
    /// <remarks>
    /// Crea una nueva cuenta de usuario en el sistema.
    /// 
    /// ### Validaciones
    /// - El nombre de usuario debe ser único
    /// - El email debe ser único
    /// - La contraseña debe tener al menos 8 caracteres
    /// </remarks>
    /// <param name="request">Datos del nuevo usuario</param>
    /// <returns>Usuario creado</returns>
    [HttpPost("register")]
    [Badge("Public")]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public ActionResult<User> Register([FromBody] CreateUserRequest request)
    {
        if (_users.Any(u => u.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase)))
            return Conflict(new ProblemDetails { Title = "Conflicto", Detail = "El nombre de usuario ya existe" });

        if (_users.Any(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)))
            return Conflict(new ProblemDetails { Title = "Conflicto", Detail = "El email ya está registrado" });

        var user = new User
        {
            Id = _nextId++,
            Username = request.Username,
            Email = request.Email,
            FullName = request.FullName,
            Role = UserRole.User,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _users.Add(user);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    /// <summary>
    /// Inicia sesión
    /// </summary>
    /// <remarks>
    /// Autentica un usuario y retorna un token de acceso.
    /// 
    /// ### Ejemplo de uso
    /// ```json
    /// {
    ///   "emailOrUsername": "john@example.com",
    ///   "password": "SecureP@ss123"
    /// }
    /// ```
    /// </remarks>
    /// <param name="request">Credenciales de login</param>
    /// <returns>Token de autenticación y datos del usuario</returns>
    [HttpPost("login")]
    [Badge("Public")]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public ActionResult<AuthResponse> Login([FromBody] LoginRequest request)
    {
        var user = _users.FirstOrDefault(u =>
            (u.Email.Equals(request.EmailOrUsername, StringComparison.OrdinalIgnoreCase) ||
             u.Username.Equals(request.EmailOrUsername, StringComparison.OrdinalIgnoreCase)) &&
            u.IsActive);

        if (user == null)
            return Unauthorized(new ProblemDetails { Title = "No autorizado", Detail = "Credenciales inválidas" });

        // Simular generación de token
        user.LastLoginAt = DateTime.UtcNow;

        return Ok(new AuthResponse
        {
            Token = $"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{{\"sub\":\"{user.Id}\",\"name\":\"{user.Username}\"}}"))}",
            TokenType = "Bearer",
            ExpiresIn = 3600,
            User = user
        });
    }

    /// <summary>
    /// Actualiza el perfil del usuario
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="fullName">Nuevo nombre completo</param>
    /// <param name="avatarUrl">Nueva URL de avatar</param>
    /// <returns>Usuario actualizado</returns>
    [HttpPatch("{id:long}")]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<User> UpdateProfile(long id, [FromQuery] string? fullName, [FromQuery] string? avatarUrl)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
            return NotFound(new ProblemDetails { Title = "Usuario no encontrado" });

        if (!string.IsNullOrWhiteSpace(fullName))
            user.FullName = fullName;

        if (!string.IsNullOrWhiteSpace(avatarUrl))
            user.AvatarUrl = avatarUrl;

        return Ok(user);
    }

    /// <summary>
    /// Desactiva un usuario
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <returns>Sin contenido</returns>
    [HttpDelete("{id:long}")]
    [Stability(Stability.Stable)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Deactivate(long id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
            return NotFound(new ProblemDetails { Title = "Usuario no encontrado" });

        user.IsActive = false;
        return NoContent();
    }

    /// <summary>
    /// Obtiene estadísticas de usuarios
    /// </summary>
    /// <returns>Estadísticas</returns>
    [HttpGet("stats")]
    [Stability(Stability.Experimental)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult GetStats()
    {
        return Ok(new
        {
            Total = _users.Count,
            Active = _users.Count(u => u.IsActive),
            Inactive = _users.Count(u => !u.IsActive),
            ByRole = _users.GroupBy(u => u.Role).ToDictionary(g => g.Key.ToString(), g => g.Count())
        });
    }
}
