import { useEffect, useState } from "react";
import { jwtDecode } from "jwt-decode";
import { useNavigate } from 'react-router-dom';
import CreateUser from "./CreateUser.tsx";
import CreateBuilding from "./CreateBuilding.tsx";
import CreateApartament from "./CreateApartament.tsx";

interface UserData {
    'FirstName'?: string;
    'email'?: string;
    'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'?: string;
    [key: string]: any;
}

export default function Dashboard() {
    const [userData, setUserData] = useState<UserData | null>(null);
    const navigate = useNavigate();
    
    useEffect(() => {
        const token = localStorage.getItem("token");
        if(token) {
            try {
                const decoded = jwtDecode(token);

                console.log("Моят токен съдържа:", decoded);

                setUserData(decoded);
            } catch (error) {
                console.error("Невалиден токен", error);
            }
        }
    }, []);

    const handleLogout = () => {
        localStorage.removeItem("token");
        
        navigate("/");
    }
    
    if (!userData) {
        return <div>Зареждане... (или не сте логнати)</div>;
    }

    const roleClaim = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
    const isAdmin = userData[roleClaim] === 'Admin';
    const isManager = userData[roleClaim] === 'Manager';
    
    return (
        <div style={{ padding: '20px', fontFamily: 'sans-serif' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <h2>Табло за управление</h2>

                {/* 4. Добавяме бутона */}
                <button
                    onClick={handleLogout}
                    style={{
                        padding: '8px 16px',
                        backgroundColor: '#dc3545',
                        color: 'white',
                        border: 'none',
                        borderRadius: '4px',
                        cursor: 'pointer'
                    }}
                >
                    Изход
                </button>
            </div>

            <p>Добре дошли в защитената част на системата!</p>

            <div style={{ background: '#f0f0f0', padding: '10px', borderRadius: '5px' }}>
                <p><strong>Роля:</strong> {userData[roleClaim]}</p>
            </div>

            {isAdmin && <CreateUser />}

            {isAdmin && <CreateBuilding />}

            {(isAdmin || isManager) && <CreateApartament />}
        </div>
    );
}