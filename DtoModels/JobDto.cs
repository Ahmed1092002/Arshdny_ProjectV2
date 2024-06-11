namespace Arshdny_ProjectV2.DtoModels
{
    public class JobDto
    {
        public string JobName { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal Salary { get; set; }

        public string Country { get; set; } = null!;

        public string Location { get; set; } = null!;

        public DateTime PublishDate { get; set; }

        public int YearsOfExperience { get; set; }

        public bool IsView { get; set; }
    }
}
