using FluentResults;

namespace DocsManager.Services.Errors;

public class NotFoundError : Error
{
    public NotFoundError(string model) : base($"{model} not found")
    {
        Metadata.Add("ResultCode", DuplicationResultCode.NotFound);
    }
}