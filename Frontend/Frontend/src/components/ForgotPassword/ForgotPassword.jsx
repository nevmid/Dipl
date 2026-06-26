import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import classes from './ForgotPassword.module.css';
import Popup from '../Popup/Popup';

const API_BASE_URL = import.meta.env.VITE_API_URL;

export default function ForgotPassword() {
    const [email, setEmail] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const [popup, setPopup] = useState(null);
    const [isSuccess, setIsSuccess] = useState(false);
    const navigate = useNavigate();

    const showMessage = (message, type = 'success') => {
        setPopup({ message, type, showConfirm: false });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        
        if (!email) {
            showMessage('Введите email', 'error');
            return;
        }

        setIsLoading(true);
        
        try {
            const response = await fetch(`${API_BASE_URL}/users/forgot-password`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email })
            });

            const data = await response.json();

            if (!response.ok) {
                throw new Error(data?.message || Object.values(data?.errors || {})[0] || 'Ошибка при отправке ссылки');
            }

            setIsSuccess(true);
            showMessage(data.message, 'success');
            
        } catch (error) {
            console.error('Error:', error);
            showMessage(error.message, 'error');
        } finally {
            setIsLoading(false);
        }
    };

    if (isSuccess) {
        return (
            <div className={classes.container}>
                <div className={classes.successCard}>
                    <div className={classes.successIcon}>📧</div>
                    <h2>Проверьте почту</h2>
                    <p>Мы отправили ссылку для восстановления пароля на <strong>{email}</strong></p>
                    <p>Перейдите по ссылке в письме, чтобы создать новый пароль.</p>
                    <div className={classes.buttons}>
                        <Link to="/login" className={classes.loginLink}>
                            Вернуться ко входу
                        </Link>
                        <button onClick={() => {
                            setEmail('');
                            setIsSuccess(false);
                        }} className={classes.resendBtn}>
                            Отправить еще раз
                        </button>
                    </div>
                </div>
            </div>
        );
    }

    return (
        <div className={classes.container}>
            <div className={classes.card}>
                <div className={classes.header}>
                    <h2>Восстановление пароля</h2>
                    <p>Введите email, указанный при регистрации</p>
                </div>

                <form onSubmit={handleSubmit} className={classes.form}>
                    <div className={classes.formGroup}>
                        <label>Email</label>
                        <input
                            type="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            placeholder="example@mail.com"
                            disabled={isLoading}
                            required
                        />
                    </div>
                    <button 
                        type="submit" 
                        className={classes.submitBtn}
                        disabled={isLoading}
                    >
                        {isLoading ? 'Отправка...' : 'Отправить ссылку'}
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