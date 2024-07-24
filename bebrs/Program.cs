using System;
using System.Diagnostics;
using System.Threading;
using Websocket.Client;
using Newtonsoft.Json.Linq;

Console.CursorVisible = false;
Stopwatch stopwatch = new Stopwatch();

try
{
    var exitEvent = new ManualResetEvent(false);
    var url = new Uri("wss://stream.binance.com:9443/stream?streams=btcusdt@kline_1m");

    using var client = new WebsocketClient(url);
    client.ReconnectTimeout = TimeSpan.FromSeconds(30);
    client.ReconnectionHappened.Subscribe(info =>
    {
        Console.WriteLine("Reconnection happened, type: " + info.Type);
    });
    client.MessageReceived.Subscribe(msg =>
    {
        Console.ForegroundColor = new Random().Next(0, 9) switch
        {
            0 => ConsoleColor.White,
            1 => ConsoleColor.Red,
            2 => ConsoleColor.Green,
            3 => ConsoleColor.Yellow,
            4 => ConsoleColor.Red,
            5 => ConsoleColor.DarkCyan,
            6 => ConsoleColor.DarkGray,
            7 => ConsoleColor.DarkGreen,
            _ => ConsoleColor.Magenta,
        };

        stopwatch.Stop();
        Console.WriteLine($"Message received (in {stopwatch.ElapsedMilliseconds} ms): " + msg);

        // Parse the JSON message
        var json = JObject.Parse(msg.Text);
        var klineData = json["data"]["k"];

        var openPrice = (string)klineData["o"];
        var closePrice = (string)klineData["c"];
        var highPrice = (string)klineData["h"];
        var lowPrice = (string)klineData["l"];
        var volume = (string)klineData["v"];

        Console.WriteLine($"Open Price: {openPrice}");
        Console.WriteLine($"Close Price: {closePrice}");
        Console.WriteLine($"High Price: {highPrice}");
        Console.WriteLine($"Low Price: {lowPrice}");
        Console.WriteLine($"Volume: {volume}");

        stopwatch.Restart();
    });

    client.Start();
    stopwatch.Start();

    exitEvent.WaitOne();
}
catch (Exception ex)
{
    Console.WriteLine("ERROR: " + ex.ToString());
}
