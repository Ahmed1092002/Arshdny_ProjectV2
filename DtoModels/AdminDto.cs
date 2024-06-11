namespace Arshdny_ProjectV2.DtoModels
{
    public class AdminDto
    {
    public int UserId { get; set; }
    public string Qualification { get; set; } = null!;
    public string Roles { get; set; } = null!;
    public int Permission { get; set; }
    }

}
