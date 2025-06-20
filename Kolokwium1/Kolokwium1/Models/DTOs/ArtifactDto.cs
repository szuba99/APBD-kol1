namespace Kolokwium1.Models.DTOs;

public class ArtifactDto
{
    public string Name { get; set; }
    public DateTime OriginDate { get; set; }
    public InstitutionDto Institution { get; set; }
}

