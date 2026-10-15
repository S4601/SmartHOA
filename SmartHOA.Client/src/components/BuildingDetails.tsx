import {useNavigate, useParams} from "react-router";
import {useEffect, useState} from "react";
import axios from "axios";
import api from "../api.ts";

interface BuildingDetailDto {
    id: string;
    name: string;
    address: string;
    city?: string;
    floorCount?: number;
    apartmentCountPerFloor?: number;
    managerName: string;
}

export default function BuildingDetails() {
    const { id } = useParams<{id: string}>();
    const navigate = useNavigate();

    const [building, setBuilding] = useState<BuildingDetailDto | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        const fetchBuilding = async () => {
            try {
                const response = await api.get(`/Buildings/${id}`);
                setBuilding(response.data);
            } catch (err: unknown) {
                console.error(err);
                if (axios.isAxiosError(err) && err.response?.status === 404) {
                    setError("Сградата не е намерена.");
                } else {
                    setError("Не успяхме да заредим данните за сградата.");
                }
            } finally {
                setLoading(false);
            }
        };

        if (id) fetchBuilding();
    }, [id]);

    if (loading) return <div style={{ padding: '20px' }}>Зареждане на детайли...</div>;
    if (error) return <div style={{ padding: '20px', color: 'red' }}>{error}</div>;
    if (!building) return <div style={{ padding: '20px' }}>Сградата не е намерена.</div>;

    return (
        <div style={{ padding: '20px', fontFamily: 'sans-serif', maxWidth: '600px', margin: '0 auto' }}>
            <button
                onClick={() => navigate('/dashboard')}
                style={{ marginBottom: '20px', padding: '8px 16px', cursor: 'pointer' }}
            >
                ← Назад към таблото
            </button>

            <div style={{ border: '1px solid #ccc', padding: '20px', borderRadius: '8px', backgroundColor: '#f9f9f9' }}>
                <h2>Детайли за сграда</h2>
                <hr style={{ margin: '15px 0' }} />

                <p><strong>Име:</strong> {building.name}</p>
                <p><strong>Град:</strong> {building.city || 'Не е посочен'}</p>
                <p><strong>Адрес:</strong> {building.address}</p>
                <p><strong>Брой етажи:</strong> {building.floorCount ?? 'N/A'}</p>
                <p><strong>Апартаменти на етаж:</strong> {building.apartmentCountPerFloor ?? 'N/A'}</p>
                <p><strong>Управител:</strong> {building.managerName || 'Няма назначен'}</p>
            </div>
        </div>
    );
}