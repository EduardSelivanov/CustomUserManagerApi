using AndroidClient.Models;
using AndroidClient.Services;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace AndroidClient.Pages;

public partial class DisplayUsersAndCreateChat : ContentPage
{
	private readonly ISaverService _saverService;
    private readonly string _baseUrl;
    private readonly HttpClient _httpClient;
	public DisplayUsersAndCreateChat(ISaverService saverService,string baseUrl)
	{
		InitializeComponent();

		_saverService = saverService;
        CookieCollection cookies = _saverService.GetCookie();
        string token = _saverService.GetMyToken();
        HttpClientHandler _httpClientHandler = new HttpClientHandler();
        _httpClientHandler.UseCookies = true;
        _httpClientHandler.CookieContainer = new System.Net.CookieContainer();


        _baseUrl = baseUrl;

        if (cookies != null)
        {
            foreach (Cookie cookie in cookies)
            {
                _httpClientHandler.CookieContainer.Add(new Uri(baseUrl), cookie);
            }
        }
        _httpClient = new HttpClient(_httpClientHandler);
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        LoadUsers(baseUrl);
    }
    private async void LoadUsers(string baseUrl)
    {
        var apiUrl = $"{baseUrl}/api/UserManager/GetAllUsers";

        try
        {
            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                List<ShowUserDTO> chats = (await response.Content.ReadFromJsonAsync<IEnumerable<ShowUserDTO>>()).ToList();

                UsersList.ItemsSource = chats;   
            }
            else
            {
                MessageLabel.Text = "No Users";
            }
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"err: {ex.Message}";
        }
    }

    private async void OnUserTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is ShowUserDTO selectedUser)
        {
            var apiUrl = $"{_baseUrl}/api/Chat/createChat";

            try
            {
                var content = new StringContent(JsonSerializer.Serialize(selectedUser.UserId),Encoding.UTF8,"application/json");
                var response = await _httpClient.PostAsync(apiUrl,content);

                if (response.IsSuccessStatusCode)
                {
                     Guid chat = await response.Content.ReadFromJsonAsync<Guid>();

                    await Navigation.PushAsync(new CertainChatPage(chat,_saverService,_baseUrl));
                    await Navigation.PopAsync();
                }
                else
                {
                    MessageLabel.Text = "hmm error ONUSERTAPPED";
                }
            }
            catch (Exception ex)
            {
                MessageLabel.Text = $"err: {ex.Message}";
            }

            
        }
    }
}