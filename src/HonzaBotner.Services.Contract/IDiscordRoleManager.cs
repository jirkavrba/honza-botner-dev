﻿using System.Collections.Generic;
using System.Threading.Tasks;
using HonzaBotner.Services.Contract.Dto;

namespace HonzaBotner.Services.Contract
{
    public interface IDiscordRoleManager
    {
        HashSet<DiscordRole> MapUsermapRoles(IReadOnlyCollection<string> kosRoles);

        Task<bool> GrantRolesAsync(ulong userId, IReadOnlySet<DiscordRole> discordRoles);
    }
}