namespace Talabat.Core.Entities.Identity
{
    public class Address :BaseEntity
    {
        public string FName { get; set; } = null!;
        public string LName { get; set; }=null!;
        public string Street { get; set; }=null!;
        public string City { get; set; }=null!;
        public string Country { get; set; }=null!;

        public string ApplicationUserId { get; set; } //FK 
        public ApplicationUser ApplicationUser { get; set; } = null!;//Navigational property [one]




    }
}