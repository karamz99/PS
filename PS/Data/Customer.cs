using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace PS.Data
{
    public class Customer
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Group))]
        [Display(Name = "Group")]
        [Required]
        public int? GroupId { get; set; }
        public Group? Group { get; set; }

        [Required]
        [Display(Name = "User name")]
        public string? Name { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string? Username { get; set; }

        public string? Salt { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [NotMapped]
        [Compare(nameof(Password))]
        [Display(Name = "Confirmation")]
        [DataType(DataType.Password)]
        public string? PasswordConfirmation { get; set; }

        [NotMapped]
        public string DataUsageSentFormated => FormatUsage(DataUsageSent);
        public long DataUsageSent { get; set; }

        [NotMapped]
        public string DataUsageRecievedFormated => FormatUsage(DataUsageRecieved);
        public long DataUsageRecieved { get; set; }

        public static string FormatUsage(long usage)
        {
            if (usage < 1024)
                return $"{usage:D}B";
            else if (usage < 1024 * 1024)
                return $"{usage / 1024:D}KB";
            else if (usage < 1024 * 1024 * 1024)
                return $"{usage / 1024 / 1024:D}MB";
            else if (usage < (long)(1024 * 1024) * (long)(1024 * 1024))
                return $"{usage / 1024 / 1024 / 1024:D}GB";
            else
                return $"{usage / 1024 / 1024 / 1024 / 1024:D}TB";
        }

        public long DataUsage => DataUsageRecieved + DataUsageSent;

        public string? IP { get; set; }
        public string? Mac { get; set; }
        public DateTime? LastRequestTime { get; set; }

        [NotMapped]
        public bool Saving { get; set; }

        public void HashPassword()
        {
            if (string.IsNullOrEmpty(Password)) return;
            var salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
                rngCsp.GetNonZeroBytes(salt);

            Salt = JsonSerializer.Serialize(salt);
            Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(Password.Trim(), salt, KeyDerivationPrf.HMACSHA256, 100000, 256 / 8));
        }

        public bool CheckPassword(string password)
        {
            if (string.IsNullOrEmpty(Password)) return false;
            var salt = JsonSerializer.Deserialize(Salt, typeof(byte[])) as byte[];
            return Password == Convert.ToBase64String(KeyDerivation.Pbkdf2(password.Trim(), salt, KeyDerivationPrf.HMACSHA256, 100000, 256 / 8));
        }
    }
}
