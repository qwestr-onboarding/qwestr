window.bffAuth = {
    post: async function (url, body, accessToken = null) {
        let req = {
            method: 'POST',
            headers: {},
            credentials: 'include'
        };
        if (body) {
            req.headers['Content-Type'] = 'application/json';
            req.body = JSON.stringify(body);
        }
        if (accessToken) {
            req.headers['Authorization'] = 'Bearer ' + accessToken;
        }
        const response = await fetch(url, req);

        const text = await response.text();
        let data = null;
        try { data = JSON.parse(text); } catch {}

        return {
            status: response.status,
            isSuccess: response.ok,
            data: data,
            error: response.ok ? null : text
        };
    },

    clearStoredToken: async function () {
        try {
            await fetch('/bff/auth/clear-token', { method: 'POST', credentials: 'include' });
        } catch {}
    }
};
