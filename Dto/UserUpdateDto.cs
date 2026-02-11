namespace NET_MVC.Dto
{
    public class UserUpdateDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string? Password { get; set; }
        public int RoleId { get; set; }
    }
}
