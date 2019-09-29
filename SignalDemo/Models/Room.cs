using System.Collections.Generic;

namespace SignalDemo.Models
{
    public class Room
    {
        public string RoomName { get; set; }

        public List<User> Users { get; set; } = new List<User>();
    }
}