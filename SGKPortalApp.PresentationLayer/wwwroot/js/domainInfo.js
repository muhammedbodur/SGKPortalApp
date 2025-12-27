// Domain bilgisi test için JavaScript helper fonksiyonları

window.getClientInfo = function () {
    return {
        userAgent: navigator.userAgent,
        platform: navigator.platform,
        language: navigator.language,
        screenResolution: `${screen.width}x${screen.height}`,
        timeZone: Intl.DateTimeFormat().resolvedOptions().timeZone
    };
};

// Tarayıcıda domain bilgisi almaya çalışma (başarısız olacak)
window.tryGetDomainInfo = function () {
    var result = {
        canGetDomainInfo: false,
        method: "None",
        domainUser: null,
        error: null
    };

    try {
        // IE/Edge eski versiyonlarda çalışabilir (ActiveXObject)
        if (typeof ActiveXObject !== "undefined") {
            try {
                var network = new ActiveXObject("WScript.Network");
                result.canGetDomainInfo = true;
                result.method = "ActiveXObject (IE Legacy)";
                result.domainUser = network.UserDomain + "\\" + network.UserName;
            } catch (e) {
                result.error = "ActiveXObject denied: " + e.message;
            }
        } else {
            result.error = "ActiveXObject not available (Modern browser)";
        }
    } catch (e) {
        result.error = e.message;
    }

    return result;
};

// Machine name almaya çalışma
window.tryGetMachineName = function () {
    try {
        // Sadece hostname alınabilir (FQDN değil, domain bilgisi yok)
        return {
            hostname: window.location.hostname,
            note: "Bu sadece web server hostname'dir, client machine değil"
        };
    } catch (e) {
        return { error: e.message };
    }
};

console.log("Domain Info JavaScript helpers loaded");
