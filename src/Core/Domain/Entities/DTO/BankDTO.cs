using Newtonsoft.Json;

namespace Domain.Entities.DTO;
public class BankDTO
{
    [JsonProperty("ispb")]
    public long Ispb { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("code")]
    public long Code { get; set; }

    [JsonProperty("fullName")]
    public string FullName { get; set; }
}
