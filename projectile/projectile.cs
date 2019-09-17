using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceWars
{
    public class projectile
    {
        [JsonProperty(PropertyName = "proj")]
        public int ID;

        [JsonProperty]
        private Vector2D loc;

        [JsonProperty]
        private Vector2D dir;

        [JsonProperty]
        private bool alive;

        [JsonProperty]
        private int owner;

        public projectile()
        {
        }
    }
}
