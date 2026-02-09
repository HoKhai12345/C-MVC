namespace NET_MVC.Dto
{
    public class AnimalViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string CategoryName { get; set; }
        public string[] ListKeeper { get; set; } // Để chuỗi cho dễ hiển thị ở View
    }
}
