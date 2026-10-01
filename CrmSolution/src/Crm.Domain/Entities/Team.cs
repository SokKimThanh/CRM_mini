namespace Crm.Domain.Entities;
public class Team
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? ManagerId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<UserProfile> UserProfiles { get; set; } = new List<UserProfile>();
}