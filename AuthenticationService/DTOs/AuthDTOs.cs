namespace AuthenticationService.DTOs {
    public class RegisterDTO {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }

    public class LoginDTO {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class AuthResponse {
        public string Token { get; set; }
        public string Username {get; set;}
        public string Email {get; set;}
    }
}