﻿using System.Text.Json.Serialization;

namespace ASP_SPR311.Data.Entities
{
    public class AccessToken
    {
        public Guid Jti {  get; set; }
        public Guid Sub { get; set; }
        public Guid Aud { get; set; }
        public DateTime Iat { get; set; } = DateTime.Now;
        public DateTime? Nbf { get; set; }
        public DateTime? Exp { get; set; }
        public String? Iss { get; set; }

        //[JsonIgnore]
        public UserData User { get; set; } = null!;
        [JsonIgnore]
        public UserAccess UserAccess { get; set; } = null!;
    }
}
