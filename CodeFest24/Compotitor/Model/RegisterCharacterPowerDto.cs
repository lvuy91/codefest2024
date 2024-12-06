using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Compotitor
{
    public class RegisterCharacterPowerDto
    {
        [JsonPropertyName("gameId")]
        public string GameId { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; } // 1 for Mountain God, 2 for Sea God
    }
}