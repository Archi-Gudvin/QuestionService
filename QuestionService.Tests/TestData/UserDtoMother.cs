using QuestionService.Domain.Dtos.ExternalEntity;

namespace QuestionService.Tests.TestData;

internal static class UserDtoMother
{
    public const int MinReputation = 1;

    public static IQueryable<UserDto> GetUserDtos()
    {
        return new UserDto[]
        {
            new()
            {
                Id = 1,
                IdentityId = Guid.NewGuid(),
                Username = "testuser1",
                Email = "TestUser1@test.com",
                LastLoginAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Reputation = 125,
                Roles = [GetRoleUser(), GetRoleAdmin()]
            },
            new()
            {
                Id = 2,
                IdentityId = Guid.NewGuid(),
                Username = "testuser2",
                Email = "TestUser2@test.com",
                LastLoginAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Reputation = 15,
                Roles = [GetRoleUser()]
            },
            new()
            {
                Id = 3,
                IdentityId = Guid.NewGuid(),
                Username = "testuser3",
                Email = "TestUser3@test.com",
                LastLoginAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Reputation = MinReputation,
                Roles = [GetRoleModer()]
            },
            new()
            {
                Id = 4,
                IdentityId = Guid.NewGuid(),
                Username = "testuser4",
                Email = "TestUser4@test.com",
                LastLoginAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Reputation = 125,
                Roles = [GetRoleModer()]
            }
        }.AsQueryable();
    }

    private static RoleDto GetRoleUser()
    {
        return new RoleDto
        {
            Id = 1,
            Name = "User"
        };
    }

    private static RoleDto GetRoleAdmin()
    {
        return new RoleDto
        {
            Id = 2,
            Name = "Admin"
        };
    }

    private static RoleDto GetRoleModer()
    {
        return new RoleDto
        {
            Id = 3,
            Name = "Moderator"
        };
    }
}
