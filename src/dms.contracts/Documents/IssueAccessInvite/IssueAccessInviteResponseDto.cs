namespace DMS.Contracts.Documents.IssueAccessInvite;

public sealed record IssueAccessInviteResponseDto(
    string Token,
    DateTime ExpiresAtUtc
);