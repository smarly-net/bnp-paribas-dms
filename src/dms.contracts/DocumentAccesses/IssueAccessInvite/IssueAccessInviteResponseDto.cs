namespace DMS.Contracts.DocumentAccesses.IssueAccessInvite;

public sealed record IssueAccessInviteResponseDto(
    string Token,
    DateTime ExpiresAtUtc
);