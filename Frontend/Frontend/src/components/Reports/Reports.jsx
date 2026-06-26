import { useEffect, useState } from 'react';
import classes from './Reports.module.css';
import Popup from '../Popup/Popup';

const API_BASE_URL = import.meta.env.VITE_API_URL;
const IMAGE_URL = import.meta.env.VITE_URL;

export default function Reports() {
    const [activeTab, setActiveTab] = useState('summary');
    const [loading, setLoading] = useState(false);
    const [popup, setPopup] = useState(null);
    const [summary, setSummary] = useState(null);
    const [salesReport, setSalesReport] = useState([]);
    const [hallLoad, setHallLoad] = useState([]);
    const [moviePopularity, setMoviePopularity] = useState([]);
    const [dateRange, setDateRange] = useState({
        startDate: new Date(new Date().setDate(1)).toISOString(),
        endDate: new Date().toISOString()
    });

    const showMessage = (message, type = 'success') => {
        setPopup({ message, type, showConfirm: false });
    };

    const fetchSummary = async () => {
        try {
            const response = await fetch(`${API_BASE_URL}/reports/summary`, {
                credentials: 'include'
            });
            const data = await response.json();
            if (!response.ok) throw new Error(data?.message);
            setSummary(data);
        } catch (error) {
            console.error('Ошибка:', error);
            showMessage(error.message, 'error');
        }
    };

    const fetchSalesReport = async () => {
        setLoading(true);
        try {
            const response = await fetch(
                `${API_BASE_URL}/reports/sales?startDate=${dateRange.startDate}&endDate=${dateRange.endDate}`,
                { credentials: 'include' }
            );
            const data = await response.json();
            if (!response.ok) throw new Error(data?.message);
            setSalesReport(data);
        } catch (error) {
            console.error('Ошибка:', error);
            showMessage(error.message, 'error');
        } finally {
            setLoading(false);
        }
    };

    const fetchHallLoadReport = async () => {
        setLoading(true);
        try {
            const response = await fetch(
                `${API_BASE_URL}/reports/hall-load?startDate=${dateRange.startDate}&endDate=${dateRange.endDate}`,
                { credentials: 'include' }
            );
            const data = await response.json();
            if (!response.ok) throw new Error(data?.message);
            setHallLoad(data);
        } catch (error) {
            console.error('Ошибка:', error);
            showMessage(error.message, 'error');
        } finally {
            setLoading(false);
        }
    };

    const fetchMoviePopularityReport = async () => {
        setLoading(true);
        try {
            const response = await fetch(
                `${API_BASE_URL}/reports/movie-popularity?startDate=${dateRange.startDate}&endDate=${dateRange.endDate}&limit=10`,
                { credentials: 'include' }
            );
            const data = await response.json();
            if (!response.ok) throw new Error(data?.message);
            setMoviePopularity(data);
        } catch (error) {
            console.error('Ошибка:', error);
            showMessage(error.message, 'error');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchSummary();
    }, []);

    useEffect(() => {
        if (activeTab === 'sales') fetchSalesReport();
        if (activeTab === 'hallLoad') fetchHallLoadReport();
        if (activeTab === 'moviePopularity') fetchMoviePopularityReport();
    }, [activeTab, dateRange]);

    return (
        <div className={classes.container}>
            <div className={classes.header}>
                <h2>Аналитика и отчёты</h2>
                <div className={classes.dateFilter}>
                    <input
                        type="date"
                        value={dateRange.startDate}
                        onChange={(e) => setDateRange(prev => ({ ...prev, startDate: e.target.value }))}
                    />
                    <span>—</span>
                    <input
                        type="date"
                        value={dateRange.endDate}
                        onChange={(e) => setDateRange(prev => ({ ...prev, endDate: e.target.value }))}
                    />
                </div>
            </div>

            <div className={classes.tabs}>
                <button 
                    className={`${classes.tab} ${activeTab === 'summary' ? classes.active : ''}`}
                    onClick={() => setActiveTab('summary')}
                >
                    Сводка
                </button>
                <button 
                    className={`${classes.tab} ${activeTab === 'sales' ? classes.active : ''}`}
                    onClick={() => setActiveTab('sales')}
                >
                    Продажи
                </button>
                <button 
                    className={`${classes.tab} ${activeTab === 'hallLoad' ? classes.active : ''}`}
                    onClick={() => setActiveTab('hallLoad')}
                >
                    Загрузка залов
                </button>
                <button 
                    className={`${classes.tab} ${activeTab === 'moviePopularity' ? classes.active : ''}`}
                    onClick={() => setActiveTab('moviePopularity')}
                >
                    Популярность фильмов
                </button>
            </div>

            {loading && (
                <div className={classes.loading}>
                    <div className={classes.spinner}></div>
                    <p>Загрузка данных...</p>
                </div>
            )}
            {activeTab === 'summary' && summary && (
                <div className={classes.summaryGrid}>
                    <div className={classes.summaryCard}>
                        <div className={classes.summaryValue}>{summary.totalIncome}</div>
                        <div className={classes.summaryLabel}>Общая выручка</div>
                    </div>
                    <div className={classes.summaryCard}>
                        <div className={classes.summaryValue}>{summary.totalTickets}</div>
                        <div className={classes.summaryLabel}>Продано билетов</div>
                    </div>
                    <div className={classes.summaryCard}>
                        <div className={classes.summaryValue}>{summary.totalUsers}</div>
                        <div className={classes.summaryLabel}>Пользователей</div>
                    </div>
                    <div className={classes.summaryCard}>
                        <div className={classes.summaryValue}>{summary.totalMovies}</div>
                        <div className={classes.summaryLabel}>Фильмов</div>
                    </div>
                    <div className={classes.summaryCard}>
                        <div className={classes.summaryValue}>{summary.activeSessions}</div>
                        <div className={classes.summaryLabel}>Активных сеансов</div>
                    </div>
                    {summary.popularMovie && (
                        <div className={classes.summaryCard}>
                            <div className={classes.summaryValue}>{summary.popularMovie.movieTitle}</div>
                            <div className={classes.summaryLabel}>Самый популярный фильм</div>
                            <div className={classes.summarySub}>{summary.popularMovie.tickets} билетов</div>
                        </div>
                    )}
                </div>
            )}
            {activeTab === 'sales' && (
                <div className={classes.tableWrapper}>
                    <table className={classes.table}>
                        <thead>
                            <tr>
                                <th>Дата</th>
                                <th>Бронирований</th>
                                <th>Билетов</th>
                                <th>Выручка</th>
                                <th>Средний чек</th>
                            </tr>
                        </thead>
                        <tbody>
                            {salesReport.map((item) => (
                                <tr key={item.date}>
                                    <td>{new Date(item.date).toLocaleDateString('ru-RU')}</td>
                                    <td>{item.totalBookings}</td>
                                    <td>{item.totalTickets}</td>
                                    <td>{item.totalIncome}</td>
                                    <td>{item.averageTicketPrice}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}
            {activeTab === 'hallLoad' && (
                <div className={classes.tableWrapper}>
                    <table className={classes.table}>
                        <thead>
                            <tr>
                                <th>Зал</th>
                                <th>Всего мест</th>
                                <th>Сеансов</th>
                                <th>Продано билетов</th>
                                <th>Загрузка</th>
                            </tr>
                        </thead>
                        <tbody>
                            {hallLoad.map((hall) => (
                                <tr key={hall.hallId}>
                                    <td>{hall.hallName}</td>
                                    <td>{hall.totalSeats}</td>
                                    <td>{hall.totalSessions}</td>
                                    <td>{hall.totalTicketsSold}</td>
                                    <td>
                                        <div className={classes.loadBar}>
                                            <div 
                                                className={classes.loadFill}
                                                style={{ width: `${hall.loadPercentage}%` }}
                                            />
                                            <span>{hall.loadPercentage}%</span>
                                        </div>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}
            {activeTab === 'moviePopularity' && (
                <div className={classes.movieGrid}>
                    {moviePopularity.map((movie, index) => (
                        <div key={movie.movieId} className={classes.movieCard}>
                            <div className={classes.movieRank}>#{index + 1}</div>
                            <div 
                                className={classes.moviePoster}
                                style={{ backgroundImage: `url("${IMAGE_URL}${movie.posterUrl}")` }}
                            />
                            <div className={classes.movieInfo}>
                                <h4>{movie.movieTitle}</h4>
                                <p>{movie.totalTickets} билетов</p>
                                <p>{movie.totalIncome}</p>
                                <p>{movie.totalSessions} сеансов</p>
                                <p>{movie.ticketsPerSession} билетов/сеанс</p>
                            </div>
                        </div>
                    ))}
                </div>
            )}
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