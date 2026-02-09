namespace NET_MVC.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } // Admin, Keeper, Guest

        // Một Role có nhiều User
        public virtual ICollection<Users> Users { get; set; }

        // Một Role có nhiều quyền (Quan hệ N-N với Permission vẫn nên giữ)
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }
}
