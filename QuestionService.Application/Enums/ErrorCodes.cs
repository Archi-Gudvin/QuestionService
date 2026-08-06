namespace QuestionService.Application.Enums;

public enum ErrorCodes
{
    //Data: 1-10
    //Reputation: 11-20
    //User: 21-30
    //Authorization: 31-40
    //Question: 41-50
    //Tags: 51-60
    //Views: 61-70
    //Vote: 71-80
    //VoteType: 81-90

    InvalidProperty = 1,
    InvalidDataFormat = 2,

    UserNotFound = 21,

    OperationForbidden = 31,

    QuestionNotFound = 41,
    QuestionsNotFound = 42,

    TagsNotFound = 51,
    TagNotFound = 52,
    TagAlreadyExists = 53,

    ViewNotFound = 61,
    ViewsNotFound = 62,

    VoteAlreadyGiven = 71,
    VoteNotFound = 72,
    VotesNotFound = 73,
    VoteTypeNotFound = 74,
    VoteTypesNotFound = 75,
    CannotVoteForOwnPost = 76
}