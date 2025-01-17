﻿using System;
using System.Threading;
using Plus.HabboHotel.GameClients;
using Plus.Utilities;
using Plus.Core;
using Plus.HabboRoleplay.Misc;
using Plus.HabboHotel.Rooms;

namespace Plus.HabboRoleplay.Timers
{
    public abstract class RoleplayTimer
    {
        /// <summary>
        /// The client
        /// </summary>
        public GameClient Client;

        /// <summary>
        /// The timer
        /// </summary>
        private Timer Timer;

        /// <summary>
        /// The type of timer
        /// </summary>
        public string Type;

        /// <summary>
        /// Time interval
        /// </summary>
        private int Time;

        /// <summary>
        /// Random number generator
        /// </summary>
        public CryptoRandom Random = new CryptoRandom();

        /// <summary>
        /// Represents if the timer should last forever
        /// </summary>
        private bool Forever;

        /// <summary>
        /// Represents the time left if specified
        /// </summary>
        public int TimeLeft = 0;

        /// <summary>
        /// Represents the amount of times the timer has looped
        /// </summary>
        public int TimeCount = 0;

        /// <summary>
        /// Represents the amount of times the timer has looped 2
        /// </summary>
        public int TimeCount2 = 0;

        /// <summary>
        /// Represents the original time
        /// </summary>
        public int OriginalTime = 0;

        /// <summary>
        /// Represents any special data
        /// </summary>
        public object[] Params;

        /// <summary>
        /// Constructor
        /// </summary>
        public RoleplayTimer(string Type, GameClient Client, int Time, bool Forever, object[] Params)
        {
            this.Type = Type;
            this.Client = Client;
            this.Time = Time;
            this.Forever = Forever;
            this.Params = Params;
            this.Timer = new Timer(Finished, null, Time, Time);
        }

        /// <summary>
        /// Called when the timer finishes/ticks
        /// </summary>
        private void Finished(object State)
        {
            try
            {
                Execute();

                if (Forever && Timer != null)
                {
                    Timer.Change(Time, Time);
                    return;
                }

                if (TimeLeft <= 0)
                    EndTimer();
            }
            catch (Exception e)
            {
                Logging.LogRPTimersError("An error occurred when trying to finish a timer: " + e);
                EndTimer();
            }
        }

        /// <summary>
        /// Ends our timer
        /// </summary>
        public void EndTimer()
        {
            try
            {
                if (Timer == null)
                    return;
                
                Timer.Change(Timeout.Infinite, Timeout.Infinite);
                Timer.Dispose();
                Timer = null;

                if (Client != null && Client.GetPlay() != null)
                {
                    RoleplayTimer Junk;
                    Client.GetPlay().TimerManager.ActiveTimers.TryRemove(Type, out Junk);
                    
                    // Limpiamos WS
                    if (Client.GetPlay().WebSocketConnection != null)
                    {
                        switch (Type)
                        {
                            #region Wanted Stars
                            case "wanted":
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "compose_wanted_stars|0");// Enviamos con Nivel de busqueda 0
                                break;
                            #endregion

                            #region Default
                            default:
                                break;
                            #endregion
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logging.LogRPTimersError("Error in EndTimer() void: " + e);
            }
        }

        /// <summary>
        /// Called when the timer finishes/ticks
        /// </summary>
        public abstract void Execute();
    }
    
    public abstract class SystemRoleplayTimer
    {
        /// <summary>
        /// The timer
        /// </summary>
        private Timer Timer;

        /// <summary>
        /// The type of timer
        /// </summary>
        public string Type;

        /// <summary>
        /// Time interval
        /// </summary>
        private int Time;

        /// <summary>
        /// Random number generator
        /// </summary>
        public CryptoRandom Random = new CryptoRandom();

        /// <summary>
        /// Represents if the timer should last forever
        /// </summary>
        private bool Forever;

        /// <summary>
        /// Represents the time left if specified
        /// </summary>
        public int TimeLeft = 0;

        /// <summary>
        /// Represents the amount of times the timer has looped
        /// </summary>
        public int TimeCount = 0;

        /// <summary>
        /// Represents the amount of times the timer has looped
        /// </summary>
        public int TimeCount2 = 0;

        /// <summary>
        /// Represents the original time
        /// </summary>
        public int OriginalTime = 0;

        /// <summary>
        /// Represents any special data
        /// </summary>
        public object[] Params;

        /// <summary>
        /// Constructor
        /// </summary>
        public SystemRoleplayTimer(string Type, int Time, bool Forever, object[] Params)
        {
            this.Type = Type;
            this.Time = Time;
            this.Forever = Forever;
            this.Params = Params;
            this.Timer = new Timer(Finished, null, Time, Time);
        }

        /// <summary>
        /// Called when the timer finishes/ticks
        /// </summary>
        private void Finished(object State)
        {
            try
            {
                Execute();

                if (Forever && Timer != null)
                {
                    Timer.Change(Time, Time);
                    return;
                }

                if (TimeLeft <= 0)
                    EndTimer();
            }
            catch (Exception e)
            {
                Logging.LogRPTimersError("An error occurred when trying to finish a timer: " + e);
                EndTimer();
            }
        }

        /// <summary>
        /// Ends our timer
        /// </summary>
        public void EndTimer()
        {
            try
            {
                if (Timer == null)
                    return;

                Timer.Change(Timeout.Infinite, Timeout.Infinite);
                Timer.Dispose();
                Timer = null;

                if (RoleplayManager.TimerManager != null)
                {
                    SystemRoleplayTimer Junk;
                    RoleplayManager.TimerManager.ActiveTimers.TryRemove(Type, out Junk);
                }
            }
            catch (Exception e)
            {
                Logging.LogRPTimersError("Error in EndTimer() void: " + e);
            }
        }

        /// <summary>
        /// Called when the timer finishes/ticks
        /// </summary>
        public abstract void Execute();
    }
}