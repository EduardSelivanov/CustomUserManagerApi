using AndroidClient.Models;
using AndroidClient.Services;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using AndroidClient.Pages;

namespace AndroidClient;

public partial class CertainChatPage : ContentPage
{
    private readonly HttpClient _httpClient;
    private readonly ISaverService _saverService;
    public ObservableCollection<MessagesFront> _messagesOBC = new ObservableCollection<MessagesFront>();
    private readonly HubConnection _hubConnection;

    public string UserName;
    private int currentPage = 1;
    private readonly string _baseUrl;
    private Guid ChatId;
    public CertainChatPage(Guid chatid, ISaverService saverService,string baseUrl)
    {
        InitializeComponent();
        BindingContext = this;
        //Test.Text = chatid.ToString();
        _saverService = saverService;
        HttpClientHandler _httpClientHandler = new HttpClientHandler();
        _httpClientHandler.UseCookies = true;
        _httpClientHandler.CookieContainer = new System.Net.CookieContainer();

        _baseUrl = baseUrl;

        CookieCollection cookies = _saverService.GetCookie();
        if (cookies != null)
        {
            foreach (Cookie cookie in cookies)
            {
                _httpClientHandler.CookieContainer.Add(new Uri(baseUrl), cookie);
            }
        }
        

        string token = saverService.GetMyToken();
        UserName = saverService.GetName();
        _httpClient = new HttpClient(_httpClientHandler);
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        Title = UserName;
        ChatId =chatid;
        

        _hubConnection = new HubConnectionBuilder()
               .WithUrl($"{baseUrl}/MyChatHub")
               .Build();



        _hubConnection.On<string>("ReceivedMessageFromMyChat", (message) =>
        {

            Dispatcher.Dispatch(async () =>
            {
                await LoadChat(chatid, currentPage, baseUrl);
            });
            
        });

      
            Dispatcher.Dispatch(async () =>
            {
                await _hubConnection.StartAsync();
            });
        LoadChat(chatid, currentPage,baseUrl);
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        var prevPage = Navigation.NavigationStack.FirstOrDefault(page => page.GetType() == typeof(DisplayUsersAndCreateChat));
        if (prevPage != null)
        {
            Navigation.RemovePage(prevPage);
        }
    }

    private async void btnSend_Clicked(object sender, EventArgs e)
    {
        await _hubConnection.InvokeCoreAsync("SendMessageToMyChat", args: new[]
        {
                txtMessage.Text
                
        });
        SendMessage sm = new SendMessage() { ChatId=ChatId,TextToSend=txtMessage.Text };

        var json = JsonSerializer.Serialize(sm);
        var content = new StringContent(json, Encoding.UTF8, "application/json");


        var apiUrl = $"{_baseUrl}/api/Chat/sendMess";
        var response = await _httpClient.PostAsync(apiUrl, content);

        if(response.IsSuccessStatusCode) 
        {
            _messagesOBC.Insert(0,new MessagesFront {SenderName=UserName,MessageText=txtMessage.Text });
        }

        txtMessage.Text = String.Empty;
    }
    private async void btnRefresh_Clicked(object sender, EventArgs e)
    {
        currentPage++; 
        await LoadChat(ChatId, currentPage,_baseUrl);
    }

    


    private async Task LoadChat(Guid chatid, int page,string baseUrl)
    {
        if (page == 1)
        {
            _messagesOBC.Clear();
        }
     
        var apiUrl = $"{baseUrl}/api/Chat/ShowCertainChat/{chatid}/{page}";

        try
        {
            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                List<MessagesFront> messages = (await response.Content.ReadFromJsonAsync<List<MessagesFront>>()).ToList();

                foreach (var message in messages)
                {
                    bool isMine = message.SenderName.Trim().Equals(UserName.Trim(), StringComparison.OrdinalIgnoreCase);
                    _messagesOBC.Add(new MessagesFront
                    {
                        MessageText = message.MessageText,
                        SenderName = message.SenderName,
                        IsMine= isMine,
                        MessageAlignment=isMine?LayoutOptions.End:LayoutOptions.Start
                        
                    });
                }
                MessagesList.ItemsSource = null;
                MessagesList.ItemsSource = _messagesOBC;
            }
            else
            {
               
            }
        }
        catch (Exception ex)
        {
           
        }
    }

}