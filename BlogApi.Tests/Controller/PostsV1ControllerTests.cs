using BlogApi.Interfaces;
using BlogApi.V1.Controllers;
using BlogApi.V1.Requests.Post;
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
    public class PostsV1ControllerTests
    {
        private readonly IPostRepository _postRepository;
        public PostsV1ControllerTests()
        {
            _postRepository = A.Fake<IPostRepository>();
        }

        [Fact]
        public async void PostsV1Controller_QueryPosts_ReturnOK()
        {
            var request = A.Fake<QueryPostsRequest>();
            string sort = string.Empty;
            var controller = new PostsV1Controller(_postRepository);

            var result = await controller.QueryPosts(request, sort);

            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async void PostsV1Controller_CreatePost_ReturnCreated()
        {
            var request = A.Fake<CreatePostRequest>();
            var controller = new PostsV1Controller(_postRepository);

            var result = await controller.CreatePost(request);

            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(CreatedResult));
        }

        [Fact]
        public async void PostsV1Controller_GetPost_ReturnOK()
        {
            int id = 1;
            var controller = new PostsV1Controller(_postRepository);

            var result = await controller.GetPost(id);

            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async void PostsV1Controller_UpdatePost_ReturnOK()
        {
            int id = 1;
            var request = A.Fake<UpdatePostRequest>();
            var controller = new PostsV1Controller(_postRepository);

            var result = await controller.UpdatePost(id, request);

            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async void PostsV1Controller_PatchPost_ReturnOK()
        {
            int id = 1;
            var request = A.Fake<JsonPatchDocument<PatchPostRequest>>();
            var controller = new PostsV1Controller(_postRepository);

            var result = await controller.PatchPost(id, request);

            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async void PostsV1Controller_DeletePost_ReturnNoContent()
        {
            int id = 1;
            var controller = new PostsV1Controller(_postRepository);

            var result = await controller.DeletePost(id);

            result.Should().BeOfType(typeof(NoContentResult));
        }
    }
}
