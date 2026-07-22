using Moq;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Tests.TestData;

namespace QuestionService.Tests.Mocks;

internal static class EntityProviderMocks
{
    public static IMock<IEntityProvider<UserDto>> GetMockUserProvider()
    {
        var mockProvider = new Mock<IEntityProvider<UserDto>>();

        mockProvider.Setup(x => x.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((long userId, CancellationToken _) =>
                UserDtoMother.GetUserDtos().FirstOrDefault(x => x.Id == userId));
        mockProvider.Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<long>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IEnumerable<long> userIds, CancellationToken _) =>
                UserDtoMother.GetUserDtos().Where(x => userIds.Contains(x.Id)));

        return mockProvider;
    }
}
