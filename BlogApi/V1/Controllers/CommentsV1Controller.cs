using Asp.Versioning;
using BlogApi.Data;
using BlogApi.Data.Entities;
using BlogApi.Interfaces;
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
        private readonly ICommentRepository _commentRepository;

        public CommentsV1Controller(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
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

            List<Comment> comments = await _commentRepository.GetAllCommentsAsync(sort);

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

            Comment newComment = request.ToEntity();

            await _commentRepository.CreateCommentAsync(newComment);

            return Created(new Uri(newComment.Id.ToString(), UriKind.Relative)
                           ,newComment.ToCreateCommentResponse());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCommentResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetComment([FromRoute] int id)
        {
            Comment? comment = await _commentRepository.GetCommentByIdAsync(id);

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
            Comment? existingComment = await _commentRepository.UpdateCommentAsync(request, id);

            if (existingComment == null)
            {
                Comment newComment = request.ToEntity();

                await _commentRepository.CreateCommentAsync(newComment);

                return Created(new Uri(newComment.Id.ToString(), UriKind.Relative)
                               ,newComment.ToUpdateCommentResponse());
            }

            UpdateCommentResponse response = existingComment.ToUpdateCommentResponse();

            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> DeleteComment([FromRoute] int id)
        {
            await _commentRepository.DeleteCommentAsync(id);

            return NoContent();
        }
    }
}
