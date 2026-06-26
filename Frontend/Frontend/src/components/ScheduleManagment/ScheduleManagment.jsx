import { useEffect, useState } from 'react';
import { useMovie } from '../../../contexts/movieContext';
import classes from './ScheduleManagment.module.css';
import Popup from '../Popup/Popup';

const API_BASE_URL = import.meta.env.VITE_API_URL;

export default function ScheduleManagment() {
    const { movies, fetchMovies } = useMovie();
    const [sessions, setSessions] = useState([]);
    const [halls, setHalls] = useState([]);
    const [selectedHall, setSelectedHall] = useState(null);
    const [loading, setLoading] = useState(true);
    const [popup, setPopup] = useState(null);
    const [editingSession, setEditingSession] = useState(null);
    const [formData, setFormData] = useState({
        movieId: '',
        hallId: '',
        startTime: '',
        price: ''
    });

    useEffect(() => {
        fetchMovies();
    }, []);

    const fetchHalls = async () => {
        try {
            const response = await fetch(`${API_BASE_URL}/halls`, {
                credentials: 'include'
            });

            const data = await response.json();

            if (!response.ok)
                 throw new Error(data?.message);

            setHalls(data.halls || []);

            if (data.halls?.length > 0 && !selectedHall) {
                setSelectedHall(data.halls[0]);
                setFormData(prev => ({ ...prev, hallId: data.halls[0].id }));
            }
        } catch (error) {
            console.error('Error fetching halls:', error);
            showMessage(error.message, 'error');
        }
    };

    const fetchSessions = async () => {
        if (!selectedHall) return;
        
        setLoading(true);
        try {
            const response = await fetch(`${API_BASE_URL}/sessions?hallId=${selectedHall.id}`, {
                credentials: 'include'
            });

            const data = await response.json();
            
            if (!response.ok) throw new Error(data?.message);
            setSessions(data.sessions || []);
        } catch (error) {
            console.error('Error fetching sessions:', error);
            showMessage(error.message, 'error');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchHalls();
    }, []);

    useEffect(() => {
        fetchSessions();
    }, [selectedHall]);

    const showMessage = (message, type = 'success') => {
        setPopup({ message, type, showConfirm: false });
        setTimeout(() => setPopup(null), 3000);
    };

    const showConfirm = (message, onConfirm) => {
        setPopup({ message, type: 'confirm', showConfirm: true, onConfirm });
    };

    const formatDateForInput = (isoDateString) => {
        if (!isoDateString) return '';
        
        const date = new Date(isoDateString);
        
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        const hours = String(date.getHours()).padStart(2, '0');
        const minutes = String(date.getMinutes()).padStart(2, '0');
        // console.log(`${year}-${month}-${day}T${hours}:${minutes}`);
        return `${year}-${month}-${day}T${hours}:${minutes}`;
    };

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
        
        if (name === 'hallId') {
            const hall = halls.find(h => h.id === parseInt(value));
            setSelectedHall(hall);
        }
    };

    const handleEdit = (session) => {
        setEditingSession(session);
        setFormData({
            movieId: session.movieId,
            hallId: session.hallId,
            startTime: formatDateForInput(session.startTime),
            price: session.price
        });
        const hall = halls.find(h => h.id === session.hallId);
        if (hall) setSelectedHall(hall);
    };

    const handleCancelEdit = () => {
        setEditingSession(null);
        setFormData({
            movieId: '',
            hallId: selectedHall?.id || '',
            startTime: '',
            price: ''
        });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        
        if (!formData.movieId) {
            showMessage('Выберите фильм', 'error');
            return;
        }
        if (!formData.startTime) {
            showMessage('Выберите время начала сеанса', 'error');
            return;
        }
        if (!formData.price || parseFloat(formData.price) <= 0) {
            showMessage('Введите корректную цену', 'error');
            return;
        }
        
        try {
            const url = editingSession 
                ? `${API_BASE_URL}/sessions/${editingSession.id}`
                : `${API_BASE_URL}/sessions`;
            
            const body = {
                movieId: parseInt(formData.movieId),
                hallId: parseInt(formData.hallId),
                startTime: formData.startTime + ':00.000Z',
                price: parseFloat(formData.price)
            };
            
            const response = await fetch(url, {
                method: editingSession ? 'PUT' : 'POST',
                headers: { 'Content-Type': 'application/json' },
                credentials: 'include',
                body: JSON.stringify(body)
            });
            
            const data = await response.json();
            
            if (!response.ok) {
                throw new Error(data?.message || 'Ошибка при сохранении сеанса');
            }
            
            showMessage(editingSession ? 'Сеанс обновлен' : 'Сеанс добавлен', 'success');
            handleCancelEdit();
            fetchSessions();
            
        } catch (error) {
            console.error('Ошибка:', error);
            showMessage(error.message, 'error');
        }
    };

    const formatDateTime = (dateString) => {
        return new Date(dateString).toLocaleString('ru-RU', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    };

    const getMovieTitle = (movieId) => {
        const movie = movies.find(m => m.id === movieId);
        return movie?.title || 'Фильм не найден';
    };

    console.log(sessions)

    return (
        <div className={classes.container}>
            <div className={classes.header}>
                <h3>Управление расписанием</h3>
            </div>

            <div className={classes.content}>
                <div className={classes.schedule_section}>
                    <h4>Расписание зала "{selectedHall?.name || '...'}"</h4>
                    
                    {loading ? (
                        <div className={classes.loading_spinner}>
                            <div className={classes.spinner}></div>
                            <p>Загрузка...</p>
                        </div>
                    ) : (
                        <div className={classes.sessions_list}>
                            {sessions.length === 0 ? (
                                <div className={classes.empty_sessions}>
                                    <p>Нет сеансов в этом зале</p>
                                    <p>Добавьте первый сеанс с помощью формы</p>
                                </div>
                            ) : (
                                sessions.map((session) => (
                                    <div key={session.id} className={classes.session_card}>
                                        <div className={classes.session_time}>
                                            <span className={classes.date}>
                                                {formatDateTime(session.startTime)}
                                            </span>
                                        </div>
                                        <div className={classes.session_info}>
                                            <span className={classes.movie_title}>
                                                {getMovieTitle(session.movieId)}
                                            </span>
                                            <span className={classes.session_price}>
                                                {session.price} ₽
                                            </span>
                                        </div>
                                        <div className={classes.session_actions}>
                                            <button 
                                                className={classes.edit_btn}
                                                onClick={() => handleEdit(session)}
                                            >
                                                ✏️
                                            </button>
                                        </div>
                                    </div>
                                ))
                            )}
                        </div>
                    )}
                </div>
                <div className={classes.form_section}>
                    <h4>{editingSession ? '✏️ Редактирование сеанса' : '➕ Добавление сеанса'}</h4>
                    <form onSubmit={handleSubmit} className={classes.form}>
                        <div className={classes.form_group}>
                            <label>Зал</label>
                            <select
                                name="hallId"
                                value={formData.hallId || selectedHall?.id || ''}
                                onChange={handleChange}
                                required
                                disabled={!!editingSession}
                            >
                                <option value="">Выберите зал</option>
                                {halls.map((hall) => (
                                    <option key={hall.id} value={hall.id}>
                                        {hall.name} ({hall.rows}×{hall.cols})
                                    </option>
                                ))}
                            </select>
                        </div>

                        <div className={classes.form_group}>
                            <label>Фильм</label>
                            <select
                                name="movieId"
                                value={formData.movieId}
                                onChange={handleChange}
                                required
                            >
                                <option value="">Выберите фильм</option>
                                {movies.map((movie) => (
                                    <option key={movie.id} value={movie.id}>
                                        {movie.title} ({movie.year}) - {movie.duration} мин
                                    </option>
                                ))}
                            </select>
                        </div>

                        <div className={classes.form_group}>
                            <label>Дата и время начала</label>
                            <input
                                type="datetime-local"
                                name="startTime"
                                value={formData.startTime}
                                onChange={handleChange}
                                required
                            />
                        </div>

                        <div className={classes.form_group}>
                            <label>Цена (руб.)</label>
                            <input
                                type="number"
                                name="price"
                                value={formData.price}
                                onChange={handleChange}
                                placeholder="Например: 350"
                                step="1"
                                min="1"
                                required
                            />
                        </div>

                        <div className={classes.form_buttons}>
                            <button type="submit" className={classes.save_btn}>
                                {editingSession ? '💾 Сохранить' : '➕ Добавить'}
                            </button>
                            {editingSession && (
                                <button type="button" className={classes.cancel_btn} onClick={handleCancelEdit}>
                                    ❌ Отмена
                                </button>
                            )}
                        </div>
                    </form>
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
    );
}