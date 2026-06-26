import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link, Navigate } from 'react-router-dom';
import { useAuth } from '../../../contexts/AuthContext';
import { useMovie } from '../../../contexts/movieContext';
import classes from './EditMovie.module.css';
import Popup from'../Popup/Popup';

const API_BASE_URL = import.meta.env.VITE_API_URL;
const IMAGE_URL = import.meta.env.VITE_URL;

export default function EditMovie() {
    const { id } = useParams();
    const navigate = useNavigate();
    const { isAdmin } = useAuth();
    const { fetchMovie } = useMovie();
    // const [isLoading, setIsLoading] = useState(true);
    const [uploadMethod, setUploadMethod] = useState('url');
    const [posterPreview, setPosterPreview] = useState(null);
    const [popup, setPopup] = useState(null);
    const [isLoading, setIsLoading] = useState(true);
    const [allGenres, setAllGenres] = useState([]);
    const [selectedGenres, setSelectedGenres] = useState([]);
    const [genreInput, setGenreInput] = useState('');
    const [filteredGenres, setFilteredGenres] = useState([]);
    const [showDropdown, setShowDropdown] = useState(false);
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

    if (!isAdmin) return <Navigate to="/" />;

    useEffect(() => {
        const loadMovie = async () => {
            try{
                const result = await fetchMovie(id);

                if (!result.success){
                    throw new Error(data?.error || 'Ошибка загрузки фильма');
                }

                const movie = result.data;
            
                setFormData({
                    title: movie.title || '',
                    originalTitle: movie.originalTitle || '',
                    posterUrl: movie.posterUrl || '',
                    posterFile: null,
                    year: movie.year || '',
                    age: movie.age || '',
                    duration: movie.duration || '',
                    description: movie.description || '',
                    trailerUrl: movie.trailerUrl || '',
                });
                
                if (movie.genres && Array.isArray(movie.genres)) {
                    setSelectedGenres(movie.genres.map(genre => genre.name));
                }

                setPosterPreview(`${IMAGE_URL}${movie.posterUrl}`);
            } catch (error) {
                console.error(error);
                showMessage(error.message, 'error');
                navigate('/admin');
            }
        };
    
        loadMovie();
    }, [id]);
    
    useEffect(() => {
        fetchGenres();
    }, [id])

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

    const showMessage = (message, type = 'success') => {
        setPopup({ message, type, showConfirm: false });
    };

    const handleChange = (e) => {
        const { name, value, files } = e.target;
        
        if (name === 'posterFile' && files && files[0]) {
            const file = files[0];
            setFormData(prev => ({ ...prev, posterFile: file }));
            
            const reader = new FileReader();
            reader.onloadend = () => {
                setPosterPreview(reader.result);
            };
            reader.readAsDataURL(file);
        } else {
            setFormData(prev => ({ ...prev, [name]: value }));
        }
    };

    const handleUploadMethodChange = (method) => {
        setUploadMethod(method);
        if (method === 'url') {
            setPosterPreview(null);
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        
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
            }
            
            const response = await fetch(`${API_BASE_URL}/movies/${id}`, {
                method: "PUT",
                body: submitData,
                credentials: 'include'
            });
            
            const data = await response.json();
            
            if (!response.ok) {
                throw new Error(data?.message ||
                                Object.values(data?.errors || {})[0] || 'Ошибка при добавлении фильма');
            }
            
            showMessage('Фильм успешно обновлен!', 'success');
            
            setTimeout(() => {
                navigate('/admin');
            }, 1500);
            
        } catch (error) {
            console.error(error);
            showMessage(error.message, 'error');
        }
    };

    return (
        <div className={classes.container}>
            <div className={classes.header}>
                <Link to="/admin" className={classes.back_btn}>
                    ← Назад
                </Link>
                <h2>Редактирование фильма</h2>
            </div>
            
            <div className={classes.content}>
                <div className={classes.preview_section}>
                    <h3>Превью постера</h3>
                    <div 
                        className={classes.poster_preview}
                        style={{ 
                            backgroundImage: posterPreview 
                                ? `url(${posterPreview})` 
                                : formData.posterUrl 
                                    ? `url(${IMAGE_URL}${formData.posterUrl})`
                                    : 'none',
                            backgroundSize: 'cover',
                            backgroundPosition: 'center'
                        }}
                    >
                        {!posterPreview && !formData.posterUrl && (
                            <span>Нет изображения</span>
                        )}
                    </div>
                </div>
                
                <div className={classes.form_section}>
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
                            Загрузить новый файл
                        </label>
                    </div>
                    
                    <form onSubmit={handleSubmit} className={classes.form}>
                        <input 
                            name="title" 
                            value={formData.title} 
                            onChange={handleChange} 
                            type="text" 
                            placeholder="Название фильма"
                            required
                        />
                        <input 
                            name="originalTitle" 
                            value={formData.originalTitle} 
                            onChange={handleChange} 
                            type="text" 
                            placeholder="Оригинальное название"
                            required
                        />
                        
                        {uploadMethod === 'url' ? (
                            <input 
                                name="posterUrl" 
                                value={formData.posterUrl} 
                                onChange={handleChange} 
                                type="text" 
                                placeholder="URL постера"
                            />
                        ) : (
                            <input 
                                name="posterFile" 
                                onChange={handleChange} 
                                type="file" 
                                accept="image/*"
                            />
                        )}
                        
                        <div className={classes.row}>
                            <input 
                                name="year" 
                                value={formData.year} 
                                onChange={handleChange} 
                                type="number" 
                                placeholder="Год"
                                required
                            />
                            <input 
                                name="age" 
                                value={formData.age} 
                                onChange={handleChange} 
                                type="number" 
                                min="0" 
                                placeholder="Возраст"
                                required
                            />
                            <input 
                                name="duration" 
                                value={formData.duration} 
                                onChange={handleChange} 
                                type="number" 
                                placeholder="Длительность (мин)"
                                required
                            />
                        </div>
                        
                        <textarea 
                            name="description" 
                            value={formData.description} 
                            onChange={handleChange} 
                            placeholder="Описание фильма"
                            rows="5"
                            required
                        />
                        
                        <input 
                            name="trailerUrl" 
                            value={formData.trailerUrl} 
                            onChange={handleChange} 
                            type="text" 
                            placeholder="URL трейлера"
                            required
                        />
                        
                        <div className={classes.buttons}>
                            <Link to="/admin" className={classes.cancel_btn}>
                                Отмена
                            </Link>
                            <button type="submit" className={classes.save_btn}>
                                Сохранить изменения
                            </button>
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
                    onConfirm={null}
                    onCancel={null}
                />
            )}
        </div>
    );
}