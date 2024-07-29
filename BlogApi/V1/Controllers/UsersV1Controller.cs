using Asp.Versioning;
using BlogApi.Data;
using BlogApi.Data.Entities;
using BlogApi.V1.Requests.User;
using BlogApi.V1.Responses.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
        private readonly BlogDbContext _blogDbContext;
        public UsersV1Controller(BlogDbContext blogDbContext)
        {
            _blogDbContext = blogDbContext;
        }

        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<QueryUsersResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> QueryUsers([FromQuery] QueryUsersRequest request)
        {
            List<QueryUsersResponse> results = new List<QueryUsersResponse>();

            var users = await _blogDbContext.Users.ToListAsync();

            foreach (var user in users)
            {
                var response = new QueryUsersResponse
                {
                    Id = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email
                };
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

            var newUser = new User()
            {
                Name = request.Name,
                Surname = request.Surname,
                Email = request.Email
            };

            await _blogDbContext.Users.AddAsync(newUser);
            await _blogDbContext.SaveChangesAsync();

            newUser = await _blogDbContext.Users.FirstOrDefaultAsync(
                u => u.Email == request.Email);

            CreateUserResponse response = new CreateUserResponse
            {
                Id = newUser.Id,
                Name = newUser.Name,
                Surname = newUser.Surname,
                Email = newUser.Email
            };

            return Created(new Uri(response.Id.ToString(), UriKind.Relative), response);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUser([FromRoute] int id)
        {
            var user = await _blogDbContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            GetUserResponse response = new GetUserResponse()
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email
            };

            return Ok(response);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateUserResponse))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UpdateUserResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] UpdateUserRequest request)
        {
            var existingUser = await _blogDbContext.Users.FindAsync(id);

            if (existingUser == null)
            {
                existingUser = new User
                {
                    Name = request.Name,
                    Surname = request.Surname,
                    Email = request.Email

                };

                await _blogDbContext.Users.AddAsync(existingUser);
                await _blogDbContext.SaveChangesAsync();

                return Created(new Uri(existingUser.Id.ToString(), UriKind.Relative), existingUser);
            }

            existingUser.Id = id;
            existingUser.Name = request.Name;
            existingUser.Surname = request.Surname;
            existingUser.Email = request.Email;

            await _blogDbContext.SaveChangesAsync();

            UpdateUserResponse response = new UpdateUserResponse()
            {
                Id = id,
                Name = request.Name,
                Surname = request.Surname,
                Email = request.Email
            };

            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            var user = await _blogDbContext.Users.FindAsync(id);
            if(user == null)
            {
                return NoContent();
            }

            _blogDbContext.Users.Remove(user);
            await _blogDbContext.SaveChangesAsync();

            return NoContent();
        }

    }
}
