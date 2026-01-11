using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notely.Shared.DTOs
{
    public record AnalyzeNoteResponse(Guid NoteId, List<TagResponse> Tags);
}
