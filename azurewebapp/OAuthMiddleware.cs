using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotFramework.Composer.WebApp
{
    public class OAuthMiddleware : IMiddleware
    {
        private ConversationState convoState;
        private IConfiguration configuration;
        private DialogSet dialogs;

        public OAuthMiddleware(IConfiguration configuration, ConversationState convoState)
        {
            this.configuration = configuration;
            this.convoState = convoState;
            this.dialogs = null;
        }

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            if (turnContext.Activity.Type == "message")
            {
                var dialogState = this.convoState.CreateProperty<DialogState>("dialogState");
                if (this.dialogs == null)
                {
                    this.dialogs = new DialogSet(dialogState);
                }

                string text = configuration["OAuthPrompt:text"] ?? "Please Sign In";
                string connectionName = configuration["OAuthPrompt:connectionName"] ?? "connectionName";
                string title = configuration["OAuthPrompt:title"] ?? "Sign In";
                this.dialogs.Add(new OAuthPrompt("OAuthPromptTest", new OAuthPromptSettings() { Text = text, ConnectionName = connectionName, Title = title, Timeout = 0 }));

                var dc = await dialogs.CreateContextAsync(turnContext, cancellationToken);

                //var results = await dc.ContinueDialogAsync(cancellationToken);
                await dc.PromptAsync("OAuthPromptTest", new PromptOptions(), cancellationToken: cancellationToken);
                await next(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
