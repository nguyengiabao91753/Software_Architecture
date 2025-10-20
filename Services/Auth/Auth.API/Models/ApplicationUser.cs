using Microsoft.AspNetCore.Identity;

namespace Auth.API.Models
{
    public class ApplicationUser :IdentityUser
    {
        public string? PublicKey { get; set; }

        public byte[]? RsaPrivateKeyEncrypted { get; set; }

        public int StorageLimitMB { get; set; } = 10240; // 10 GB default

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
