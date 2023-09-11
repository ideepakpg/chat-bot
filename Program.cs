using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


var botClient = new TelegramBotClient("6696522224:AAHKep6aG6491NMT8ARSzUoI-lSIclHeHCw");


using CancellationTokenSource cts = new();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
};


botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);


var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    //// Only process Message updates
    //if (update.Message is not { } message)
    //    return;
    //// Only process text messages
    //if (message.Text is not { } messageText)
    //    return;

    Message message = await botClient.SendTextMessageAsync(
    chatId: 1232323487,
    text: "*Hello 👋  I'm Levi, humanity's strongest soldier*",
    parseMode: ParseMode.MarkdownV2,
    disableNotification: true,
    replyToMessageId: update.Message.MessageId,
    replyMarkup: new InlineKeyboardMarkup(
        InlineKeyboardButton.WithUrl(
            text: "My Owner",
            url: "https://t.me/ideepakpg")),
    cancellationToken: cancellationToken);


    //var chatId = message.Chat.Id;

    //Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

    //// Echo received message text
    //Message sentMessage = await botClient.SendTextMessageAsync(
    //    chatId: chatId,
    //    text: "You said :\n" + messageText,
    //    cancellationToken: cancellationToken);
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}
