namespace DMS.Contracts.Documents.Invite;

public sealed record IssueAccessInviteResponseDto(
    string Token,
    DateTime ExpiresAtUtc
);