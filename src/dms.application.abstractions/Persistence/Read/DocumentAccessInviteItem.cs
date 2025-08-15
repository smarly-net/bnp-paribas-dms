using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Application.Abstractions.Persistence.Read;

public sealed record DocumentAccessInviteItem(
    Guid InviteId,
    Guid DocumentId,
    string Token,
    Guid UserId,
    DateTime ExpiresAtUtc
);