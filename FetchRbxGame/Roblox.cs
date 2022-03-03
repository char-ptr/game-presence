using System.Diagnostics;
using System.Net.Http.Json;
using System.Runtime.InteropServices;

namespace FetchRbxGame
{

    public class Roblox
    {

        private GameMemory? _robloxProcess;
        private static Roblox _instance = new Roblox();
        private long _knownId = 0L;
        private HttpClient _httpClient = new HttpClient();

        public void Attach()
        {
            try
            {
                _robloxProcess = new GameMemory("RobloxPlayerBeta");
                GetId();
            }
            catch (Exception e)
            {
                Console.WriteLine("Not running x");
            }
            finally
            {
                if (_robloxProcess == null)
                {
                    Console.WriteLine("Unable to find roblox!");
                }
            }
        }

        public async Task<HttpData.Game?> GetGameData(long Universe)
        {
            return await _httpClient.GetFromJsonAsync<HttpData.Game>(
                $"https://develop.roblox.com/v1/universes/{Universe}");
        }

        public async Task<long> GetUniverseId()
        {
            return (await _httpClient.GetFromJsonAsync<HttpData.UniverseIdData>(
                       $"https://api.roblox.com/universes/get-universe-containing-place?placeid={_knownId}"))
                   ?.UniverseId ??
                   0L;

        }

        public async Task<string> GetThumbnail(long universeId)
        {
            var data = await _httpClient.GetFromJsonAsync<HttpData.Thumbnail.Root>(
                $"https://thumbnails.roblox.com/v1/games/icons?universeIds={universeId}&size=256x256&format=Png&isCircular=false");
            return data?.data.First().imageUrl ?? "";
        }

        private void GetId()
        {
            try
            {
                _knownId = _robloxProcess.ReadProcessMem<long>(
                    (_robloxProcess.gameProcess?.MainModule?.BaseAddress ?? IntPtr.Zero) + 0x2C0ADB0);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to read mem, try attaching again");
            }
        }

        public static Roblox Get()
        {
            return _instance;
        }

    }
}
namespace HttpData
{
    namespace Thumbnail
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Datum
        {
            public long targetId { get; set; }
            public string state { get; set; }
            public string imageUrl { get; set; }
        }

        public class Root
        {
            public List<Datum> data { get; set; }
        }
    }

    public class Game
    {
        public long id { get; set; }
        public string name { get; set; }
        public object description { get; set; }
        public bool isArchived { get; set; }
        public long rootPlaceId { get; set; }
        public bool isActive { get; set; }
        public string privacyType { get; set; }
        public string creatorType { get; set; }
        public int creatorTargetId { get; set; }
        public string creatorName { get; set; }
        public DateTime created { get; set; }
        public DateTime updated { get; set; }
    }



    public class UniverseIdData
    {
        public long UniverseId { get; set; }
    }
}