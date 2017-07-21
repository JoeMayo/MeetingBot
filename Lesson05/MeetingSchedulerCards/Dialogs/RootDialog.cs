using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using AdaptiveCards;
using Microsoft.Bot.Connector.Payments;

namespace MeetingSchedulerCards.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            var options = new List<string> {
                "Upload Agenda", "Download Agenda",
                "Hero Card", "Handle Events",
                "Audio Card", "Video Card", "Animation Card",
                "Adaptive Card" };

            PromptDialog.Choice(context, ResumeAfterChoiceAsync, options, "What would you like to do?");
        }

        async Task ResumeAfterChoiceAsync(IDialogContext context, IAwaitable<string> result)
        {
            string choice = await result;

            switch (choice)
            {
                case "Upload Agenda":
                    await context.PostAsync("Ready for upload.");
                    context.Wait(ReceiveUploadAsync);
                    break;
                case "Download Agenda":
                    await HandleDownloadAsync(context);
                    break;
                case "Hero Card":
                    await HandleHeroAsync(context);
                    break;
                case "Handle Events":
                    await HandleEventsAsync(context);
                    break;
                case "Audio Card":
                    await HandleAudioAsync(context);
                    break;
                case "Animation Card":
                    await HandleAnimationAsync(context);
                    break;
                case "Video Card":
                    await HandleVideoAsync(context);
                    break;
                case "Adaptive Card":
                    await HandleAdaptiveAsync(context);
                    break;
                default:
                    context.Wait(MessageReceivedAsync);
                    break;
            }

        }

        async Task ReceiveUploadAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if (activity.Attachments.Any())
            {
                Attachment agenda = activity.Attachments.First();
                byte[] fileBytes = await new HttpClient().GetByteArrayAsync(agenda.ContentUrl);
                string path = HttpContext.Current.Server.MapPath("/") + agenda.Name;
                File.WriteAllBytes(path, fileBytes);
            }
            else
            {
                await context.PostAsync("Sorry, I didn't see the agenda in the attachment. Type \"Help\" to try again.");
            }

            context.Wait(MessageReceivedAsync);
        }

        async Task HandleDownloadAsync(IDialogContext context)
        {
            string contentUrl = GetBinaryUrl("Smile.png");
            var agenda = new Attachment("image/png", contentUrl, "Smile.png");
            Activity activity = context.Activity as Activity;
            Activity reply = activity.CreateReply();
            reply.Attachments.Add(agenda);
            await
                new ConnectorClient(new Uri(reply.ServiceUrl))
                    .Conversations
                    .SendToConversationAsync(reply);
            context.Wait(MessageReceivedAsync);
        }

        async Task HandleHeroAsync(IDialogContext context)
        {
            string contentUrl = GetBinaryUrl("Smile.png");
            var venueCard = new HeroCard(
                "Create Venue",
                "Add a new venue",
                "Adding a new venue allows you to add venues to appointments and meetings.",
                new List<CardImage> { new CardImage(contentUrl, "Smile") });
            var appointmentCard = new HeroCard(
                "Create Appointment",
                "Add a new appointment",
                "Adding a new appointment allows you to populate a list of your pre-existing appontments.",
                new List<CardImage> { new CardImage(contentUrl, "Smile") });
            Activity activity = context.Activity as Activity;
            Activity reply = activity.CreateReply();
            reply.Attachments.Add(venueCard.ToAttachment());
            reply.Attachments.Add(appointmentCard.ToAttachment());
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            await
                new ConnectorClient(new Uri(reply.ServiceUrl))
                    .Conversations
                    .SendToConversationAsync(reply);
            context.Wait(MessageReceivedAsync);
        }

        async Task HandleEventsAsync(IDialogContext context)
        {
            string contentUrl = GetBinaryUrl("Smile.png");
            var venueCard = new HeroCard(
                "Create Venue",
                "Add a new venue",
                "Adding a new venue allows you to add venues to appointments and meetings.",
                images: new List<CardImage> { new CardImage(contentUrl, "Smile") },
                buttons: new List<CardAction>
                {
                    new CardAction { Type="openUrl", Title="Visit Bot Framework", Value="http://dev.botframework.com/" }
                });
            Activity activity = context.Activity as Activity;
            Activity reply = activity.CreateReply();
            reply.Attachments.Add(venueCard.ToAttachment());
            await
                new ConnectorClient(new Uri(reply.ServiceUrl))
                    .Conversations
                    .SendToConversationAsync(reply);
            context.Wait(MessageReceivedAsync);
        }

        async Task HandleVideoAsync(IDialogContext context)
        {
            string contentUrl = GetBinaryUrl("cortana.mp4");
            var videoCard = new VideoCard(
                "Chatbot Video",
                "Calling With the Cortana Channel",
                "This demonstrates how to speak with a chatbot via Cortana.",
                media: new List<MediaUrl> { new MediaUrl(contentUrl) });
            Activity activity = context.Activity as Activity;
            Activity reply = activity.CreateReply();
            reply.Attachments.Add(videoCard.ToAttachment());
            await
                new ConnectorClient(new Uri(reply.ServiceUrl))
                    .Conversations
                    .SendToConversationAsync(reply);
            context.Wait(MessageReceivedAsync);
        }

        async Task HandleAudioAsync(IDialogContext context)
        {
            string contentUrl = GetBinaryUrl("MeetingAudio.m4a");
            var audioCard = new AudioCard(
                "Chatbot Audio",
                "Meeting Message",
                "This demonstrates how to share audio content.",
                media: new List<MediaUrl> { new MediaUrl(contentUrl) });
            Activity activity = context.Activity as Activity;
            Activity reply = activity.CreateReply();
            reply.Attachments.Add(audioCard.ToAttachment());
            await
                new ConnectorClient(new Uri(reply.ServiceUrl))
                    .Conversations
                    .SendToConversationAsync(reply);
            context.Wait(MessageReceivedAsync);
        }

        async Task HandleAnimationAsync(IDialogContext context)
        {
            string contentUrl = GetBinaryUrl("FlippingBot.gif");
            var animationCard = new AnimationCard(
                "Chatbot Animation",
                "A Flipping Bot",
                "Demonstrates how to display a *.gif animation.",
                media: new List<MediaUrl> { new MediaUrl(contentUrl) });
            Activity activity = context.Activity as Activity;
            Activity reply = activity.CreateReply();
            reply.Attachments.Add(animationCard.ToAttachment());
            await
                new ConnectorClient(new Uri(reply.ServiceUrl))
                    .Conversations
                    .SendToConversationAsync(reply);
            context.Wait(MessageReceivedAsync);
        }

        async Task HandleAdaptiveAsync(IDialogContext context)
        {
            string contentUrl = GetBinaryUrl("Smile.png");
            var card = new AdaptiveCard();

            card.Speak = "This is an Adaptive Card";

            card.Body.Add(new TextBlock()
            {
                Text = "Meeting Scheduler",
                Size = TextSize.Large,
                Weight = TextWeight.Bolder
            });

            card.Body.Add(new TextBlock()
            {
                Text = "This is text for an adaptive card.",
                Color = TextColor.Accent
            });

            card.Body.Add(new Image()
            {
                Url = GetBinaryUrl("Smile.png")
            });

            card.Actions.Add(new HttpAction()
            {
                Url = "http://dev.botframework.com/",
                Title = "Bot Framework",
                Type = "Action.OpenUrl"
            });

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
            Activity activity = context.Activity as Activity;
            Activity reply = activity.CreateReply();
            reply.Attachments.Add(attachment);
            await
                new ConnectorClient(new Uri(reply.ServiceUrl))
                    .Conversations
                    .SendToConversationAsync(reply);
            context.Wait(MessageReceivedAsync);
        }

        string GetBinaryUrl(string fileName)
        {
            string absoluteUri = HttpContext.Current.Request.Url.AbsoluteUri + "/binaries/" + fileName;
            string contentUrl = absoluteUri.Replace("api/messages/", "");
            return contentUrl;
        }
    }
}