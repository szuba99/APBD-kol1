using Kolokwium1.Models.DTOs;
using Microsoft.Data.SqlClient;

namespace Kolokwium1.Services;

public class DbService : IDbService
{
    private readonly IConfiguration _configuration;

    public DbService(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    public async Task<ProjectDetailsDto> GetProjectDetailsAsync(int projectId)
    {
        await using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using var command = new SqlCommand();
        command.Connection = connection;
        await connection.OpenAsync();

        ProjectDetailsDto? project = null;

        command.CommandText = @"
            SELECT 
                P.ProjectId, P.Objective, P.StartDate, P.EndDate,
                A.Name AS ArtifactName, A.OriginDate, 
                I.InstitutionId, I.Name AS InstitutionName, I.FoundedYear,
                S.FirstName, S.LastName, S.HireDate, SA.Role
            FROM Preservation_Project P
            JOIN Artifact A ON P.ArtifactId = A.ArtifactId
            JOIN Institution I ON A.InstitutionId = I.InstitutionId
            LEFT JOIN Staff_Assignment SA ON SA.ProjectId = P.ProjectId
            LEFT JOIN Staff S ON S.StaffId = SA.StaffId
            WHERE P.ProjectId = @ProjectId";

        command.Parameters.AddWithValue("@ProjectId", projectId);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            if (project == null)
            {
                project = new ProjectDetailsDto
                {
                    ProjectId = reader.GetInt32(reader.GetOrdinal("ProjectId")),
                    Objective = reader.GetString(reader.GetOrdinal("Objective")),
                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                    EndDate = reader.IsDBNull(reader.GetOrdinal("EndDate")) ? null : reader.GetDateTime(reader.GetOrdinal("EndDate")),
                    Artifact = new ArtifactDto
                    {
                        Name = reader.GetString(reader.GetOrdinal("ArtifactName")),
                        OriginDate = reader.GetDateTime(reader.GetOrdinal("OriginDate")),
                        Institution = new InstitutionDto
                        {
                            InstitutionId = reader.GetInt32(reader.GetOrdinal("InstitutionId")),
                            Name = reader.GetString(reader.GetOrdinal("InstitutionName")),
                            FoundedYear = reader.GetInt32(reader.GetOrdinal("FoundedYear"))
                        }
                    },
                    StaffAssignments = new List<StaffAssignmentDto>()
                };
            }

            if (!reader.IsDBNull(reader.GetOrdinal("FirstName")))
            {
                project.StaffAssignments.Add(new StaffAssignmentDto
                {
                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                    HireDate = reader.GetDateTime(reader.GetOrdinal("HireDate")),
                    Role = reader.GetString(reader.GetOrdinal("Role"))
                });
            }
        }

        if (project == null)
        {
            throw new Exception("Projekt nie istnieje");
        }

        return project;
    }

    public async Task AddArtifactWithProjectAsync(AddArtifactWithProjectDto dto)
    {
        await using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using var command = new SqlCommand();
        command.Connection = connection;
        await connection.OpenAsync();

        var transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;

        try
        {
            command.CommandText = @"
                INSERT INTO Artifact (ArtifactId, Name, OriginDate, InstitutionId)
                VALUES (@ArtifactId, @Name, @OriginDate, @InstitutionId)";
            command.Parameters.AddWithValue("@ArtifactId", dto.Artifact.ArtifactId);
            command.Parameters.AddWithValue("@Name", dto.Artifact.Name);
            command.Parameters.AddWithValue("@OriginDate", dto.Artifact.OriginDate);
            command.Parameters.AddWithValue("@InstitutionId", dto.Artifact.InstitutionId);
            await command.ExecuteNonQueryAsync();
            command.Parameters.Clear();
            
            command.CommandText = @"
                INSERT INTO Preservation_Project (ProjectId, ArtifactId, StartDate, EndDate, Objective)
                VALUES (@ProjectId, @ArtifactId, @StartDate, @EndDate, @Objective)";
            command.Parameters.AddWithValue("@ProjectId", dto.Project.ProjectId);
            command.Parameters.AddWithValue("@ArtifactId", dto.Artifact.ArtifactId);
            command.Parameters.AddWithValue("@StartDate", dto.Project.StartDate);
            command.Parameters.AddWithValue("@EndDate", (object?)dto.Project.EndDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@Objective", dto.Project.Objective);
            await command.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
}



