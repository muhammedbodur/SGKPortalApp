window.blazorReconnect = {
    showReconnectMessage: function (message, timeout) {
        // Varsayılan İngilizce mesajı Türkçeye çevir
        if (message === "Rejoin failed. Trying again in {0} seconds.") {
            message = "Yeniden bağlanma başarısız. {0} saniye içinde tekrar deneniyor.";
        } else if (message === "Connection is down. Attempting to reconnect...") {
            message = "Bağlantı kesildi. Yeniden bağlanılmaya çalışılıyor...";
        } else if (message === "Connection down. Rejoining chat...") {
            message = "Bağlantı kesildi. Sohbete yeniden katılınıyor...";
        }
        // {0} yerine timeout değerini yerleştir
        message = message.replace("{0}", timeout);
        Blazor._internal.defaultReconnectHandler.showReconnectMessage(message, timeout);
    },
    startReconnect: function (timeout) {
        Blazor._internal.defaultReconnectHandler.startReconnect(timeout);
    },
    endReconnect: function () {
        Blazor._internal.defaultReconnectHandler.endReconnect();
    },
    showConnectionDownMessage: function () {
        Blazor._internal.defaultReconnectHandler.showConnectionDownMessage();
    }
};