import classes from './MovieManagment.module.css';
import { useState, useEffect } from 'react';
import { useMovie } from '../../../contexts/movieContext';
import { useAuth } from '../../../contexts/AuthContext';
import { Navigate, Link } from 'react-router-dom';
import Popup from '../Popup/Popup';

const API_BASE_URL = import.meta.env.VITE_API_URL;
const IMAGE_URL = import.meta.env.VITE_URL;

export default function MovieManagment() {
    const { fetchMovies, movies, isLoading, updateMoviesState } = useMovie();
    const { isAdmin } = useAuth();
    const [error, setError] = useState('');
    const [uploadMethod, setUploadMethod] = useState('url');
    const [allGenres, setAllGenres] = useState([]);
    const [selectedGenres, setSelectedGenres] = useState([]);
    const [genreInput, setGenreInput] = useState('');
    const [filteredGenres, setFilteredGenres] = useState([]);
    const [formData, setFormData] = useState({
        title: '',
        originalTitle: '',
        posterUrl: '',
        posterFile: null,
        year: '',
        age: '',
        duration: '',
        description: '',
        trailerUrl: '',
    });
    const [popup, setPopup] = useState(null);
    const [deleteTarget, setDeleteTarget] = useState(null);
    const [showDropdown, setShowDropdown] = useState(false);

    if (!isAdmin) return <Navigate to="/" />;

    const showMessage = (message, type = 'success') => {
        setPopup({ message, type, showConfirm: false });
    };

    const showConfirmDelete = (id, title) => {
        setDeleteTarget({ id, title });
        setPopup({
            message: `Вы уверены, что хотите удалить фильм "${title}"?`,
            type: 'confirm',
            showConfirm: true
        });
    };

    useEffect(() => {
        fetchMovies();
        fetchGenres();
    }, []);

    const fetchGenres = async () => {
        try {
            const response = await fetch(`${API_BASE_URL}/movies/genres`, {
                method: "GET",
                credentials: 'include'
            });
            const data = await response.json();
            if (response.ok) {
                setAllGenres(data);
            }
        } catch (error) {
            console.error('Error fetching genres:', error);
        }
    };

    useEffect(() => {
        if (genreInput.trim()) {
            const filtered = allGenres.filter(genre =>
                genre.name.toLowerCase().includes(genreInput.toLowerCase()) &&
                !selectedGenres.includes(genre.name)
            );
            setFilteredGenres(filtered);
            setShowDropdown(true);
        } else {
            setFilteredGenres([]);
            setShowDropdown(false);
        }
    }, [genreInput, allGenres, selectedGenres]);

    const handleChange = (e) => {
        const { name, value, files } = e.target;
        
        if (name === 'posterFile' && files && files[0]) {
            const file = files[0];
            setFormData(prev => ({ ...prev, posterFile: file }));
        } else {
            setFormData(prev => ({ ...prev, [name]: value }));
        }
    };

    const handleUploadMethodChange = (method) => {
        setUploadMethod(method);
    };

    const handleCancelDelete = () => {
        setDeleteTarget(null);
        setPopup(null);
    };

    const handleSelectGenre = (genreName) => {
        if (!selectedGenres.includes(genreName)) {
            setSelectedGenres([...selectedGenres, genreName]);
        }
        setGenreInput('');
        setShowDropdown(false);
    };

    const handleRemoveGenre = (genreName) => {
        setSelectedGenres(selectedGenres.filter(name => name !== genreName));
    };

    const handleAddNewGenre = () => {
        if (genreInput.trim() && !selectedGenres.includes(genreInput.trim())) {
            const existingGenre = allGenres.find(g => 
                g.name.toLowerCase() === genreInput.trim().toLowerCase()
            );
            
            if (existingGenre) {
                if (!selectedGenres.includes(existingGenre.name)) {
                    setSelectedGenres([...selectedGenres, existingGenre.name]);
                }
            } else {
                setSelectedGenres([...selectedGenres, genreInput.trim()]);
            }
            setGenreInput('');
            setShowDropdown(false);
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');

        if (selectedGenres.length === 0) {
            showMessage('Выберите или добавьте хотя бы один жанр', 'error');
            return;
        }

        try {
            const submitData = new FormData();
            
            submitData.append('title', formData.title);
            submitData.append('originalTitle', formData.originalTitle);
            submitData.append('year', formData.year);
            submitData.append('age', formData.age);
            submitData.append('duration', formData.duration);
            submitData.append('description', formData.description);
            submitData.append('trailerUrl', formData.trailerUrl);
            
            selectedGenres.forEach((name) => submitData.append('genres', name));

            if (uploadMethod === 'url') {
                submitData.append('posterUrl', formData.posterUrl);
            } else if (uploadMethod === 'file' && formData.posterFile) {
                submitData.append('posterFile', formData.posterFile);
            } else {
                throw new Error('Загрузите постер (URL или файл)');
            }
            
            const response = await fetch(`${API_BASE_URL}/movies`, {
                method: "POST",
                body: submitData,
                credentials: 'include'
            });
            
            const data = await response.json();

            if (!response.ok) {
                throw new Error(data?.message ||
                                Object.values(data?.errors || {})[0] || 'Ошибка при добавлении фильма');
            }
            
            setFormData({
                title: '',
                originalTitle: '',
                posterUrl: '',
                posterFile: null,
                year: '',
                age: '',
                duration: '',
                description: '',
                trailerUrl: '',
            });
            setSelectedGenres([]);
            setGenreInput('');
            setUploadMethod('url');
            
            await fetchMovies();
            await fetchGenres();
            
            showMessage('Фильм успешно добавлен!');
            e.target.reset();
            
        } catch (error) {
            console.error(error);
            setError(error.message);
            showMessage(
                error.message,
                'error'
            );
        }
    };

    const handleDelete  = async (id) => {
        if (!deleteTarget) return;
        
        try {
            const response = await fetch(`${API_BASE_URL}/movies/${deleteTarget.id}`, {
                method: "DELETE",
                headers: { 'Content-Type': 'application/json' },
                credentials: 'include'
            });
            
            const data = await response.json();
            
            if (!response.ok) {
                throw new Error(data?.message || 'Ошибка при удалении фильма');
            }
            
            showMessage('Фильм успешно удален!');
            console.log("ta");
            updateMoviesState(movies.filter(m => m.id !== deleteTarget.id));
        } catch (error) {
            console.error(error);
            setError(error.message);
            showMessage(
                error.message,
                'error'
            );

        }

        setDeleteTarget(null);
        setPopup(null);
    };

    if (isLoading) {
        return (
            <div className={classes.loading}>
                <div className={classes.spinner}></div>
                <p>Загрузка...</p>
            </div>
        );
    }

    return (
        <>
            <h2>Управление фильмами</h2>
            
            {error && <div className={classes.error}>{error}</div>}
            
            <div className={classes.manage_section}>
                <div className={classes.movies_list}>
                    <h3>Существующие фильмы</h3>
                    <div className={classes.movies}>
                    {movies.map((el) => (
                        <div key={el.id} className={classes.list_item}
                            style={{ backgroundImage: `url("${IMAGE_URL}${el.posterUrl}")` }}>
                            <h2>{el.title}</h2>
                            <Link to={`/admin/movies/edit/${el.id}`} className={classes.edit_btn}>
                                Редактировать
                            </Link>
                            <button className={classes.delete_btn} onClick={() => showConfirmDelete(el.id, el.title)}>
                                Удалить
                            </button>
                        </div>
                    ))}
                    </div>
                </div>
                <div className={classes.add_movie_section}>
                    <h3>Добавление фильма</h3>
                    <div className={classes.genres_section}>
                        <label className={classes.genres_label}>Жанры:</label>
                        
                        {selectedGenres.length > 0 && (
                            <div className={classes.selected_genres}>
                                {selectedGenres.map(name => (
                                    <span key={name} className={classes.genre_tag}>
                                        {name}
                                        <button 
                                            type="button" 
                                            className={classes.remove_genre}
                                            onClick={() => handleRemoveGenre(name)}
                                        >
                                            x
                                        </button>
                                    </span>
                                ))}
                            </div>
                        )}
                        
                        <div className={classes.genre_input_container}>
                            <input
                                type="text"
                                placeholder="Введите название жанра..."
                                value={genreInput}
                                onChange={(e) => setGenreInput(e.target.value)}
                                onFocus={() => genreInput.trim() && setShowDropdown(true)}
                                className={classes.genre_input}
                            />
                            <button 
                                type="button" 
                                onClick={handleAddNewGenre}
                                className={classes.add_genre_btn}
                            >
                                Добавить
                            </button>
                        </div>
                        
                        {showDropdown && filteredGenres.length > 0 && (
                            <div className={classes.genre_dropdown}>
                                {filteredGenres.map(genre => (
                                    <div
                                        key={genre.id}
                                        className={classes.genre_option}
                                        onClick={() => handleSelectGenre(genre.name)}
                                    >
                                        {genre.name}
                                    </div>
                                ))}
                            </div>
                        )}
                    </div>
                    <div className={classes.upload_method}>
                        <label>
                            <input 
                                type="radio" 
                                value="url" 
                                checked={uploadMethod === 'url'} 
                                onChange={() => handleUploadMethodChange('url')}
                            />
                            По URL
                        </label>
                        <label>
                            <input 
                                type="radio" 
                                value="file" 
                                checked={uploadMethod === 'file'} 
                                onChange={() => handleUploadMethodChange('file')}
                            />
                            Загрузить файл
                        </label>
                    </div>
                    
                    <form id='form_add_movie' className={classes.form_add_movie} onSubmit={handleSubmit}>
                        <input 
                            name="title" 
                            value={formData.title} 
                            onChange={handleChange} 
                            type="text" 
                            placeholder='Введите название фильма' 
                            required
                        />
                        <input 
                            name="originalTitle" 
                            value={formData.originalTitle} 
                            onChange={handleChange} 
                            type="text"
                            required
                            placeholder='Введите оригинальное название фильма' 
                        />
                        
                        {uploadMethod === 'url' ? (
                            <input 
                                name="posterUrl" 
                                value={formData.posterUrl} 
                                onChange={handleChange} 
                                type="text" 
                                placeholder='Введите URL адрес постера' 
                            />
                        ) : (
                            <input 
                                name="posterFile" 
                                onChange={handleChange} 
                                type="file" 
                                accept="image/*"
                                className={classes.file_input}
                            />
                        )}
                        
                        <input 
                            name="year" 
                            value={formData.year} 
                            onChange={handleChange} 
                            type="number" 
                            placeholder='Введите год фильма' 
                            required
                        />
                        <input 
                            name="age" 
                            value={formData.age} 
                            onChange={handleChange} 
                            type="number" 
                            min="0" 
                            placeholder='Укажите возрастное ограничение'
                            required
                        />
                        <input 
                            name="duration" 
                            value={formData.duration} 
                            onChange={handleChange} 
                            type="number" 
                            placeholder='Введите продолжительность фильма (мин)' 
                            required
                        />
                        <textarea 
                            name="description" 
                            value={formData.description} 
                            onChange={handleChange} 
                            placeholder='Введите описание фильма' 
                            rows="4"
                            required
                        />
                        <input 
                            name="trailerUrl" 
                            value={formData.trailerUrl} 
                            onChange={handleChange} 
                            type="text" 
                            placeholder='Укажите URL трейлера (YouTube, RuTube)' 
                            required
                        />
                    </form>
                    <button form='form_add_movie' type='submit' className={classes.add_btn}>
                        Добавить фильм
                    </button>
                </div>
            </div>
            {popup && (
                <Popup
                    message={popup.message}
                    type={popup.type}
                    showConfirm={popup.showConfirm}
                    onClose={() => setPopup(null)}
                    onConfirm={handleDelete}
                    onCancel={handleCancelDelete}
                />
            )}
        </>
    );
}