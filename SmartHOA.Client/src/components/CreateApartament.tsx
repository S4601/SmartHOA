import { useState} from "react";
import api from "../api.ts";

interface CreateApartamentProps {
    buildingId: string,
    onCreateApartamentRefreshList: () => void;
}

export default function CreateApartament({ buildingId, onCreateApartamentRefreshList }: CreateApartamentProps) {
    const [formData, setFormData] = useState({
        number: "",
        floor: 0,
        buildingId: buildingId ? buildingId : "",
        
    });

    const [message, setMessage] = useState("");

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    }

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setMessage("");

        try {
            await api.post('/Apartment', {
                ...formData,
                floor: Number(formData.floor),
            });

            setMessage('✅ Сградата е създадена успешно!');
            onCreateApartamentRefreshList();
            setFormData({ number: '', floor: 0, buildingId: ''});
        } catch (error) {
            console.error(error);
            //console.error("Детайли за грешката:", error.response?.data);
            setMessage('❌ Възникна грешка при създаването.');
        }

    }


    return (
        <div style={{ marginTop: '30px', padding: '20px', border: '1px solid #ccc', borderRadius: '8px' }}>
            <h3>Добави нов апартамент (Само за Админи и Управители)</h3>

            {message && <div style={{ marginBottom: '15px', fontWeight: 'bold' }}>{message}</div>}

            <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '10px', maxWidth: '300px' }}>
                <input name="number" placeholder="Номер на апартамента" value={formData.number} onChange={handleChange} required />
                <input name="floor" placeholder="Етаж" value={formData.floor} onChange={handleChange} required />
                {!buildingId && (<input name="buildingId" placeholder="ID на сграда" value={formData.buildingId} onChange={handleChange} required />)}

                <button type="submit" style={{ padding: '8px', backgroundColor: '#007bff', color: 'white', border: 'none', cursor: 'pointer' }}>
                    Създай апартамент
                </button>
            </form>
        </div>
    );
}