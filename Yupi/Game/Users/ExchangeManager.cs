using System;
using System.Linq;
using System.Timers;
using Yupi.Core.Io;
using Yupi.Core.Settings;

namespace Yupi.Game.Users
{
    /// <summary>
    ///     Class CoinsManager.
    /// </summary>
    internal class ExchangeManager
    {
        /// <summary>
        ///     The _timer
        /// </summary>
        private static Timer _timer;

        /// <summary>
        ///     Starts the timer.
        /// </summary>
        internal void StartTimer()
        {
            if (!ServerExtraSettings.CurrencyLoopEnabled)
                return;
            _timer = new Timer(ServerExtraSettings.CurrentlyLoopTimeInMinutes*60000);
            _timer.Elapsed += GiveCoins;
            _timer.Enabled = true;
        }

        /// <summary>
        ///     Gives the coins.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs" /> instance containing the event data.</param>
        internal void GiveCoins(object source, ElapsedEventArgs e)
        {
            try
            {
                var clients = Yupi.GetGame().GetClientManager().Clients.Values;

                foreach (var client in clients.Where(client => client?.GetHabbo() != null))
                {
                    client.GetHabbo().Credits += (uint)ServerExtraSettings.CreditsToGive;
                    client.GetHabbo().UpdateCreditsBalance();
                    client.GetHabbo().ActivityPoints += (uint)ServerExtraSettings.PixelsToGive;

                    if (ServerExtraSettings.DiamondsLoopEnabled && ServerExtraSettings.DiamondsVipOnly)
                        client.GetHabbo().Diamonds += client.GetHabbo().Vip || client.GetHabbo().Rank >= 6 ? (uint) ServerExtraSettings.DiamondsToGive : (uint) ServerExtraSettings.DiamondsToGive;

                    client.GetHabbo().UpdateSeasonalCurrencyBalance();
                }
            }
            catch (Exception ex)
            {
                Writer.LogException(ex.ToString());
            }
        }

        /// <summary>
        ///     Destroys this instance.
        /// </summary>
        internal void Destroy()
        {
            _timer.Dispose();
            _timer = null;
        }
    }
}