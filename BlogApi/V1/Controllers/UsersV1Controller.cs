using Asp.Versioning;
using BlogApi.Data.Entities;
using BlogApi.Interfaces;
using BlogApi.Mapping;
using BlogApi.V1.Requests.User;
using BlogApi.V1.Responses.User;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/users")]
    [ApiExplorerSettings(GroupName = "Users")]
    public class UsersV1Controller : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UsersV1Controller(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<QueryUsersResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> QueryUsers([FromQuery] QueryUsersRequest request)
        {
            List<QueryUsersResponse> results = new List<QueryUsersResponse>();

            List<User> users = await _userRepository.GetAllUsersAsync();

            foreach (var user in users)
            {
                var response = user.ToQueryUsersResponse();
                results.Add(response);
            }

            return Ok(results);
        }

        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateUserResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {

            User newUser = request.ToEntity();

            await _userRepository.CreateUserAsync(newUser);

            return Created(new Uri(newUser.Id.ToString(), UriKind.Relative)
                           ,newUser.ToCreateUserResponse());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUser([FromRoute] int id)
        {
            User? user = await _userRepository.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            GetUserResponse response = user.ToGetUserResponse();

            return Ok(response);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateUserResponse))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UpdateUserResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] UpdateUserRequest request)
        {
            User? existingUser = await _userRepository.UpdateUserAsync(request, id);

            if (existingUser == null)
            {
                User newUser = request.ToEntity();

                await _userRepository.CreateUserAsync(newUser);

                return Created(new Uri(newUser.Id.ToString(), UriKind.Relative),
                               newUser.ToUpdateUserResponse());
            }

            UpdateUserResponse response = existingUser.ToUpdateUserResponse();

            return Ok(response);
        }

        
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PatchUserResponse))]
        public async Task<IActionResult> PatchUser([FromRoute] int id, [FromBody] JsonPatchDocument<PatchUserRequest> request)
        {
            User? existingUser = await _userRepository.GetUserByIdAsync(id);

            if (existingUser == null)
            {
                return NotFound();
            }

            var patchedUser = new PatchUserRequest()
            {
                Id = id,
                Name = existingUser.Name,
                Surname = existingUser.Surname,
                Email = existingUser.Email
            };

            request.ApplyTo(patchedUser, ModelState);

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            existingUser.Name = patchedUser.Name;
            existingUser.Surname = patchedUser.Surname;
            existingUser.Email = patchedUser.Email;
            await _userRepository.SaveChangesAsync();

            var response = new PatchUserResponse
            {
                Id = existingUser.Id,
                Name = existingUser.Name,
                Surname = existingUser.Surname,
                Email = existingUser.Email
            };

            return Ok(response);
        }
        


        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            await _userRepository.DeleteUserAsync(id);

            return NoContent();
        }

    }
}
