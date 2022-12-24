﻿using Moq.Protected;
using Moq;
using System.Net;

namespace OpenAI.Net.Tests.OpenAIHttpClientTests
{
    internal class GetModelTests
    {
        const string responseJson = @"{
                                            ""id"": ""babbage"",
                                            ""object"": ""model"",
                                            ""created"": 1649358449,
                                            ""owned_by"": ""openai"",
                                            ""permission"": [
                                                {
                                                    ""id"": ""modelperm-49FUp5v084tBB49tC4z8LPH5"",
                                                    ""object"": ""model_permission"",
                                                    ""created"": 1669085501,
                                                    ""allow_create_engine"": false,
                                                    ""allow_sampling"": true,
                                                    ""allow_logprobs"": true,
                                                    ""allow_search_indices"": false,
                                                    ""allow_view"": true,
                                                    ""allow_fine_tuning"": false,
                                                    ""organization"": ""*"",
                                                    ""group"": null,
                                                    ""is_blocking"": false
                                                }
                                            ],
                                            ""root"": ""babbage"",
                                            ""parent"": null
                                        }
            ";

        const string errorResponseJson = @"{""error"":{""message"":""an error occured"",""type"":""invalid_request_error"",""param"":""prompt"",""code"":""unsupported""}}";
     
        
        [TestCase(true, HttpStatusCode.OK, responseJson,null, Description = "Successfull Request")]
        [TestCase(false, HttpStatusCode.BadRequest, errorResponseJson, "an error occured", Description = "Failed Request")]
        public async Task Test_GetModel(bool isSuccess,HttpStatusCode responseStatusCode,string responseJson,string errorMessage)
        {
            var res = new HttpResponseMessage { StatusCode = responseStatusCode, Content = new StringContent(responseJson) };
            var handlerMock = new Mock<HttpMessageHandler>();
            string path = null;
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(() => res)
               .Callback<HttpRequestMessage, CancellationToken>((r, c) =>
               {
                   path = r.RequestUri.AbsolutePath;
               });

            var httpClient = new HttpClient(handlerMock.Object) { BaseAddress = new Uri("https://api.openai.com") };

            var openAIHttpClient = new OpenAIHttpClient(httpClient);
            var response = await openAIHttpClient.GetModel("babbage");

            Assert.That(response.IsSuccess, Is.EqualTo(isSuccess));
            Assert.That(response.Result != null, Is.EqualTo(isSuccess));
            Assert.That(response.Result?.Id == "babbage", Is.EqualTo(isSuccess));
            Assert.That(response.StatusCode, Is.EqualTo(responseStatusCode));
            Assert.That(response.Exception == null, Is.EqualTo(isSuccess));
            Assert.That(response.ErrorMessage == null, Is.EqualTo(isSuccess));
            Assert.That(response.ErrorResponse == null, Is.EqualTo(isSuccess));
            Assert.That(response.ErrorResponse?.Error?.Message, Is.EqualTo(errorMessage));
            Assert.That(response.ErrorResponse?.Error?.Type == null, Is.EqualTo(isSuccess));
            Assert.That(response.ErrorResponse?.Error?.Code == null, Is.EqualTo(isSuccess));
            Assert.That(response.ErrorResponse?.Error?.Param == null, Is.EqualTo(isSuccess));
            Assert.That(path, Is.EqualTo("/v1/models/babbage"),"Apth is incorrect");
        }
    }
}
