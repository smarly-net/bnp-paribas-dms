using DMS.Domain.DocumentAccesses;

namespace DMS.Contracts.DocumentAccesses.Approve;

public sealed record ApproveAccessRequestRequestDto(DocumentRequestDecisionStatus Status, string? Comment);