namespace Arshdny_ProjectV2.Models
{
    public class RefugeeRegistrationModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Phone1 { get; set; }
        public string? Phone2 { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool? IsBlocked { get; set; }
        public int RefugeeJobId { get; set; }
        public string FileNo { get; set; }
        public string RefugeeCardNo { get; set; }
        public int CountryId { get; set; }
        public int NationaltyId { get; set; }
        public string? Cv { get; set; }
        public string ImagePath { get; set; }
        public int? FamilyNo { get; set; }
        public DateTime CardStartDate { get; set; }
        public DateTime CardEndDate { get; set; }
    }
}
