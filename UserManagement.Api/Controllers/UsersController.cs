using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Contracts.Users;
using UserManagement.Application.Users.Commands;

namespace UserManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(ISender sender) : ControllerBase
    {
        private readonly ISender _sender = sender;

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserRequest request, CancellationToken ct)
        {
            var id = await _sender.Send(new RegisterUserCommand(
                request.FirstName,
                request.PaternalSurname,
                request.MaternalSurname,
                request.DocumentNumber,
                request.Email,
                request.BirthDate), ct);

            return CreatedAtAction(nameof(GetUserById), new { id }, new { Id = id });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUserById(Guid id, CancellationToken ct)
        {
            // Aquí puedes implementar la consulta y el mapeo a UserResponse
            return Ok(); // Implementa la lógica real
        }
    }
}