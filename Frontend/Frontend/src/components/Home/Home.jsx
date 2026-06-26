import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../../contexts/AuthContext';
import classes from './Home.module.css';

const API_BASE_URL = import.meta.env.VITE_API_URL;
const IMAGE_URL = import.meta.env.VITE_URL;

export default function Home() {
    const { isAuthenticated } = useAuth();
    const [recommendations, setRecommendations] = useState(null);
    const [todayMovies, setTodayMovies] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchRecommendations = async () => {
            try {
                const response = await fetch(`${API_BASE_URL}/recommendations/for-user`, {
                    credentials: 'include'
                });

                const data = await response.json();

                if (response.ok) {
                    setRecommendations(data);
                }
            } catch (error) {
                console.error('Error fetching recommendations:', error);
            }
        };

        fetchRecommendations();
    }, [isAuthenticated]);

    useEffect(() => {
        const fetchTodayMovies = async () => {
            try {
                const today = new Date().toISOString().split('T')[0];
                const response = await fetch(`${API_BASE_URL}/sessions?date=${today}`, {
                    credentials: 'include'
                });
                const data = await response.json();
                if (response.ok) {
                    const uniqueMovies = {};
                    (data.sessions || []).forEach(session => {
                        if (!uniqueMovies[session.movieId]) {
                            uniqueMovies[session.movieId] = session.movie;
                        }
                    });
                    setTodayMovies(Object.values(uniqueMovies));
                }
            } catch (error) {
                console.error('Error fetching today movies:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchTodayMovies();
    }, []);

    const renderMovieCard = (movie) => (
        <Link to={`/movies/${movie.id}`} key={movie.id} className={classes.movie_card}>
            <div 
                className={classes.movie_poster}
                style={{ backgroundImage: `url("${IMAGE_URL}${movie.posterUrl}")` }}
            >
            </div>
            <div className={classes.movie_info}>
                <h4>{movie.title}</h4>
                <p>{movie.year} год</p>
            </div>
        </Link>
    );

    if (loading) {
        return (
            <div className={classes.loading}>
                <div className={classes.spinner}></div>
                <p>Загрузка...</p>
            </div>
        );
    }

    console.log(recommendations)

    return (
        <div className={classes.container}>
            <div className={classes.hero}>
                <div className={classes.hero_content}>
                    <h1>Space Cinema</h1>
                    <p>Кинотеатр с космической атмосферой</p>
                    <Link to="/movies" className={classes.hero_btn}>В афишу</Link>
                </div>
            </div>
            {todayMovies.length > 0 && (
                <div className={classes.section}>
                    <h2>Сегодня в кино</h2>
                    <div className={classes.movies_grid}>
                        {todayMovies.map(renderMovieCard)}
                    </div>
                </div>
            )}
            {isAuthenticated && recommendations && (
                <div className={classes.section}>
                    {recommendations.personalized?.length > 0 && (
                        <>
                            <h2>Возможно, вам понравится</h2>
                            <div className={classes.movies_grid}>
                                {recommendations.personalized.map(renderMovieCard)}
                            </div>
                        </>
                    )}
                    {recommendations.collaborative?.length > 0 && (
                        <>
                            <h2>Пользователи также смотрят</h2>
                            <div className={classes.movies_grid}>
                                {recommendations.collaborative.map(renderMovieCard)}
                            </div>
                        </>
                    )}
                    {recommendations.trending?.length > 0 && (
                        <>
                            <h2>Популярное сейчас</h2>
                            <div className={classes.movies_grid}>
                                {recommendations.trending.map(renderMovieCard)}
                            </div>
                        </>
                    )}
                </div>
            )}
            {!isAuthenticated && recommendations?.trending?.length > 0 && (
                <div className={classes.section}>
                    <h2>Популярные фильмы</h2>
                    <div className={classes.movies_grid}>
                        {recommendations.trending.map(renderMovieCard)}
                    </div>
                </div>
            )}
        </div>
    );
}