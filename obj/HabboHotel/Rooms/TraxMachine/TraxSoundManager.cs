﻿using log4net;
using Plus.Core;
using Plus.HabboHotel.Items;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.HabboHotel.Rooms.TraxMachine
{
    public class TraxSoundManager
    {
        public static List<TraxMusicData> Songs = new List<TraxMusicData>();

        //public static Dictionary<int, Item> RoomsMusicItems = new Dictionary<int, Item>();

        private static ILog Log = LogManager.GetLogger("Plus.HabboHotel.Rooms.TraxMachine");
        public static void Init()
        {
            Songs.Clear();

            DataTable table;
            using (var adap = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                adap.RunQuery("SELECT * FROM jukebox_songs_data");
                table = adap.getTable();
            }

            foreach (DataRow row in table.Rows)
            {
                Songs.Add(TraxMusicData.Parse(row));
            }

            /*using (var adap = DatabaseManager.GetQueryReactor())
            {
                adap.RunQuery("SELECT * FROM room_jukebox_songs");
                table = adap.getTable();
            }

            foreach (DataRow row in table.Rows)
            {
                var roomid = int.Parse(row["room_id"].ToString());
                var itemid = int.Parse(row["item_id"].ToString());
            }*/

            Log.Info("Loaded " + Songs.Count + " Jukebox Songs.");
        }

        public static TraxMusicData GetMusic(int id)
        {
            foreach (var item in Songs)
                if (item.Id == id)
                    return item;

            return null;
        }
    }
}
