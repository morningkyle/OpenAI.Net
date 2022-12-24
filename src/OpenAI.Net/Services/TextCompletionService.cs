﻿using OpenAI.Net.Models.OperationResult;
using OpenAI.Net.Models.Requests;
using OpenAI.Net.Models.Responses.Common;
using OpenAI.Net.Models.Responses;
using OpenAI.Net.Extensions;
using OpenAI.Net.Services.Interfaces;

namespace OpenAI.Net.Services
{
    public class TextCompletionService : BaseService, ITextCompletionService
    {
        public TextCompletionService(HttpClient client) : base(client)
        {
        }

        public Task<OpenAIHttpOperationResult<TextCompletionResponse, ErrorResponse>> Get(TextCompletionRequest request)
        {
            return HttpClient.OperationPostResult<TextCompletionResponse, ErrorResponse>("v1/completions", request, JsonSerializerOptions);
        }
        public IAsyncEnumerable<OpenAIHttpOperationResult<TextCompletionResponse, ErrorResponse>> GetStream(TextCompletionRequest request)
        {
            request.Stream = true;
            return HttpClient.OperationPostStreamResult<TextCompletionResponse, ErrorResponse>("v1/completions", request, JsonSerializerOptions);
        }
    }
}
