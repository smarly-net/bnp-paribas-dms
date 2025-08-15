using DMS.Application.Abstractions.Outbox;
using DMS.Application.Abstractions.Repositories;
using DMS.Application.Common;
using DMS.Contracts.Events;

using MediatR;

namespace DMS.Application.DocumentAccesses.RequestAccess;

public sealed class SubmitAccessRequestCommandHandler
    : IRequestHandler<SubmitAccessRequestCommand, Result<Guid>>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDocumentAccessRequestRepository _accessRequestRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutbox _outbox;

    public SubmitAccessRequestCommandHandler(
        IDocumentRepository documentRepository
        , IUserRepository userRepository
        , IDocumentAccessRequestRepository accessRequestRepository
        , IUnitOfWork unitOfWork
        , IOutbox outbox)
    {
        _documentRepository = documentRepository;
        _userRepository = userRepository;
        _accessRequestRepository = accessRequestRepository;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
    }

    public async Task<Result<Guid>> Handle(SubmitAccessRequestCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, ct);
        if (user is null)
        {
            return Result<Guid>.Fail("User not found.");
        }

        var invite = await _accessRequestRepository.GetActiveInviteAsync(request.UserId, request.Token, ct);
        if (invite is null)
        {
            return Result<Guid>.Fail("Invalid or expired invite token.");
        }

        var doc = await _documentRepository.GetByIdAsync(invite.DocumentId, ct);
        if (doc is null)
        {
            return Result<Guid>.Fail("Document not found.");
        }

        var submittedDate = DateTime.UtcNow;

        var resultOfApplying = await _accessRequestRepository.ApplyUserRequestAsync(invite.Id, request.Reason, request.AccessType, submittedDate, ct);

        if (resultOfApplying)
        {
            await _outbox.EnqueueAsync(
                new AccessRequestSubmittedEvent(invite.Id, request.UserId, user.Username, doc.Id, doc.Title, request.Reason, request.AccessType, submittedDate, null, null, null, null), ct);

            await _unitOfWork.Commit(ct);
        }

        return Result<Guid>.Ok(invite.Id);
    }
}