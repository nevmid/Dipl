import { useEffect, useState } from "react"
import classes from './UserManagment.module.css'

const API_BASE_URL = import.meta.env.VITE_API_URL;

export default function UserManagment() {
    const [users, setUsers] = useState([])
    const [loading, setLoading] = useState(true)
    const [message, setMessage] = useState("")
    const [searchTerm, setSearchTerm] = useState("")
    const [searchResults, setSearchResults] = useState([])
    const [isSearching, setIsSearching] = useState(false)
    const [pagination, setPagination] = useState({
        currentPage: 1,
        hasMore: false,
        pageSize: 10,
        totalCount: 0
    })

    const fetchUsers = async (page = 1) => {
        setLoading(true);
        try {
            const response = await fetch(`${API_BASE_URL}/users?page=${page}&pageSize=${pagination.pageSize}`, {
                credentials: 'include'
            });
            const data = await response.json();
            
            if (!response.ok){
                throw new Error(data?.message || "Ошибка при загрузке пользователей")
            }

            if (page === 1) {
                setUsers(data.users || [])
            } else {
                setUsers(prev => [...prev, ...(data.users || [])])
            }
            
            setPagination({
                currentPage: data.pagination?.currentPage || page,
                hasMore: data.pagination?.hasMore || false,
                pageSize: data.pagination?.pageSize || 10,
                totalCount: data.pagination?.totalCount || 0
            });
        } catch (error) {
            console.error("Ошибка:", error);
            setMessage(`Ошибка при загрузке пользователей ${error}`);
        } finally {
            setLoading(false);
        }
    }

    const searchUsers = async () => {
        if (!searchTerm.trim()) {
            setIsSearching(false);
            fetchUsers(1);
            return;
        }

        setIsSearching(true);
        setLoading(true);
        try {
            const response = await fetch(`${API_BASE_URL}/users/admin/users?search=${encodeURIComponent(searchTerm)}&limit=20`, {
                credentials: 'include'
            })

            const data = await response.json();
            setSearchResults(data);
        } catch (error) {
            console.error("Ошибка поиска:", error);
            setMessage(`Ошибка при поиске пользователей: ${error}`);
        } finally {
            setLoading(false);
        }
    }

    useEffect(() => {
        const timeoutId = setTimeout(() => {
            if (searchTerm) {
                searchUsers();
            } else if (isSearching) {
                setIsSearching(false);
                fetchUsers(1);
            }
        }, 500)
        return () => clearTimeout(timeoutId);
    }, [searchTerm])

    useEffect(() => {
        if (!isSearching) {
            fetchUsers(1);
        }
    }, [])

    const handleRoleChanged = async (userId, newRole) => {
        setLoading(true);
        
        try {
            const response = await fetch(`${API_BASE_URL}/users/${userId}/role`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include',
                body: JSON.stringify({
                    role: newRole
                })
            })

            const data = await response.json()

            if (response.ok) {
                setMessage(`Роль пользователя ${data.email} обновлена на ${newRole}`)
                
                if (isSearching) {
                    setSearchResults(prev => 
                        prev.map(el => el.id === userId ? {...el, role: newRole} : el)
                    );
                } else {
                    setUsers(prev =>
                        prev.map(el => el.id === userId ? {...el, role: newRole} : el)
                    );
                }
                
                setTimeout(() => setMessage(""), 3000);
            } else {
                setMessage(`Ошибка: ${data?.message || 
                         Object.values(data?.errors || {})[0]|| 'Не удалось обновить роль'}`)
            }
        } catch (error) {
            console.error('Ошибка:', error);
            setMessage(error);
        } finally {
            setLoading(false);
        }
    }

    const loadMore = () => {
        if (pagination.hasMore && !loading && !isSearching) {
            fetchUsers(pagination.currentPage + 1)
        }
    }

    const getRoleDisplay = (role) => {
        switch(role) {
            case 'admin':
                return 'Администратор'
            case 'staff':
                return 'Сотрудник'
            case 'user':
                return 'Пользователь'
            default:
                return role
        }
    }

    const getRoleClass = (role) => {
        switch(role) {
            case 'admin':
                return classes.admin
            case 'staff':
                return classes.staff
            case 'user':
                return classes.user
            default:
                return classes.tuser
        }
    }

    const displayUsers = isSearching ? searchResults : users

    return (
        <div className={classes.main_container_users}>
            <div className={classes.header}>
                <h3>Управление пользователями</h3>
                <div className={classes.search_container}>
                    <input
                        type="text"
                        className={classes.search_input}
                        placeholder="Поиск по email..."
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                    />
                    {searchTerm && (
                        <button 
                            className={classes.clear_btn}
                            onClick={() => {
                                setSearchTerm("")
                                setIsSearching(false)
                                fetchUsers(1)
                            }}
                        >
                            ✖
                        </button>
                    )}
                </div>
            </div>

            {message && (
                <div className={classes.message}>
                    {message}
                </div>
            )}

            <div className={classes.table_wrapper}>
                <table className={classes.table_users}>
                    <thead>
                        <tr>
                            <th>Email</th>
                            <th>Роль</th>
                            <th>Действия</th>
                        </tr>
                    </thead>
                    <tbody>
                        {loading && displayUsers.length === 0 ? (
                            <tr>
                                <td colSpan="3" className={classes.loading_cell}>
                                    <div className={classes.spinner}></div>
                                    <p>Загрузка...</p>
                                </td>
                            </tr>
                        ) : displayUsers.length === 0 ? (
                            <tr>
                                <td colSpan="3" className={classes.empty_cell}>
                                    <p>Пользователи не найдены</p>
                                </td>
                            </tr>
                        ) : (
                            displayUsers.map((el) => (
                                <tr key={el.id}>
                                    <td>{el.email}</td>
                                    <td className={getRoleClass(el.role)}>
                                        {getRoleDisplay(el.role)}
                                    </td>
                                    <td>
                                        <select 
                                            className={classes.input_select_role} 
                                            defaultValue=""
                                            onChange={(e) => handleRoleChanged(el.id, e.target.value)} 
                                            disabled={loading}
                                        >
                                            <option value="" disabled>Выберите новую роль</option>
                                            <option value="user">Пользователь</option>
                                            <option value="admin">Администратор</option>
                                            <option value="staff">Сотрудник</option>
                                        </select>
                                    </td>
                                </tr>
                            ))
                        )}
                    </tbody>
                </table>
            </div>

            {!isSearching && pagination.hasMore && (
                <div className={classes.load_more_container}>
                    <button 
                        className={classes.load_more_btn}
                        onClick={loadMore}
                        disabled={loading}
                    >
                        {loading ? 'Загрузка...' : 'Загрузить еще'}
                    </button>
                </div>
            )}

            {isSearching && searchResults.length > 0 && (
                <div className={classes.search_info}>
                    Найдено пользователей: {searchResults.length}
                </div>
            )}
        </div>
    )
}