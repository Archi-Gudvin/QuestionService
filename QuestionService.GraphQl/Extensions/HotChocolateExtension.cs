using HotChocolate.Language;
using QuestionService.Domain.Dtos.Page;
using QuestionService.Domain.Enums;

namespace QuestionService.GraphQl.Extensions;

public static class HotChocolateExtension
{
    public static IEnumerable<OrderDto> ToOrderDto(this ListValueNode? listValueNode)
    {
        if (listValueNode == null) return [];

        return
        [
            .. listValueNode.Items
                .Select(item => item as ObjectValueNode
                                ?? throw new ArgumentException($"Item must be of type {nameof(ObjectValueNode)}."))
                .SelectMany(objectValueNode => objectValueNode.Fields.Select(ParseOrderFromField))
        ];
    }

    private static OrderDto ParseOrderFromField(ObjectFieldNode field)
    {
        var columnName = field.Name.Value;

        if (field.Value is not EnumValueNode directionEnumNode)
            throw new ArgumentException($"The direction value for field '{columnName}' must be an enum (ASC or DESC).");

        var directionString = directionEnumNode.Value;

        if (!Enum.TryParse<SortDirection>(directionString, ignoreCase: true, out var direction))
            throw new ArgumentException($"Invalid sort direction '{directionString}' for field '{columnName}'.");

        return new OrderDto(columnName, direction);
    }
}