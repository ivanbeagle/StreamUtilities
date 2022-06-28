using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        }

        public void Save()
        {
            var cfg = StreamUtilitiesSettings.Default;
            cfg.Hotkey = Hotkey;
            cfg.TwitchUsername = TwitchUsername;
            cfg.TwitchAccessToken = TwitchAccessToken;
            cfg.TwitchChannel = TwitchChannel;

            cfg.Save();
        }

        public void LoadFromConfig()
        {
            var cfg = StreamUtilitiesSettings.Default;
            Hotkey = cfg.Hotkey;
            TwitchUsername = cfg.TwitchUsername;
            TwitchAccessToken = cfg.TwitchAccessToken;
            TwitchChannel = cfg.TwitchChannel;
        }
    }
}
