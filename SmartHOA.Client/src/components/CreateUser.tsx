import { useState} from "react";
import api from "../api.ts";

export default function CreateUser() {
    const [formData, setFormData] = useState({
        firstName: "",
        lastName: "",
        email: "",
        phoneNumber: "",
        password: "",
        userType: 0
    });
    
    const [message, setMessage] = useState("");
    
    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    }
    
    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setMessage("");
        
        try {
            await api.post('/Users', {
                ...formData,
                userType: Number(formData.userType)
            });

            setMessage('✅ Потребителят е създаден успешно!');

            setFormData({ firstName: '', lastName: '', email: '', phoneNumber: '', password: '', userType: 0 });
        } catch (error) {
            console.error(error);
            setMessage('❌ Възникна грешка при създаването.');
        }
        
    }
    
    return (
        <div style={{ marginTop: '30px', padding: '20px', border: '1px solid #ccc', borderRadius: '8px' }}>
            <h3>Добави нов потребител (Само за Админи)</h3>

            {message && <div style={{ marginBottom: '15px', fontWeight: 'bold' }}>{message}</div>}

            <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '10px', maxWidth: '300px' }}>
                <input name="firstName" placeholder="Име" value={formData.firstName} onChange={handleChange} required />
                <input name="lastName" placeholder="Фамилия" value={formData.lastName} onChange={handleChange} required />
                <input name="email" type="email" placeholder="Имейл" value={formData.email} onChange={handleChange} required />
                <input name="phoneNumber" placeholder="Телефон" value={formData.phoneNumber} onChange={handleChange} />
                <input name="password" type="password" placeholder="Парола" value={formData.password} onChange={handleChange} required />

                <select name="userType" value={formData.userType} onChange={handleChange}>
                    <option value={0}>Администратор (Admin)</option>
                    <option value={1}>Собственик / Наемател (Resident)</option>
                    <option value={2}>Гост (Guest)</option>
                    <option value={3}>Управител (Manager)</option>
                </select>

                <button type="submit" style={{ padding: '8px', backgroundColor: '#007bff', color: 'white', border: 'none', cursor: 'pointer' }}>
                    Създай потребител
                </button>
            </form>
        </div>
    );
}