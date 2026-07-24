namespace QuestionService.Domain.Dtos.Pagination;

public record OffsetPaginationParams(int? Skip, int? Take);