namespace LeafUpload.Core.Models
{
    public class AuthResult
    {
        public bool Succeeded { get; set; }
        public string? ErrorMessage { get; set; }
        public Farmer? Farmer { get; set; }

        public static AuthResult Success(Farmer farmer) => new() { Succeeded = true, Farmer = farmer };
        public static AuthResult Failure(string errorMessage) => new() { Succeeded = false, ErrorMessage = errorMessage };
    }
}
