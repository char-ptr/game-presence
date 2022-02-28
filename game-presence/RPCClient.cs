using DiscordRPC;

namespace game_presence;

public class RPCClient
{
    public RichPresence Presence { get; set; } = new RichPresence();
    private RPCClient()
    {
        // Prevent outside instantiation
    }

    private static readonly RPCClient _singleton = new RPCClient();

    public static RPCClient Get()
    {
        return _singleton;
    }
}