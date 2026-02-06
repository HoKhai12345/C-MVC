namespace NET_MVC.Models
{
    public class Animal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        // Khóa ngoại trỏ đến Category
        public int? CategoryId { get; set; }
        public Category Category { get; set; }

        // Trỏ đến bảng trung gian
        public List<AnimalKeeper> AnimalKeepers { get; set; }
    }
}
