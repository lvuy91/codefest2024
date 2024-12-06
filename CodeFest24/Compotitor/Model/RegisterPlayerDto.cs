using System.Text.Json.Serialization;

namespace Compotitor.Model
{
    public class RegisterPlayerDto
    {
        [JsonPropertyName("game_id")]
        public string GameId { get; set; }
        [JsonPropertyName("player_id")]
        public string PlayerId { get; set; }
    }
    public class UserDto
    {
        [JsonPropertyName("gameId")]
        public string gameId { get; set; }
        [JsonPropertyName("type")]
        public string type { get; set; }
        [JsonPropertyName("playerId")]
        public string playerId { get; set; }
    }
}