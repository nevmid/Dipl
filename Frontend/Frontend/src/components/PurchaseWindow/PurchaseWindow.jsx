import classes from './PurchaseWindow.module.css';
import { useEffect, useState } from 'react';
import {useAuth} from '../../../contexts/AuthContext';
import { Link } from 'react-router-dom';

const API_BASE_URL = import.meta.env.VITE_API_URL;

export default function PurchaseWindow({session, hallId, onBack}){
    const {isAuthenticated} = useAuth();
    const [seats, setSeats] = useState([]);
    const [selectedSeats, setSelectedSeats] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [bookings, setBookings] = useState([]);
    const [bookedSeats, setBookedSeats] = useState([]);

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
    }, []);

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
    }, [])

    useEffect(() => {
        if (!bookings || !Array.isArray(bookings)) {
            setBookedSeats([]);
            return;
        }

        const booked = [];
        bookings.forEach(booking => {
            if(booking.status === 'confirmed' || booking.status === 'pending'){
                if (booking.bookingSeats && Array.isArray(booking.bookingSeats)){
                    booking.bookingSeats.forEach(seat => {
                        booked.push(seat.seatId);
                    })
                }
            }
        });
        
        setBookedSeats(booked);
    }, [bookings]);

    const isSeatBooked = (seatId) => {
        return bookedSeats.includes(seatId);
    }

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
    // console.log(rows);

    return(
        <div className={classes.container}>
            <button className={classes.btn_back} onClick={() => onBack()}>
                ← Назад
            </button>
            <div className={classes.main_info}>
                <div className={classes.info}>
                    <div className={classes.seats_section}>
                        <div className={classes.screen}>
                            <p>Экран</p>
                        </div>
                        <div className={classes.seats_grid}>
                            {Object.entries(rows).map(([row, rowSeats]) => (
                                <div key={row} className={classes.row}>
                                    <p>{row}</p>
                                    <div className={classes.seats_row}>
                                        {rowSeats.map(seat => (
                                            <div
                                                key={seat.id}
                                                className={`${classes.seat} ${classes[getSeatStatus(seat.id)]}`}
                                                onClick={() => handleSeatClick(seat)}
                                            >
                                                {seat.colNum}
                                            </div>
                                        ))}
                                    </div>
                                </div>
                            ))}
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
                        <button 
                            className={classes.purchase_button}
                            // onClick={handlePurchase}
                            disabled={(!isAuthenticated || selectedSeats.length === 0)}
                        >
                            Забронировать ({totalPrice} руб.)
                        </button>
                    </div>
                    <div className={classes.movie_info}>
                        <div 
                            className={classes.poster}
                            style={{ backgroundImage: `url("http://localhost:5014${session.movie.posterUrl}")` }}>
                        </div>
                        <div>
                            <h3>{session.movie.title}</h3>
                            <h4>{session.movie.originalTitle} ({session.movie.year})</h4>
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
        </div>
    )
}