using Asp.Versioning;
using Azure;
using BlogApi.Data;
using BlogApi.Data.Entities;
using BlogApi.Interfaces;
using BlogApi.Mapping;
using BlogApi.V1.Requests.Post;
using BlogApi.V1.Responses.Post;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.V1.Controllers
{
    
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/posts")]
    [ApiExplorerSettings(GroupName = "Posts")]
    public class PostsV1Controller : ControllerBase
    {

        private readonly IPostRepository _postRepository;
        public PostsV1Controller(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(QueryPostsResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> QueryPosts([FromQuery] QueryPostsRequest request,
                                                       [FromQuery] string sort = "",
                                                       [FromQuery] int pageNumber = 1,
                                                       [FromQuery] int pageSize = 5)
        {
            List<QueryPostsResponse> results = new List<QueryPostsResponse>();

            List<Post> posts = await _postRepository.GetAllPostsAsync(sort);

            foreach (var post in posts)
            {
                var response = post.ToQueryPostsResponse();
                results.Add(response);
            }

            return Ok(results.Skip((pageNumber - 1) * pageSize).Take(pageSize));
        }

        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreatePostResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
        {

            Post newPost = request.ToEntity();

            await _postRepository.CreatePostAsync(newPost);

            return Created(new Uri(newPost.Id.ToString(), UriKind.Relative)
                           ,newPost.ToCreatePostResponse());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetPostResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPost([FromRoute] int id)
        {
            Post? post = await _postRepository.GetPostByIdAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            GetPostResponse response = post.ToGetPostResponse();

            return Ok(response);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdatePostResponse))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UpdatePostResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> UpdatePost([FromRoute] int id, [FromBody] UpdatePostRequest request)
        {
            Post? existingPost = await _postRepository.UpdatePostAsync(request, id);

            if (existingPost == null)
            {
                Post newPost = request.ToEntity();

                await _postRepository.CreatePostAsync(newPost);

                return Created(new Uri(newPost.Id.ToString(), UriKind.Relative)
                               ,newPost.ToUpdatePostResponse());
            }

            UpdatePostResponse response = existingPost.ToUpdatePostResponse();

            return Ok(response);
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PatchPostResponse))]
        public async Task<IActionResult> PatchPost([FromRoute] int id, [FromBody] JsonPatchDocument<PatchPostRequest> request)
        {
            Post? existingPost = await _postRepository.GetPostByIdAsync(id);
            if (existingPost == null)
            {
                return NotFound();
            }

            var patchedPost = new PatchPostRequest
            {
                Title = existingPost.Title,
                Content = existingPost.Content,
                CreatedAt = existingPost.CreatedAt,
                AuthorId = existingPost.AuthorId
            };

            request.ApplyTo(patchedPost, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            existingPost.Title = patchedPost.Title;
            existingPost.Content = patchedPost.Content;
            existingPost.CreatedAt = patchedPost.CreatedAt;
            existingPost.AuthorId = patchedPost.AuthorId;
            _postRepository.SaveChangesAsync();

            var response = new PatchPostResponse
            {
                Id = existingPost.Id,
                Title = existingPost.Title,
                Content = existingPost.Content,
                CreatedAt = existingPost.CreatedAt,
                AuthorId = existingPost.AuthorId
            };

            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> DeletePost([FromRoute] int id)
        {
            await _postRepository.DeletePostAsync(id);

            return NoContent();
        }
    }
}
