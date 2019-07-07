using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DeOlho.ETL.Sources;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Xunit;

namespace DeOlho.ETL.UnitTests
{
    public class SourcesTest
    {


        [Fact]
        public async void HttpJsonSource_Execute_StatusCode_OK()
        {
            var stringContent  = "[{'id':1, 'value':'Sucesso'}]";
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[{'id':1, 'value':'Sucesso'}]"),
                })
                .Verifiable();

            var httpClient = new HttpClient(httpMessageHandlerMock.Object);

            var uri = "http://teste.com/api";
            var httpJsonSource = new HttpJsonSource(httpClient, uri);
            var result = await httpJsonSource.Execute();
            
            result.Should().Be(stringContent);

            httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1), // we expected a single external request
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get  // we expected a GET request
                    && req.RequestUri == new Uri($"{uri}") // to this uri
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async void HttpStreamSource_Execute_StatusCode_OK()
        {
            var stringContent = "[{'id':1, 'value':'Sucesso'}]";
            using(var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(stringContent)))
            {
                ms.Position = 0;
                var httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
                httpMessageHandlerMock
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>()
                    )
                    .ReturnsAsync(new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StreamContent(ms)
                    })
                    .Verifiable();

                var httpClient = new HttpClient(httpMessageHandlerMock.Object);

                var uri = "http://teste.com/api";
                var httpJsonSource = new DeOlho.ETL.Sources.HttpStreamSource(httpClient, uri);
                var result = await httpJsonSource.Execute();
                
                var reader = new StreamReader(result);
                var text = reader.ReadToEnd();
                text.Should().Be(stringContent);

                httpMessageHandlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Exactly(1),
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get  // we expected a GET request
                        && req.RequestUri == new Uri($"{uri}")
                    ),
                    ItExpr.IsAny<CancellationToken>()
                );
            }
        }
    }
}