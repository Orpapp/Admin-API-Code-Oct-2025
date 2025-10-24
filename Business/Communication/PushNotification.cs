using Newtonsoft.Json;
using Shared.Common;
using Shared.Common.Enums;
using System.Text;

namespace Business.Communication
{
    public class PushNotification
    {
        public static async Task<string> SendPushNotification(short deviceType, string deviceToken, string message, string type, string title)
        {
            try
            {
                string applicationID = SiteKeys.FCMServerKey ?? string.Empty;
                string senderId = SiteKeys.FCMSenderId ?? string.Empty;

                string json = "";

                if (deviceType == (int)DeviceTypeEnum.Android)
                {
                    var body = new
                    {
                        to = deviceToken,
                        data = new
                        {
                            click_action = "FLUTTER_NOTIFICATION_CLICK",
                            notificationtype = type,
                            title = title,
                            body = message
                        },
                        
                    };

                    json = JsonConvert.SerializeObject(body);
                }
                else if (deviceType == (int)DeviceTypeEnum.IOS)
                {
                    var body = new
                    {
                        to = deviceToken,
                        notification = new
                        {
                            title = title,
                            body = message,
                            
                        },
                       
                    };

                    json = JsonConvert.SerializeObject(body);
                }

                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri("https://fcm.googleapis.com");
                

                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={applicationID}");
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sender", $"id={senderId}");

                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync("/fcm/send", content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        return responseContent;
                    }
                    else
                    {
                        // Handle error here
                        return "Push notification failed";
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    }
}
