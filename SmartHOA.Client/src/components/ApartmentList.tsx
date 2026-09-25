import {useEffect, useState} from "react";
import api from "../api.ts";


interface ApartmentDto {
    id: string;
    number: string;
    floor: number;
    name: string; // Име на сградата
}

interface ApartmentListProps {
    refreshKey?: number;
}

export default function ApartmentList({refreshKey = 0} : ApartmentListProps) {
    const [apartments, setApartments] = useState<ApartmentDto[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string>("");
    
    useEffect(() => {
        const fetchApartments = async () => {
            try {
                const response = await api.get("/Apartment");
                setApartments(response.data);
            } catch (err) {
                console.error("Грешка при извличане на апартаменти:", err);
                setError("Не успяхме да заредим апартаментите.");
            } finally {
                setLoading(false);
            }
        };
        
        fetchApartments();
    }, [refreshKey])

    if (loading) {
        return <div>Зареждане на апартаменти...</div>;
    }

    if (error) {
        return <div style={{ color: "red" }}>{error}</div>;
    }
    
    return (
        <div style={{ marginTop: '30px', padding: '20px', border: '1px solid #ccc', borderRadius: '8px' }}>
            <h3>Списък с апартаменти</h3>

            {apartments.length === 0 ? (
                <p>Нямате достъп до никакви апартаменти или все още няма създадени такива.</p>
            ) : (
                <table style={{ width: '100%', borderCollapse: 'collapse', marginTop: '15px' }}>
                    <thead>
                    <tr style={{ backgroundColor: '#f4f4f4', textAlign: 'left' }}>
                        <th style={{ padding: '10px', borderBottom: '1px solid #ddd' }}>Сграда</th>
                        <th style={{ padding: '10px', borderBottom: '1px solid #ddd' }}>Етаж</th>
                        <th style={{ padding: '10px', borderBottom: '1px solid #ddd' }}>Апартамент №</th>
                    </tr>
                    </thead>
                    <tbody>
                    {apartments.map((apt) => (
                        <tr key={apt.id}>
                            <td style={{ padding: '10px', borderBottom: '1px solid #eee' }}>{apt.name}</td>
                            <td style={{ padding: '10px', borderBottom: '1px solid #eee' }}>{apt.floor}</td>
                            <td style={{ padding: '10px', borderBottom: '1px solid #eee' }}>{apt.number}</td>
                        </tr>
                    ))}
                    </tbody>
                </table>
            )}
        </div>
    );
}