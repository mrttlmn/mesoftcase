using System.Text.Json.Serialization;

namespace MeSoftCase.UI.ResponseModels;

public class GetUsersDataResponseModel
{
    [JsonPropertyName("role")]
    public string Role { get; set; }
    [JsonPropertyName("count")]
    public int Count { get; set; }
}



