using QuestionService.Domain.Enums;

namespace QuestionService.Domain.Dtos.Pagination;

public record SortOrder(string Field, SortDirection Direction);