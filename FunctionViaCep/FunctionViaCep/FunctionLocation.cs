using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Runtime.ConstrainedExecution;
using System.Net.Http.Json;

namespace FunctionViaCep
{
    public static class FunctionLocation
    {
        [FunctionName("get-cep")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetCep")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string cep = req.Query["cep"];
            ViaCep viaCep = null;

            string apiUrl = $"https://viacep.com.br/ws/{cep}/json/";
            HttpResponseMessage response;

            using (HttpClient client = new HttpClient())
            {
                response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    //string responseBody = await response.Content.ReadAsStringAsync();
                    //viaCepTask = await response.Content.ReadFromJsonAsync<Task<ViaCep?>>();
                    viaCep = await response.Content.ReadFromJsonAsync<ViaCep>();

                    if (viaCep?.cep is null)
                    {
                        //throw new Exception($"Ex: Sorry! Apparently the CEP {cep} is invalid. ");
                        return new BadRequestObjectResult(response);
                    }
                }
                else
                {
                    return new BadRequestObjectResult(response);
                    //throw new Exception($"Ex: Sorry! Unexpected error trying to get this CEP: {cep}.");
                }
            }

            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            //name = name ?? data?.name;

            //string responseMessage = string.IsNullOrEmpty(name)
            //    ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //    : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(viaCep);
        }
    }

    public class ViaCep
    {
        public string cep { get; set; }
        public string logradouro { get; set; }
        public string complemento { get; set; }
        public string bairro { get; set; }
        public string localidade { get; set; }
        public string uf { get; set; }
        public string ibge { get; set; }
        public string gia { get; set; }
        public string ddd { get; set; }
        public string siafi { get; set; }

        public ViaCep()
        {

        }
    }
}
