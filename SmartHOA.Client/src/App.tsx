import './App.css';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import Login from './components/Login';
import Dashboard from './components/Dashboard';

function App() {
  return (
      <BrowserRouter>
        <Routes>
          {/* Автоматично пренасочва от началната страница към логин */}
          <Route path="/" element={localStorage.getItem("token") ? <Navigate to={"/dashboard"}/> : <Navigate to="/login" />} />

          {/* Страницата за вход */}
          <Route path="/login" element={<Login />} />

          {/* Защитената страница след вход */}
          <Route path="/dashboard" element={<Dashboard />} />
        </Routes>
      </BrowserRouter>
  );
}

export default App;