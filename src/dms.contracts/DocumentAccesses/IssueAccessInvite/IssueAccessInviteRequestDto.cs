namespace DMS.Contracts.DocumentAccesses.IssueAccessInvite;

public sealed record IssueAccessInviteRequestDto(
    Guid UserId,
    DateTime? ExpiresAtUtc
);