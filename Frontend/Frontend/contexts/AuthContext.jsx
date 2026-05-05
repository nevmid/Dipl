import {createContext, useState, useContext, useEffect} from 'react'
import Cookies from 'js-cookie';

const API_BASE_URL = import.meta.env.VITE_API_URL;

const AuthContext = createContext();

export const useAuth = () => {
    return useContext(AuthContext);
}

export const AuthProvider = ({ children }) => {
    const [isAuthenticated, setIsAuthenticated] = useState(false)
    const [user, setUser] = useState(null)
    const [isLoading, setIsLoading] = useState(true);
    
    useEffect(() => {
        const checkAuth = async () => {
            try{
                const response = await fetch(`${API_BASE_URL}/users/profile`, {
                    method: "GET",
                    credentials: 'include',
                });

                if(response.ok){
                    const userData = await response.json();
                    setUser(userData.user);
                    setIsAuthenticated(true);
                }
            }
            catch (error){
                console.error('Ошибка проверки авторизации:', error);
            }
            finally{
                setIsLoading(false);
            }
        };

        checkAuth();
    }, [])

    const login = async (email, password) => {
        setIsLoading(true)
        try{
            const response = await fetch(`${API_BASE_URL}/auth/login`, {
                method: 'POST',
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({ email, password}),
                credentials: 'include',
            });
            
            const data = await response.json();

            if (!response.ok) {
                const errorMessage = data?.errors?.Email ||
                               data?.errors?.Password ||
                               data?.message ||
                               'Ошибка авторизации';

                throw new Error(errorMessage);
            }

            setIsAuthenticated(true);

            if(data)
                setUser(data);

            return {success: true, data: data};
        } catch (error) {
            return {success: false, error: error.message};
        }   
        finally{
            setIsLoading(false);
        }
    }

    const register = async (email, password, repeatPassword) => {
        try{
            setIsLoading(true);

            const response = await fetch(`${API_BASE_URL}/auth/register`,{
                method: "POST",
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({email, password, ConfirmPassword: repeatPassword}),
                credentials: 'include',
            });
            
            const data = await response.json();

            if(!response.ok){
                const errorMessage = data?.errors?.Email ||
                               data?.errors?.Password ||
                               data?.errors?.ConfirmPassword ||
                               data?.errors?.Role ||
                               data?.message ||
                               'Ошибка авторизации';

                throw new Error(errorMessage);
            };

            setIsAuthenticated(true);

            if(data)
                setUser(data);

            return {success: true, data};
        }
        catch (error) {
            return {success: false, error: error.message};
        }   
        finally{
            setIsLoading(false);
        }
    }

    const logout = async () => {
    try {
        await fetch(`${API_BASE_URL}/auth/logout`, {
            method: 'POST',
            credentials: 'include',
      });
    } catch (error) {
        console.error('Ошибка при выходе:', error);
    } finally {
        Cookies.remove('FeelsGoodMan', { path: '/' });
        
        setUser(null);
        setIsAuthenticated(false);
    }
  };

    const value = {
        isAuthenticated,
        user,
        isLoading,

        login,
        logout,
        register,

        isAdmin: user?.role == 'admin',
    };

    return (<>
        <AuthContext.Provider value={value}>
            {children}
        </AuthContext.Provider>
    </>);
};