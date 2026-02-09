namespace NET_MVC.Models
{
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; } // ViewAnimals, EditAnimals, DeleteAnimals, ManageUsers, etc.
        // Một Permission có nhiều Role (Quan hệ N-N với Role)
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }
}
