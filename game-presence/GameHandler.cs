using System.Diagnostics;
using System.Runtime.InteropServices;
using FetchRbxGame;
using Microsoft.Win32;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;

namespace game_presence;

public class GameHandler
{
    static public SteamWebInterfaceFactory SteamInterface =
        new SteamWebInterfaceFactory("1762C1EA2CDAF1E953F7804D9D9F7DBB");

    static public ulong GetCurrentSteamUser()
    {
        var reg = Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam\\ActiveProcess")?.GetValue("ActiveUser");
        return Convert.ToUInt64(reg);
    }

    static public uint GetCurrentSteamGame()
    {
        var reg = Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam")?.GetValue("RunningAppID");
        return Convert.ToUInt32(reg);
    }

    static public bool IsGaming()
    {
        // is on steam
        uint RunningSteamGame = GetCurrentSteamGame();
        var Robloxes = Process.GetProcessesByName("RobloxPlayerBeta");
        return (RunningSteamGame != 0) || Robloxes.Length > 0;
    }

    static public async Task<Game?> FetchGaming(Game? lastGame)
    {
        if (!IsGaming())
            return null;
        uint RunningSteamGame = GetCurrentSteamGame();
        if (RunningSteamGame != 0)
        {
            if (lastGame?.SteamId == RunningSteamGame) return lastGame;
            var store = SteamInterface.CreateSteamStoreInterface(new HttpClient());
            var gameData = await store.GetStoreAppDetailsAsync(RunningSteamGame);
            Console.WriteLine("fetch steam data");
            return new Game()
            {
                Name = gameData.Name, 
                Provider = "Steam", 
                SteamId = RunningSteamGame,
                ArtworkGame = gameData.HeaderImage, 
                ArtworkProvider = "https://upload.wikimedia.org/wikipedia/commons/thumb/8/83/Steam_icon_logo.svg/2048px-Steam_icon_logo.svg.png"
            };
        }

        if (lastGame?.Provider == "Roblox")
            return lastGame;
        try
        {
            var roblox = Roblox.Get();
            roblox.Attach();
            var uni = await roblox.GetUniverseId();
            var thumbnail = await roblox.GetThumbnail(uni);
            var gameData = await roblox.GetGameData(uni);
            Console.WriteLine("Fetched roblox data");
            return new Game()
            {
                Name = gameData?.name ?? "Unknown",
                Provider = "Roblox",
                ArtworkGame = thumbnail,
                ArtworkProvider = "https://doy2mn9upadnk.cloudfront.net/uploads/default/original/4X/0/e/e/0eeeb19633422b1241f4306419a0f15f39d58de9.png"
            };
        } catch (Exception ex)
        {
            return null;
        }
        
    }
    
}

public class Game
{
    public String Name {get; set; }
    public String Provider { get; set; }
    public String ArtworkGame { get; set; }
    public String ArtworkProvider { get; set; }
    public uint? SteamId { get; set; }
}