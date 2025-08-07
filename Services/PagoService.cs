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
                    return new PagoResponse
                    {
                        Aprobado = false,
                        Mensaje = $"Error en el pago: {response.StatusCode}"
                    };
                }

                var json = await response.Content.ReadFromJsonAsync<JsonElement>();

                return new PagoResponse
                {
                    Aprobado = true,
                    Mensaje = json.GetProperty("mensaje").GetString(),
                    IdTransaccion = json.TryGetProperty("idTransaccion", out var idProp) ? idProp.GetString() : Guid.NewGuid().ToString(),
                    MetodoPago = "Tarjeta de crédito"
                };
            }
            catch (Exception ex)
            {
                return new PagoResponse
                {
                    Aprobado = false,
                    Mensaje = $"Error al procesar el pago: {ex.Message}"
                };
            }
        }
    }
}