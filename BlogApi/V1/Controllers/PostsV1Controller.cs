using Asp.Versioning;
using Azure;
using BlogApi.Data;
using BlogApi.Data.Entities;
using BlogApi.Mapping;
using BlogApi.V1.Requests.Post;
using BlogApi.V1.Requests.User;
using BlogApi.V1.Responses.Post;
using BlogApi.V1.Responses.User;
using Microsoft.AspNetCore.Http;
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

        private readonly BlogDbContext _blogDbContext;
        public PostsV1Controller(BlogDbContext blogDbContext)
        {
            _blogDbContext = blogDbContext;
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

            IQueryable<Post> posts;

            switch(sort)
            {
                case "desc":
                    posts = _blogDbContext.Posts.OrderByDescending(p => p.CreatedAt);
                    break;
                case "asc":
                    posts = _blogDbContext.Posts.OrderBy(p => p.CreatedAt);
                    break;
                default:
                    posts = _blogDbContext.Posts;
                    break;
            }

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

            await _blogDbContext.Posts.AddAsync(newPost);
            await _blogDbContext.SaveChangesAsync();

            return Created(new Uri(newPost.Id.ToString(), UriKind.Relative),
                           newPost.ToCreatePostResponse());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetPostResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPost([FromRoute] int id)
        {
            Post post = await _blogDbContext.Posts.FindAsync(id);

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
            Post existingPost = await _blogDbContext.Posts.FindAsync(id);

            if (existingPost == null)
            {
                Post newPost = request.ToEntity();

                await _blogDbContext.Posts.AddAsync(newPost);
                await _blogDbContext.SaveChangesAsync();

                return Created(new Uri(newPost.Id.ToString(), UriKind.Relative),
                               newPost.ToUpdatePostResponse());
            }

            _blogDbContext.Posts
                          .Entry(existingPost)
                          .CurrentValues
                          .SetValues(request.ToEntity(id));
            await _blogDbContext.SaveChangesAsync();

            UpdatePostResponse response = existingPost.ToUpdatePostResponse();

            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> DeletePost([FromRoute] int id)
        {
            var post = await _blogDbContext.Posts.FindAsync(id);
            if (post == null)
            {
                return NoContent();
            }

            _blogDbContext.Posts.Remove(post);
            await _blogDbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
