﻿namespace Core.DTO
{
    public class UserLoginDto
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool RememberMe { get; set; } = false;
    }
}
