namespace SGKPortalApp.BusinessObjectLayer.Interfaces.SignalR
{
    /// <summary>
    /// SignalR mesaj yayınlama soyutlaması
    /// Business katmanından Presentation katmanındaki hub'a mesaj göndermek için kullanılır
    /// </summary>
    public interface ISignalRBroadcaster
    {
        /// <summary>
        /// Belirli connection ID'lere mesaj gönder
        /// </summary>
        /// <param name="connectionIds">Hedef connection ID listesi</param>
        /// <param name="eventName">Event adı</param>
        /// <param name="payload">Gönderilecek veri</param>
        Task SendToConnectionsAsync(IEnumerable<string> connectionIds, string eventName, object payload);

        /// <summary>
        /// Belirli bir gruba mesaj gönder
        /// </summary>
        /// <param name="groupName">Grup adı (örn: BANKO_1, TV_1)</param>
        /// <param name="eventName">Event adı</param>
        /// <param name="payload">Gönderilecek veri</param>
        Task SendToGroupAsync(string groupName, string eventName, object payload);

        /// <summary>
        /// Tüm bağlı istemcilere mesaj gönder
        /// </summary>
        /// <param name="eventName">Event adı</param>
        /// <param name="payload">Gönderilecek veri</param>
        Task BroadcastAllAsync(string eventName, object payload);
    }
}
