using System.Net.Http;
using System.Text.Json;
using System.Text;
using AndroidClient.Resources;
using System.Net;
using AndroidClient.Services;
using Microsoft.Extensions.Configuration;

namespace AndroidClient
{
    public partial class MainPage : ContentPage
    {
       
        private readonly HttpClient _httpClient;
        private readonly HttpClientHandler _httpClientHandler;

        
        private readonly IConfiguration _config;
        private  ISaverService _saverService;


        public MainPage(IConfiguration config)
        {
            InitializeComponent();
            
            ISaverService savSer=new SaverService();
            SetDeps(savSer);


            _httpClientHandler = new HttpClientHandler();
            _httpClientHandler.CookieContainer = new CookieContainer();
            _httpClient = new HttpClient(_httpClientHandler);

            _config = config;
        }

        public void SetDeps(ISaverService sav)
        {
            _saverService= sav;
        }


        private async  void OnLogin(object sender, EventArgs e)
        {
            var loginData = new { UserEmail = UsernameEntry.Text, Password = PasswordEntry.Text };
            var json = JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            string apiUrl = _config["ApiUrl"];
        
            var response = await _httpClient.PostAsync($"{apiUrl}/api/UserRegisterAndLogin/Login", content); //check Api 

            if (response.IsSuccessStatusCode)
            {
                string token = await response.Content.ReadAsStringAsync();
                _saverService.SetMyToken(token);
                
                CookieCollection cookies = _httpClientHandler.CookieContainer.GetCookies(new Uri(apiUrl));
                
                _saverService.SetCookies(cookies);
                _saverService.SetName(UsernameEntry.Text);

                await Navigation.PopToRootAsync();
                await Navigation.PushAsync(new UserPage(_saverService,_httpClient,apiUrl));
                await Navigation.PopAsync();
            }
            else
            {
                MessageLabel.Text = "Login failed. Please check your credentials.";
            }
        }
    }

}
