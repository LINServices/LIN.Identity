namespace LIN.Cloud.Identity.Services.Utils;

public class EmailSender
{

  public async Task<bool> Send(string body)
    {
        Global.Http.Services.Client client = new("https://hangfire.linplatform.com/api/mailsender")
        {
            TimeOut = 10
        };

        client.AddParameter("subject", "");
        client.AddParameter("mail", "");

        var aa = await client.Post(body);


        return true;


    }

}