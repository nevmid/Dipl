import { useState, useEffect } from 'react';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';
import classes from './ResetPassword.module.css';
import Popup from '../Popup/Popup';

const API_BASE_URL = import.meta.env.VITE_API_URL;

export default function ResetPassword() {
    const [searchParams] = useSearchParams();
    const navigate = useNavigate();
    const [token, setToken] = useState('');
    const [password, setPassword] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const [isSuccess, setIsSuccess] = useState(false);
    const [popup, setPopup] = useState(null);
    const [isValidating, setIsValidating] = useState(true);
    const [isTokenValid, setIsTokenValid] = useState(false);

    useEffect(() => {
        const urlToken = searchParams.get('token');
        if (!urlToken) {
            setPopup({ message: 'Ссылка для восстановления недействительна', type: 'error', showConfirm: false });
            setIsValidating(false);
            return;
        }
        setToken(urlToken);
        setIsTokenValid(true);
        setIsValidating(false);
    }, [searchParams]);

    const showMessage = (message, type = 'success') => {
        setPopup({ message, type, showConfirm: false });
        setTimeout(() => setPopup(null), 3000);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (password.length < 8) {
            showMessage('Пароль должен содержать минимум 8 символов', 'error');
            return;
        }

        setIsLoading(true);

        try {
            const response = await fetch(`${API_BASE_URL}/users/reset-password`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    token: token,
                    newPassword: password
                })
            });

            const data = await response.json();

            if (!response.ok) {
                throw new Error(Object.values(data?.errors || {})[0] || data?.message || 'Ошибка при сбросе пароля');
            }

            setIsSuccess(true);
            showMessage(data.message, 'success');

            setTimeout(() => {
                navigate('/login');
            }, 1000);

        } catch (error) {
            console.error('Error:', error);
            showMessage(error.message, 'error');
        } finally {
            setIsLoading(false);
        }
    };

    if (isValidating) {
        return (
            <div className={classes.container}>
                <div className={classes.loadingCard}>
                    <div className={classes.spinner}></div>
                    <p>Проверка ссылки...</p>
                </div>
            </div>
        );
    }

    if (!isTokenValid) {
        return (
            <div className={classes.container}>
                <div className={classes.errorCard}>
                    <div className={classes.errorIcon}>🔗</div>
                    <h2>Недействительная ссылка</h2>
                    <p>Ссылка для восстановления пароля недействительна или истекла.</p>
                    <div className={classes.buttons}>
                        <Link to="/forgot-password" className={classes.resendLink}>
                            Запросить новую ссылку
                        </Link>
                        <Link to="/login" className={classes.backLink}>
                            Вернуться ко входу
                        </Link>
                    </div>
                </div>
            </div>
        );
    }

    if (isSuccess) {
        return (
            <div className={classes.container}>
                <div className={classes.successCard}>
                    <div className={classes.successIcon}>✅</div>
                    <h2>Пароль успешно изменен!</h2>
                    <p>Теперь вы можете войти в аккаунт с новым паролем.</p>
                    <Link to="/login" className={classes.loginBtn}>
                        Войти в аккаунт
                    </Link>
                </div>
            </div>
        );
    }

    return (
        <div className={classes.container}>
            <div className={classes.card}>
                <div className={classes.header}>
                    <h2>🔑 Создание нового пароля</h2>
                    <p>Придумайте новый надежный пароль</p>
                </div>

                <form onSubmit={handleSubmit} className={classes.form}>
                    <div className={classes.formGroup}>
                        <label>Новый пароль</label>
                        <input
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            placeholder="Введите новый пароль"
                            disabled={isLoading}
                            required
                        />
                    </div>

                    <div className={classes.passwordHint}>
                        <span></span>
                        <p>Пароль должен содержать минимум 8 символов</p>
                    </div>

                    <button 
                        type="submit" 
                        className={classes.submitBtn}
                        disabled={isLoading}
                    >
                        {isLoading ? 'Сохранение...' : 'Сохранить новый пароль'}
                    </button>

                    <div className={classes.footer}>
                        <Link to="/login" className={classes.backLink}>
                            ← Вернуться ко входу
                        </Link>
                    </div>
                </form>
            </div>

            {popup && (
                <Popup
                    message={popup.message}
                    type={popup.type}
                    showConfirm={popup.showConfirm}
                    onClose={() => setPopup(null)}
                />
            )}
        </div>
    );
}