using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AskalePortal.API.Infrastructure.Errors;

public static class ApiValidation
{
    public static IReadOnlyDictionary<string, string[]> ToErrors(ModelStateDictionary modelState)
    {
        return modelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(
                entry => NormalizeFieldName(entry.Key),
                entry => entry.Value!.Errors
                    .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                        ? "Alan değeri geçersiz."
                        : error.ErrorMessage)
                    .Distinct(StringComparer.Ordinal)
                    .ToArray(),
                StringComparer.OrdinalIgnoreCase);
    }

    private static string NormalizeFieldName(string fieldName)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            return "request";
        }

        var separatorIndex = fieldName.LastIndexOf('.');
        var value = separatorIndex >= 0 ? fieldName[(separatorIndex + 1)..] : fieldName;
        if (string.IsNullOrWhiteSpace(value))
        {
            return "request";
        }

        return char.ToLowerInvariant(value[0]) + value[1..];
    }
}
