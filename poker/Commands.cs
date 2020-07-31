using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace TutorialBot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        // System.Drawing.Imaging.ImageFormat;

        [Command("start")]
        public async Task Start()
        {
            var channelId = Context.Message.Channel.Id.ToString();
            var game = GameFactory.GetGame(channelId);
            if (game == null)
                await ReplyAsync("No game is running in this channel");
            else
            {
                if (game.owner.id == Context.Message.Author.Id.ToString())
                {
                    game.distributeCards();
                    await ReplyAsync("Game has started");

                }
                else await ReplyAsync("You are not the owner of the game");

            }
        }


        [Command("bet")]
        public async Task Bet(int value)
        {
            try
            {
                var userId = Context.Message.Author.Id.ToString();
                var channelId = Context.Message.Channel.Id.ToString();
                var game = GameFactory.GetGame(channelId);
                if (game == null)
                    await ReplyAsync("No game is running in this channel");
                else
                {
                    var result = game.bet(userId, value);
                    await ReplyAsync(result.message);

                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.GetBaseException().ToString());
            }
        }

        [Command("check")]
        public async Task Check()
        {
            var userId = Context.Message.Author.Id.ToString();
            var channelId = Context.Message.Channel.Id.ToString();
            var game = GameFactory.GetGame(channelId);
            if (game == null)
                await ReplyAsync("No game is running in this channel");
            else
            {
                var result = game.check(userId);
                await ReplyAsync(result.message);

            }
        }

        [Command("call")]
        public async Task Call()
        {
            try
            {
                var userId = Context.Message.Author.Id.ToString();
                var channelId = Context.Message.Channel.Id.ToString();
                var game = GameFactory.GetGame(channelId);
                if (game == null)
                    await ReplyAsync("No game is running in this channel");
                else
                {
                    var result = game.call(userId);
                    await ReplyAsync(result.message);

                }
            }
            catch (Exception e)
            {
            await ReplyAsync(e.GetBaseException().ToString());
            }
        }

        [Command("fold")]
        public async Task Fold()
        {
            var userId = Context.Message.Author.Id.ToString();
            var channelId = Context.Message.Channel.Id.ToString();
            var game = GameFactory.GetGame(channelId);
            if (game == null)
                await ReplyAsync("No game is running in this channel");
            else
            {
                var result = game.fold(userId);
                await ReplyAsync(result.message);

            }
        }


        [Command("play")]
        public async Task Play(int buyIn = 100)
        {
            try
            {
                var userId = Context.Message.Author.Id.ToString();
                var channelId = Context.Message.Channel.Id.ToString();
                var gameId = GameFactory.createGame(userId, buyIn, channelId);
                if (gameId == null)
                    await ReplyAsync("Loja nuk u krijua");
                else
                    await ReplyAsync($"Loja u krijua nga useri me id:{userId}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException());
            }

        }

        [Command("join")]
        public async Task Join()
        {
            string channelId = Context.Message.Channel.Id.ToString();
            var game = GameFactory.GetGame(channelId);
            if (game == null)
                await ReplyAsync("No game is running in this channel");
            else
            {
                var result = game.addPlayer(Context.Message.Author.Id.ToString());
                await ReplyAsync(result.message);






            }

        }


        [Command("ping")]
        public async Task Ping()
        {
            await ReplyAsync("pong");
            //  await ReplyAsync(new Embed("asdads"));
        }

        [Command("ban")]
        [RequireUserPermission(GuildPermission.BanMembers, ErrorMessage = "You don't have the permission ``ban_member``!")]
        public async Task BanMember(IGuildUser user = null, [Remainder] string reason = null)
        {
            if (user == null)
            {
                await ReplyAsync("Please specify a user!");
                return;
            }
            if (reason == null) reason = "Not specified";

            await Context.Guild.AddBanAsync(user, 1, reason);

            var EmbedBuilder = new EmbedBuilder()
                .WithDescription($":white_check_mark: {user.Mention} was banned\n**Reason** {reason}")
                .WithFooter(footer =>
                {
                    footer
                    .WithText("User Ban Log")
                    .WithIconUrl("https://i.imgur.com/6Bi17B3.png");
                });
            Embed embed = EmbedBuilder.Build();
            await ReplyAsync(embed: embed);

            ITextChannel logChannel = Context.Client.GetChannel(642698444431032330) as ITextChannel;
            var EmbedBuilderLog = new EmbedBuilder()
                .WithDescription($"{user.Mention} was banned\n**Reason** {reason}\n**Moderator** {Context.User.Mention}")
                .WithFooter(footer =>
                {
                    footer
                    .WithText("User Ban Log")
                    .WithIconUrl("https://i.imgur.com/6Bi17B3.png");
                });
            Embed embedLog = EmbedBuilderLog.Build();
            await logChannel.SendMessageAsync(embed: embedLog);

        }
    }
}