using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebClient.Utilities;

public static class ModelStateExtensions
{
    public static string GetErrorsAsString(this ModelStateDictionary modelState)
        => modelState.ErrorCount > 0 ?
            $"- {string.Join("\n- ", modelState.SelectMany(x => x.Value?.Errors).Select(x => x.ErrorMessage))}" : "";

    public static string GetErrorsAsHtml(this ModelStateDictionary modelState)
        => modelState.ErrorCount > 0 ?
            $"<br />- {string.Join("<br />- ", modelState.SelectMany(x => x.Value?.Errors).Select(x => x.ErrorMessage))}" : "";
}
