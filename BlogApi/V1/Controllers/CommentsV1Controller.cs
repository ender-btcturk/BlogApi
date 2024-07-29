using Asp.Versioning;
using BlogApi.Data;
using BlogApi.Data.Entities;
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
                var response = new QueryCommentsResponse
                {
                    Id = comment.Id,
                    Content = comment.Content,
                    CreatedAt = comment.CreatedAt,
                    AuthorId = comment.AuthorId,
                    PostId = comment.PostId
                };
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

            var newComment = new Comment
            {
                Content = request.Content,
                CreatedAt = request.CreatedAt,
                AuthorId = request.AuthorId,
                PostId = request.PostId
            };

            await _blogDbContext.Comments.AddAsync(newComment);
            await _blogDbContext.SaveChangesAsync();

            newComment = await _blogDbContext.Comments.FirstOrDefaultAsync(p => p.PostId == request.PostId);

            CreateCommentResponse response = new CreateCommentResponse()
            {
                Id = newComment.Id,
                Content = newComment.Content,
                CreatedAt = newComment.CreatedAt,
                AuthorId = newComment.AuthorId,
                PostId = newComment.PostId
            };

            return Created(new Uri(response.Id.ToString(), UriKind.Relative), response);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCommentResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetComment([FromRoute] int id)
        {
            var comment = await _blogDbContext.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            GetCommentResponse response = new GetCommentResponse()
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                AuthorId = comment.AuthorId,
                PostId = comment.PostId
            };

            return Ok(response);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdatePostResponse))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UpdateCommentResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> UpdateComment([FromRoute] int id, [FromBody] UpdateCommentRequest request)
        {
            var existingComment = await _blogDbContext.Comments.FindAsync(id);

            if (existingComment == null)
            {
                existingComment = new Comment()
                {
                    Content = request.Content,
                    CreatedAt = request.CreatedAt,
                    AuthorId = request.AuthorId,
                    PostId = request.PostId
                };

                await _blogDbContext.Comments.AddAsync(existingComment);
                await _blogDbContext.SaveChangesAsync();

                return Created(new Uri(existingComment.Id.ToString(), UriKind.Relative), existingComment);
            }

            existingComment.Id = id;
            existingComment.Content = request.Content;
            existingComment.CreatedAt = request.CreatedAt;
            existingComment.AuthorId = request.AuthorId;
            existingComment.PostId = request.PostId;

            await _blogDbContext.SaveChangesAsync();

            UpdateCommentResponse response = new UpdateCommentResponse()
            {
                Id = existingComment.Id,
                Content = existingComment.Content,
                CreatedAt = existingComment.CreatedAt,
                AuthorId = existingComment.AuthorId,
                PostId = existingComment.PostId
            };

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
