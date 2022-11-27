using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vanara.Extensions;
using Vanara.PInvoke;

namespace StreamUtilities
{
    internal sealed class UserConfig
    {
        #region Fields
        private static UserConfig _singleton;
        #endregion

        #region Properties
        public Keys Hotkey { get; set; }
        public string TwitchUsername { get; set; }

        public string TwitchAccessToken { get; set; }

        public string TwitchChannel { get; set; }

        public string TwitchIgnoreNames { get; set; }

        public static UserConfig Singleton => _singleton ?? (_singleton = new UserConfig());
        #endregion

        #region Ctors
        private UserConfig() { }
        #endregion

        public void LoadFactoryDefault()
        {
            Hotkey = Keys.D0;
            TwitchUsername = "beagleinteractive";
            TwitchAccessToken = "[access_token]";
            TwitchChannel = TwitchUsername;
            TwitchIgnoreNames = "commanderroot,aliceydra,0ax2,streamlabs,streamlabsbot,restream,restreambot,lurxx,saralna";
        }

        public void Save()
        {
            var key = new List<byte>(TwitchUsername.GetBytes());
            key.AddRange(new byte[] { 7, 12, 20, 18 });

            var crypto = Crypt.Protect(key.ToArray(), TwitchAccessToken.GetBytes());
            var base64 = Convert.ToBase64String(crypto);

            var cfg = StreamUtilitiesSettings.Default;
            cfg.Hotkey = Hotkey;
            cfg.TwitchUsername = TwitchUsername;
            cfg.TwitchAccessToken = base64;
            cfg.TwitchChannel = TwitchChannel;
            cfg.TwitchIgnoreNames = TwitchIgnoreNames;

            cfg.Save();
        }

        public void LoadFromConfig()
        {
            var cfg = StreamUtilitiesSettings.Default;

            try {
                var key = new List<byte>(cfg.TwitchUsername.GetBytes());
                key.AddRange(new byte[] { 7, 12, 20, 18 });

                var crypto = Convert.FromBase64String(cfg.TwitchAccessToken);
                var accesstoken = Crypt.Unprotect(key.ToArray(), crypto);
                TwitchAccessToken = Encoding.Unicode.GetString(accesstoken);
            }
            catch
            {
                TwitchAccessToken = "[invalid]";
            }

            Hotkey = cfg.Hotkey;
            TwitchUsername = cfg.TwitchUsername;
            TwitchChannel = cfg.TwitchChannel;
            TwitchIgnoreNames = cfg.TwitchIgnoreNames;
        }
    }
}
