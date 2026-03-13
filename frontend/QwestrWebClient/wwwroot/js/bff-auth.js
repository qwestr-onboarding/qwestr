window.bffAuth = {
    post: async function (url, body) {
        const response = await fetch(url, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'include',
            body: body ? JSON.stringify(body) : '{}'
        });
        const text = await response.text();
        let data = null;
        try { data = JSON.parse(text); } catch {}
        return {
            status: response.status,
            isSuccess: response.ok,
            data: data,
            error: response.ok ? null : text
        };
    }
};