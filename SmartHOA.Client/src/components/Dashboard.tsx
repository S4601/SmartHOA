import { useEffect, useState } from "react";
import { jwtDecode } from "jwt-decode";
import {useNavigate} from 'react-router-dom';
import CreateUser from "./CreateUser.tsx";
import CreateBuilding from "./CreateBuilding.tsx";
import CreateApartament from "./CreateApartament.tsx";
import ApartmentList from "./ApartmentList.tsx";
import BuildingList from "./BuildingList.tsx";
import Modal from "./Modal.tsx";

interface UserData {
    'FirstName'?: string;
    'email'?: string;
    'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'?: string;
    [key: string]: any;
}

export default function Dashboard() {
    const [userData, setUserData] = useState<UserData | null>(null);
    const navigate = useNavigate();

    const [isUserModalOpen, setIsUserModalOpen] = useState(false);
    const [isBuildingModalOpen, setIsBuildingModalOpen] = useState(false);
    const [isApartamentModalOpen, setIsApartamentModalOpen] = useState(false);
    const [buildingId, setBuildingId] = useState<string>("");
    
    const [refreshApartmentsKey, setRefreshApartmentsKey] = useState(0);
    const [refreshBuildingsKey, setRefreshBuildingsKey] = useState(0);
    
    useEffect(() => {
        const token = localStorage.getItem("token");
        
        if(token) {
            try {
                const decoded = jwtDecode(token);
    
                console.log("Моят токен съдържа:", decoded);
                console.log(token);

                setUserData(decoded);
            } catch (error) {
                console.error("Невалиден токен", error);
                localStorage.removeItem("token");
                navigate("/login", { replace: true });
            }
        } else {
            navigate("/login", { replace: true });
        }
    }, [navigate]);

    const handleLogout = () => {
        localStorage.removeItem("token");

        navigate("/login", { replace: true });
    }

    // Open Create Apartament Modal when clicked Add apartament button on BuildingList
    const handleCreateApartament = (buildingId : string) => {
        setBuildingId(buildingId);
        setIsApartamentModalOpen(true);
    }

    const onCreateApartamentRefreshList = () => {
        setRefreshApartmentsKey(prev => prev + 1);
    }
    
    const onCreateBuildingRefreshList = () => {
        setRefreshBuildingsKey(prev => prev + 1);
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

            <div style={{ display: 'flex', gap: '10px', marginBottom: '20px' }}>
                {isAdmin && (
                    <>
                        <button
                            onClick={() => setIsUserModalOpen(true)}
                            style={{ padding: '10px', backgroundColor: '#28a745', color: 'white', border: 'none', borderRadius: '5px', cursor: 'pointer' }}
                        >
                            + Нов Потребител
                        </button>
    
                        <button
                            onClick={() => setIsBuildingModalOpen(true)}
                            style={{ padding: '10px', backgroundColor: '#28a745', color: 'white', border: 'none', borderRadius: '5px', cursor: 'pointer' }}
                        >
                            + Нова Сграда
                        </button>

                        
                    </>
                )}

                {(isAdmin || isManager) && (
                    <button
                        onClick={() => setIsApartamentModalOpen(true)}
                        style={{ padding: '10px', backgroundColor: '#28a745', color: 'white', border: 'none', borderRadius: '5px', cursor: 'pointer' }}
                    >
                        + Нов Апартамент
                    </button>
                )}
            </div>

            <Modal
                isOpen={isUserModalOpen}
                onClose={() => setIsUserModalOpen(false)}
                title="Създаване на потребител"
            >
                <CreateUser />
            </Modal>

            <Modal
                isOpen={isBuildingModalOpen}
                onClose={() => setIsBuildingModalOpen(false)}
                title="Създаване на сграда"
            >
                <CreateBuilding onCreateBuildingRefreshList={onCreateBuildingRefreshList}/>
            </Modal>

            <Modal
                isOpen={isApartamentModalOpen}
                onClose={() => {
                        setIsApartamentModalOpen(false);
                        setBuildingId("");
                    }
                }
                title="Създаване на апартамент"
            >
                <CreateApartament onCreateApartamentRefreshList={onCreateApartamentRefreshList} buildingId={buildingId}/>
            </Modal>

            <ApartmentList refreshKey={refreshApartmentsKey}/>
            <BuildingList refreshKey={refreshBuildingsKey} onCreateApartanent={handleCreateApartament}/>
        </div>
    );
}