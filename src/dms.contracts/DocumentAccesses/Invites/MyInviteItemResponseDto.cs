namespace DMS.Contracts.DocumentAccesses.Invites;

public sealed record MyInviteItemResponseDto(
    Guid InviteId,
    Guid DocumentId,
    Guid UserId,
    string Token,
    DateTime? ExpiresAtUtc
);