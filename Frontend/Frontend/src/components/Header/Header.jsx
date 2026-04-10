import { Link } from "react-router-dom"
// import { useState } from "react"
import classes from './Header.module.css'
import account from '../../assets/logoaccount.svg'
import { useAuth } from '../../../contexts/AuthContext'
import { useLocation } from "react-router-dom"

export default function Header(){
    const {isAuthenticated, isAdmin, logout} = useAuth()
    const location = useLocation();

    const noHeader = ['/login', '/register'];
    const hideMovieInfo = location.pathname.startsWith("/movies/");
    const hide = noHeader.includes(location.pathname) || hideMovieInfo;

    if(hide)
        return null;

    return(
        <header className={classes.header}>
            <div>
                Space<span className={classes.logo_cinema}>Cinema</span>
            </div>
            <nav className={classes.navigation}>
                <ul>
                    <li><Link to="/">Главная</Link></li>
                    <li><Link to="/sessions">Расписание</Link></li>
                    <li><Link to="/movies">Афиша</Link></li>
                    {/* <li><Link to="/bar">Кино-бар</Link></li> */}
                </ul>
            </nav>
            <div>
                {isAuthenticated ? (<>
                <div className={classes.user_menu}>
                    {isAdmin && (
                        <Link to="/admin" className={classes.admin_btn}>
                            Админка
                        </Link>
                    )}
                    <Link to="/profile" className={classes.profile} onClick={() => {logout()}}>
                        <img src={account} alt="account" className={classes.acc_img}/>
                    </Link>
                </div>
                </>) : (
                <>
                <div className={classes.auth_btns}>
                    <Link to="/login" className={classes.login_btn}>Вход</Link>
                    <Link to="/register" className={classes.reg_btn}>Регистрация</Link>
                </div>
                </>
                )}
            </div>
        </header>
    )
}