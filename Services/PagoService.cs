using System.Net.Http.Json;
using ProyectoPrograVI.Models;
using System.Net.Http;
using System.Text.Json;

namespace ProyectoPrograVI.Services
{
    // Servicio encargado de procesar pagos a través de una API externa
    public class PagoService
    {
        private readonly HttpClient _httpClient; // Cliente HTTP para hacer las peticiones
        private const string ApiUrl = "https://pagosapi-b8op.onrender.com/pago";
        // URL base de la API de pagos

        // Constructor: recibe el HttpClient inyectado (Dependency Injection)
        public PagoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Método asíncrono que procesa un pago enviando los datos a la API
        public async Task<PagoResponse> ProcesarPago(PagoRequest pagoRequest)
        {
            try
            {
                // Se envía el objeto "pagoRequest" como JSON al endpoint de la API
                var response = await _httpClient.PostAsJsonAsync(ApiUrl, pagoRequest);

                // Si la respuesta de la API no es exitosa, se devuelve error
                if (!response.IsSuccessStatusCode)
                {
                    return new PagoResponse
                    {
                        Aprobado = false,
                        Mensaje = $"Error en el pago: {response.StatusCode}"
                    };
                }

                // Se lee el contenido JSON de la respuesta
                var json = await response.Content.ReadFromJsonAsync<JsonElement>();

                // Se construye un objeto de respuesta con los datos recibidos
                return new PagoResponse
                {
                    Aprobado = true, // Si la API respondió correctamente, se marca como aprobado
                    Mensaje = json.GetProperty("mensaje").GetString(), // Mensaje de confirmación desde la API
                    IdTransaccion = json.TryGetProperty("idTransaccion", out var idProp)
                        ? idProp.GetString()  // Si la API envía el ID de transacción, se usa
                        : Guid.NewGuid().ToString(), // Si no, se genera uno nuevo
                    MetodoPago = "Tarjeta de crédito" // Forma de pago usada
                };
            }
            catch (Exception ex)
            {
                // Si ocurre un error en la conexión o en el proceso, se captura aquí
                return new PagoResponse
                {
                    Aprobado = false,
                    Mensaje = $"Error al procesar el pago: {ex.Message}"
                };
            }
        }
    }
}
