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
import EditMovie from './components/EditMovie/EditMovie'
import ProfilePage from './components/ProfilePage/ProfilePage'
import ScapPage from './components/ScanPage/ScanPage'
import ForgotPassword from './components/ForgotPassword/ForgotPassword'
import ResetPassword from './components/ResetPassword/ResetPassword'
import PurchaseWindow from './components/PurchaseWindow/PurchaseWindow'
import PrivacyPolicy from './components/PrivacyPolicy/PrivacyPolicy'

function App() {
  return (
    <>
    <AuthProvider>
      <MovieProvider>
      <Router>
        <Header />
        <Routes>
          <Route path="/" element={<Schedule />} />
          <Route path="/schedule/:id" element={<PurchaseWindow />} />
          <Route path="/home" element={<Home />} />
          <Route path="/login" element={<Login />}/>
          <Route path="/register" element={<Register />}/>
          <Route path="/privacy-policy" element={<PrivacyPolicy />}/>
          <Route path="/movies" element={<MoviesList />}/>
          <Route path="/profile" element={<ProfilePage />}/>
          <Route path="/scan" element={<ScapPage />}/>
          <Route path="/forgot-password" element={<ForgotPassword />}/>
          <Route path="/reset-password" element={<ResetPassword />}/>
          <Route path="/movies/:id" element={<MovieInfo />}/>
          <Route path="/admin" element={<AdminPanel />}/>
          <Route path="/admin/movies/edit/:id" element={<EditMovie />} />
          <Route path="*" element={<Navigate to="/" />} />
        </Routes>
      </Router>
      </MovieProvider>
    </AuthProvider>
    </>
  )
}

export default App
