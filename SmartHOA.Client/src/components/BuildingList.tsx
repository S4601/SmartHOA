import {useEffect, useState} from "react";
import api from "../api.ts";
import { useNavigate} from "react-router";

interface BuildingDto {
    id: string;
    name: string;
    address: string;
    managerName: string;
}

interface BuildingListProps {
    onCreateApartanent: (buildingId: string) => void,
    refreshKey: number,
}

export default function BuildingList({ onCreateApartanent, refreshKey }: BuildingListProps) {
    const [buildings, setBuildings] = useState<BuildingDto[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string>("");

    const navigate = useNavigate();

    useEffect(() => {
        const fetchBuildings = async () => {
            try {
                const response = await api.get("/Buildings");
                setBuildings(response.data);
            } catch (err) {
                console.error("Грешка при извличане на сгради:", err);
                setError("Не успяхме да заредим сградите.");
            } finally {
                setLoading(false);
            }
        };

        fetchBuildings();
    }, [refreshKey])

    if (loading) {
        return <div>Зареждане на сгради...</div>;
    }

    if (error) {
        return <div style={{ color: "red" }}>{error}</div>;
    }

    return (
        <div style={{ marginTop: '30px', padding: '20px', border: '1px solid #ccc', borderRadius: '8px' }}>
            <h3>Списък със сгради</h3>

            {buildings.length === 0 ? (
                <p>Нямате достъп до никакви сгради или все още няма създадени такива.</p>
            ) : (
                <table style={{ width: '100%', borderCollapse: 'collapse', marginTop: '15px' }}>
                    <thead>
                    <tr style={{ backgroundColor: '#f4f4f4', textAlign: 'left' }}>
                        <th style={{ padding: '10px', borderBottom: '1px solid #ddd' }}>Сграда</th>
                        <th style={{ padding: '10px', borderBottom: '1px solid #ddd' }}>Адрес</th>
                        <th style={{ padding: '10px', borderBottom: '1px solid #ddd' }}>Управител</th>
                        <th style={{ padding: '10px', borderBottom: '1px solid #ddd' }}></th>
                        <th style={{ padding: '10px', borderBottom: '1px solid #ddd' }}></th>
                    </tr>
                    </thead>
                    <tbody>
                    {buildings.map((building) => (
                        <tr key={building.id}>
                            <td style={{ padding: '10px', borderBottom: '1px solid #eee' }}>{building.name}</td>
                            <td style={{ padding: '10px', borderBottom: '1px solid #eee' }}>{building.address}</td>
                            <td style={{ padding: '10px', borderBottom: '1px solid #eee' }}>{building.managerName || 'Няма назначен'}</td>
                            <td style={{ padding: '10px', borderBottom: '1px solid #eee' }}><button onClick={() => onCreateApartanent(`${building.id}`)}>+ Апартамент</button></td>
                            <td style={{ padding: '10px', borderBottom: '1px solid #eee' }}><button onClick={() => navigate(`/buildings/${building.id}`)}>Детайли</button></td>
                        </tr>
                    ))}
                    </tbody>
                </table>
            )}
        </div>
    );
}