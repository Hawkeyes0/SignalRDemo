using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SignalDemo.Models;

namespace SignalDemo.Hubs
{
    public class ChatHub : Hub
    {
        private List<User> _users = new List<User>();

        private List<Room> _rooms = new List<Room>();

        public void JoinIn(string username)
        {
            if (_users.Any(e => e.Username == username))
            {
                throw new HubException("User name has exists.");
            }
            _users.Add(new User { Username = username, ConnectionId = Context.ConnectionId });
        }

        public async Task SendMessage(MessagePack message)
        {
            var user = _users.Where(e => e.Username == message.Receiver);
            if (!user.Any())
            {
                throw new HubException("Unknown user");
            }
            if (message.Receiver == null)
            {
                await Clients.All.SendAsync("ReceiveMessage", message);
            }
            else
            {
                await Clients.Client(user.Single().ConnectionId).SendAsync("ReceiveMessage", message);
            }
        }

        public async Task EnterRoom(string roomName)
        {
            var user = _users.Where(e => e.ConnectionId == Context.ConnectionId);
            if (!user.Any())
            {
                throw new HubException("Unknown user");
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("Send", $"{user.Single().Username} has joined the room {roomName}.");
            await Clients.Group(roomName).SendAsync("AddMember", roomName, user.Single().Username);
            var room = _rooms.Where(e => e.RoomName == roomName);
            if (room.Any())
            {
                room.Single().Users.Add(user.Single());
            }
            else
            {
                Room r = new Room { RoomName = roomName };
                r.Users.Add(user.Single());
                _rooms.Add(r);
            }
        }

        public async Task LeaveRoom(string roomName)
        {
            var user = _users.Where(e => e.ConnectionId == Context.ConnectionId);
            if (!user.Any())
            {
                throw new HubException("Unknown user");
            }
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("Send", $"{user.Single().Username} has leaved the room {roomName}.");
            await Clients.Group(roomName).SendAsync("RemoveMember", roomName, user.Single().Username);
            var room = _rooms.Where(e => e.RoomName == roomName);
            if (room.Any())
            {
                room.Single().Users.Remove(user.Single());
            }
            else
            {
                throw new HubException("Unknown room");
            }
        }
    }
}