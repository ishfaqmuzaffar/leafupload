using LeafUpload.Core.Models;

namespace LeafUpload.Core.Abstractions
{
    // Issues bearer tokens for the mobile app. The web app keeps using cookie auth
    // (see AccountController.SignInFarmerAsync) - this is purely for api/mobile/* clients.
    public interface IJwtTokenService
    {
        string IssueToken(Farmer farmer);
    }
}
