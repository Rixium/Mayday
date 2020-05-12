using System;
using Steamworks;

namespace Yetiface.Steamworks
{
    public class Steam
    {
        
        public Action Exit { get; set; }

        public Steam(uint appId)
        {
            try
            {
                if (SteamAPI.RestartAppIfNecessary(new AppId_t(appId)))
                {
                    Console.Out.WriteLine("Game wasn't started by Steam-client. Restarting.");
                    Exit?.Invoke();
                }
            }
            catch (DllNotFoundException exception)
            {
                Console.Out.WriteLine("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib." +
                                      " It's likely not in the correct location. Refer to the README for more details.\n" +
                                      exception);
                Exit?.Invoke();
            }

            Initialize();
        }
        
        public void Initialize()
        {
            SteamAPI.Init();
        }

        public string GetSteamName() => SteamFriends.GetPersonaName();
        public int GetSteamFriendCount() => SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagAll);
    }
}