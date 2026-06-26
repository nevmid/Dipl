import { useEffect, useState } from 'react';
import classes from './HallManagment.module.css';
import Popup from '../Popup/Popup';

const API_BASE_URL = import.meta.env.VITE_API_URL;

export default function HallManagment() {
    const [halls, setHalls] = useState([]);
    const [loading, setLoading] = useState(true);
    const [popup, setPopup] = useState(null);
    const [editingHall, setEditingHall] = useState(null);
    const [formData, setFormData] = useState({
        name: '',
        rows: '',
        cols: ''
    });

    const fetchHalls = async () => {
        setLoading(true);
        try {
            const response = await fetch(`${API_BASE_URL}/halls`, {
                credentials: 'include'
            });
            const data = await response.json();
            
            if (!response.ok) {
                throw new Error(data?.message || 'Ошибка загрузки залов');
            }
            
            setHalls(data.halls || []);
        } catch (error) {
            console.error('Ошибка:', error);
            showMessage(error.message, 'error');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchHalls();
    }, []);

    const showMessage = (message, type = 'success') => {
        setPopup({ message, type, showConfirm: false });
    };

    const showConfirm = (message, onConfirm) => {
        setPopup({ message, type: 'confirm', showConfirm: true, onConfirm });
    };

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        
        if (!formData.name.trim()) {
            showMessage('Введите название зала', 'error');
            return;
        }
        
        const rows = parseInt(formData.rows);
        const cols = parseInt(formData.cols);
        
        if (!rows || rows < 1 || rows > 20) {
            showMessage('Количество рядов должно быть от 1 до 20', 'error');
            return;
        }
        
        if (!cols || cols < 1 || cols > 20) {
            showMessage('Количество мест в ряду должно быть от 1 до 20', 'error');
            return;
        }
        
        const totalSeats = rows * cols;
        
        try {
            const url = editingHall 
                ? `${API_BASE_URL}/halls/${editingHall.id}`
                : `${API_BASE_URL}/halls`;
            
            const body = {
                name: formData.name,
                description: formData.description || '',
                rowNum: rows,
                colNum: cols
            };
            
            const response = await fetch(url, {
                method: editingHall ? 'PUT' : 'POST',
                headers: { 'Content-Type': 'application/json' },
                credentials: 'include',
                body: JSON.stringify(body)
            });
            
            const data = await response.json();
            
            if (!response.ok) {
                throw new Error(data?.message || Object.values(data?.errors || {})[0] || 'Ошибка при сохранении зала');
            }
            
            showMessage(editingHall ? 'Зал обновлен' : 'Зал добавлен', 'success');
            setFormData({ name: '', description: '', rows: '', cols: '' });
            setEditingHall(null);
            fetchHalls();
            
        } catch (error) {
            console.error('Ошибка:', error);
            showMessage(error.message, 'error');
        }
    };

    const handleEdit = (hall) => {
        setEditingHall(hall);
        setFormData({
            name: hall.name,
            description: hall.description || '',
            rows: hall.rows?.toString() || '',
            cols: hall.cols?.toString() || ''
        });
    };

    const handleCancelEdit = () => {
        setEditingHall(null);
        setFormData({ name: '', description: '', rows: '', cols: '' });
    };

    const handleDelete = (hall) => {        
        showConfirm(
            `Вы уверены, что хотите удалить зал "${hall.name}"?`,
            async () => {
                try {
                    const response = await fetch(`${API_BASE_URL}/halls/${hall.id}`, {
                        method: 'DELETE',
                        credentials: 'include'
                    });
                    
                    const data = await response.json();
                    
                    if (!response.ok) {
                        throw new Error(data?.message || 'Ошибка при удалении зала');
                    }
                    
                    showMessage('Зал удален', 'success');
                    fetchHalls();
                    
                } catch (error) {
                    console.error('Ошибка:', error);
                    showMessage(error.message, 'error');
                }
            }
        );
    };

    const getTotalSeats = (rows, cols) => {
        if (!rows || !cols) return 0;
        return rows * cols;
    };

    if (loading) {
        return (
            <div className={classes.loading}>
                <div className={classes.spinner}></div>
                <p>Загрузка залов...</p>
            </div>
        );
    }

    return (
        <div className={classes.container}>
            <div className={classes.header}>
                <h3>Управление залами</h3>
                <div className={classes.stats}>
                    Всего залов: {halls.length}
                </div>
            </div>

            <div className={classes.content}>
                <div className={classes.form_section}>
                    <h4>{editingHall ? '✏️ Редактирование зала' : '➕ Добавление зала'}</h4>
                    <form onSubmit={handleSubmit} className={classes.form}>
                        <input
                            type="text"
                            name="name"
                            value={formData.name}
                            onChange={handleChange}
                            placeholder="Название зала (например: Зал 1, IMAX, VIP)"
                            required
                        />
                        
                        <div className={classes.inputGroup}>
                            <label>Количество рядов</label>
                            <input
                                type="number"
                                name="rows"
                                value={formData.rows}
                                onChange={handleChange}
                                placeholder="Ряды"
                                min="1"
                                max="20"
                                required
                            />
                        </div>
                        <div className={classes.inputGroup}>
                            <label>Мест в ряду</label>
                            <input
                                type="number"
                                name="cols"
                                value={formData.cols}
                                onChange={handleChange}
                                placeholder="Мест в ряду"
                                min="1"
                                max="20"
                                required
                            />
                        </div>
                        
                        {formData.rows && formData.cols && (
                            <div className={classes.info_text}>
                                Всего мест: {getTotalSeats(parseInt(formData.rows), parseInt(formData.cols))}
                            </div>
                        )}
                        
                        <div className={classes.form_buttons}>
                            <button type="submit" className={classes.save_btn}>
                                {editingHall ? '💾 Сохранить' : '➕ Добавить'}
                            </button>
                            {editingHall && (
                                <button type="button" className={classes.cancel_btn} onClick={handleCancelEdit}>
                                    ❌ Отмена
                                </button>
                            )}
                        </div>
                    </form>
                </div>
                <div className={classes.halls_section}>
                    <h4>Список залов</h4>
                    <div className={classes.halls_grid}>
                        {halls.length === 0 ? (
                            <div className={classes.empty}>
                                <p>Нет добавленных залов</p>
                                <p>Добавьте первый зал с помощью формы</p>
                            </div>
                        ) : (
                            halls.map((hall) => (
                                <div key={hall.id} className={classes.hall_card}>
                                    <div className={classes.hall_header}>
                                        <h5>{hall.name}</h5>
                                        <div className={classes.hall_actions}>
                                            <button 
                                                className={classes.edit_btn}
                                                onClick={() => handleEdit(hall)}
                                            >
                                                ✏️
                                            </button>
                                            <button 
                                                className={classes.delete_btn}
                                                onClick={() => handleDelete(hall)}
                                                disabled={hall.sessionsCount > 0}
                                                title={hall.sessionsCount > 0 ? "Нельзя удалить зал с сеансами" : ""}
                                            >
                                                🗑️
                                            </button>
                                        </div>
                                    </div>
                                    <div className={classes.hall_info}>
                                        <div className={classes.info_item}>
                                            <span>Схема:</span>
                                            <strong>{hall.rows} × {hall.cols}</strong>
                                        </div>
                                        <div className={classes.info_item}>
                                            <span>Всего мест:</span>
                                            <strong>{getTotalSeats(hall.rows, hall.cols)}</strong>
                                        </div>
                                        {hall.sessionsCount !== undefined && (
                                            <div className={classes.info_item}>
                                                <span>Сеансов:</span>
                                                <strong>{hall.sessionsCount}</strong>
                                            </div>
                                        )}
                                    </div>
                                </div>
                            ))
                        )}
                    </div>
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