﻿
namespace Kolokwium1.Models.DTOs;

public class AddArtifactWithProjectDto
{
    public ArtifactInsertDto Artifact { get; set; }
    public ProjectInsertDto Project { get; set; }
}

public class ArtifactInsertDto
{
    public int ArtifactId { get; set; }
    public string Name { get; set; }
    public DateTime OriginDate { get; set; }
    public int InstitutionId { get; set; }
}

public class ProjectInsertDto
{
    public int ProjectId { get; set; }
    public string Objective { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

}



