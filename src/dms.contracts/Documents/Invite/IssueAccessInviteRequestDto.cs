namespace DMS.Contracts.Documents.Invite;

public sealed record IssueAccessInviteRequestDto(
    Guid UserId,
    DateTime? ExpiresAtUtc
);