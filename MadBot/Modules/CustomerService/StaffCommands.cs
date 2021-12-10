using System.Linq;
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
        private int ticket = 1000;
        
        [Command("openTicket")]
        public async Task PingAsync([Remainder]string user = "")
        {
            if (!((SocketGuildUser)Context.User).GuildPermissions.ManageGuild)
            {
                await ReplyAsync("Please ask a staff member for help.", messageReference: new MessageReference(Context.Message.Id)).ConfigureAwait(false);
                return;
            }
            
            await ReplyAsync("Pong!");
        }
        
        
        private async Task CreateChannel(string key, string message, string action, string user)
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
                    await ReplyAsync("nu! that is not a real person xd", messageReference: new MessageReference(Context.Message.Id)).ConfigureAwait(false);
                    return;
                }
            }

            var index = Context.Guild.Users.Select(x => x.Mention).IndexOf(user);
            var mentioned  = Context.Guild.Users.ElementAt(index);

            SocketGuild guild = Context.Guild;
            RestTextChannel newChannel =
                await Context.Guild.CreateTextChannelAsync("ticket-" + ticket++,
                    x => x.CategoryId = 916037031610703902);
            await newChannel.AddPermissionOverwriteAsync(guild.EveryoneRole, OverwritePermissions.DenyAll(newChannel));
            await newChannel.AddPermissionOverwriteAsync(mentioned, OverwritePermissions.AllowAll(newChannel));
        }
    }
}