namespace NET_MVC.Models
{
    public class Keeper
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }

        // Navigation property: Một người chăm sóc nhiều bản ghi trong bảng trung gian
        public List<AnimalKeeper> AnimalKeepers { get; set; }
    }
}
