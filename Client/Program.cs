using Microsoft.AspNetCore.SignalR.Client;

class Program
{
    static async Task Main(string[] args)
    {
        // Get hub URL from environment variable or use a default value
        var hubUrl = Environment.GetEnvironmentVariable("SIGNALR_HUB_URL") ?? "http://localhost:5000/chatHub";
        var hubTypeName = Environment.GetEnvironmentVariable("HUB_TYPE_NAME") ?? "ReceiveMessage";


        // Connect to the SignalR hub
        var connection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .Build();

        if (hubTypeName == "ReceiveMessage")
            await HubTypes.ReceiveMessage(connection);
        if (hubTypeName == "JoinChannel")
        {
            var providerId = Environment.GetEnvironmentVariable("PROVIDER_ID") ?? throw new ArgumentException("PROVIDER_ID can not be null!!");
            await HubTypes.JoinChannel(connection, providerId);
        }
        if (hubTypeName == "ReceiveTyping")
            HubTypes.ReceiveTyping(connection);
        if (hubTypeName == "ReceiveMember")
            await HubTypes.ReceiveMember(connection);
        if (hubTypeName == "JoinTyping")
        {
            var providerId = Environment.GetEnvironmentVariable("PROVIDER_ID") ?? throw new ArgumentException("PROVIDER_ID can not be null!!");
            await MultiHubs.JoinTyping(connection, providerId);
        }
    }
}

public class HubTypes
{
    public static async Task ReceiveMessage(HubConnection connection)
    {
        Console.Write("Enter your name: ");
        var user = Console.ReadLine();

        connection.On<string, string, string, string>("ReceiveMessage", (message, providerId, channelId, memberId) =>
        {
            Console.WriteLine($"providerId: {providerId} channelId: {channelId} memberId: {memberId}");
        });

        await connection.StartAsync();
        // Console.WriteLine($"Connected as {user}. Type your message:");

        // // Start listening for user input
        while (true)
        {
            var message = Console.ReadLine();
            if (string.IsNullOrEmpty(message))
                continue;

            // Send the message to the hub
            await connection.InvokeAsync("SendMessage", user, message, "", "");
        }
    }
    public static async Task JoinChannel(HubConnection connection, string providerId)
    {
        await connection.StartAsync();

        // Send the message to the hub
        await connection.InvokeAsync("JoinChannel", providerId);
    }

    public static void ReceiveTyping(HubConnection connection)
    {
        // Console.Write("Enter your name: ");
        // var user = Console.ReadLine();

        connection.On<string, string, string>("ReceiveTyping", (providerId, channelId, memberId) =>
        {
            Console.WriteLine($"providerId: {providerId} channelId: {channelId} memberId: {memberId}");
        });

    }

    public static async Task ReceiveMember(HubConnection connection)
    {
        // Console.Write("Enter your name: ");
        // var user = Console.ReadLine();

        connection.On<string>("ReceiveMember", (providerId) =>
        {
            Console.WriteLine($"providerId: {providerId}");
        });

        await connection.StartAsync();
    }
}

public class MultiHubs
{
    public static async Task JoinTyping(HubConnection connection, string providerId)
    {
        connection.On<string, string, string>("ReceiveTyping", (providerId, channelId, memberId) =>
        {
            Console.WriteLine($"providerId: {providerId} channelId: {channelId} memberId: {memberId}");
        });

        await HubTypes.JoinChannel(connection, providerId);

        // await connection.StartAsync();

        // Send the message to the hub
        while (true)
        {
            var message = Console.ReadLine();
            if (string.IsNullOrEmpty(message))
                continue;

            // Send the message to the hub
            await connection.InvokeAsync("SendTyping", "1", "2", "3");
        }
    }
}
