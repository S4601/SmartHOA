import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router";
import axios from "axios";
import api from "../api.ts";

interface ApartmentDetailDto {
    id: string;
    number: string;
    floor: number;
    buildingName: string;
    name?: string;
}

export default function ApartmentDetails() {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();

    const [apartment, setApartment] = useState<ApartmentDetailDto | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        const fetchApartment = async () => {
            try {
                const response = await api.get(`/Apartments/${id}`);
                setApartment(response.data);
            } catch (err: unknown) {
                console.error(err);
                if (axios.isAxiosError(err)) {
                    if (err.response?.status === 404) {
                        setError("Апартаментът не е намерен.");
                    } else if (err.response?.status === 403) {
                        setError("Нямате права за преглед на този апартамент.");
                    } else {
                        setError("Не успяхме да заредим данните за апартамента.");
                    }
                } else {
                    setError("Възникна неочаквана грешка.");
                }
            } finally {
                setLoading(false);
            }
        };

        if (id) fetchApartment();
    }, [id]);

    if (loading) return <div style={{ padding: '20px' }}>Зареждане...</div>;
    if (error) return <div style={{ padding: '20px', color: 'red' }}>{error}</div>;
    if (!apartment) return <div style={{ padding: '20px' }}>Апартаментът не е намерен.</div>;

    const buildingDisplay = apartment.buildingName || apartment.name || 'Не е посочена';

    return (
        <div style={{ padding: '20px', fontFamily: 'sans-serif', maxWidth: '600px', margin: '0 auto' }}>
            <button
                onClick={() => navigate('/dashboard')}
                style={{ marginBottom: '20px', padding: '8px 16px', cursor: 'pointer' }}
            >
                ← Назад към таблото
            </button>

            <div style={{ border: '1px solid #ccc', padding: '20px', borderRadius: '8px', backgroundColor: '#f9f9f9' }}>
                <h2>Детайли за Апартамент {apartment.number}</h2>
                <hr style={{ margin: '15px 0' }} />

                <p><strong>Номер:</strong> {apartment.number}</p>
                <p><strong>Етаж:</strong> {apartment.floor}</p>
                <p><strong>Сграда:</strong> {buildingDisplay}</p>
            </div>
        </div>
    );
}