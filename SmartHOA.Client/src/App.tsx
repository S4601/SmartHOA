import './App.css';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router';
import Login from './components/Login';
import Dashboard from './components/Dashboard';
import BuildingDetails from "./components/BuildingDetails.tsx";
import ApartmentDetails from "./components/ApartmentDetails.tsx";

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
            
            <Route path="/buildings/:id" element={<BuildingDetails/>}/>
            <Route path="/apartments/:id" element={<ApartmentDetails/>}/>
        </Routes>
      </BrowserRouter>
  );
}

export default App;