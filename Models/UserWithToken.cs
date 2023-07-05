namespace Novels.Models
{
    public class UserWithToken
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public int Id { get; set; }
        public string FullName { get; set; } = null!;

        public int? Age { get; set; }
        public int? UserType { get; set; }

        public bool IsAdmin { get; set; }

        public virtual Type? UserTypeNavigation { get; set; }


        public UserWithToken(User user)
        {
            this.Id = user.Id;
            this.FullName = user.FullName;
            this.Age = user.Age;
            this.UserType = user.UserType;
            this.IsAdmin = user.isAdmin;
            this.UserTypeNavigation = user.UserTypeNavigation;
            
            
            
        }
    }
}
