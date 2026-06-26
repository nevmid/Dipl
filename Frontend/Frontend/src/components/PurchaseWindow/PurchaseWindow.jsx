import classes from './PurchaseWindow.module.css';
import { useEffect, useState } from 'react';
import {useAuth} from '../../../contexts/AuthContext';
import { Link } from 'react-router-dom';
import Popup from '../Popup/Popup';
import { useLocation, useNavigate } from 'react-router-dom';

const API_BASE_URL = import.meta.env.VITE_API_URL;
const IMAGE_URL = import.meta.env.VITE_URL;

export default function PurchaseWindow(){
    const location = useLocation();
    const navigate = useNavigate();
    const {session, hallId} = location.state || {}; 
    const [popup, setPopup] = useState();
    const {isAuthenticated, user} = useAuth();
    const [seats, setSeats] = useState([]);
    const [selectedSeats, setSelectedSeats] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [isProcessing, setIsProcessing] = useState(false);
    const [bookings, setBookings] = useState([]);
    const [bookedSeats, setBookedSeats] = useState([]);
    const [paymentUrl, setPaymentUrl] = useState(null);
    const [useBonus, setUseBonus] = useState(false);
    const [zoom, setZoom] = useState(0.8);
    const [isDragging, setIsDragging] = useState(false);
    const [dragStart, setDragStart] = useState({ x: 0, y: 0 });
    const [position, setPosition] = useState({ x: 0, y: 0 });
    const [agreeToPrivacy, setAgreeToPrivacy] = useState(false);

    const handleZoomIn = () => setZoom(prev => Math.min(prev + 0.1, 2));
    const handleZoomOut = () => setZoom(prev => Math.max(prev - 0.1, 0.5));
    const handleResetZoom = () => {
        setZoom(0.8);
        setPosition({ x: 0, y: 0 });
    };

    const handleMouseDown = (e) => {
        setIsDragging(true);
        setDragStart({ x: e.clientX - position.x, y: e.clientY - position.y });
    };

    const handleMouseMove = (e) => {
        if (!isDragging) return;
        setPosition({
            x: e.clientX - dragStart.x,
            y: e.clientY - dragStart.y
        });
    };

    const handleMouseUp = () => setIsDragging(false);


    useEffect(() => {
        const fetchHallSeats = async () => {
            setIsLoading(true);
            try{
                const response = await fetch(`${API_BASE_URL}/halls/${hallId}`, {
                    method: "GET"
                });

                const data = await response.json();

                if (!response.ok){
                    throw new Error(data?.message || 'Error fetching hall info')
                }

                setSeats(data?.hall.seats || []);
            }
            catch (error) {
                console.error('Error fetching hall info:', error);
                setSeats([]);
            }
            finally{
                setIsLoading(false);
            }
        }

        fetchHallSeats();
    }, [hallId]);

    useEffect(() => {
        const fetchBookings = async () => {
            setIsLoading(true);
            try{
                const response = await fetch(`${API_BASE_URL}/sessions/${session.id}`, {
                    method: "GET"
                });

                const data = await response.json();

                if (!response.ok){
                    throw new Error(data?.message || 'Error fetching hall info')
                }

                setBookings(data.session.bookings || []);
            }
            catch (error) {
                console.error(error);
                setBookings([])
            }
            finally{
                setIsLoading(false);
            }
        }

        fetchBookings();
    }, [session.id])

    useEffect(() => {
        if (!bookings || !Array.isArray(bookings)) {
            setBookedSeats([]);
            return;
        }

        const booked = [];
        bookings.forEach(booking => {
            if(booking.status.name === 'confirmed' || booking.status.name === 'pending'){
                if (booking.bookingSeats && Array.isArray(booking.bookingSeats)){
                    booking.bookingSeats.forEach(seat => {
                        booked.push(seat.seatId);
                    })
                }
            }
        });
        console.log(booked);
        setBookedSeats(booked);
    }, [bookings]);

    const showMessage = (message, type = 'success') => {
        setPopup({ message, type, showConfirm: false });
    };

    const isSeatBooked = (seatId) => {
        return bookedSeats.includes(seatId);
    }

    const calculatePrices = () => {
        const originalPrice = selectedSeats.length * session.price;
        
        if (!useBonus || !user?.loyaltyAccount) {
            return {
                originalPrice,
                bonusUsed: 0,
                discount: 0,
                finalPrice: originalPrice
            };
        }
        
        const maxDiscount = originalPrice * 0.7;
        const bonusValue = user.loyaltyAccount.balance;
        const discount = Math.min(bonusValue, maxDiscount);
        
        return {
            originalPrice,
            bonusUsed: discount,
            discount,
            finalPrice: originalPrice - discount
        };
    };

    const handleSeatClick = (seat) => {
        if (isSeatBooked(seat.id)) return;
        
        setSelectedSeats(prev => {
            const isSelected = prev.some(s => seat.id === s.id);
            if (isSelected){
                return prev.filter(s => s.id !== seat.id);
            }
            else{
                return [...prev, seat];
            }
        });
    }

    const getSeatStatus = (seatId) => {
        if (isSeatBooked(seatId)) return 'booked';
        return selectedSeats.some(s => s.id === seatId) ? 'selected' : 'available';
    }

    const groupSeatsByRows = () => {
        const rows = {};
        seats.forEach((seat) => {
            if(!rows[seat.rowNum]){
                rows[seat.rowNum] = [];
            }
            rows[seat.rowNum].push(seat);
        });

        return rows;
    }

    const handlePurchase = async () => {
        if (!isAuthenticated) {
            showMessage('Для бронирования необходимо авторизоваться', 'warning');
            return;
        }

        if (selectedSeats.length === 0) {
            showMessage('Выберите места для бронирования', 'warning');
            return;
        }

        if (!agreeToPrivacy) {
            showMessage('Для бронирования необходимо согласие на обработку персональных данных', 'warning');
        return;
        }
        // console.log(selectedSeats);
        setIsProcessing(true);

        try {
            const bookingResponse = await fetch(`${API_BASE_URL}/bookings`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                credentials: 'include',
                body: JSON.stringify({
                    sessionId: session.id,
                    seatIds: selectedSeats.map(s => s.id),
                    isBonusUsed: useBonus
                })
            });

            const bookingData = await bookingResponse.json();

            if (!bookingResponse.ok) {
                throw new Error(bookingData?.message
                         || Object.values(data?.errors || {})[0] || 'Ошибка при создании бронирования');
            }

            const bookingId = bookingData.booking.id;
            // console.log(bookingData);
            const paymentResponse = await fetch(`${API_BASE_URL}/bookings/${bookingId}/pay`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                credentials: 'include'
            });

            const paymentData = await paymentResponse.json();
            console.log(paymentData);
            if (!paymentResponse.ok) {
                throw new Error(paymentData?.message || 'Ошибка при инициации платежа');
            }

            // console.log(paymentData);

            if (paymentData.paymentUrl) {
                window.open(paymentData.paymentUrl, '_blank');
                
                showMessage(
                    'Бронирование создано! Оплатите в открывшемся окне. После оплаты билеты будут доступны в личном кабинете.',
                    'info'
                );
            } else {
                showMessage(`Билеты успешно забронированы! ${selectedSeats.length} мест(а)`, 'success');
            }

        } catch (error) {
            console.error('Ошибка:', error);
            showMessage(error.message, 'error');
        } finally {
            setIsProcessing(false);
        }
    };

    if (isLoading) {
        return (
            <div className={classes.loading}>
                <div className={classes.spinner}></div>
                <p>Загрузка...</p>
            </div>
        );
    }
    // console.log(bookings);
    // console.log(bookedSeats);
    const rows = groupSeatsByRows();
    const totalPrice = selectedSeats.length * session.price;
    const prices = calculatePrices();
    // console.log(user);

    return(
        <div className={classes.container}>
            <button className={classes.btn_back} onClick={() => navigate("/")}>
                ← Назад
            </button>
            <div className={classes.main_info}>
                <div className={classes.info}>
                    <div className={classes.seats_section}>
                        <div className={classes.screen}>
                            <p>Экран</p>
                        </div>
                        <div className={classes.zoom_controls}>
                            <button onClick={handleZoomIn} className={classes.zoom_btn}>🔍 +</button>
                            <button onClick={handleResetZoom} className={classes.zoom_btn}>⟳</button>
                            <button onClick={handleZoomOut} className={classes.zoom_btn}>🔍 -</button>
                            <span className={classes.zoom_level}>{Math.round(zoom * 100)}%</span>
                        </div>

                        <div 
                            className={classes.seats_container}
                            onMouseDown={handleMouseDown}
                            onMouseMove={handleMouseMove}
                            onMouseUp={handleMouseUp}
                            onMouseLeave={handleMouseUp}
                            style={{ cursor: isDragging ? 'grabbing' : 'grab' }}
                        >
                            <div 
                                className={classes.seats_grid}
                                style={{
                                    transform: `scale(${zoom}) translate(${position.x / zoom}px, ${position.y / zoom}px)`,
                                    transition: isDragging ? 'none' : 'transform 0.2s ease'
                                }}
                            >
                                {Object.entries(rows).map(([row, rowSeats]) => (
                                    <div key={row} className={classes.row}>
                                        <div className={classes.row_num}>{row}</div>
                                        <div className={classes.seats_row}>
                                            {rowSeats.map(seat => (
                                                <div
                                                    key={seat.id}
                                                    className={`${classes.seat} ${classes[getSeatStatus(seat.id)]}`}
                                                    onClick={() => handleSeatClick(seat)}
                                                    style={{ width: `${30 * zoom}px`, height: `${30 * zoom}px` }}
                                                >
                                                    {seat.colNum}
                                                </div>
                                            ))}
                                        </div>
                                    </div>
                                ))}
                            </div>
                        </div>
                        <div className={classes.legend}>
                            <div className={classes.legend_item}>
                                <div className={`${classes.legend_color} ${classes.available}`}></div>
                                <span>Свободно</span>
                            </div>
                            <div className={classes.legend_item}>
                                <div className={`${classes.legend_color} ${classes.selected}`}></div>
                                <span>Выбрано</span>
                            </div>
                            <div className={classes.legend_item}>
                                <div className={`${classes.legend_color} ${classes.booked}`}></div>
                                <span>Занято</span>
                            </div>
                        </div>
                        {!isAuthenticated && (
                            <div className={classes.not_auth}>
                                Для оплаты необходимо авторизоваться
                            </div>)}
                        {isAuthenticated && user?.loyaltyAccount.balance > 0 && (
                            <div className={classes.bonusSection}>
                                <div className={classes.bonusInfo}>
                                    <span>Ваши бонусы: {user.loyaltyAccount.balance}</span>
                                    <span className={classes.bonusHint}>
                                        (1 бонус = 1 рубль, можно использовать до 70% стоимости)
                                    </span>
                                </div>
                                <label className={classes.bonusCheckbox}>
                                    <input
                                        type="checkbox"
                                        checked={useBonus}
                                        onChange={(e) => setUseBonus(e.target.checked)}
                                    />
                                    <span>Использовать бонусы</span>
                                </label>
                                
                                {useBonus && prices.discount > 0 && (
                                    <div className={classes.bonusDiscount}>
                                        <p>Скидка: {prices.discount} ₽</p>
                                        <p>Потрачено бонусов: {prices.bonusUsed}</p>
                                    </div>
                                )}
                            </div>
                        )}
                        <div className={classes.checkbox_group}>
                            <label className={classes.checkbox_label}>
                                <input
                                    type="checkbox"
                                    checked={agreeToPrivacy}
                                    onChange={(e) => setAgreeToPrivacy(e.target.checked)}
                                    disabled={isLoading}
                                    required
                                />
                                <span>
                                    Я принимаю условия{' '}
                                    <Link to="/privacy-policy" target="_blank" className={classes.privacy_link}>
                                        политики обработки персональных данных
                                    </Link>
                                </span>
                            </label>
                        </div>
                        <button 
                            className={classes.purchase_button}
                            onClick={handlePurchase}
                            disabled={(!isAuthenticated || selectedSeats.length === 0)}
                        >
                            {isProcessing ? (
                                '⏳ Обработка...'
                            ) : (
                                `Забронировать (${totalPrice} руб.)`
                            )}
                        </button>
                        {isAuthenticated && !useBonus && (
                            <div className={classes.bonusEarnInfo}>
                                <p>После оплаты вы получите ≈{Math.floor(prices.finalPrice * 0.05)} бонусов</p>
                            </div>
                        )}
                    </div>
                    <div className={classes.movie_info}>
                        <div 
                            className={classes.poster}
                            style={{ backgroundImage: `url("${IMAGE_URL}${session.movie.posterUrl}")` }}>
                        </div>
                        <div>
                            <h3>{session.movie.title}</h3>
                            <h4>{session.movie.originalTitle} ({session.movie.year}) {session.movie.age}+</h4>
                            <p>Зал: {session.hall.name}</p>
                            <p>Дата: {new Date(session.startTime).toLocaleDateString("ru-RU")}</p>
                            <p>Время: {new Date(session.startTime).toLocaleTimeString("ru-RU", {hour: "2-digit", minute: "2-digit"})}</p>
                            <p>Цена за место: {session.price} руб.</p>

                            {selectedSeats.length > 0 && (
                            <div className={classes.selected_info}>
                                <p>Выбрано мест: {selectedSeats.length}</p>
                                <p>Общая стоимость: {totalPrice} руб.</p>
                            </div>
                        )}
                        </div>
                    </div>
                </div>
            </div>
            {popup && (
                <Popup
                    message={popup.message}
                    type={popup.type}
                    showConfirm={popup.showConfirm}
                    onClose={() => setPopup(null)}
                    onConfirm={popup.onConfirm}
                    onCancel={() => setPopup(null)}
                />
            )}
        </div>
    )
}