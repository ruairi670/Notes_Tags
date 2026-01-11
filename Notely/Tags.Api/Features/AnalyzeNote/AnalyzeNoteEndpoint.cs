using Microsoft.AspNetCore.Mvc;
using Notely.Shared.DTOs;
using Tags.Api.Data;

namespace Tags.Api.Features.AnalyzeNote
{
    internal static class AnalyzeNoteEndpoint
    {
        public static async Task<IResult> AnalyzeNote (
            [FromBody] AnalyzeNoteRequest request,
            TagsDbContext dbContext,
            ILogger<Program> logger)
        {
            try
            {
                var tags = AnalyzeContentForTags(request.Title, request.Content);

                var tagEntities = tags.Select(t => new Tag
                {
                    Id = Guid.NewGuid(),
                    Name = t.Name,
                    Colour = t.Colour,
                    NoteId = request.Id,
                    CreatedAtUtc = DateTime.UtcNow
                }).ToList();

                dbContext.Tags.AddRange(tagEntities);
                await dbContext.SaveChangesAsync();

                var response = new AnalyzeNoteResponse(request.Id, tags);
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred when analyzing note {NoteId}", request.Id);
                return Results.Problem("Error occurredwhen analyzing note");
            }
        }

        private static List<TagResponse> AnalyzeContentForTags(object title, object content)
        {
            var tags = new List<TagResponse>();
            var allText = $"{title} {content}".ToLowerInvariant();

            var tagKeywords = new Dictionary<string, (string name, string colour)>
            {
                {"work", ("Work", "#3B82F6") },
                {"personal", ("Personal", "#3B82F6") },
                {"important", ("Important", "#3B82F6") },
            };

            foreach (var keyword in tagKeywords)
            {
                if (allText.Contains(keyword.Key))
                {
                    tags.Add(new TagResponse(Guid.NewGuid(),
                                             keyword.Value.name,
                                             keyword.Value.colour,
                                             DateTime.UtcNow));
                }
            }

            if (!tags.Any())
            {
                tags.Add(new TagResponse(Guid.NewGuid(),
                                         "General",
                                         "#6b7280",
                                         DateTime.UtcNow));
            }

            return tags;
        }
    }
}
