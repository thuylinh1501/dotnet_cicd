using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using AuthenticationService.Data;
using AuthenticationService.DTOs;
using AuthenticationService.Models;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Service {
    public class AuthService {
        private readonly AuthDbContext _context;
        private readonly IConfiguration _configuration;
        // private readonly IHttpClientFactory _httpClientFactory;

        private readonly KafkaProducerService _kafkaProducerService;

        public AuthService(
            AuthDbContext context,
            IConfiguration configuration,
            // IHttpClientFactory httpClientFactory
            KafkaProducerService kafkaProducerService
        ) {
            _context = context;
            _configuration = configuration;
            // _httpClientFactory = httpClientFactory;
            _kafkaProducerService = kafkaProducerService;
        }

        public async Task<AuthResponse> Register(RegisterDTO registerDTO) {
            if(await _context.Users.AnyAsync(u => u.Email ==registerDTO.Email)) {
                throw new Exception("Email already exists");
            }

            var user = new User {
                Username = registerDTO.Username,
                Email = registerDTO.Email,
                PasswordHash = registerDTO.Password
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // send email welcome -> call API Email service
            // var client = _httpClientFactory.CreateClient("EmailService");
            // await client.PostAsJsonAsync("api/Email/welcome", new {
            //     Email=user.Email,
            //     Username=user.Username
            // });

            // dùng kafka producer
            await _kafkaProducerService.PublishUserRegisteredEvent(user.Email, user.Username);

            return new AuthResponse {
                Token = "test",
                Username=user.Username,
                Email=user.Email
            };
        }

        public async Task<AuthResponse?> Login(LoginDTO loginDTO) {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDTO.Email);
            
            if (user == null || user.PasswordHash != loginDTO.Password) {
                return null; // Invalid credentials
            }

            return new AuthResponse {
                Token = "test-login-token",
                Username = user.Username,
                Email = user.Email
            };
        }
    }
}