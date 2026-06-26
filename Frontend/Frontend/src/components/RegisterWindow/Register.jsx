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
    const [agreeToPrivacy, setAgreeToPrivacy] = useState(false);
    const navigate = useNavigate()

    const handleSubmit = async(e) =>{
        e.preventDefault();
        setError('');
        if (!agreeToPrivacy) {
            setError('Необходимо согласие на обработку персональных данных');
            return;
        }
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
                    <div className={classes.checkbox_group}>
                        <label className={classes.checkbox_label}>
                            <input
                                type="checkbox"
                                checked={agreeToPrivacy}
                                onChange={(e) => setAgreeToPrivacy(e.target.checked)}
                                disabled={isLoading}
                                required
                            />
                            <span>
                                Я принимаю условия{' '}
                                <Link to="/privacy-policy" target="_blank" className={classes.privacy_link}>
                                    политики обработки персональных данных
                                </Link>
                            </span>
                        </label>
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