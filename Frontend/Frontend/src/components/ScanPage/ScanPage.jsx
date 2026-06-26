import { useState, useEffect } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import { useAuth } from '../../../contexts/AuthContext';
import classes from './ScanPage.module.css';
import Popup from '../Popup/Popup';

const API_BASE_URL = import.meta.env.VITE_API_URL;

export default function ScanPage() {
    const [searchParams] = useSearchParams();
    const navigate = useNavigate();
    const { isAuthenticated, user } = useAuth();
    const [ticket, setTicket] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [popup, setPopup] = useState(null);
    const [isConfirming, setIsConfirming] = useState(false);
    const [isValidating, setIsValidating] = useState(false);

    const token = searchParams.get("token");
    // console.log(token);

    useEffect(() => {
        const validateTicket = async () => {
            if (!token) {
                setError('QR-код не содержит информацию о билете');
                setLoading(false);
                return;
            }

            setIsValidating(true);
            try {
                const response = await fetch(`${API_BASE_URL}/scan?token=${encodeURIComponent(token)}`);
                const data = await response.json();

                if (!response.ok) {
                    throw new Error(data?.message || 'Недействительный билет');
                }
                setTicket(data.ticket);
            } catch (error) {
                console.error('Ошибка:', error);
                setError(error.message);
            } finally {
                setIsValidating(false);
                setLoading(false);
            }
        };

        validateTicket();
    }, [token]);

    const showMessage = (message, type = 'success') => {
        setPopup({ message, type, showConfirm: false });
    };

    const handleConfirmEntry = async () => {
        if (!isAuthenticated) {
            showMessage('Для подтверждения входа необходимо авторизоваться', 'error');
            return;
        }

        if (user?.role !== 'admin' && user?.role !== 'staff') {
            showMessage('У вас нет прав для подтверждения входа', 'error');
            return;
        }

        if (ticket?.isUsed) {
            showMessage('Этот билет уже был использован', 'error');
            return;
        }

        setIsConfirming(true);
        try {
            const response = await fetch(`${API_BASE_URL}/scan/confirm`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                credentials: 'include',
                body: JSON.stringify({ token: token })
            });

            const data = await response.json();

            if (!response.ok) {
                throw new Error(data?.error || 'Ошибка при подтверждении входа');
            }

            showMessage('Вход подтвержден!', 'success');
            
            setTicket(prev => prev ? { ...prev, isUsed: true } : null);
            
            setTimeout(() => {
                navigate('/');
            }, 1000);
            
        } catch (error) {
            console.error('Error confirming entry:', error);
            showMessage(error.message, 'error');
        } finally {
            setIsConfirming(false);
        }
    };

    const formatDateTime = (dateString) => {
        if (!dateString) return '—';
        return new Date(dateString).toLocaleString('ru-RU', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    };

    console.log(ticket);

    if (loading || isValidating) {
        return (
            <div className={classes.loading}>
                <div className={classes.spinner}></div>
                <p>Проверка билета...</p>
            </div>
        );
    }

    if (error) {
        return (
            <div className={classes.errorContainer}>
                <div className={classes.errorCard}>
                    <div className={classes.errorIcon}>❌</div>
                    <h2>Ошибка</h2>
                    <p>{error}</p>
                    <button onClick={() => navigate('/')} className={classes.backBtn}>
                        На главную
                    </button>
                </div>
            </div>
        );
    }

    if (!ticket) {
        return (
            <div className={classes.errorContainer}>
                <div className={classes.errorCard}>
                    <div className={classes.errorIcon}>🎫</div>
                    <h2>Билет не найден</h2>
                    <p>Информация о билете отсутствует</p>
                    <button onClick={() => navigate('/')} className={classes.backBtn}>
                        На главную
                    </button>
                </div>
            </div>
        );
    }

    const isUsed = ticket.isUsed;
    const canConfirm = !isUsed && isAuthenticated && (user?.role === 'admin' || user?.role === 'staff');

    return (
        <div className={classes.container}>
            <div className={classes.ticketCard}>
                <div className={classes.ticketHeader}>
                    <div className={classes.headerIcon}>🎬</div>
                    <h1>Электронный билет</h1>
                    <p>Кинотеатр Space Cinema</p>
                </div>

                <div className={classes.ticketStatus}>
                    <div className={`${classes.statusBadge} ${isUsed ? classes.used : classes.valid}`}>
                        {isUsed ? '✓ ИСПОЛЬЗОВАН' : '✓ ДЕЙСТВИТЕЛЕН'}
                    </div>
                </div>

                <div className={classes.ticketContent}>
                    <div className={classes.movieSection}>
                        <h2>{ticket.movieTitle}</h2>
                        <div className={classes.movieDetails}>
                            <span>{ticket.sessionTime && formatDateTime(ticket.sessionTime)}</span>
                            <span>Зал: {ticket.hallName}</span>
                        </div>
                    </div>

                    <div className={classes.infoSection}>
                        <div className={classes.infoRow}>
                            <span className={classes.infoLabel}>Номер билета:</span>
                            <span className={classes.infoValue}>{ticket.ticketNumber}</span>
                        </div>
                        <div className={classes.infoRow}>
                            <span className={classes.infoLabel}>Пользователь:</span>
                            <span className={classes.infoValue}>{ticket.userEmail}</span>
                        </div>
                        {ticket.bookingId && (
                            <div className={classes.infoRow}>
                                <span className={classes.infoLabel}>ID бронирования:</span>
                                <span className={classes.infoValue}>#{ticket.bookingId}</span>
                            </div>
                        )}
                    </div>

                    {isUsed && (
                        <div className={classes.usedMessage}>
                            <span>⚠️</span>
                            <p>Данный билет уже был использован</p>
                        </div>
                    )}
                </div>

                <div className={classes.ticketFooter}>
                    {canConfirm ? (
                        <button 
                            className={classes.confirmBtn}
                            onClick={handleConfirmEntry}
                            disabled={isConfirming}
                        >
                            {isConfirming ? 'Обработка...' : '✅ Подтвердить вход'}
                        </button>
                    ) : isAuthenticated && (user?.role !== 'admin' && user?.role !== 'staff') ? (
                        <div className={classes.noAccessMessage}>
                            <p>У вас нет прав для подтверждения входа</p>
                        </div>
                    ) : !isAuthenticated ? (
                        <div className={classes.noAccessMessage}>
                            <p>Для подтверждения входа необходимо авторизоваться</p>
                            <button onClick={() => navigate('/login')} className={classes.loginBtn}>
                                Войти
                            </button>
                        </div>
                    ) : null}
                </div>
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