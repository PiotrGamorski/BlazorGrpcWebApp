namespace BlazorGrpcWebApp.Shared.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public int Bananas { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public List<UserUnit> Units { get; set; }
        public int Battles { get; set; }
        public int Victories { get; set; }
        public int Defeats { get; set; }
        public List<UserRole> Roles { get; set; }
        public string VerificationCode { get; set; }
        public DateTime VerificationCodeExpireDate { get; set; } = DateTime.Now.AddMinutes(10);
        public bool IsVerified { get; set; } = false;
    }
}
