using DiscordRPC;
using DiscordRPC.Logging;
using game_presence;

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


async void updateGame() {
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
    client.SetPresence(RPCClient.Get().Presence);

}


var inter = new System.Timers.Timer();
inter.Interval = 5000;
inter.AutoReset = true;
inter.Enabled = true;
inter.Elapsed += async (sender, e) => updateGame();
Console.WriteLine("Press any key to exit... ");
Console.ReadKey();