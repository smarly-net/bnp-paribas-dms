using DMS.Application.Abstractions.Outbox;
using DMS.Application.Abstractions.Repositories;
using DMS.Application.Common;
using DMS.Contracts.Documents.Invite;
using MediatR;

using System.Security.Cryptography;
using DMS.Application.Abstractions.Persistence.Write;
using DMS.Contracts.Events;

namespace DMS.Application.Documents.IssueAccessInvite;

public sealed class IssueAccessInviteCommandHandler
    : IRequestHandler<IssueAccessInviteCommand, Result<IssueAccessInviteResponseDto>>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDocumentAccessRequestRepository _documentAccessRequestRepository;
    private readonly IOutbox _outbox;
    private readonly IUnitOfWork _unitOfWork;

    public IssueAccessInviteCommandHandler(
        IDocumentRepository documentRepositoryRepository
        , IUserRepository userRepository
        , IDocumentAccessRequestRepository documentAccessRequestRepository
        , IUnitOfWork unitOfWork
        , IOutbox outbox)
    {
        _documentRepository = documentRepositoryRepository;
        _userRepository = userRepository;
        _documentAccessRequestRepository = documentAccessRequestRepository;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
    }

    public async Task<Result<IssueAccessInviteResponseDto>> Handle(IssueAccessInviteCommand request, CancellationToken ct)
    {
        var docExists = await _documentRepository.GetByIdAsync(request.DocumentId, ct);
        if (docExists == null)
        {
            return Result<IssueAccessInviteResponseDto>.Fail("Document not found.");
        }

        var userExists = await _userRepository.GetByIdAsync(request.UserId, ct);
        if (userExists == null)
        {
            return Result<IssueAccessInviteResponseDto>.Fail("User not found.");
        }

        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        var expriresAtUtc = request.ExpiresAtUtc ?? DateTime.UtcNow.AddDays(7);

        await _documentAccessRequestRepository.IssueAccessInvite(new AccessInvite(request.UserId, request.DocumentId, token, expriresAtUtc), ct: ct);
        await _outbox.EnqueueAsync(new AccessInviteIssuedEvent(request.UserId, request.DocumentId, token, expriresAtUtc), ct);

        await _unitOfWork.Commit(ct);

        return Result<IssueAccessInviteResponseDto>.Ok(new IssueAccessInviteResponseDto(token, expriresAtUtc));
    }

}