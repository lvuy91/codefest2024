﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFest24.Model
{
    public class ActionDto
    {
        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("payload")]
        public Payload Payload { get; set; }

        [JsonPropertyName("characterType")]
        public string CharacterType { get; set; } // For "child" character, optional
    }

    public class Payload
    {
        [JsonPropertyName("destination")]
        public Destination Destination { get; set; }
    }

    public class Destination
    {
        [JsonPropertyName("col")]
        public int Column { get; set; }

        [JsonPropertyName("row")]
        public int Row { get; set; }
    }
}