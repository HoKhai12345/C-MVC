namespace NET_MVC.Dto
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public List<PermissionViewModel> Permission { get; set; }
    }
}
