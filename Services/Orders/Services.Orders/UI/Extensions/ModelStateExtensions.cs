using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Services.Orders.UI.Extensions;

public static class ModelStateExtensions
{
    public static string GetErrorsAsString(this ModelStateDictionary modelState)
        => modelState.ErrorCount > 0 ?
            $"- {string.Join("\n- ", modelState.Select(x => x.Value?.Errors))}" : "";
}
