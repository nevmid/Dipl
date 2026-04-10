import classes from './AdminPanel.module.css';
import {useAuth} from '../../../contexts/AuthContext';
import { Navigate } from 'react-router-dom';
import { useState } from 'react';

export default function AdminPanel(){
    const {isAdmin, isLoading} = useAuth();
    const [currentTab, setCurrentTab] = useState("users");

    if(!isAdmin)
        return <Navigate to="/" />;

    if (isLoading) {
        return (
            <div className={classes.loading}>
                <div className={classes.spinner}></div>
                <p>Загрузка...</p>
            </div>
        );
    }

    return(
        <div className={classes.main_container}>
            <div className={classes.admin_panel}>
                <h2>Админ-панель</h2>
                <nav className={classes.nav}>
                    <button className={classes.nav_btn} onClick={() => {setCurrentTab("users")}}>
                        Управление пользователями
                    </button>
                    <button className={classes.nav_btn} onClick={() => {setCurrentTab("movies")}}>
                        Управление фильмами
                    </button>
                    <button className={classes.nav_btn} onClick={() => {setCurrentTab("halls")}}>
                        Управление залами
                    </button>
                    <button className={classes.nav_btn} onClick={() => {setCurrentTab("screenings")}}>
                        Управление расписанием
                    </button>
                </nav>
            </div>
        </div>
    )
}