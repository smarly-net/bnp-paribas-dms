namespace DMS.Contracts.Documents.IssueAccessInvite;

public sealed record IssueAccessInviteRequestDto(
    Guid UserId,
    DateTime? ExpiresAtUtc
);