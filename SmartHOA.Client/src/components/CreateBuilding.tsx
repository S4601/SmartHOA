import { useState} from "react";
import api from "../api.ts";

interface CreateBuildingProps {
    onCreateBuildingRefreshList: () => void;
}

export default function CreateBuilding({onCreateBuildingRefreshList}: CreateBuildingProps) {
    const [formData, setFormData] = useState({
        name: "",
        address: "",
        city: "",
        floorCount: 0,
        apartmentCount: 0,
        managerId: ""
    });

    const [message, setMessage] = useState("");

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    }

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setMessage("");

        try {
            await api.post('/Buildings', {
                ...formData,
                floorCount: Number(formData.floorCount),
                apartmentCount: Number(formData.apartmentCount),
            });

            setMessage('✅ Сградата е създадена успешно!');
            onCreateBuildingRefreshList();

            setFormData({ name: '', address: '', city: '', floorCount: 0, apartmentCount: 0, managerId: "" });
        } catch (error) {
            console.error(error);
            setMessage('❌ Възникна грешка при създаването.');
        }

    }
    
    
    return (
        <div style={{ marginTop: '30px', padding: '20px', border: '1px solid #ccc', borderRadius: '8px' }}>
            <h3>Добави нова сграда (Само за Админи)</h3>

            {message && <div style={{ marginBottom: '15px', fontWeight: 'bold' }}>{message}</div>}

            <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '10px', maxWidth: '300px' }}>
                <input name="name" placeholder="Име на сградата" value={formData.name} onChange={handleChange} required />
                <input name="address" placeholder="Адрес" value={formData.address} onChange={handleChange} required />
                <input name="city" placeholder="Град" value={formData.city} onChange={handleChange} required />
                <input name="floorCount" type="number" placeholder="Брой етажи" value={formData.floorCount} onChange={handleChange} />
                <input name="apartmentCount" type="number" placeholder="Брой апартаменти" value={formData.apartmentCount} onChange={handleChange} required />
                <input name="managerId" placeholder="ID на мениджър" value={formData.managerId} onChange={handleChange} required />

                <button type="submit" style={{ padding: '8px', backgroundColor: '#007bff', color: 'white', border: 'none', cursor: 'pointer' }}>
                    Създай сграда
                </button>
            </form>
        </div>
    );
}