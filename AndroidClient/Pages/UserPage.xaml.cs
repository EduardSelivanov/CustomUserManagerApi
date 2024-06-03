

using AndroidClient.Models;
using AndroidClient.Pages;
using AndroidClient.Services;

using System.Net;

using System.Net.Http.Json;

namespace AndroidClient.Resources;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class UserPage : ContentPage
{

    private readonly HttpClient _httpClient;
    private readonly  ISaverService _saverService;
    private readonly string _baseUrl;

    public UserPage(ISaverService saverService,HttpClient httpClient,string baseUrl)
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
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        LoadData(baseUrl);
        
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var mainPage = Navigation.NavigationStack.FirstOrDefault(page => page.GetType() == typeof(MainPage));
        if (mainPage != null)
        {
            Navigation.RemovePage(mainPage);
        }
    }

    private async void LoadData(string baseUrl)
    {
        var apiUrl = $"{baseUrl}/api/Chat/ShowMyChats";

        try
        {
            var response =  await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {

                List<ShowChatDTO> chats = (await response.Content.ReadFromJsonAsync<IEnumerable<ShowChatDTO>>()).ToList();

                ChatsList.ItemsSource = chats;
                //MessageLabel.Text = content;
            }
            else
            {
                MessageLabel.Text = "You Have noChats";
            }
        }
        catch (Exception ex)
        {
            MessageLabel.Text = $"err: {ex.Message}";
        }
    }

    private async void OnChatItemTapped(object sender, ItemTappedEventArgs e)
    {
        if(e.Item is ShowChatDTO selectedChat)
        {
            await Navigation.PushAsync(new CertainChatPage(selectedChat.ChatId, _saverService,_baseUrl));            
        }
    }

    private async void LoadUsers(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DisplayUsersAndCreateChat(_saverService,_baseUrl));
    }
}