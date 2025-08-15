using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMS.Domain.DocumentAccesses;

namespace DMS.Contracts.DocumentAccesses.RequestAccess;

public sealed record SubmitAccessRequestRequestDto(
    string Token,
    string Reason,
    DocumentAccessRequestType Type
);