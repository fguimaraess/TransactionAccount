using Domain.Entities.DTO;
using Domain.Interfaces.RestService;
using Newtonsoft.Json;
using Polly;
using Polly.Registry;

namespace Infrastructure.RestService;

public class BankApi : IBankApi
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ResiliencePipelineProvider<string> _pipelineProvider;

    public BankApi(IHttpClientFactory httpClientFactory, ResiliencePipelineProvider<string> pipelineProvider)
    {
        _httpClientFactory = httpClientFactory;
        _pipelineProvider = pipelineProvider;
    }


    public BankDTO? GetBankByCode(int code)
    {
        var url = $"https://brasilapi.com.br/api/banks/v1/{code}";
        var httpClient = _httpClientFactory.CreateClient();

        var pipeline = _pipelineProvider.GetPipeline("default");
        var resultResponse = pipeline.ExecuteAsync(async ct => await httpClient.GetAsync(url, ct)).Result;
        if (resultResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        var result = resultResponse.Content.ReadAsStringAsync().Result;

        return JsonConvert.DeserializeObject<BankDTO>(result);
    }
}
