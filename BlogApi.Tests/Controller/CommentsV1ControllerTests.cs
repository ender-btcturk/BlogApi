using BlogApi.Data.Entities;
using BlogApi.Interfaces;
using BlogApi.Repository;
using BlogApi.V1.Controllers;
using BlogApi.V1.Requests.Comment;
using BlogApi.V1.Requests.Post;
using BlogApi.V1.Responses.Comment;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.Tests.Controller
{
    public class CommentsV1ControllerTests
    {
        private readonly ICommentRepository _commentRepository;
        public CommentsV1ControllerTests()
        {
            _commentRepository = A.Fake<ICommentRepository>();
        }

        [Fact]
        public async void CommentsV1Controller_QueryComments_ReturnOK()
        {
            var request = A.Fake<QueryCommentsRequest>();
            string sort = string.Empty;
            var controller = new CommentsV1Controller(_commentRepository);

            var result = await controller.QueryComments(request, sort);

            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async void CommentsV1Controller_CreateComment_ReturnCreated()
        {
            var request = A.Fake<CreateCommentRequest>();
            var controller = new CommentsV1Controller(_commentRepository);

            var result = await controller.CreateComment(request);

            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(CreatedResult));
        }

        [Fact]
        public async void CommentsV1Controller_GetComment_ReturnOK()
        {
            int id = 1;
            var controller = new CommentsV1Controller(_commentRepository);

            var result = await controller.GetComment(id);

            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async void CommentsV1Controller_UpdateComment_ReturnOK()
        {
            int id = 1;
            var request = A.Fake<UpdateCommentRequest>();
            var controller = new CommentsV1Controller(_commentRepository);

            var result = await controller.UpdateComment(id, request);

            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async void CommentsV1Controller_PatchComment_ReturnOK()
        {
            int id = 1;
            var request = A.Fake<JsonPatchDocument<PatchCommentRequest>>();
            var controller = new CommentsV1Controller(_commentRepository);

            var result = await controller.PatchComment(id, request);

            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async void CommentsV1Controller_DeleteComment_ReturnNoContent()
        {
            int id = 1;
            var controller = new CommentsV1Controller(_commentRepository);

            var result = await controller.DeleteComment(id);

            result.Should().BeOfType(typeof(NoContentResult));
        }
    }
}
