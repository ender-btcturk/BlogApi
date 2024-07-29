using Asp.Versioning;
using BlogApi.Data;
using BlogApi.Data.Entities;
using BlogApi.Mapping;
using BlogApi.V1.Requests.Comment;
using BlogApi.V1.Requests.Post;
using BlogApi.V1.Responses.Comment;
using BlogApi.V1.Responses.Post;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/comments")]
    [ApiExplorerSettings(GroupName = "Comments")]
    public class CommentsV1Controller : ControllerBase
    {
        private readonly BlogDbContext _blogDbContext;

        public CommentsV1Controller(BlogDbContext blogDbContext)
        {
            _blogDbContext = blogDbContext;
        }

        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(QueryCommentsResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> QueryComments([FromQuery] QueryCommentsRequest request,
                                                       [FromQuery] string sort = "",
                                                       [FromQuery] int pageNumber = 1,
                                                       [FromQuery] int pageSize = 5)
        {
            List<QueryCommentsResponse> results = new List<QueryCommentsResponse>();

            IQueryable<Comment> comments;

            switch (sort)
            {
                case "desc":
                    comments = _blogDbContext.Comments.OrderByDescending(p => p.CreatedAt);
                    break;
                case "asc":
                    comments = _blogDbContext.Comments.OrderBy(p => p.CreatedAt);
                    break;
                default:
                    comments = _blogDbContext.Comments;
                    break;
            }

            foreach (var comment in comments)
            {
                var response = comment.ToQueryCommentResponse();
                results.Add(response);
            }

            return Ok(results.Skip((pageNumber - 1)*pageSize).Take(pageSize));
        }

        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateCommentResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest request)
        {

            var newComment = request.ToEntity();

            await _blogDbContext.Comments.AddAsync(newComment);
            await _blogDbContext.SaveChangesAsync();

            return Created(new Uri(newComment.Id.ToString(), UriKind.Relative),
                           newComment.ToCreateCommentResponse());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCommentResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetComment([FromRoute] int id)
        {
            Comment comment = await _blogDbContext.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            GetCommentResponse response = comment.ToGetCommentResponse();

            return Ok(response);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdatePostResponse))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UpdateCommentResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> UpdateComment([FromRoute] int id, [FromBody] UpdateCommentRequest request)
        {
            Comment existingComment = await _blogDbContext.Comments.FindAsync(id);

            if (existingComment == null)
            {
                Comment newComment = request.ToEntity();

                await _blogDbContext.Comments.AddAsync(newComment);
                await _blogDbContext.SaveChangesAsync();

                return Created(new Uri(newComment.Id.ToString(), UriKind.Relative),
                               newComment.ToUpdateCommentResponse());
            }

            _blogDbContext.Comments
                          .Entry(existingComment)
                          .CurrentValues
                          .SetValues(request.ToEntity(id));
            await _blogDbContext.SaveChangesAsync();

            UpdateCommentResponse response = existingComment.ToUpdateCommentResponse();

            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> DeleteComment([FromRoute] int id)
        {
            var comment = await _blogDbContext.Comments.FindAsync(id);
            if (comment == null)
            {
                return NoContent();
            }

            _blogDbContext.Comments.Remove(comment);
            await _blogDbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
