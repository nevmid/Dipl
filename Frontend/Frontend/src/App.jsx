import {BrowserRouter as Router, Routes, Route, Navigate} from 'react-router-dom'
import "./App.css"
import {AuthProvider} from '../contexts/AuthContext'
import {MovieProvider} from '../contexts/movieContext'

import Header from './components/Header/Header'
import Home from './components/Home/Home'
import Login from './components/LoginWindow/Login'
import Register from './components/RegisterWindow/Register'
import MoviesList from './components/MoviesList/Movies'
import MovieInfo from './components/MovieInfo/MovieInfo'
import AdminPanel from './components/AdminPanel/AdminPanel'
import Schedule from './components/Schedule/Schedule'

function App() {
  return (
    <>
    <AuthProvider>
      <MovieProvider>
      <Router>
        <Header />
        <Routes>
          <Route path="/" element={<Schedule />} />
          <Route path="/login" element={<Login />}/>
          <Route path="/register" element={<Register />}/>
          <Route path="/movies" element={<MoviesList />}/>
          {/* <Route path="/schedule" element={<Schedule />}/> */}
          <Route path="/movies/:id" element={<MovieInfo />}/>
          <Route path="/admin" element={<AdminPanel />}/>
          <Route path="*" element={<Navigate to="/" />} />
        </Routes>
      </Router>
      </MovieProvider>
    </AuthProvider>
    </>
  )
}

export default App
