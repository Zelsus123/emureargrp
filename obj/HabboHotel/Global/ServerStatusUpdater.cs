﻿using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Generic;

using log4net;
using Plus.Database.Interfaces;


namespace Plus.HabboHotel.Global
{
    public class ServerStatusUpdater : IDisposable
    {
        private static ILog log = LogManager.GetLogger("Mango.Global.ServerUpdater");

        private const int UPDATE_IN_SECS = 30;

        private static int _userPeak, _lastDatePeak;

        private static string _lastDate;

        private Timer _timer;

        public ServerStatusUpdater()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT userpeak FROM server_status");
                _userPeak = dbClient.getInteger();

                
            }
        }

        public void Init()
        {
            this._timer = new Timer(new TimerCallback(this.OnTick), null, TimeSpan.FromSeconds(UPDATE_IN_SECS), TimeSpan.FromSeconds(UPDATE_IN_SECS));

            Console.Title = "RDP Emulator - 0 users online - 0 rooms loaded - 0 day(s) 0 hour(s) uptime";

            log.Info("Server Status Updater has been started.");
        }

        public void OnTick(object Obj)
        {
            this.UpdateOnlineUsers();
        }

        private void UpdateOnlineUsers()
        {
            TimeSpan Uptime = DateTime.Now - PlusEnvironment.ServerStarted;

            int UsersOnline = Convert.ToInt32(PlusEnvironment.GetGame().GetClientManager().Count);
            int RoomCount = PlusEnvironment.GetGame().GetRoomManager().Count;

            Console.Title = "RDP Emulator - " + UsersOnline + " users online - " + RoomCount + " rooms loaded - " + Uptime.Days + " day(s) " + Uptime.Hours + " hour(s) uptime";

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                if (UsersOnline > _userPeak)
                    _userPeak = UsersOnline;

                
                _lastDate = DateTime.Now.ToShortDateString();
                dbClient.SetQuery("UPDATE server_status SET users_online = @users, loaded_rooms = @loadedRooms, userpeak = @upeak LIMIT 1;");
                dbClient.AddParameter("users", UsersOnline);
                dbClient.AddParameter("loadedRooms", RoomCount);
                dbClient.AddParameter("upeak", _userPeak);
                dbClient.RunQuery();
            }
        }


        public void Dispose()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE server_status SET users_online = '0', loaded_rooms = '0', status = '0'");
            }

            this._timer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}