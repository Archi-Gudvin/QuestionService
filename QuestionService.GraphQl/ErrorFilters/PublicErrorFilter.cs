using System.Diagnostics;
using Serilog;
using QuestionService.Application.Resources;
using QuestionService.GraphQl.Helpers;

namespace QuestionService.GraphQl.ErrorFilters;

public class PublicErrorFilter(ILogger logger) : IErrorFilter
{
    private const string UnexpectedErrorMessage = "Unexpected Execution Error";

    public IError OnError(IError error)
    {
        if (error.Extensions != null
            && error.Extensions.TryGetValue(GraphQlExceptionHelper.IsBusinessErrorExtension, out var value)
            && value is true)
            return error.RemoveExtension(GraphQlExceptionHelper.IsBusinessErrorExtension).WithMessage(error.Message);

        if (!error.Message.StartsWith(UnexpectedErrorMessage)) return error;

        var traceId = Activity.Current?.TraceId.ToHexString();
        logger.Error(error.Exception, "Unhandled GraphQL error {Message}", error.Message);

        return error.WithMessage(traceId is null
            ? ErrorMessage.InternalServerError
            : $"{ErrorMessage.InternalServerError} (ref: {traceId})");
    }
}