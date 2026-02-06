namespace NET_MVC.Models
{
    public class AnimalKeeper
    {
        public int AnimalId { get; set; }
        public Animal Animal { get; set; }

        public int KeeperId { get; set; }
        public Keeper Keeper { get; set; }

        public DateTime AssignedDate { get; set; }
        public string Shift { get; set; }


    }
}
