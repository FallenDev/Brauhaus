namespace Brauhaus.Models;

public class Journals
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime StartDate { get; set; }
    public string SGravity { get; set; }
    public DateTime RackOneDate { get; set; } 
    public DateTime RackTwoDate { get; set; }
    public string FGravity { get; set; }
    public string BGravity { get; set; }
    public DateTime BottleDate { get; set; }
    public string Content { get; set; }
    public string DisplayName => $"{Name} {StartDate:MM-dd-yyyy}";
}