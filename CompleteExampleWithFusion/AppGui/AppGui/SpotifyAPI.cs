using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpotifyAPI.Web;
using System.Windows;

namespace AppGui
{
    public class SpotifyAPI
    {
        private static SpotifyWebAPI _spotify;
        public SpotifyAPI()
        {
            MainAsync().Wait();
        }
        public async Task MainAsync()
        {
            WebAPIFactory webApiFactory = new WebAPIFactory(
                 "http://localhost",
                 8500,
                 "826dee0fe8264cd58a31afda79f2128e",
                 //Scope.UserReadPrivate,
                 Scope.PlaylistModifyPublic,
                 TimeSpan.FromSeconds(20)
            );

            try
            {
                //This will open the user's browser and returns once
                //the user is authorized.
                _spotify = await webApiFactory.GetWebApi();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (_spotify == null)
                return;
        }

        public SpotifyWebAPI getAPI()
        {
            return _spotify;
        }
    }
}
