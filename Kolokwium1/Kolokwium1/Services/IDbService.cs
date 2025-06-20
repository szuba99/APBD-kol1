using Kolokwium1.Models.DTOs;

namespace Kolokwium1.Services;

public interface IDbService
{
    Task<ProjectDetailsDto> GetProjectDetailsAsync(int projectId);
    Task AddArtifactWithProjectAsync(AddArtifactWithProjectDto dto);
}


