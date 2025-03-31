using Chowbro.Core.Entities.Location;
using Chowbro.Core.Models.SeedData;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Chowbro.Infrastructure.Persistence.Seeders
{
    public static class NigeriaLocationSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (!await context.States.AnyAsync())
            {
                // Get the directory where the application is running
                var basePath = AppContext.BaseDirectory;

                // Navigate up from bin/Debug/netX.0 to the project root
                var rootPath = Path.GetFullPath(Path.Combine(basePath, "../../../../"));

                var filePath = Path.Combine(rootPath, "Chowbro.Infrastructure", "Data", "SeedData", "nigeria-states-lgas.json");

                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"Seed data file not found at: {filePath}");


                var jsonData = await File.ReadAllTextAsync(filePath);
                var statesDict = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(jsonData);

                if (statesDict == null || !statesDict.Any())
                    return;


                var states = statesDict.Select(s => new State
                {
                    Name = s.Key,
                    Lgas = s.Value.Select(l => new Lga { Name = l }).ToList()
                }).ToList();

                await context.States.AddRangeAsync(states);
                await context.SaveChangesAsync();
            }
        }
    }
}