import classes from './Schedule.module.css';
import { useEffect, useState } from 'react';
// import {useMovie} from '../../../contexts/movieContext';
import PurchaseWindow from '../PurchaseWindow/PurchaseWindow';

const API_BASE_URL = import.meta.env.VITE_API_URL;

export default function Schedule() {
    const [halls, setHalls] = useState([]);
    const [sessions, setSessions] = useState([]);
    const [isLoadingSessions, setIsLoadingSessions] = useState(true);
    // const [isLoadingHalls, setIsLoadingHalls] = useState(true);
    const [selectedDate, setSelectedDate] = useState(null);
    const [dates, setDates] = useState([]);
    const [selectedSession, setSelectedSession] = useState(null);

    const generateDates = () => {
        const today = new Date();
        const weekDays = [];

        for (let i = 0; i < 7; i++){
            const date = new Date(today);
            date.setDate(today.getDate() + i);

            const dayNames = ['Воскресенье', 'Понедельник', 'Вторник', 'Среда', 'Четверг', 'Пятница', 'Суббота'];
            const months = ['января', 'февраля', 'марта', 'апреля', 'мая', 'июня',
                 'июля', 'августа', 'сентября', 'октября', 'ноября', 'декабря'];
            
            const year = date.getFullYear();
            const month = String(date.getMonth() + 1).padStart(2, '0');
            const day = String(date.getDate()).padStart(2, '0');
            const fullDate = `${year}-${month}-${day}`;

            weekDays.push({
                date: date,
                dayName: dayNames[date.getDay()],
                day: date.getDate(),
                month: months[date.getMonth()],
                fullDate: fullDate,
                isToday: i === 0
            });
        }
        // console.log(weekDays);
        setDates(weekDays);
        setSelectedDate(weekDays[0].fullDate);
    }

    useEffect(() => {
    //     const fetchHalls = async () => {
    //         setIsLoadingHalls(true);
    //         try {
    //             const response = await fetch(`http://localhost:5014/api/halls`, {
    //                 method: "GET"
    //             });

    //             const data = await response.json();

    //             if (!response.ok)
    //                 throw new Error(data?.message
    //                                 || 'Error fetching halls')

    //             setHalls(data.halls || []);
    //         } catch (error) {
    //             console.error('Error fetching halls:', error);
    //             setHalls([]);
    //         }
    //         finally{
    //             setIsLoadingHalls(false);
    //         }
    //     }

    //     fetchHalls();
        generateDates();
    }, [])

    useEffect(() => {
        const fetchSessions = async () => {
            setIsLoadingSessions(true);
            try {
                const url = selectedDate 
                    ? `${API_BASE_URL}/sessions?date=${selectedDate}`
                    : `${API_BASE_URL}/sessions`;
                    
                const response = await fetch(url, {
                    method: "GET"
                });

                const data = await response.json();
                // console.log(data);
                if (!response.ok)
                    throw new Error(data?.message
                                    || 'Error fetching sessions')

                setSessions(data.sessions || []);
                // console.log(data.sessions);
            }
            catch (error) {
                console.error('Error fetching sessions:', error);
                setSessions([]);
            }
            finally{
                setIsLoadingSessions(false);
            }
        }
        
        if (selectedDate) {
            fetchSessions();
        }
    }, [selectedDate])

    // if (isLoadingHalls || isLoadingSessions) {
    //     return (
    //         <div className={classes.loading}>
    //             <div className={classes.spinner}></div>
    //             <p>Загрузка...</p>
    //         </div>
    //     );
    // }

    const groupSessionsByMovieInHall = (movieSessions) => {
        const grouped = {};
        movieSessions.forEach((session) => {
            if (!grouped[session.hallId]) {
                grouped[session.hallId] = {
                    hall: session.hall,
                    sessions: []
                };
            }
            grouped[session.hallId].sessions.push(session);
        });
        return Object.values(grouped);
    };

    const groupSessionsByMovie = () => {
        const grouped = {};
        sessions.forEach((session) => {
            if(!grouped[session.movieId]){
                grouped[session.movieId] = {
                    movie: session.movie,
                    sessions: []
                };
            }
            grouped[session.movieId].sessions.push(session);
        });

        return Object.values(grouped);
    }

    const handleOnBack = () => {
        setSelectedSession(null);
    }
    
    if (selectedSession) {
        return (
            <PurchaseWindow
                session = {selectedSession}
                hallId={selectedSession.hallId}
                onBack={handleOnBack}
            />
        );
    }

    const groupedByMovie  = groupSessionsByMovie();
    // console.log(grouped);
    // if (sessions.length == 0){
    //     return(
    //         <div className={classes.error}>
    //             <p>Расписание отсутствует</p>
    //         </div>
    //     );
    // }

    return (
        <div className={classes.container}>
            <div className={classes.main_list}>
                <h2>Расписание сеансов</h2>
                <div className={classes.date_container}>
                    {dates.map((date, index) => (
                        <button
                            key={index}
                            className={`${classes.date_card} ${selectedDate === date.fullDate ? classes.active : ''}`}
                            onClick={() => setSelectedDate(date.fullDate)}
                        >
                            {date.isToday ? (
                                <span className={classes.day_name}>Сегодня</span>
                            ) : (
                                <span className={classes.day_name}>{date.dayName}</span>
                            )}
                            <span className={classes.date}>{date.day} {date.month}</span>
                        </button>
                    ))}
                </div>
                <div className={classes.list}>
                    {groupedByMovie.length === 0 ? (
                        <div className={classes.no_sessions}>
                            <p>На выбранную дату сеансов нет</p>
                            <p>Попробуйте выбрать другую дату</p>
                        </div>
                    ) : (
                        groupedByMovie.map((movieGroup) => (
                            // <div key={movieGroup.movie.id} className={classes.hall_section}>
                            <div key={movieGroup.movie.id} className={classes.movie_card}>
                                <div className={classes.movie_poster}>
                                    <img 
                                        src={`http://localhost:5014${movieGroup.movie.posterUrl}`} 
                                    />
                                </div>
                                <div className={classes.movie_info}>
                                    <h4>{movieGroup.movie.title}</h4>
                                    <p className={classes.movie_year}>{movieGroup.movie.year} год</p>
                                    <p className={classes.movie_duration}>
                                        {movieGroup.movie.duration} мин
                                    </p>
                                    {groupSessionsByMovieInHall(movieGroup.sessions).map((hallGroup) => (
                                        <div className={classes.time_slots} key={hallGroup.hall.id}>
                                            <span className={classes.hall_name}>{hallGroup.hall.name}</span>
                                            {hallGroup.sessions.map((session) => (
                                                <div className={classes.time_slot} key={session.id} >
                                                    <button
                                                        className={classes.time_btn}
                                                        onClick={() => setSelectedSession(session)}
                                                    >
                                                        {new Date(session.startTime).toLocaleTimeString('ru-RU', {
                                                            hour: '2-digit',
                                                            minute: '2-digit'
                                                        })}
                                                    </button>
                                                    <span className={classes.price}>{session.price}₽</span>
                                                </div>
                                            ))}
                                        </div>
                                    ))}
                                </div>
                            </div>
                                // <div className={classes.movies_container}>
                                //     {groupSessionsByMovieInHall(movieGroup.sessions).map((hallGroup) => (
                                        
                                //     ))}
                                // </div>
                            // </div>
                        ))
                    )}
                </div>
            </div>
        </div>
    );
}