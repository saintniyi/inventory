using FakeItEasy;



namespace InventoryWebTest.Helper
{
    public static class HttpClientTestHelper
    {
        public static HttpClient CreateFakeHttpClient(params HttpResponseMessage[] responses)
        {
            var handler = A.Fake<HttpMessageHandler>(opt => opt.CallsBaseMethods());
            var callIndex = 0;

            A.CallTo(handler)
                .Where(call => call.Method.Name == "SendAsync")
                .WithReturnType<Task<HttpResponseMessage>>()
                .ReturnsLazily(() => Task.FromResult(responses[callIndex++]));

            return new HttpClient(handler)
            {
                BaseAddress = new Uri("http://localhost/")
            };
        }
    }
}
