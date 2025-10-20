using System;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Core.Auth
{
    public class ApplicationUser : IdentityUser
    {
        public string? PublicKey { get; set; }

        public byte[]? RsaPrivateKeyEncrypted { get; set; }

        public int StorageLimitMB { get; set; } = 10240; // 10 GB default

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
