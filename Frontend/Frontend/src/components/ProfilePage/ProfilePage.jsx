import { useEffect, useState } from 'react';
import { useAuth } from '../../../contexts/AuthContext';
import { Link, useNavigate } from 'react-router-dom';
import classes from './ProfilePage.module.css';
import Popup from '../../components/Popup/Popup';

const API_BASE_URL = import.meta.env.VITE_API_URL;

export default function ProfilePage() {
    const { user, isAuthenticated, isLoading: authLoading, logout } = useAuth();
    const [bookings, setBookings] = useState([]);
    const [loading, setLoading] = useState(true);
    const [popup, setPopup] = useState(null);
    const [refundBooking, setRefundBooking] = useState(null);
    const [selectedTicket, setSelectedTicket] = useState(null);
    const [showQrModal, setShowQrModal] = useState(false);
    const [showChangeEmailModal, setShowChangeEmailModal] = useState(false);
    const [showChangePasswordModal, setShowChangePasswordModal] = useState(false);
    const [showDeleteAccountModal, setShowDeleteAccountModal] = useState(false);
    const [emailForm, setEmailForm] = useState({ email: '', password: '' });
    const [passwordForm, setPasswordForm] = useState({ 
        currentPassword: '', 
        newPassword: '', 
    });
    const navigate = useNavigate();

    useEffect(() => {
        const fetchBookings = async () => {
            try {
                const response = await fetch(`${API_BASE_URL}/bookings`, {
                    credentials: 'include'
                });
                const data = await response.json();

                if (!response.ok) {
                    throw new Error(data?.message || 'Ошибка загрузки бронирований');
                }

                setBookings(Array.isArray(data) ? data : []);
                console.log(data);
            } catch (error) {
                console.error('Error fetching bookings:', error);
                showMessage(error.message, 'error');
            } finally {
                setLoading(false);
            }
        };

        if (isAuthenticated) {
            fetchBookings();
        }
    }, [isAuthenticated]);

    const showMessage = (message, type = 'success') => {
        setPopup({ message, type, showConfirm: false });
    };

    const showConfirmRefund = (bookingId) => {
        setRefundBooking(bookingId);
        setPopup({message: `Вы уверены, что хотите вернуть билет?`, type: "confirm", showConfirm: true})
    }

    const handleLogout = async () => {
        await logout();
        navigate('/');
        showMessage('Вы вышли из аккаунта', 'info');
    };

    const handleChangeEmail = async () => {
        try {
            const response = await fetch(`${API_BASE_URL}/users/change-email`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                credentials: 'include',
                body: JSON.stringify(emailForm)
            });

            const data = await response.json();

            if (!response.ok) {
                throw new Error(data?.message|| Object.values(data?.errors || {})[0] || 'Ошибка смены email');
            }

            showMessage('Email успешно изменен! Перезайдите в аккаунт.', 'success');
            setShowChangeEmailModal(false);
            setEmailForm({ email: '', password: '' });
            
            setTimeout(() => {
                logout();
                navigate('/login');
            }, 1000);
            
        } catch (error) {
            showMessage(error.message, 'error');
        }
    };

    const handleChangePassword = async () => {
        if (passwordForm.newPassword.length < 8) {
            showMessage('Пароль должен быть не менее 8 символов', 'error');
            return;
        }

        try {
            const response = await fetch(`${API_BASE_URL}/users/change-password`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                credentials: 'include',
                body: JSON.stringify({
                    currentPassword: passwordForm.currentPassword,
                    newPassword: passwordForm.newPassword
                })
            });

            
            const data = await response.json();
            if (!response.ok) {
                throw new Error(data?.message || Object.values(data?.errors || {})[0] || 'Ошибка смены пароля');
            }

            showMessage('Пароль успешно изменен! Перезайдите в аккаунт.', 'success');
            setShowChangePasswordModal(false);
            setPasswordForm({ currentPassword: '', newPassword: '' });
            
            setTimeout(() => {
                logout();
                navigate('/login');
            }, 1000);
            
        } catch (error) {
            showMessage(error.message, 'error');
        }
    };

    const handleDeleteAccount = async () => {
        try {
            const response = await fetch(`${API_BASE_URL}/users`, {
                method: 'DELETE',
                credentials: 'include'
            });

            const data = await response.json();

            if (!response.ok) {
                throw new Error(data?.message || 'Ошибка удаления аккаунта');
            }

            showMessage('Аккаунт успешно удален', 'success');
            setShowDeleteAccountModal(false);
            
            setTimeout(() => {
                logout();
                navigate('/');
            }, 1000);
        
        } catch (error) {
            showMessage(error.message, 'error');
        }
    };

    const handleShowTicket = async (booking) => {
        try {
            const response = await fetch(`${API_BASE_URL}/bookings/${booking.id}/ticket`, {
                credentials: 'include'
            });
            const data = await response.json();

            if (!response.ok) {
                throw new Error(data?.message || 'Билет не найден');
            }

            setSelectedTicket({
                booking: booking,
                ticket: data.ticket
            });
            setShowQrModal(true);
        } catch (error) {
            console.error('Error fetching ticket:', error);
            showMessage(error.message, 'error');
        }
    };

    const handleCancel = async (bookingId) => {
        try {
            const response = await fetch(`${API_BASE_URL}/bookings/${bookingId}/cancel`, {
                method: 'POST',
                credentials: 'include',
                headers: { 'Content-Type': 'application/json' }
            });

            const data = await response.json();

            if (!response.ok) {
                throw new Error(data?.message || 'Ошибка при отмене бронирования');
            }

            showMessage(data?.message || 'Бронирование отменено', 'success');

            setTimeout(() => {
                window.location.reload();
            }, 1500);

        } catch (error) {
            console.error('Error cancelling:', error);
            showMessage(error.message, 'error');
        }
    };

    const getStatusText = (status) => {
        switch (status?.toLowerCase()) {
            case 'pending':
                return { text: '⏳ Ожидает оплаты', class: classes.statusPending };
            case 'confirmed':
            case 'paid':
                return { text: '✅ Подтвержден', class: classes.statusConfirmed };
            case 'cancelled':
                return { text: '❌ Отменен', class: classes.statusCancelled };
            case 'expired':
                return { text: '⌛ Истек', class: classes.statusExpired };
            default:
                return { text: status || '📋 Неизвестно', class: classes.statusUnknown };
        }
    };

    const handleRefund = async (bookingId) => {
        if(!refundBooking) return;
        try {
            const response = await fetch(`${API_BASE_URL}/bookings/${refundBooking}/refund`, {
                method: 'POST',
                credentials: 'include',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({refundBooking, reason: "Refund"})
            });

            const data = await response.json();

            if (!response.ok) {
                throw new Error(data?.message || 'Ошибка при возврате билета');
            }

            showMessage(data.message, 'success');

            setTimeout(() => {
                window.location.reload();
            }, 1000);

        } catch (error) {
            console.error('Error refunding:', error);
            showMessage(error.message, 'error');
        }    
    };

    const getPaymentDeadline = (createdAt) => {
        if (!createdAt) return '—';

        const createdAtDate = new Date(createdAt);
        const deadline = new Date(createdAtDate.getTime() + (15 + 180) * 60000);
        return deadline.toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' });
        };

    const canRefund = (booking) => {
        const isPaid = booking.status?.toLowerCase() === 'confirmed';
        const sessionNotStarted = new Date(booking.session?.startTime) > new Date();
        const notUsed = !booking.ticket?.isUsed;
        return isPaid && sessionNotStarted && notUsed;
    };

    const formatDate = (dateString) => {
        if (!dateString) return '—';
        return new Date(dateString).toLocaleDateString('ru-RU', {
            day: 'numeric',
            month: 'long',
            year: 'numeric'
        });
    };

    const formatSeats = (booking) => {
    if (!booking.seatIds || booking.seatIds.length === 0) return 'Не указаны';
    
    if (booking.seatsFormatted && booking.seatsFormatted.length > 0) {
        return booking.seatsFormatted.join(', ');
    }
    
    return booking.seatIds.map(id => `Место ${id}`).join(', ');
};

    const formatTime = (dateString) => {
        if (!dateString) return '—';
        return new Date(dateString).toLocaleTimeString('ru-RU', {
            hour: '2-digit',
            minute: '2-digit'
        });
    };

    
    if (!isAuthenticated) {
        return (
            <div className={classes.notAuth}>
                <h2>🔐 Доступ ограничен</h2>
                <p>Пожалуйста, войдите в аккаунт</p>
                <Link to="/login" className={classes.loginBtn}>Войти</Link>
            </div>
        );
    }

    if (authLoading || loading) {
        return (
            <div className={classes.loading}>
                <div className={classes.spinner}></div>
                <p>Загрузка...</p>
            </div>
        );
    }
    
    // console.log(bookings);

    return (
        <div className={classes.container}>
            <div className={classes.profileWrapper}>
                <div className={classes.profileInfo}>
                    <h2>{user?.email || 'Пользователь'}</h2>
                    <p className={classes.role}>
                        {user?.role === 'admin' && 'Администратор'}
                        {user?.role === 'staff' && 'Сотрудник'}
                        {user?.role === 'user' && 'Пользователь'}
                    </p>
                    <div className={classes.bonusSection}>
                        <div className={classes.bonusHeader}>
                            <span>Бонусный счет</span>
                            <span className={classes.bonusValue}>{user?.loyaltyAccount.balance || 0}</span>
                        </div>
                    </div>
                    <div className={classes.stats}>
                        <div className={classes.statItem}>
                            <span className={classes.statValue}>{bookings.length}</span>
                            <span className={classes.statLabel}>Билетов</span>
                        </div>
                        <div className={classes.statItem}>
                            <span className={classes.statValue}>
                                {bookings.filter(b => 
                                    b.status?.toLowerCase() === 'confirmed' || 
                                    b.status?.toLowerCase() === 'paid'
                                ).length}
                            </span>
                            <span className={classes.statLabel}>Активных</span>
                        </div>
                    </div>
                    <div className={classes.accountActions}>
                        <button 
                            className={classes.actionBtn}
                            onClick={() => setShowChangeEmailModal(true)}
                        >
                            Сменить email
                        </button>
                        <button 
                            className={classes.actionBtn}
                            onClick={() => setShowChangePasswordModal(true)}
                        >
                            Сменить пароль
                        </button>
                        <button 
                            className={classes.logoutBtn}
                            onClick={handleLogout}
                        >
                            Выйти
                        </button>
                        <button 
                            className={classes.deleteBtn}
                            onClick={() => setShowDeleteAccountModal(true)}
                        >
                            Удалить аккаунт
                        </button>
                    </div>
                </div>
                <div className={classes.bookingsHistory}>
                    <h3>История бронирований</h3>
                    
                    {bookings.length === 0 ? (
                        <div className={classes.emptyHistory}>
                            <p>🎫 У вас пока нет бронирований</p>
                            <Link to="/" className={classes.buyBtn}>Купить билет</Link>
                        </div>
                    ) : (
                        <div className={classes.bookingsList}>
                            {bookings.map((booking) => {
                                const status = getStatusText(booking.status);
                                return (
                                    <div key={booking.id} className={classes.bookingCard}>
                                        <div className={classes.bookingHeader}>
                                            <div className={classes.movieInfo}>
                                                <h4>{booking.movie?.title || 'Фильм'}</h4>
                                                <p className={classes.movieDetails}>
                                                    {booking.movie?.year} · {booking.movie?.duration} мин · {booking.movie?.age}+
                                                </p>
                                            </div>
                                            <div className={`${classes.statusBadge} ${status.class}`}>
                                                {status.text}
                                            </div>
                                        </div>
                                        
                                        <div className={classes.bookingDetails}>
                                            <div className={classes.detailRow}>
                                                <span>Дата:</span>
                                                <strong>{formatDate(booking.session?.startTime)}</strong>
                                            </div>
                                            <div className={classes.detailRow}>
                                                <span>Время:</span>
                                                <strong>{formatTime(booking.session?.startTime)}</strong>
                                            </div>
                                            <div className={classes.detailRow}>
                                                <span>Зал:</span>
                                                <strong>{booking.hall?.name || 'Не указан'}</strong>
                                            </div>
                                            <div className={classes.detailRow}>
                                                <span>Сумма:</span>
                                                <strong>{booking.finalAmount} ₽</strong>
                                            </div>
                                            <div className={classes.detailRow}>
                                                <span>Места:</span>
                                                <strong>{formatSeats(booking)}</strong>
                                            </div>
                                            {booking.bonusUsed > 0 && (
                                                <div className={classes.detailRow}>
                                                    <span>Использовано бонусов:</span>
                                                    <strong>{booking.bonusUsed}</strong>
                                                </div>
                                            )}
                                            <div className={classes.detailRow}>
                                                <span>Бронирование:</span>
                                                <strong>{formatDate(booking.createdAt)}</strong>
                                            </div>
                                        </div>
                                        
                                        {(booking.status?.toLowerCase() === 'confirmed' || 
                                          booking.status?.toLowerCase() === 'paid') && (
                                            <button 
                                                className={classes.ticketBtn}
                                                onClick={() => handleShowTicket(booking)}
                                            >
                                                Показать билет
                                            </button>
                                        )}
                                        
                                        {booking.status?.toLowerCase() === 'pending' && (
                                            <button 
                                                className={classes.payBtn}
                                                onClick={() => {
                                                    window.open(booking.payment.paymentUrl, "_blank");
                                                }}
                                            >
                                                Оплатить (до {getPaymentDeadline(booking.createdAt)})
                                            </button>
                                        )}

                                        {booking.status?.toLowerCase() === 'pending' && (
                                            <button 
                                                className={classes.cancelBtn}
                                                onClick={() => {
                                                    handleCancel(booking.id);
                                                }}
                                            >
                                                Отменить
                                            </button>
                                        )}

                                        {canRefund(booking) && (
                                            <button 
                                                className={classes.refundBtn}
                                                onClick={() => showConfirmRefund(booking.id)}
                                            >
                                                Вернуть билет
                                            </button>
                                        )}
                                    </div>
                                );
                            })}
                        </div>
                    )}
                </div>
            </div>
            {showChangeEmailModal && (
                <div className={classes.modalOverlay} onClick={() => setShowChangeEmailModal(false)}>
                    <div className={classes.modalContent} onClick={(e) => e.stopPropagation()}>
                        <button className={classes.modalClose} onClick={() => setShowChangeEmailModal(false)}>✖</button>
                        <h3>Смена email</h3>
                        <div className={classes.modalForm}>
                            <input
                                type="email"
                                placeholder="Новый email"
                                value={emailForm.newEmail}
                                onChange={(e) => setEmailForm({ ...emailForm, email: e.target.value })}
                            />
                            <input
                                type="password"
                                placeholder="Пароль"
                                value={emailForm.confirmEmail}
                                onChange={(e) => setEmailForm({ ...emailForm, password: e.target.value })}
                            />
                            <div className={classes.modalButtons}>
                                <button onClick={handleChangeEmail} className={classes.saveBtn}>Сохранить</button>
                                <button onClick={() => setShowChangeEmailModal(false)} className={classes.cancelBtn}>Отмена</button>
                            </div>
                        </div>
                    </div>
                </div>
            )}
            {showChangePasswordModal && (
                <div className={classes.modalOverlay} onClick={() => setShowChangePasswordModal(false)}>
                    <div className={classes.modalContent} onClick={(e) => e.stopPropagation()}>
                        <button className={classes.modalClose} onClick={() => setShowChangePasswordModal(false)}>✖</button>
                        <h3>Смена пароля</h3>
                        <div className={classes.modalForm}>
                            <input
                                type="password"
                                placeholder="Текущий пароль"
                                value={passwordForm.currentPassword}
                                onChange={(e) => setPasswordForm({ ...passwordForm, currentPassword: e.target.value })}
                            />
                            <input
                                type="password"
                                placeholder="Новый пароль"
                                value={passwordForm.newPassword}
                                onChange={(e) => setPasswordForm({ ...passwordForm, newPassword: e.target.value })}
                            />
                            <div className={classes.modalButtons}>
                                <button onClick={handleChangePassword} className={classes.saveBtn}>Сохранить</button>
                                <button onClick={() => setShowChangePasswordModal(false)} className={classes.cancelBtn}>Отмена</button>
                            </div>
                        </div>
                    </div>
                </div>
            )}
            {showDeleteAccountModal && (
                <div className={classes.modalOverlay} onClick={() => setShowDeleteAccountModal(false)}>
                    <div className={classes.modalContent} onClick={(e) => e.stopPropagation()}>
                        <button className={classes.modalClose} onClick={() => setShowDeleteAccountModal(false)}>✖</button>
                        <h3>⚠️ Удаление аккаунта</h3>
                        <p>Вы уверены, что хотите удалить свой аккаунт? Это действие необратимо.</p>
                        <p>Все ваши бронирования и билеты будут потеряны.</p>
                        <div className={classes.modalButtons}>
                            <button onClick={handleDeleteAccount} className={classes.deleteConfirmBtn}>Да, удалить</button>
                            <button onClick={() => setShowDeleteAccountModal(false)} className={classes.cancelBtn}>Отмена</button>
                        </div>
                    </div>
                </div>
            )}
            {showQrModal && selectedTicket && (
                <div className={classes.modalOverlay} onClick={() => setShowQrModal(false)}>
                    <div className={classes.modalContent} onClick={(e) => e.stopPropagation()}>
                        <button className={classes.modalClose} onClick={() => setShowQrModal(false)}>✖</button>
                        
                        <div className={classes.ticketContainer}>
                            <div className={classes.ticketHeader}>
                                <h3>Электронный билет</h3>
                                <p>Кинотеатр Space Cinema</p>
                            </div>
                            
                            <div className={classes.ticketInfo}>
                                <div className={classes.ticketMovie}>
                                    <h4>{selectedTicket.booking.movie?.title}</h4>
                                    <p>{selectedTicket.booking.movie?.year} · {selectedTicket.booking.movie?.duration} мин</p>
                                </div>
                                
                                <div className={classes.ticketDetails}>
                                    <div className={classes.ticketRow}>
                                        <span>📅 Дата:</span>
                                        <strong>{formatDate(selectedTicket.booking.session?.startTime)}</strong>
                                    </div>
                                    <div className={classes.ticketRow}>
                                        <span>⏰ Время:</span>
                                        <strong>{formatTime(selectedTicket.booking.session?.startTime)}</strong>
                                    </div>
                                    <div className={classes.ticketRow}>
                                        <span>🎬 Зал:</span>
                                        <strong>{selectedTicket.booking.hall?.name}</strong>
                                    </div>
                                    <div className={classes.detailRow}>
                                        <span>🪑 Места:</span>
                                        <strong>{formatSeats(selectedTicket.booking)}</strong>
                                    </div>
                                    <div className={classes.ticketRow}>
                                        <span>Пользователь:</span>
                                        <strong>{user?.email}</strong>
                                    </div>
                                </div>
                            </div>
                            
                            <div className={classes.qrSection}>
                                {selectedTicket.ticket?.qrCode ? (
                                    <img 
                                        src={`data:image/png;base64,${selectedTicket.ticket.qrCode}`} 
                                        alt="QR Code" 
                                        className={classes.qrCode} 
                                    />
                                ) : (
                                    <div className={classes.qrPlaceholder}>
                                        <span>🎫</span>
                                        <p>QR-код билета</p>
                                    </div>
                                )}
                                <div className={classes.ticketNumber}>
                                    Номер билета: {selectedTicket.ticket?.ticketNumber || '—'}
                                </div>
                            </div>
                            
                            <div className={classes.ticketFooter}>
                                <p>Предъявите данный QR-код при входе в зал</p>
                                <p className={classes.note}>Билет действителен только для указанного сеанса</p>
                            </div>
                        </div>
                        
                        <button 
                            className={classes.downloadBtn}
                            onClick={() => window.print()}
                        >
                            Распечатать билет
                        </button>
                    </div>
                </div>
            )}
            
            {popup && (
                <Popup
                    message={popup.message}
                    type={popup.type}
                    showConfirm={popup.showConfirm}
                    onClose={() => setPopup(null)}
                    onConfirm={handleRefund}
                    onCancel={() => setPopup(null)}
                />
            )}
        </div>
    );
}