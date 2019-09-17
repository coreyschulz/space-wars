using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceWars
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ship
    {
        [JsonProperty(PropertyName = "ship")]
        private int ID;

        [JsonProperty]
        private Vector2D loc;

        [JsonProperty]
        private Vector2D dir;

        [JsonProperty]
        private bool thrust;

        [JsonProperty]
        private string name;

        [JsonProperty]
        private int hp;

        [JsonProperty]
        private int score;

        public ship()
        {
        }

    }
}
