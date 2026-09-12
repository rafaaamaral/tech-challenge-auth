using System;
using System.Collections.Generic;
using System.Text;

namespace TechChallenge.Auth.Models
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
    }
}
