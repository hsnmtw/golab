

namespace WebApp.Otp;


public class OtpManager : IOtpManager
{
    public static IOtpManager Instance { get; }
	
    public void AutoRefresh() {
        //Instance.AutoRefresh();
    }

    public string? Curl(string url) {
        return null;
		//return Instance.Curl(url);
    }

    public (string? chat_id, string? secret) GetLastUpdate() {
        return (null,null);
		//return Instance.GetLastUpdate();
    }

    public string? GetLastUpdateIdLocal() {
        return null;//Instance.GetLastUpdateIdLocal();
    }

    public void LogError(Exception ex) {
        //Instance.LogError(ex);
    }

    public void PerofrmAdminAction(string? instruction) {
        //Instance.PerofrmAdminAction(instruction);
    }

    public void Refresh() {
        //Instance.Refresh();
    }

    public string? SendMessage(string chatId, string text) {
        return null;//Instance.SendMessage(chatId, text);
    }

    public void SendMessageAdmin(string v) {
        //Instance.SendMessageAdmin(v);
    }
}