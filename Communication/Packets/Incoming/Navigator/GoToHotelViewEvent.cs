﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Rooms.Session;
using Plus.HabboRoleplay.Misc;

namespace Plus.Communication.Packets.Incoming.Navigator
{
    class GoToHotelViewEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            int OldRoom = Session.GetHabbo().HomeRoom;

            if (OldRoom <= 0)
                OldRoom = 1;

            RoleplayManager.SendUserOld(Session, OldRoom, "");

            /*
            if (Session.GetHabbo().InRoom)
            {
                Room OldRoom;

                if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out OldRoom))
                    return;

                if (OldRoom.GetRoomUserManager() != null)
                    OldRoom.GetRoomUserManager().RemoveUserFromRoom(Session, true, false);
            }
            */
        }
    }
}
