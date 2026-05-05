import classes from './MovieInfo.module.css';
import { Link, useParams } from 'react-router-dom';
import { useMovie } from '../../../contexts/movieContext';
import { useEffect} from 'react';

export default function MovieInfo (){
    const {isLoading, fetchMovie, movie} = useMovie();
    const { id } = useParams(); 

    useEffect(() => {
        fetchMovie(id);
    }, [id]);

    if (isLoading) {
        return (
            <div className={classes.loading}>
                <div className={classes.spinner}></div>
                <p>Загрузка фильма...</p>
            </div>
        );
    }

    if (!movie) {
        return (
            <div className={classes.error}>
                <p>Фильм не найден</p>
                <Link to="/movies" className={classes.btn_link}>
                    ← Вернуться к списку
                </Link>
            </div>
        );
    }

        const getEmbedUrl = (url) => {
        if (!url) return null;
        
        if (url.includes('youtube.com/watch?v=')) {
            const videoId = url.split('v=')[1].split('&')[0];
            return `https://www.youtube.com/embed/${videoId}`;
        }
        
        if (url.includes('youtu.be/')) {
            const videoId = url.split('youtu.be/')[1].split('?')[0];
            return `https://www.youtube.com/embed/${videoId}`;
        }

        if (url.includes('rutube.ru/video/')){
            const videoId = url.split('rutube.ru/video/')[1];
            return `https://rutube.ru/play/embed/${videoId}`
        }
        
        return url;
    };

    const embedUrl = movie?.trailerUrl ? getEmbedUrl(movie.trailerUrl) : null;

    return(
    <div className={classes.container}>
        <div className={classes.btn_back}>
            <Link to="/movies" className={classes.btn_link}>← Назад</Link>
        </div>

        <div className={classes.content}>
            <div className={classes.poster_section}>
                <div 
                    className={classes.poster}
                    style={{ backgroundImage: `url("http://localhost:5014${movie.posterUrl}")` }}
                >
                    {/* <h2>{movie.title}</h2>
                    <h4>{movie.year}</h4> */}
                </div>
            </div>

            <div className={classes.info_section}>
                <div className={classes.title}>
                    <span>{movie.title} ({movie.year})</span>
                </div>
                <div className={classes.orig_title}>
                    <span>{movie.originalTitle}</span>
                </div>
                <div className={classes.description}>
                    <h2>Описание</h2>
                    <p>{movie.description}</p>
                </div>

                <div className={classes.trailer}>
                    <h2>Трейлер</h2>
                    {embedUrl ? (
                        <iframe
                            src={embedUrl}
                            title="Трейлер"
                            allowFullScreen
                        />
                    ) : (
                        <div className={classes.trailer_placeholder}>
                            Трейлер скоро появится
                        </div>
                    )}
                </div>

                <div className={classes.main_info}>
                    <h2>Основная информация</h2>
                    <ul className={classes.list}>
                        <li>
                            <span className={classes.info_label}>Оригинальное название:</span>
                            <span className={classes.info_value}>{movie.originalTitle}</span>
                        </li>
                        <li>
                            <span className={classes.info_label}>Год:</span>
                            <span className={classes.info_value}>{movie.year}</span>
                        </li>
                        <li>
                            <span className={classes.info_label}>Длительность:</span>
                            <span className={classes.info_value}>{movie.duration} мин.</span>
                        </li>
                        <li>
                            <span className={classes.info_label}>Рейтинг:</span>
                            <span className={classes.info_value}>⭐ {movie.rating}/10</span>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    </div>)
}