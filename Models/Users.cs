namespace NET_MVC.Models
{
    public class Users
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int rOLEiD { get; set; }
        public virtual Role Role { get; set; }
    }
}
