

namespace WebApp.Otp;
public interface IOtpManager {
    (string? chat_id, string? secret) GetLastUpdate();
    string? GetLastUpdateIdLocal();
    string? SendMessage(string chatId, string text);
    string? Curl(string url);
    void LogError(Exception ex);
    void AutoRefresh();
    void Refresh();
    void PerofrmAdminAction(string? instruction);
    void SendMessageAdmin(string v);
}