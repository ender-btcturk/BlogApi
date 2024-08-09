using BlogApi.Data;
using BlogApi.Data.Entities;
using BlogApi.Interfaces;
using BlogApi.V1.Controllers;
using BlogApi.V1.Requests.User;
using BlogApi.V1.Responses.User;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.Tests.Controller
{
    public class UsersV1ControllerTests
    {
        private readonly IUserRepository _userRepository;
        public UsersV1ControllerTests()
        {
            _userRepository = A.Fake<IUserRepository>();
        }

        [Fact]
        public async void UsersV1Controller_QueryUsers_ReturnOK()
        {
            var request = A.Fake<QueryUsersRequest>();
            var controller = new UsersV1Controller(_userRepository);

            var result = await controller.QueryUsers(request);

            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async void UsersV1Controller_CreateUser_ReturnCreated()
        {
            var request = A.Fake<CreateUserRequest>();
            var controller = new UsersV1Controller(_userRepository);

            var result = await controller.CreateUser(request);

            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(CreatedResult));
        }

        [Fact]
        public async void UsersV1Controller_GetUser_ReturnOK()
        {
            int id = 1;
            var controller = new UsersV1Controller(_userRepository);

            var result = await controller.GetUser(id);

            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async void UsersV1Controller_UpdateUser_ReturnOK()
        {
            int id = 1;
            var request = A.Fake<UpdateUserRequest>();
            var controller = new UsersV1Controller(_userRepository);

            var result = await controller.UpdateUser(id, request);

            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async void UsersV1Controller_DeleteUser_ReturnNoContent()
        {
            int id = 1;
            var controller = new UsersV1Controller(_userRepository);

            var result = await controller.DeleteUser(id);

            result.Should().BeOfType(typeof(NoContentResult));
        }
    }
}
