using Azure.Core;
using BlogApi.Data.Entities;
using BlogApi.V1.Requests.Comment;
using BlogApi.V1.Responses.Comment;

namespace BlogApi.Mapping
{
    public static class CommentMapping
    {
        public static Comment ToEntity(this CreateCommentRequest request)
        {
            return new Comment
            {
                Content = request.Content,
                CreatedAt = request.CreatedAt,
                AuthorId = request.AuthorId,
                PostId = request.PostId
            };
        }

        public static Comment ToEntity(this UpdateCommentRequest request, int id = 0)
        {
            return new Comment
            {
                Id = id,
                Content = request.Content,
                CreatedAt = request.CreatedAt,
                AuthorId = request.AuthorId,
                PostId = request.PostId
            };
        }

        public static QueryCommentsResponse ToQueryCommentResponse(this Comment comment)
        {
            return new QueryCommentsResponse
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                AuthorId = comment.AuthorId,
                PostId = comment.PostId
            };
        }
        public static CreateCommentResponse ToCreateCommentResponse(this Comment comment)
        {
            return new CreateCommentResponse
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                AuthorId = comment.AuthorId,
                PostId = comment.PostId
            };
        }
        public static GetCommentResponse ToGetCommentResponse(this Comment comment)
        {
            return new GetCommentResponse
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                AuthorId = comment.AuthorId,
                PostId = comment.PostId
            };
        }
        public static UpdateCommentResponse ToUpdateCommentResponse(this Comment comment)
        {
            return new UpdateCommentResponse
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                AuthorId = comment.AuthorId,
                PostId = comment.PostId
            };
        }
    }
}
