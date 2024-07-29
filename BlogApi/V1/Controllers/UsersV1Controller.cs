using Asp.Versioning;
using BlogApi.Data;
using BlogApi.Data.Entities;
using BlogApi.Mapping;
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

            results = await _blogDbContext.Users
                .Select(user => user.ToQueryUsersResponse())
                .AsNoTracking()
                .ToListAsync();

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

            await _blogDbContext.Users.AddAsync(newUser);
            await _blogDbContext.SaveChangesAsync();


            return Created(new Uri(newUser.Id.ToString(), UriKind.Relative),
                           newUser.ToCreateUserResponse());
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
            var existingUser = await _blogDbContext.Users.FindAsync(id);

            if (existingUser == null)
            {
                User newUser = request.ToEntity();

                await _blogDbContext.Users.AddAsync(newUser);
                await _blogDbContext.SaveChangesAsync();

                return Created(new Uri(newUser.Id.ToString(), UriKind.Relative),
                               newUser.ToUpdateUserResponse());
            }

            _blogDbContext.Entry(existingUser)
                .CurrentValues
                .SetValues(request.ToEntity(id));
            await _blogDbContext.SaveChangesAsync();

            UpdateUserResponse response = existingUser.ToUpdateUserResponse();

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
