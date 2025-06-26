using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

class Animal
{
    [JsonPropertyName("hewan")]
    public string Hewan { get; set; }

    [JsonPropertyName("fakta")]
    public string Fakta { get; set; }
}

class Program
{
    static async Task<string> TranslateText(string text, string targetLang = "en")
    {
        using var client = new HttpClient();
        var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=id&tl={targetLang}&dt=t&q={Uri.EscapeDataString(text)}";
        var result = await client.GetStringAsync(url);

        var jsonDoc = JsonDocument.Parse(result);
        var translated = jsonDoc.RootElement[0][0][0].GetString();
        return translated;
    }

    static async Task Main()
    {
        var apiUrl = "https://animalsfact-api.p.rapidapi.com/animals"; 
        var client = new HttpClient();

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(apiUrl),
            Headers =
            {
                { "x-rapidapi-host", "animalsfact-api.p.rapidapi.com" },
                { "x-rapidapi-key", "fa3d8c2351mshc8877dd3eabeb5cp13e5fajsn14537b357450" },
            },
        };

        try
        {
            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();

            var animals = JsonSerializer.Deserialize<Animal[]>(body);

            Console.WriteLine("Translated Animal Facts:\n");

            foreach (var animal in animals)
            {
                var translatedAnimal = await TranslateText(animal.Hewan);
                var translatedFact = await TranslateText(animal.Fakta);
                Console.WriteLine($"Animal: {translatedAnimal}");
                Console.WriteLine($"Fact  : {translatedFact}\n");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
