import { useState } from 'react';
import axios from 'axios';
import './App.css';

// Задай порта, на който върви твоето API (виж го от Swagger)
const API_BASE_URL = 'http://localhost:5207/api';

function App() {
  const [formData, setFormData] = useState({ name: '', address: '', city: '', managerId: '' });
  const [errors, setErrors] = useState<Record<string, string[]>>({});
  const [success, setSuccess] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setErrors({});
    setSuccess(false);

    try {
      await axios.post(`${API_BASE_URL}/Buildings`, formData);
      setSuccess(true);
      setFormData({ name: '', address: '', city: '', managerId: '' }); // Изчистваме формата
    } catch (error: any) {
      // Прихващаме красивия JSON, който нашият ExceptionHandler генерира!
      if (error.response && error.response.status === 400) {
        setErrors(error.response.data.errors);
      }
    }
  };

  return (
      <div style={{ maxWidth: '400px', margin: '50px auto', fontFamily: 'sans-serif' }}>
        <h2>Добави нова сграда</h2>

        {success && <div style={{ color: 'green', marginBottom: '15px' }}>Сградата е създадена успешно!</div>}

        <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '15px' }}>
          <div>
            <label>Име на сградата:</label>
            <input
                type="text"
                value={formData.name}
                onChange={(e) => setFormData({...formData, name: e.target.value})}
                style={{ width: '100%', padding: '8px' }}
            />
            {/* Показваме грешката за Name, ако има такава */}
            {errors.Name && <span style={{ color: 'red', fontSize: '12px' }}>{errors.Name[0]}</span>}
          </div>

          <div>
            <label>Адрес:</label>
            <input
                type="text"
                value={formData.address}
                onChange={(e) => setFormData({...formData, address: e.target.value})}
                style={{ width: '100%', padding: '8px' }}
            />
            {errors.Address && <span style={{ color: 'red', fontSize: '12px' }}>{errors.Address[0]}</span>}
          </div>

          <div>
            <label>ID на Домоуправител:</label>
            <input
                type="text"
                value={formData.managerId}
                onChange={(e) => setFormData({...formData, managerId: e.target.value})}
                style={{ width: '100%', padding: '8px' }}
            />
            {errors.ManagerId && <span style={{ color: 'red', fontSize: '12px' }}>{errors.ManagerId[0]}</span>}
          </div>

          <button type="submit" style={{ padding: '10px', cursor: 'pointer' }}>Създай</button>
        </form>
      </div>
  );
}

export default App;