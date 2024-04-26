using Microsoft.AspNetCore.SignalR;
using SignalRChat.Classes;
using System.Collections.Generic;

namespace SignalRChat.Hubs
{
    public class GameHub : Hub
    {
        public async Task<object> createSession(string name, string owner)
        {
            var ttt = TTTManager.Instance;

            if (ttt.sessions.Find(s => s.name == name) != null)
            {
                return new { response = "Session already exists." };
            }

            if (ttt.sessions.Find(s => s.player_1 == owner) != null || ttt.sessions.Find(s => s.player_2 == owner) != null)
            {
                return new { response = "You are already in a session." };
            }

            ttt.sessions.Add(new TTTSession(name, owner));

            await Clients.All.SendAsync("sessionUpdated");
            return new { response = "Session succesufully created." };
        }

        public async Task<object> joinSession(string session, string user)
        {
            var ttt = TTTManager.Instance;
            var foundSession = ttt.sessions.Find(s => s.name == session);

            if(foundSession == null)
            { 
                return new { response = "Session does not exist." };
            }

            if(foundSession.player_2 != null)
            {
                return new { response = "Session already full." };
            }

            if(foundSession.player_1 == user || foundSession.player_2 == user)
            {
                return new { response = "You cannot join your own session." };
            }

            foundSession.addOpponent(user);
            await Clients.All.SendAsync("sessionUpdated");
            await Clients.All.SendAsync("matchStart", new { session=foundSession, user=user});

            return new { response = "Session joined!" };
        }

        public async Task<object> leaveSession(string session, string user)
        {
            var ttt = TTTManager.Instance;
            var foundSession = ttt.sessions.Find(s => s.name == session);

            if (foundSession == null)
            {
                return new { response = "Session does not exist." };
            }

            if (foundSession.player_1 == user)
            {
                foundSession.player_1 = null;
            }
            else if (foundSession.player_2 == user)
            {
                foundSession.player_2 = null;
            }
            else
            {
                return new { response = "You are not in this session." };
            }

            if (foundSession.player_1 == null && foundSession.player_2 == null)
            {
                ttt.sessions.Remove(foundSession);
            }

            await Clients.All.SendAsync("sessionUpdated");
            return new { response = "You have left the session." };
        }

        public TTTSession getSessionFromUser(string user)
        {
            var ttt = TTTManager.Instance;
            var session = ttt.sessions.Find(s => s.player_1 == user || s.player_2 == user);
            return session;
        }

        public object fetchBoard(string sessionName)
        {
            var ttt = TTTManager.Instance;
            var session = ttt.sessions.Find(s => s.name == sessionName);
            if (session == null)
            {
                return new { 
                    success = false,
                    response = "Session does not exist." 
                };
            }

            return new
            {
                success = true,
                response = "Got board",
                board = session.getBoard()
            };
        }

        public async Task<object> gameOver(string session_name, string result)
        {
            var ttt = TTTManager.Instance;
            var session = ttt.sessions.Find(s => s.name == session_name);

            if (session == null)
            {
                return new { response = "Session does not exist." };
            }

            await Clients.All.SendAsync("gameOver", session, result);
            return new { response = "successful." };
        }

        public Task<object> deleteSession(string sessionName)
        {
            var ttt = TTTManager.Instance;
            var session = ttt.sessions.Find(s => s.name == sessionName);
            ttt.sessions.Remove(session);
            return Task.FromResult<object>(new { response = "Success" });
        }

        public async Task<object> makeMove(string sessionName, int position, string player)
        {
            var ttt = TTTManager.Instance;
            var session = ttt.sessions.Find(s => s.name == sessionName);
            if (session == null)
            {
                return new { response = "Session does not exist." };
            }

            if (!session.move(position, player))
            {
                return new { response = "Invalid move." };
            }

            string result = session.checkWinner();
            if (!string.IsNullOrEmpty(result))
            {
                await Clients.All.SendAsync("gameOver", session, result);

//                ttt.sessions.Remove(session);
            }

            await Clients.All.SendAsync("updateBoard", session, session.getBoard());
            return new { response = "Move successful." };
        }

        public IEnumerable<TTTSession> getSessions()
        {
            var TTTMananger = TTTManager.Instance;
            return TTTMananger.sessions;
        }
    }
}