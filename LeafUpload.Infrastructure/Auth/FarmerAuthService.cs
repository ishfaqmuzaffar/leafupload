using LeafUpload.Core.Abstractions;
using LeafUpload.Core.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace LeafUpload.Infrastructure.Auth
{
    public class FarmerAuthService : IFarmerAuthService
    {
        private readonly IFarmerRepository _repository;
        private readonly IPasswordHasher<Farmer> _passwordHasher;

        public FarmerAuthService(IFarmerRepository repository, IPasswordHasher<Farmer> passwordHasher)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
        }

        public async Task<AuthResult> RegisterAsync(string username, string password, Farm farm)
        {
            var existing = await _repository.GetFarmerByUsernameAsync(username);
            if (existing != null)
                return AuthResult.Failure("That username is already taken.");

            var farmer = new Farmer { Username = username };
            farmer.PasswordHash = _passwordHasher.HashPassword(farmer, password);

            await _repository.CreateFarmerWithFarmAsync(farmer, farm);
            return AuthResult.Success(farmer);
        }

        public async Task<Farmer?> ValidateCredentialsAsync(string username, string password)
        {
            var farmer = await _repository.GetFarmerByUsernameAsync(username);
            if (farmer == null)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(farmer, farmer.PasswordHash, password);
            return result == PasswordVerificationResult.Failed ? null : farmer;
        }
    }
}
