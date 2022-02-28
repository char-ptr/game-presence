// See https://aka.ms/new-console-template for more information

using FetchRbxGame;

var roblox = Roblox.Get();
roblox.Attach();
var uni = await roblox.GetUniverseId();
Console.WriteLine(uni);
var thumbnail = await roblox.GetThumbnail(uni);
Console.WriteLine(thumbnail);
var data = await roblox.GetGameData(uni);
Console.WriteLine(data.name);