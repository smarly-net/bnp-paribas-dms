using DMS.Application.Abstractions.Outbox;
using DMS.Application.Abstractions.Repositories;
using DMS.Application.Common;
using MediatR;

using System.Security.Cryptography;
using DMS.Application.Abstractions.Persistence.Write;
using DMS.Application.Abstractions.Services;
using DMS.Contracts.Events;

namespace DMS.Application.DocumentAccesses.IssueAccessInvite;

public sealed class IssueAccessInviteCommandHandler
    : IRequestHandler<IssueAccessInviteCommand, Result<AccessInvite>>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDocumentAccessRequestRepository _documentAccessRequestRepository;
    private readonly IOutbox _outbox;
    private readonly IDateTimeService _dateTimeService;
    private readonly IUnitOfWork _unitOfWork;

    public IssueAccessInviteCommandHandler(
        IDocumentRepository documentRepositoryRepository
        , IUserRepository userRepository
        , IDocumentAccessRequestRepository documentAccessRequestRepository
        , IUnitOfWork unitOfWork
        , IOutbox outbox
        , IDateTimeService dateTimeService)
    {
        _documentRepository = documentRepositoryRepository;
        _userRepository = userRepository;
        _documentAccessRequestRepository = documentAccessRequestRepository;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _dateTimeService = dateTimeService;
    }

    public async Task<Result<AccessInvite>> Handle(IssueAccessInviteCommand request, CancellationToken ct)
    {
        var docExists = await _documentRepository.GetByIdAsync(request.DocumentId, ct);
        if (docExists == null)
        {
            return Result<AccessInvite>.Fail("Document not found.");
        }

        var userExists = await _userRepository.GetByIdAsync(request.UserId, ct);
        if (userExists == null)
        {
            return Result<AccessInvite>.Fail("User not found.");
        }

        AccessInvite? existingInvite = await _documentAccessRequestRepository.GetActiveInviteAsync(request.UserId, request.DocumentId, ct);

        if (existingInvite is not null)
        {
            return Result<AccessInvite>.Ok(new AccessInvite(existingInvite.Id, request.UserId, request.DocumentId, existingInvite.Token, existingInvite.ExpiresAtUtc));
        }

        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        var expriresAtUtc = request.ExpiresAtUtc ?? _dateTimeService.UtcNow.AddDays(7);

        var accessInviteId = await _documentAccessRequestRepository.IssueAccessInvite(new AccessInvite(Guid.Empty, request.UserId, request.DocumentId, token, expriresAtUtc), ct: ct);
        await _outbox.EnqueueAsync(new AccessInviteIssuedEvent(accessInviteId, request.UserId, request.DocumentId, token, expriresAtUtc), ct);

        await _unitOfWork.Commit(ct);

        return Result<AccessInvite>.Ok(new AccessInvite(accessInviteId, request.UserId, request.DocumentId, token, expriresAtUtc));
    }

}