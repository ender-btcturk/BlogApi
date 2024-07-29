using BlogApi.Data.Entities;
using BlogApi.V1.Requests.User;
using BlogApi.V1.Responses.User;

namespace BlogApi.Mapping
{
    public static class UserMapping
    {
        public static User ToEntity(this CreateUserRequest request)
        {
            return new User
            {
                Name = request.Name,
                Surname = request.Surname,
                Email = request.Email
            };
        }

        public static User ToEntity(this UpdateUserRequest request, int id = 0)
        {
            return new User
            {
                Id = id,
                Name = request.Name,
                Surname = request.Surname,
                Email = request.Email
            };
        }
        public static QueryUsersResponse ToQueryUsersResponse(this User user)
        {
            return new QueryUsersResponse
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email
            };
        }

        public static CreateUserResponse ToCreateUserResponse(this User user)
        {
            return new CreateUserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email
            };
        }

        public static GetUserResponse ToGetUserResponse(this User user)
        {
            return new GetUserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email
            };
        }

        public static UpdateUserResponse ToUpdateUserResponse(this User user)
        {
            return new UpdateUserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email
            };
        }
    }
}
