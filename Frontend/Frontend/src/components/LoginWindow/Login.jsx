import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../../../contexts/AuthContext'
import classes from './Login.module.css'

export default function Login() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const {login, isLoading} = useAuth();
    const [error, setError] = useState('');
    const navigate = useNavigate()

    const handleSubmit = async(e) =>{
        e.preventDefault();
        setError('');

        const result = await login(email, password);

        if(result?.success)
            navigate('/');
        else{
            setError(result?.error || 'Ошибка входа');
        }
        
    }

    return(
        <div className={classes.main}>
            <div className={classes.form_div}>
            <h2>Вход</h2>

            {error && <div className={classes.error}>{error}</div> }

                <form onSubmit={handleSubmit}>
                    <div className={classes.form_group}>
                        <label>Email</label>
                        <input type="text" value={email} placeholder="example@mail.com" disabled={isLoading} required onChange={(e) => {setEmail(e.target.value)}}/>
                    </div>
                    <div className={classes.form_group}>
                        <label>Пароль</label>
                        <input type="password" value={password} disabled={isLoading} required onChange={(e) => {setPassword(e.target.value)}}/>
                    </div>
                    <div className={classes.forgot_link}>
                        <Link to="/forgot-password">Забыли пароль?</Link>
                    </div>
                    <button type="submit" disabled={isLoading} className={classes.submit_btn}>
                        {isLoading ? "Вход.." : "Войти"}
                    </button>
                </form>
                <div className={classes.register_link}>
                    Нет аккаунта? <Link to="/register">Зарегистрироваться</Link>
                </div>
            </div>
        </div>
    )
}