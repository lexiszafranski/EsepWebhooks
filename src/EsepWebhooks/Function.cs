using System.Text;
using Amazon.Lambda.Core;
using System.Net.Http; // Ensure this using directive is present for HttpClient
using System.IO; // For StreamReader

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EsepWebhooks;

public class Function
{
    // HttpClient should ideally be static and reused between function invocations to enhance performance
    private static readonly HttpClient client = new HttpClient();

    /// <summary>
    /// A simple function that takes a JSON string, extracts an issue URL, and sends it to a Slack webhook
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns>The response from the Slack API</returns>
    public string FunctionHandler(object input, ILambdaContext context)
    {
        dynamic json = JsonConvert.DeserializeObject<dynamic>(input.ToString());
        string payload = $"{{'text':'Issue Created: {json.issue.html_url}'}}";

        var webRequest = new HttpRequestMessage(HttpMethod.Post, Environment.GetEnvironmentVariable("SLACK_URL"))
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };

        var response = client.Send(webRequest);
        using var reader = new StreamReader(response.Content.ReadAsStream());
            
        return reader.ReadToEnd();
    }
}
