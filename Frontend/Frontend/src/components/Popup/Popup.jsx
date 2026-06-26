import { useEffect } from 'react';
import classes from './Popup.module.css';

export default function Popup({ 
    message, 
    type = 'success', 
    onClose, 
    onConfirm,
    onCancel,
    duration = 3000,
    showConfirm = false 
}) {
    useEffect(() => {
        if (!showConfirm && duration > 0) {
            const timer = setTimeout(() => {
                onClose();
            }, duration);
            return () => clearTimeout(timer);
        }
    }, [duration, onClose, showConfirm]);

    return (
        <div className={classes.overlay} onClick={showConfirm ? undefined : onClose}>
            <div className={`${classes.popup} ${classes[type]}`} onClick={(e) => e.stopPropagation()}>
                <div className={classes.icon}>
                    {type === 'success' && '✅'}
                    {type === 'error' && '❌'}
                    {type === 'warning' && '⚠️'}
                    {type === 'confirm' && '❓'}
                    {type === 'info' && 'ℹ️'}
                </div>
                
                <div className={classes.content}>
                    <p>{message}</p>
                </div>
                
                {showConfirm ? (
                    <div className={classes.buttons}>
                        <button className={classes.confirm_btn} onClick={onConfirm}>
                            Да
                        </button>
                        <button className={classes.cancel_btn} onClick={onCancel || onClose}>
                            Нет
                        </button>
                    </div>
                ) : (
                    <button className={classes.close_btn} onClick={onClose}>✖</button>
                )}
            </div>
        </div>
    );
}