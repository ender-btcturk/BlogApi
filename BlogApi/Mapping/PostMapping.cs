using BlogApi.Data.Entities;
using BlogApi.V1.Requests.Post;
using BlogApi.V1.Responses.Post;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BlogApi.Mapping
{
    public static class PostMapping
    {
        public static Post ToEntity(this CreatePostRequest request)
        {
            return new Post
            {
                Title = request.Title,
                Content = request.Content,
                CreatedAt = request.CreatedAt,
                AuthorId = request.AuthorId
            };
        }

        public static Post ToEntity(this UpdatePostRequest request, int id = 0)
        {
            return new Post
            {
                Id = id,
                Title = request.Title,
                Content = request.Content,
                CreatedAt = request.CreatedAt,
                AuthorId = request.AuthorId
            };
        }
        public static QueryPostsResponse ToQueryPostsResponse(this Post post)
        {
            return new QueryPostsResponse
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                AuthorId = post.AuthorId
            };
        }

        public static CreatePostResponse ToCreatePostResponse(this Post post)
        {
            return new CreatePostResponse
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                AuthorId = post.AuthorId
            };
        }

        public static GetPostResponse ToGetPostResponse(this Post post)
        {
            return new GetPostResponse
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                AuthorId = post.AuthorId
            };
        }

        public static UpdatePostResponse ToUpdatePostResponse(this Post post)
        {
            return new UpdatePostResponse
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                AuthorId = post.AuthorId
            };
        }
    }
}
