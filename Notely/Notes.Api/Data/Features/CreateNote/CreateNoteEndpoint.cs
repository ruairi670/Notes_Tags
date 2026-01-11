using Microsoft.AspNetCore.Mvc;
using Notely.Shared.DTOs;
using System.Reflection;

namespace Notes.Api.Data.Features.CreateNote
{
    internal static class CreateNoteEndpoint
    {
        public record Request(string Title, string Content);
        public record Response(Guid Id, string Title, string Content, DateTime CreatedAt, List<TagResponse> Tags);

        public static async Task<IResult> CreateNote(
            [FromBody] Request request,
            NotesDbContext dbContext,
            IHttpClientFactory httpClientFactory,
            ILogger<Program> logger)
        {
            try
            {
                var note = new Note
                {
                    Id = Guid.NewGuid(),
                    Title = request.Title,
                    Content = request.Content,
                    CreatedAtUtc = DateTime.UtcNow,
                };

                dbContext.Notes.Add(note);
                await dbContext.SaveChangesAsync();

                var analyzeNoteRequest = new AnalyzeNoteRequest(note.Id, note.Title, note.Content);

                var tags = await AnalyzeNoteForTags(analyzeNoteRequest, httpClientFactory, logger);

                var response = new Response(note.Id, note.Title, note.Content, note.CreatedAtUtc, []);

                return Results.Created($"notes/{note.Id}", response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating note");
                return Results.Problem("An error occurred while creating the note.");
            }
        }

        private static async Task<List<TagResponse>> AnalyzeNoteForTags(
            AnalyzeNoteRequest analyzeNoteRequest,
            IHttpClientFactory httpClientFactory,
            ILogger logger)
        {
            try
            {
                var client = httpClientFactory.CreateClient("TagsApi");
                var response = await client.PostAsJsonAsync("tags/analyze", analyzeNoteRequest);

                if (response.IsSuccessStatusCode)
                {
                    var tags = await response.Content.ReadFromJsonAsync<AnalyzeNoteResponse>();
                    return tags?.Tags ?? [];
                }

                logger.LogWarning("Failed to analyze note for tags. Status code: {StatusCode}", response.StatusCode);
                return [];
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error analyzing note for tags");
                return [];
            }
        }
    }
}
