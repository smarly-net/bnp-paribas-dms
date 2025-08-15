using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Contracts.Users.List;

public sealed record UserListItemResponseDto(
    Guid Id,
    string Username
);