using ApiCollection.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static Newtonsoft.Json.JsonConvert;

namespace ApiCollection.Client
{
    /// <summary>
    /// ServiceCollection 的擴充方法
    /// </summary>
    public static partial class ServiceCollectionExtension
    {
        /// <summary>
        /// 加入 OMDB 平台的 HTTP Client <see cref="http://www.omdbapi.com/"/>
        /// </summary>
        /// <typeparam name="TImplementation">OMDB 平台的 HTTP Client 實作類別</typeparam>
        /// <param name="services"></param>
        /// <param name="omdbApiKey">OMDB 的 API 金鑰</param>
        /// <returns></returns>
        public static IServiceCollection AddOmdbClient<TImplementation>(this IServiceCollection services, string omdbApiKey)
        where TImplementation : class
        {
            return services
                .AddHttpClient<TImplementation>(client => client.BaseAddress = new Uri($"http://www.omdbapi.com/?apikey={omdbApiKey}", UriKind.Absolute))
                .ConfigurePrimaryHttpMessageHandler(h => new DefaultHttpClientHandler())
                .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(3, retryAttempt))))
                .Services;
        }
    }

    /// <summary>
    /// OMDB 平台的 HTTP Client 實作類別
    /// </summary>
    public class OmdbClient
    {
        private readonly HttpClient _httpClient;

        public OmdbClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("cache-control", "no-cache");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.ConnectionClose = false;
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// 使用 IMDb ID 或電影名稱的查詢
        /// </summary>
        /// <param name="request">查詢參數</param>
        /// <returns></returns>
        public async Task<ResponseByIdOrTitle> GetByIdOrTitleAsync(RequestByIdOrTitle request)
        {
            try
            {
                var query = $"{_httpClient.BaseAddress.Query}&i={request.IMDbId}&t={request.Title}&type={request.Type}&y={request.Year}&plot={request.Plot}&r={request.ReturnType}&callback={request.Callback}&v={request.Version}";
                var response = await _httpClient.GetAsync(query);

                response.EnsureSuccessStatusCode();

                return DeserializeObject<ResponseByIdOrTitle>(await response.Content.ReadAsStringAsync());
            }
            catch (Exception e)
            {
                Console.WriteLine($"OMDB HTTP Client: {nameof(GetByIdOrTitleAsync)} Error! {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// 使用電影名稱搜尋的查詢
        /// </summary>
        /// <param name="request">查詢參數</param>
        /// <returns></returns>
        public async Task<ResponseBySearch> GetBySearchAsync(RequestBySearch request)
        {
            try
            {
                var query = $"{_httpClient.BaseAddress.Query}&s={request.Search}&type={request.Type}&y={request.Year}&r={request.ReturnType}&page={request.Page}&callback={request.Callback}&v={request.Version}";
                var response = await _httpClient.GetAsync(query);

                response.EnsureSuccessStatusCode();

                return DeserializeObject<ResponseBySearch>(await response.Content.ReadAsStringAsync());
            }
            catch (Exception e)
            {
                Console.WriteLine($"OMDB HTTP Client: {nameof(GetBySearchAsync)} Error! {e.Message}");
                throw;
            }
        }
    }

    /// <summary>
    /// 使用 IMDb ID 或電影名稱的查詢參數
    /// </summary>
    public class RequestByIdOrTitle
    {
        /// <summary>
        /// A valid IMDb ID (e.g. tt1285016)
        /// </summary>
        public string IMDbId { get; set; }
        /// <summary>
        /// Movie title to search for.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Type of result to return. (e.g. movie, series, episode)
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Year of release.
        /// </summary>
        public string Year { get; set; }
        /// <summary>
        /// Return short or full plot. (e.g. short, full)
        /// </summary>
        public string Plot { get; set; }
        /// <summary>
        /// The data type to return. (e.g. JSON, XML)
        /// </summary>
        public string ReturnType { get; set; }
        /// <summary>
        /// JSONP callback name.
        /// </summary>
        public string Callback { get; set; }
        /// <summary>
        /// API version (reserved for future use).
        /// </summary>
        public string Version { get; set; }
    }

    /// <summary>
    /// 使用 IMDb ID 或電影名稱的查詢結果
    /// </summary>
    public class ResponseByIdOrTitle
    {
        [JsonProperty("Title")]
        public string Title { get; set; }
        [JsonProperty("Year")]
        public string Year { get; set; }
        [JsonProperty("Rated")]
        public string Rated { get; set; }
        [JsonProperty("Released")]
        public string Released { get; set; }
        [JsonProperty("Runtime")]
        public string Runtime { get; set; }
        [JsonProperty("Genre")]
        public string Genre { get; set; }
        [JsonProperty("Director")]
        public string Director { get; set; }
        [JsonProperty("Writer")]
        public string Writer { get; set; }
        [JsonProperty("Actors")]
        public string Actors { get; set; }
        [JsonProperty("Plot")]
        public string Plot { get; set; }
        [JsonProperty("Language")]
        public string Language { get; set; }
        [JsonProperty("Country")]
        public string Country { get; set; }
        [JsonProperty("Awards")]
        public string Awards { get; set; }
        [JsonProperty("Poster")]
        public string Poster { get; set; }
        [JsonProperty("Ratings")]
        public List<Rating> Ratings { get; set; }
        [JsonProperty("Metascore")]
        public string Metascore { get; set; }
        [JsonProperty("imdbRating")]
        public string IMDbRating { get; set; }
        [JsonProperty("imdbVotes")]
        public string IMDbVotes { get; set; }
        [JsonProperty("imdbID")]
        public string IMDbID { get; set; }
        [JsonProperty("Type")]
        public string Type { get; set; }
        [JsonProperty("DVD")]
        public string DVD { get; set; }
        [JsonProperty("BoxOffice")]
        public string BoxOffice { get; set; }
        [JsonProperty("Production")]
        public string Production { get; set; }
        [JsonProperty("Website")]
        public string Website { get; set; }
        [JsonProperty("Response")]
        public string Response { get; set; }
    }

    /// <summary>
    /// 評分
    /// </summary>
    public class Rating
    {
        /// <summary>
        /// 評分來源
        /// </summary>
        [JsonProperty("Source")]
        public string Source { get; set; }
        /// <summary>
        /// 評分
        /// </summary>
        [JsonProperty("Value")]
        public string Value { get; set; }
    }

    /// <summary>
    /// 使用電影名稱搜尋的查詢參數
    /// </summary>
    public class RequestBySearch
    {
        /// <summary>
        /// Movie title to search for.
        /// </summary>
        public string Search { get; set; }
        /// <summary>
        /// Type of result to return. (eg. movie, series, episode)
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Year of release.
        /// </summary>
        public string Year { get; set; }
        /// <summary>
        /// The data type to return. eg. JSON, XML
        /// </summary>
        public string ReturnType { get; set; }
        /// <summary>
        /// Page number to return.
        /// </summary>
        public string Page { get; set; }
        /// <summary>
        /// JSONP callback name.
        /// </summary>
        public string Callback { get; set; }
        /// <summary>
        /// API version (reserved for future use).
        /// </summary>
        public string Version { get; set; }
    }

    /// <summary>
    /// 使用電影名稱搜尋的查詢結果
    /// </summary>
    public class ResponseBySearch
    {
        [JsonProperty("Search")]
        public IEnumerable<SearchResultItem> Search { get; set; }
        [JsonProperty("totalResults")]
        public string TotalResults { get; set; }
        [JsonProperty("Response")]
        public string Response { get; set; }
    }

    /// <summary>
    /// 結果項
    /// </summary>
    public class SearchResultItem
    {
        [JsonProperty("Title")]
        public string Title { get; set; }
        [JsonProperty("Year")]
        public string Year { get; set; }
        [JsonProperty("imdbID")]
        public string IMDbId { get; set; }
        [JsonProperty("Type")]
        public string Type { get; set; }
        [JsonProperty("Poster")]
        public string Poster { get; set; }
    }
}
