using DiscordRPC;
using DiscordRPC.Logging;
using DiscordRPC.Message;
using System;
using game_presence;
using SteamWebAPI2.Utilities;

DiscordRpcClient client;
bool isRunning = false;
//Called when your application first starts.
//For example, just before your main loop, on OnEnable for unity.

/*
Create a Discord client
NOTE: 	If you are using Unity3D, you must use the full constructor and define
         the pipe connection.
*/
client = new DiscordRpcClient("852652391182499901");			

//Set the logger
client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };

//Subscribe to events
client.OnReady += (sender, e) =>
{
    Console.WriteLine("Received Ready from user {0}", e.User.Username);
};
	
client.OnPresenceUpdate += (sender, e) =>
{
    Console.WriteLine("Received Update! {0}", e.Presence);
};

//Connect to the RPC
client.Initialize();

RPCClient.Get().Presence.State = "aaaaaaaaa";
RPCClient.Get().Presence.Assets = new Assets()
    {};


//Set the rich presence
//Call this as many times as you want and anywhere in your code.
Game? lastGame = null;
client.SetPresence(RPCClient.Get().Presence);
await MainLoop();
async Task<bool> MainLoop()
{
    /*
     * Enter a infinite loop, polling the Discord Client for events.
     * In game termonology, this will be equivalent to our main game loop. 
     * If you were making a GUI application without a infinite loop, you could implement
     * this with timers.
    */
    isRunning = true;
    while (client != null && isRunning)
    {
        //We will invoke the client events. 
        // In a game situation, you would do this in the Update.
        // Not required if AutoEvents is enabled.
        //if (client != null && !client.AutoEvents)
        //	client.Invoke();

        //Try to read any keys if available
        if (Console.KeyAvailable)
            Console.ReadKey();
        var game = await GameHandler.FetchGaming(lastGame);
        lastGame = game;
        if (game != null)
        {
            var pres = RPCClient.Get().Presence;
            pres.State = $"Playing {game.Name} on {game.Provider}";
            pres.Assets.LargeImageKey = game.ArtworkGame;
            pres.Assets.SmallImageKey = game.ArtworkProvider;
        } else
        {
            var pres = RPCClient.Get().Presence;
            pres.State = $"Idling";
            pres.Assets.LargeImageKey = "";
            pres.Assets.SmallImageKey = "";
        }
        //This can be what ever value you want, as long as it is faster than 30 seconds.
        //Console.Write("+");
        Thread.Sleep(5);

        client.SetPresence(RPCClient.Get().Presence);
    }

    Console.WriteLine("Press any key to terminate");
    Console.ReadKey();
    return true;
}
