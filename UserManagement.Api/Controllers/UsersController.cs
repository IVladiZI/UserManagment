using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Common.Mediator;
using UserManagement.Application.Users.Commands.RegisterUser;
using UserManagement.Contracts.Users;
using UserManagement.Application.Users.Queries;

namespace UserManagement.Api.Controllers
{
    /// <summary>
    /// Controlador para la gestión de usuarios.
    /// Proporciona endpoints para registrar y consultar usuarios.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(ISender sender) : ControllerBase
    {
        private readonly ISender _sender = sender;

        /// <summary>
        /// Registra un nuevo usuario en el sistema.
        /// </summary>
        /// <param name="request">Datos del usuario a registrar.</param>
        /// <param name="ct">Token de cancelación.</param>
        /// <returns>Identificador del usuario creado.</returns>
        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserRequest request, CancellationToken ct)
        {
            var id = await _sender.Send(new RegisterUserCommand
            {
                Name = request.Name,
                LastName = request.LastName,
                SecondLastName = request.SecondLastName,
                DocumentNumber = request.DocumentNumber,
                Email = request.Email,
                BirthDate = request.BirthDate
            }, ct);

            return CreatedAtAction(nameof(GetUserById), new { id }, new { Id = id });
        }

        /// <summary>
        /// Obtiene la información de un usuario por su identificador.
        /// </summary>
        /// <param name="id">Identificador único del usuario.</param>
        /// <param name="ct">Token de cancelación.</param>
        /// <returns>Datos del usuario encontrado.</returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUserById(Guid id, CancellationToken ct)
        {
            var result = await _sender.Send(new GetUserByIdQuery(id), ct);
            return result is null ? NotFound() : Ok(result);
        }
    }
}