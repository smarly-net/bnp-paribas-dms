using DMS.Application.Abstractions.Outbox;
using DMS.Application.Abstractions.Repositories;
using DMS.Application.Abstractions.Services;
using DMS.Application.Common;
using DMS.Contracts.Events;
using DMS.Domain.DocumentAccesses;

using MediatR;

namespace DMS.Application.DocumentAccesses.Approve;

public sealed class ApproveAccessRequestCommandHandler
    : IRequestHandler<ApproveAccessRequestCommand, Result<Guid>>
{
    private readonly IDocumentAccessRequestRepository _documentAccessRequestRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutbox _outbox;
    private readonly IDateTimeService _dateTimeService;

    public ApproveAccessRequestCommandHandler(
        IDocumentAccessRequestRepository documentAccessRequestRepository
        , IUnitOfWork unitOfWork
        , IOutbox outbox
        , IDateTimeService dateTimeService)
    {
        _documentAccessRequestRepository = documentAccessRequestRepository;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _dateTimeService = dateTimeService;
    }

    public async Task<Result<Guid>> Handle(ApproveAccessRequestCommand request, CancellationToken ct)
    {
        var current = await _documentAccessRequestRepository.GetForDecisionAsync(request.InviteId, ct);
        if (current is null)
        {
            return Result<Guid>.Fail("Access request not found.");
        }

        if (current.DecisionStatus is DocumentRequestDecisionStatus.Approved or DocumentRequestDecisionStatus.Rejected)
        {
            return Result<Guid>.Fail("Access request already decided.");
        }

        var decidedAt = _dateTimeService.UtcNow;
        await _documentAccessRequestRepository.ApplyDecisionAsync(
             request.InviteId,
             request.ApproverId,
             request.status,
             request.Comment,
             decidedAt,
            ct: ct);

        await _outbox.EnqueueAsync(
            new AccessRequestDecidedEvent(
                request.InviteId,
                current.DocumentId,
                current.UserId,
                request.ApproverId,
                request.status,
                request.Comment,
                decidedAt),
            ct);

        await _unitOfWork.Commit(ct);
        return Result<Guid>.Ok(request.InviteId);
    }
}