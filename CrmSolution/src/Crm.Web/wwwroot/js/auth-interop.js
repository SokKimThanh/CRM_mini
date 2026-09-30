window.crmAuth = {
    login: async function (email, password, rememberMe, returnUrl) {
        const formData = new FormData();
        formData.append('Email', email);
        formData.append('Password', password);
        formData.append('RememberMe', rememberMe);
        formData.append('ReturnUrl', returnUrl || '/');

        try {
            const response = await fetch('/api/account/login', {
                method: 'POST',
                body: formData,
                credentials: 'include'
            });

            const data = await response.json();

            if (response.ok && data.success) {
                return { success: true, redirect: data.redirect, error: null };
            }

            return { success: false, redirect: null, error: data.error || 'Đăng nhập thất bại' };
        } catch (error) {
            return { success: false, redirect: null, error: 'Không thể kết nối server' };
        }
    },

    logout: async function () {
        try {
            await fetch('/api/account/logout', {
                method: 'POST',
                credentials: 'include'
            });
        } catch (error) {
            console.error('Logout error', error);
        }
        window.location.href = '/login';
    }
};