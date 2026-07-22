using MockQueryable.Moq;
using Moq;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Database;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Tests.TestData;
using View = QuestionService.Domain.Entities.View;

namespace QuestionService.Tests.Mocks;

internal static class RepositoryMocks
{
    private static IMock<ITransaction> GetMockTransaction()
    {
        return new Mock<ITransaction>();
    }

    public static IMock<IUnitOfWork> GetMockUnitOfWork()
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        mockUnitOfWork.Setup(x => x.Questions).Returns(GetMockQuestionRepository().Object);
        mockUnitOfWork.Setup(x => x.Votes).Returns(GetMockVoteRepository().Object);
        mockUnitOfWork.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetMockTransaction().Object);

        return mockUnitOfWork;
    }

    public static IMock<IBaseRepository<Question>> GetMockQuestionRepository()
    {
        var mockRepository = new Mock<IBaseRepository<Question>>();
        var questions = QuestionMother.GetQuestions().BuildMockDbSet();

        mockRepository.Setup(x => x.GetAll()).Returns(questions.Object);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<Question>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Question question, CancellationToken _) => question);
        mockRepository.Setup(x => x.Remove(It.IsAny<Question>())).Returns((Question question) => question);
        mockRepository.Setup(x => x.Update(It.IsAny<Question>())).Returns((Question question) => question);

        return mockRepository;
    }

    public static IMock<IBaseRepository<Vote>> GetMockVoteRepository()
    {
        var mockRepository = new Mock<IBaseRepository<Vote>>();
        var votes = VoteMother.GetVotes().BuildMockDbSet();

        mockRepository.Setup(x => x.GetAll()).Returns(votes.Object);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<Vote>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vote vote, CancellationToken _) => vote);
        mockRepository.Setup(x => x.Remove(It.IsAny<Vote>())).Returns((Vote vote) => vote);
        mockRepository.Setup(x => x.Update(It.IsAny<Vote>())).Returns((Vote vote) => vote);

        return mockRepository;
    }

    public static IMock<IBaseRepository<VoteType>> GetMockVoteTypeRepository()
    {
        var mockRepository = new Mock<IBaseRepository<VoteType>>();
        var voteTypes = VoteTypeMother.GetVoteTypes().BuildMockDbSet();

        mockRepository.Setup(x => x.GetAll()).Returns(voteTypes.Object);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<VoteType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((VoteType voteType, CancellationToken _) => voteType);
        mockRepository.Setup(x => x.Remove(It.IsAny<VoteType>())).Returns((VoteType voteType) => voteType);
        mockRepository.Setup(x => x.Update(It.IsAny<VoteType>())).Returns((VoteType voteType) => voteType);

        return mockRepository;
    }

    public static IMock<IBaseRepository<Tag>> GetMockTagRepository()
    {
        var mockRepository = new Mock<IBaseRepository<Tag>>();

        // Adding questions to tags

        var tags = TagMother.GetTags().ToList();
        tags.ForEach(x => x.Questions = []);
        var questions = QuestionMother.GetQuestions().ToList();
        var questionTags = TagMother.GetQuestionTags().ToList();

        foreach (var questionTag in questionTags)
        {
            var tag = tags.First(x => x.Id == questionTag.TagId);
            var question = questions.First(x => x.Id == questionTag.QuestionId);

            tag.Questions.Add(question);
        }


        var tagsDbSet = tags.BuildMockDbSet();

        mockRepository.Setup(x => x.GetAll()).Returns(tagsDbSet.Object);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<Tag>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tag tag, CancellationToken _) => tag);
        mockRepository.Setup(x => x.Remove(It.IsAny<Tag>())).Returns((Tag tag) => tag);
        mockRepository.Setup(x => x.Update(It.IsAny<Tag>())).Returns((Tag tag) => tag);

        return mockRepository;
    }

    public static IMock<IBaseRepository<View>> GetMockViewRepository()
    {
        var mockRepository = new Mock<IBaseRepository<View>>();

        var views = ViewMother.GetViews().BuildMockDbSet();

        mockRepository.Setup(x => x.GetAll()).Returns(views.Object);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<View>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((View view, CancellationToken _) => view);
        mockRepository.Setup(x => x.Remove(It.IsAny<View>())).Returns((View view) => view);
        mockRepository.Setup(x => x.Update(It.IsAny<View>())).Returns((View view) => view);

        return mockRepository;
    }

    public static IMock<IBaseRepository<T>> GetEmptyMockRepository<T>() where T : class
    {
        var mockRepository = new Mock<IBaseRepository<T>>();
        var entities = Array.Empty<T>().BuildMockDbSet();

        mockRepository.Setup(x => x.GetAll()).Returns(entities.Object);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((T entity, CancellationToken _) => entity);
        mockRepository.Setup(x => x.Update(It.IsAny<T>())).Returns((T entity) => entity);
        mockRepository.Setup(x => x.Remove(It.IsAny<T>())).Returns((T entity) => entity);

        return mockRepository;
    }
}
