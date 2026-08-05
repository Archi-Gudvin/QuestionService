using AutoMapper;
using QuestionService.Application.Mappings;

namespace QuestionService.Tests.UnitTests.Fixtures;

internal static class MapperFixture
{
    public static IMapper GetMapperConfiguration()
    {
        var mockMapper = new AutoMapper.MapperConfiguration(cfg => cfg.AddMaps(typeof(QuestionMapping)));
        return mockMapper.CreateMapper();
    }
}