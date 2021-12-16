using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using MadBot.Util;

namespace MadBot.Modules.CustomerService
{
    public class StaffCommands : ModuleBase<SocketCommandContext>
    {
        [Command("openTicket")]
        public async Task OpenTicket([Remainder]string user = "")
        {
            if (!((SocketGuildUser)Context.User).GuildPermissions.ManageGuild)
            {
                await ReplyAsync("Please ask a staff member for help.", messageReference: new MessageReference(Context.Message.Id)).ConfigureAwait(false);
                return;
            }

            await CreateChannel(user);
        }
        
        [Command("closeTicket")]
        public async Task CloseTicket()
        {
            if (!((SocketGuildUser)Context.User).GuildPermissions.ManageGuild)
            {
                await ReplyAsync("Please ask a staff member for help.", messageReference: new MessageReference(Context.Message.Id)).ConfigureAwait(false);
                return;
            }

            await CloseChannel();
        }

        private async Task SendMessage(RestTextChannel channel, SocketGuildUser user)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle("**MadMerch Customer Service**");
            builder.WithDescription("Ticket-" + Madmerch.Ticket +
                                    ": A customer service representative will be with you shortly. Please provide your name and order number.");
            builder.WithImageUrl("https://i.ibb.co/P6Lrx89/Screen-Shot-2021-12-09-at-10-44-31-PM.png");
            builder.WithFooter($"Requested by: {user.Username}#{user.Discriminator}", user.GetAvatarUrl());
            builder.WithColor(0xeeffee);
            
            await channel.SendMessageAsync("", false, builder.Build());
        }

        private async Task CloseChannel()
        {
            if (((SocketTextChannel) Context.Channel).Category.Id == 916037031610703902)
            {
                await ((SocketTextChannel) Context.Channel).ModifyAsync(x => x.CategoryId = 918777956720062504);
                await ((SocketTextChannel) Context.Channel).ModifyAsync(x => x.Name = $"closed-{Context.Channel.Name}");
            }
        }
        
        private async Task CreateChannel(string user)
        {
            if (user == "")
            {
                user = Context.User.Mention;
            } else
            {
                try
                {
                    user = Context.Message.MentionedUsers.First().Mention;
                }
                catch 
                {
                    await ReplyAsync("Could not find customer.", messageReference: new MessageReference(Context.Message.Id)).ConfigureAwait(false);
                    return;
                }
            }
            Madmerch.Ticket++;
            
            var index = Context.Guild.Users.Select(x => x.Mention).IndexOf(user);
            if (index == -1)
            {
                await ReplyAsync("Could not find customer.", messageReference: new MessageReference(Context.Message.Id)).ConfigureAwait(false);
                return;
            }

            var mentioned = Context.Guild.Users.ElementAt(index);
            
            SocketGuild guild = Context.Guild;
            RestTextChannel newChannel =
                await Context.Guild.CreateTextChannelAsync($"ticket-" + Madmerch.Ticket,
                    x => x.CategoryId = 916037031610703902);
            await newChannel.AddPermissionOverwriteAsync(guild.EveryoneRole, OverwritePermissions.DenyAll(newChannel));
            await newChannel.AddPermissionOverwriteAsync(mentioned, OverwritePermissions.AllowAll(newChannel));

            await SendMessage(newChannel, mentioned);

            await Context.Channel.SendMessageAsync(mentioned.Mention + " please visit this channel: " +
                                                   newChannel.Mention);
        }
    }
}