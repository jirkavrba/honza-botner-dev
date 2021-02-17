﻿using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using HonzaBotner.Discord.Services.Commands.Polls;

namespace HonzaBotner.Discord.Services.Commands
{
    [Group("poll")]
    [Description("Commands to create polls.")]
    public class PollCommands : BaseCommandModule
    {
        private readonly string _pollErrorMessage =
            "Poll build failed. Make sure question has less than 256 characters and each option less than 1024 characters";

        [GroupCommand]
        [Description("Creates either yes/no or ABC poll.")]
        [Priority(1)]
        public async Task PollCommand(CommandContext ctx,
            [Description("Options for the pool.")] params string[] options)
        {
            if (options.Length == 0)
            {
                await ctx.RespondAsync($"You have to ask a question");
                return;
            }

            IPoll poll;

            if (options.Length == 1)
            {
                poll = new YesNoPoll(ctx.Member.RatherNicknameThanUsername(), ctx.Member.AvatarUrl, options.First());
            }
            else if (options.Length - 1 <= AbcPoll.MaxOptions)
            {
                poll = new AbcPoll(ctx.Member.RatherNicknameThanUsername(), ctx.Member.AvatarUrl, options.First(),
                    options.Skip(1).ToList());
            }
            else
            {
                await ctx.RespondAsync($"Too many options, maximum number of responses is ${AbcPoll.MaxOptions}.");
                return;
            }

            try
            {
                await poll.Post(ctx.Client, ctx.Channel);
                await ctx.Message.DeleteAsync();
            }
            catch
            {
                await ctx.RespondAsync(_pollErrorMessage);
            }
        }

        [Command("yesno")]
        [Description("Creates a yesno pool.")]
        [Priority(2)]
        public async Task YesnoPollCommand(CommandContext ctx,
            [RemainingText, Description("Poll's question.")]
            string question)
        {
            try
            {
                await new YesNoPoll(ctx.Member.RatherNicknameThanUsername(), ctx.Member.AvatarUrl, question)
                    .Post(ctx.Client, ctx.Channel);
                await ctx.Message.DeleteAsync();
            }
            catch
            {
                await ctx.RespondAsync(_pollErrorMessage);
            }
        }

        [Command("abc")]
        [Description("Creates an abc pool.")]
        [Priority(2)]
        public async Task AbcPollCommand(CommandContext ctx,
            [Description("Poll's question.")] string question,
            [Description("Poll's options.")] params string[] answers)
        {
            try
            {
                await new AbcPoll(ctx.Member.RatherNicknameThanUsername(), ctx.Member.AvatarUrl, question, answers.ToList())
                    .Post(ctx.Client, ctx.Channel);
                await ctx.Message.DeleteAsync();
            }
            catch
            {
                await ctx.RespondAsync(_pollErrorMessage);
            }
        }
    }
}
