namespace DMS.Application.Abstractions.Persistence.Write;

public sealed record RefreshToken(Guid UserId, string Token, DateTime ExpiryDate, string ParentAccessJti);