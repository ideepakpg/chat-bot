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


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{

    // Only process Message updates
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    //Display user entered commands details in console
    if (messageText.StartsWith("/"))
    {
        // Get the current date and time (to show in console)
        DateTime currentTime = DateTime.Now;

        // Display the user's chat ID, date, and time in console
        Console.WriteLine(
            $"User with Chat ID {message.Chat.Id} sent the command '{messageText}' at {currentTime.ToLocalTime()}.");
    }


    //This code manages various Telegram bot commands, offering responses for commands such as "/start", "/help", "/about", and "/contact"
    switch (messageText.ToLower())
    {
        case "/start":
            // Reply with a welcome message for the "/start" command
            var startMessage = "Yes I'm alive bitch !";
            await botClient.SendTextMessageAsync(message.Chat.Id, startMessage, cancellationToken: cancellationToken);
            return;

        case "/help":
            var helpMessage = "പോയിട്ട് അടുത്ത വെള്ളിയാഴ്ച്ച വാ";
            await botClient.SendTextMessageAsync(message.Chat.Id, helpMessage, cancellationToken: cancellationToken);
            return;

        case "/about":
            var aboutMessage = "C# Telegram Chat Bot built using .NET Client Telegram Bot API Framework";
            await botClient.SendTextMessageAsync(message.Chat.Id, aboutMessage, cancellationToken: cancellationToken);
            return;

        case "/contact":
            var contactMessage = "why are you gay ?";
            await botClient.SendTextMessageAsync(message.Chat.Id, contactMessage, cancellationToken: cancellationToken);
            return;

        default:
            // Handle unknown commands or other text messages here
            break;
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    long chatId = update.Message.Chat.Id; // Get the chat ID from the incoming message (dynamic chatId)

    //Send an introduction message with an InlineKeyboardMarkup
    var inlineKeyboard = new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>
{
    // First row
    new List<InlineKeyboardButton>
    {
        InlineKeyboardButton.WithUrl("My Owner", "https://t.me/ideepakpg"),
        InlineKeyboardButton.WithUrl("Repo", "https://github.com/ideepakpg/chat-bot")
    },
    // Second row
    //new List<InlineKeyboardButton>
    //{
    //    InlineKeyboardButton.WithCallbackData("Button 1", "data1"),
    //    InlineKeyboardButton.WithUrl("Open Website", "https://example.com")
    //},
    // Add more rows with buttons as needed
});

    Message newMessage = await botClient.SendTextMessageAsync(
        chatId: chatId,/*(dynamic chatId)*/
        text: "*Hello 👋  I'm Levi Ackerman, humanity's strongest soldier*",
        parseMode: ParseMode.MarkdownV2,
        disableNotification: true,
        replyToMessageId: update.Message.MessageId,
        replyMarkup: inlineKeyboard,
        cancellationToken: cancellationToken);

    // Display information about the sent(normal text message(newMessage only) not commands) message, including sender's name, message ID, local timestamp, reply status, and message entities count in console.
    Console.WriteLine(
    $"{newMessage.From.FirstName} sent message {newMessage.MessageId} " +
    $"to chat {newMessage.Chat.Id} at {newMessage.Date.ToLocalTime()}. " +
    $"It is a reply to message {newMessage.ReplyToMessage.MessageId} " +
    $"and has {newMessage.Entities.Length} message entities.");


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    // This code creates a custom keyboard with options, sends a text message with the keyboard to user, allowing user interaction.
    //    ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
    //    {
    //    new KeyboardButton[] { "Repo", "About me" },

    //    // for second row of keyboard button
    //    //new KeyboardButton[] { "Call me ☎️", "Share" },
    //})
    //    {
    //        ResizeKeyboard = true
    //    };

    // Turned off "Choose a response" text message to choose from the replyKeyboardMarkup button (need to turn on ,otherwise the KeyboardButton don't display)
    //Message sentMessage = await botClient.SendTextMessageAsync(
    //    chatId: chatId,
    //    text: "Choose a response",
    //    replyMarkup: replyKeyboardMarkup,
    //    cancellationToken: cancellationToken);


    // To remove the KeyboardButton from the bot
    //Message sentMessage1 = await botClient.SendTextMessageAsync(
    //    chatId: chatId,
    //    text: "Removing keyboard",
    //    replyMarkup: new ReplyKeyboardRemove(),
    //    cancellationToken: cancellationToken);


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    //var chatId = message.Chat.Id;

    //Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

    //// Echo received message text
    //Message sentMessage = await botClient.SendTextMessageAsync(
    //    chatId: chatId,
    //    text: "You said :\n" + messageText,
    //    cancellationToken: cancellationToken);
}


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



//Handles errors that occur during the polling process of the Telegram bot.
//Logs error messages to the console, providing detailed information about the exception.
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
