using System.Net.Http.Json;
using ProyectoPrograVI.Models;
using System.Net.Http;
using System.Text.Json;

namespace ProyectoPrograVI.Services
{
    public class PagoService
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://pagosapi-b8op.onrender.com/pago";

        public PagoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PagoResponse> ProcesarPago(PagoRequest pagoRequest)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(ApiUrl, pagoRequest);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    return new PagoResponse
                    {
                        Aprobado = false,
                        Mensaje = $"Error al conectar con la pasarela de pagos: {response.StatusCode} - {errorMsg}"
                    };
                }

                // Leer el contenido como JSON
                var json = await response.Content.ReadFromJsonAsync<JsonElement>();

                // Obtener el campo "mensaje" del JSON
                var mensaje = json.GetProperty("mensaje").GetString();

                return new PagoResponse
                {
                    Aprobado = true,
                    Mensaje = mensaje
                };
            }
            catch (Exception ex)
            {
                return new PagoResponse
                {
                    Aprobado = false,
                    Mensaje = $"Excepción: {ex.Message}"
                };
            }
        }
    }
}