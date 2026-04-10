import { useContext, createContext, useState } from "react";

const API_BASE_URL = 'http://localhost:5014/api/';
const MovieContext = createContext();

export const useMovie = () => {
    return useContext(MovieContext);
}

export const MovieProvider = ({children}) => {
    const [movie, setMovie] = useState(null);
    const [movies, setMovies] = useState([]);
    const [isLoading, setIsLoading] = useState(true);

    const fetchMovies = async () => {
        setIsLoading(true);
        try{
            const response = await fetch(`${API_BASE_URL}movies`, {
                method: "GET",
            });

            const data = await response.json();

            if(!response.ok)
                throw new Error(data?.message
                                || "Ошибка получения фильмов");
            
            setMovies(data.movies);
            return {success: true, data: data.movies}
        }
        catch (error) {
            return {success: false, error: error.message};
        }   
        finally{
            setIsLoading(false);
        }
    }

    const fetchMovie = async (id) => {
        setIsLoading(true);
        try{
            const response = await fetch(`${API_BASE_URL}movies/${id}`, {
                method: "GET",
            });

            const data = await response.json();

            if(!response.ok)
                throw new Error(data?.message
                                || "Ошибка получения фильма");
            
            setMovie(data.movie);
            return {success: true, data: data.movie}
        }
        catch (error) {
            return {success: false, error: error.message};
        }   
        finally{
            setIsLoading(false);
        }
    }

    const value = {
        isLoading,
        movies,
        movie,

        fetchMovies,
        fetchMovie,
    }
    return(<>
        <MovieContext.Provider value={value}>
            {children}
        </MovieContext.Provider>
    </>);
}