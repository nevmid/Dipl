import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../../../contexts/AuthContext'
import classes from './Register.module.css'

export default function Register() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [repeatPassword, setRepeatPassword] = useState('');
    const {register, isLoading} = useAuth();
    const [error, setError] = useState('');
    const navigate = useNavigate()

    const handleSubmit = async(e) =>{
        e.preventDefault();
        setError('');

        const result = await register(email, password, repeatPassword);

        if(result?.success)
            navigate('/');
        else{
            setError(result?.error || 'Ошибка регистрации');
        }
    }

    return(
        <div className={classes.main}>
            <div className={classes.form_div}>
            <h2>Регистрация</h2>

            {error && <div className={classes.error}>{error}</div> }

                <form onSubmit={handleSubmit}>
                    <div className={classes.form_group}>
                        <label>Email</label>
                        <input type="text" value={email} placeholder="example@mail.com"
                         disabled={isLoading} required onChange={(e) => {setEmail(e.target.value)}}/>
                    </div>
                    <div className={classes.form_group}>
                        <label>Пароль</label>
                        <input type="password" value={password}
                         disabled={isLoading} required onChange={(e) => {setPassword(e.target.value)}}/>
                    </div>   
                    <div className={classes.form_group}>
                        <label>Подтвердите пароль</label>
                        <input type="password" value={repeatPassword}
                         disabled={isLoading} required onChange={(e) => {setRepeatPassword(e.target.value)}}/>
                    </div> 
                    <button type="submit" disabled={isLoading} className={classes.submit_btn}>
                        {isLoading ? "Регистрация.." : "Зарегистрироваться"}
                    </button>  
                </form>
                <div className={classes.login_link}>
                    Уже есть аккаунт? <Link to="/login">Войти</Link>
                </div>
            </div>
        </div>
    )
}