using LeafUpload.Core.Models;
using System.Threading.Tasks;

namespace LeafUpload.Core.Abstractions
{
    public interface IFarmerAuthService
    {
        Task<AuthResult> RegisterAsync(string username, string password, Farm farm);
        Task<Farmer?> ValidateCredentialsAsync(string username, string password);
    }
}
