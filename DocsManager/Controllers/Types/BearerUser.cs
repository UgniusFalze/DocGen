namespace DocsManager.Controllers.Types;

public readonly record struct BearerUser(Guid UserId, string Name, string LastName);