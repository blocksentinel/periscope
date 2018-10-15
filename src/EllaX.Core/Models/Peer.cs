﻿using System;

namespace EllaX.Core.Models
{
    public class Peer
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string LocalAddress { get; set; }
        public string RemoteAddress { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public DateTimeOffset FirstSeenDate { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset LastSeenDate { get; set; } = DateTimeOffset.UtcNow;
    }
}
