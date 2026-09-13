import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../api';

export default function Login() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');

    const navigate = useNavigate();
    
    const handleSubmit = async (e : any) => {
        e.preventDefault();
        setError('');

        try {
            const response = await api.post('/Users/login', {
                email: email,
                password: password
            });
            
            const token = response.data.token; //|| response.data;
            
            localStorage.setItem('token', token);

            navigate('/dashboard');
        } catch (err : any) {
            console.error(err);
            if (err.response?.status === 401) {
                setError('Грешен имейл или парола.');
            } else {
                setError('Възникна грешка при свързване със сървъра.');
            }
        }
    };

    return (
        <div style={{ maxWidth: '400px', margin: '50px auto', fontFamily: 'sans-serif' }}>
            <h2>Вход в SmartHOA</h2>

            {error && <div style={{ color: 'red', marginBottom: '10px' }}>{error}</div>}

            <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '15px' }}>
                <div>
                    <label>Имейл:</label>
                    <input
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                        style={{ width: '100%', padding: '8px' }}
                    />
                </div>

                <div>
                    <label>Парола:</label>
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                        style={{ width: '100%', padding: '8px' }}
                    />
                </div>

                <button type="submit" style={{ padding: '10px', cursor: 'pointer' }}>
                    Вход
                </button>
            </form>
        </div>
    );
}