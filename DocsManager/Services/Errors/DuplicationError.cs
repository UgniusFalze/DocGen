using FluentResults;

namespace DocsManager.Services.Errors;

public class DuplicationError : Error
{
    public DuplicationError(string duplicationModel, string existingColumn) :
        base($"{duplicationModel} with {existingColumn} already exists")
    {
        Metadata.Add("ResultCode", DuplicationResultCode.Duplication);
    }
}