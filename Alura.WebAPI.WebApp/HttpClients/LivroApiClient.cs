using Alura.ListaLeitura.Modelos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.HttpClients
{
    public class LivroApiClient : BaseClient
    {
        private readonly HttpClient _httpClient;

        public LivroApiClient(HttpClient httpClient, IHttpContextAccessor accessor) : base(httpClient, accessor)
        {
            _httpClient = httpClient;
        }

        public async Task<LivroApi> GetLivroAsync(int id)
        {
            var resposta = await _httpClient.GetAsync($"{id}");
            resposta.EnsureSuccessStatusCode();

            return await resposta.Content.ReadAsAsync<LivroApi>();
        }

        public async Task<byte[]> GetCapaLivroAsync(int id)
        {
            var resposta = await _httpClient.GetAsync($"{id}/capa");
            resposta.EnsureSuccessStatusCode();

            return await resposta.Content.ReadAsByteArrayAsync();
        }

        public async Task DeleteLivroAsync(int id)
        {
            var resposta = await _httpClient.DeleteAsync($"{id}");
            resposta.EnsureSuccessStatusCode();
        }

        public async Task PostLivroAsync(LivroUpload model)
        {
            var content = CreateMultipartFormDataContent(model);
            var resposta = await _httpClient.PostAsync("", content);
            resposta.EnsureSuccessStatusCode();
        }

        public async Task PutLivroAsync(LivroUpload model)
        {
            var content = CreateMultipartFormDataContent(model);
            var resposta = await _httpClient.PutAsync("", content);
            resposta.EnsureSuccessStatusCode();
        }

        private HttpContent CreateMultipartFormDataContent([FromForm] LivroUpload model)
        {
            var content = new MultipartFormDataContent
            {
                { new StringContent(model.Titulo ?? ""), EnvolveComAspasDuplas("titulo") },
                { new StringContent(model.Subtitulo ?? ""), EnvolveComAspasDuplas("subtitulo") },
                { new StringContent(model.Resumo ?? ""), EnvolveComAspasDuplas("resumo") },
                { new StringContent(model.Autor ?? ""), EnvolveComAspasDuplas("autor") },
                { new StringContent(model.Lista.ParaString()), EnvolveComAspasDuplas("lista") }
            };

            if (model.Id > 0)
            {
                content.Add(new StringContent(model.Id.ToString()), EnvolveComAspasDuplas("id"));
            }

            if (model.Capa != null)
            {
                var imagemContent = new ByteArrayContent(model.Capa.ConvertToBytes());
                imagemContent.Headers.Add("content-type", "image/png");
                content.Add(
                    imagemContent,
                    EnvolveComAspasDuplas("capa"),
                    EnvolveComAspasDuplas("capa.png")
                );
            }

            return content;
        }
    }
}
