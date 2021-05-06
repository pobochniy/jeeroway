using JeerowayWiki.Client.models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace JeerowayWiki.Client.Pages
{
    public partial class FetchData : ComponentBase
    {
        private WeatherForecast[] forecasts;

        protected override async Task OnInitializedAsync()
        {
            forecasts = await Http.GetFromJsonAsync<WeatherForecast[]>("sample-data/weather.json");
        }
    }
}
