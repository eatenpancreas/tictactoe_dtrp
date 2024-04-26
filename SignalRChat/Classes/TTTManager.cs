using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;

namespace SignalRChat.Classes
{
    public class TTTManager
    {
        private static TTTManager instance;

        private static readonly object lockObject = new object();

        public List<TTTSession> sessions = new List<TTTSession>();

        private TTTManager() { }

        public static TTTManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new TTTManager();
                        }
                    }
                }
                return instance;
            }
        }
    }
}
