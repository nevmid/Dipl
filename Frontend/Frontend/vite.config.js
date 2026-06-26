import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    // host: '0.0.0.0',
    // port: 5173,
    // allowedHosts: [
    //   'all',                                   // вариант "пропускать все"
    //   '.tuna.am',                              // все поддомены tuna.am
    //   'https://6rk3i9-178-141-21-81.ru.tuna.am'        // или именно ваш адрес
    // ]
    }
})
