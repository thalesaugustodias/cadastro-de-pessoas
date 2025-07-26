using CadastroDePessoas.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace CadastroDePessoas.Application.Services
{
    public class ServicoCache : IServiceCache
    {
        private readonly IDistributedCache _cache;
        private readonly JsonSerializerOptions _jsonOptions;

        public ServicoCache(IDistributedCache cache)
        {
            _cache = cache;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<T> ObterAsync<T>(string chave)
        {
            var valorCache = await _cache.GetStringAsync(chave);
            if (string.IsNullOrEmpty(valorCache))
                return default;

            return JsonSerializer.Deserialize<T>(valorCache, _jsonOptions);
        }

        public async Task DefinirAsync<T>(string chave, T valor, TimeSpan? tempoExpiracao = null)
        {
            var valorJson = JsonSerializer.Serialize(valor, _jsonOptions);
            var opcoes = new DistributedCacheEntryOptions();

            if (tempoExpiracao.HasValue)
                opcoes.AbsoluteExpirationRelativeToNow = tempoExpiracao.Value;
            else
                opcoes.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

            await _cache.SetStringAsync(chave, valorJson, opcoes);
        }

        public async Task RemoverAsync(string chave)
        {
            await _cache.RemoveAsync(chave);
        }
    }
}
