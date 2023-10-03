using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


var botClient = new TelegramBotClient("Token");


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


// Fetch information about the bot's identity using GetMeAsync() and display a message to start listening for messages from the bot's username.
var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{

    // Only process Message updates
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;

    //Send an introduction message with an InlineKeyboardMarkup
    Message newMessage = await botClient.SendTextMessageAsync(
    chatId: ChatId,
    text: "*Hello 👋  I'm Levi Ackerman, humanity's strongest soldier*",
    parseMode: ParseMode.MarkdownV2,
    disableNotification: true,
    replyToMessageId: update.Message.MessageId,
    replyMarkup: new InlineKeyboardMarkup(
        InlineKeyboardButton.WithUrl(
            text: "My Owner",
            url: "https://t.me/ideepakpg")),
    cancellationToken: cancellationToken);



    // This code creates a custom keyboard with options, sends a text message with the keyboard to user, allowing user interaction.
    ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
    {
    new KeyboardButton[] { "Repo", "About me" },
    new KeyboardButton[] { "Call me ☎️", "Share" },
})
    {
        ResizeKeyboard = true
    };

    Message sentMessage = await botClient.SendTextMessageAsync(
        chatId: ChatId,
        text: "Choose a response",
        replyMarkup: replyKeyboardMarkup,
        cancellationToken: cancellationToken);


    // To remove the KeyboardButton from the bot
    //Message sentMessage1 = await botClient.SendTextMessageAsync(
    //    chatId: chatId,
    //    text: "Removing keyboard",
    //    replyMarkup: new ReplyKeyboardRemove(),
    //    cancellationToken: cancellationToken);

    // Display information about the sent message, including sender's name, message ID, local timestamp, reply status, and message entities count.
    Console.WriteLine(
    $"{newMessage.From.FirstName} sent message {newMessage.MessageId} " +
    $"to chat {newMessage.Chat.Id} at {newMessage.Date.ToLocalTime()}. " +
    $"It is a reply to message {newMessage.ReplyToMessage.MessageId} " +
    $"and has {newMessage.Entities.Length} message entities.");



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
