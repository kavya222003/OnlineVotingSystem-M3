using OnlineVoting_API.DTOs.Auth;
using OnlineVoting_API.Helpers;
using OnlineVoting_API.Models;
using OnlineVoting_API.Repositories;
using static OnlineVoting_API.DTOs.Auth.AuthDto;

namespace OnlineVoting_API.Services
{
    public class AuthService
    {
        private readonly UserRepository _userRepo;
        private readonly PasswordHasher _hasher;
        private readonly JwtHelper _jwt;

        public AuthService(UserRepository userRepo, PasswordHasher hasher, JwtHelper jwt)
        {
            _userRepo = userRepo;
            _hasher = hasher;
            _jwt = jwt;
        }

        public async Task<string> Register(RegisterDto dto)
        {
            var existing = await _userRepo.GetByEmailAsync(dto.Email);
            if (existing != null)
                throw new Exception("User already exists");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = _hasher.Hash(dto.Password)
            };

            await _userRepo.AddUserAsync(user);

            return "User registered successfully";
        }

        public async Task<AuthResponseDto> Login(LoginDto dto)
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email);
            if (user == null || !_hasher.Verify(dto.Password, user.PasswordHash))
                throw new Exception("Invalid credentials");

            var token = _jwt.GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email
            };
        }
    }
}