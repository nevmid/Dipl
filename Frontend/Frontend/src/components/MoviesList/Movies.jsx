import { useMovie } from "../../../contexts/movieContext";
import {useEffect} from 'react';
import {Link} from 'react-router-dom';
import classes from './Movies.module.css';

const IMAGE_URL = import.meta.env.VITE_URL;

export default function MoviesList(){
    const {movies, fetchMovies, isLoading} = useMovie();

    useEffect(() => {
        fetchMovies();
    }, []);

    if (isLoading) {
        return (
            <div className={classes.loading}>
                <div className={classes.spinner}></div>
                <p>Загрузка...</p>
            </div>
        );
    }

    return(
    <>
        <div className={classes.main}>
            <div className={classes.main_list}>
                <h2>Фильмы в прокате</h2>
                <div className={classes.list}>
                    {movies.map((el) => {
                        return(
                            <div key={el.id} className={classes.list_item}
                                style={{ backgroundImage: `url("${IMAGE_URL}${el.posterUrl}")` }}>
                                <h2>{el.title}</h2>
                                <h4>{el.year}</h4>
                                <Link to={`/movies/${el.id}`} className={classes.info}>
                                    Подробнее
                                </Link>
                            </div>
                        );
                    })}
                </div>
            </div>
        </div>
    </>);
}