import {useEffect, useState} from "react";
import axios from "axios";
import api from "../api.ts";

interface CreateApartamentProps {
    buildingId: string,
    onCreateApartamentRefreshList: () => void;
}

interface BuildingOption {
    id: string;
    name: string;
}

export default function CreateApartament({ buildingId, onCreateApartamentRefreshList }: CreateApartamentProps) {
    const [formData, setFormData] = useState({
        number: "",
        floor: 0,
        buildingId: buildingId ? buildingId : "",
        
    });

    const [message, setMessage] = useState("");

    const [buildings, setBuildings] = useState<BuildingOption[]>([]);
    const [searchTerm, setSearchTerm] = useState("");
    const [isDropdownOpen, setIsDropdownOpen] = useState(false);

    useEffect(() => {
        const fetchBuildings = async () => {
            try {
                const response = await api.get('/Buildings');
                setBuildings(response.data);
                
                if (buildingId) {
                    const selected = response.data.find((b: BuildingOption) => b.id === buildingId);
                    if (selected) setSearchTerm(selected.name);
                }
            } catch (err) {
                console.error("Грешка при зареждане на сградите", err);
            }
        };
        fetchBuildings();
    }, [buildingId]);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    }

    const filteredBuildings = buildings.filter(b =>
        b.name.toLowerCase().includes(searchTerm.toLowerCase())
    );

    const handleBuildingSelect = (id: string, name: string) => {
        setFormData({ ...formData, buildingId: id });
        setSearchTerm(name);
        setIsDropdownOpen(false);
    };

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setMessage("");

        try {
            await api.post('/Apartment', {
                ...formData,
                floor: Number(formData.floor),
            });

            setMessage('✅ Апартаментът е създаден успешно!');
            onCreateApartamentRefreshList();
            setFormData({ number: '', floor: 0, buildingId: ''});
        } catch (error: unknown) {
            console.error(error);
            let errDetail = 'Възникна грешка при създаването.';
            if (axios.isAxiosError(error)) {
                errDetail = error.response?.data?.detail || error.response?.data?.title || errDetail;
            }
            setMessage(`❌ ${errDetail}`);
        }

    }


    return (
        <div style={{ marginTop: '30px', padding: '20px', border: '1px solid #ccc', borderRadius: '8px' }}>
            <h3>Добави нов апартамент (Само за Админи и Управители)</h3>

            {message && <div style={{ marginBottom: '15px', fontWeight: 'bold' }}>{message}</div>}

            <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '10px', maxWidth: '300px' }}>
                <input name="number" placeholder="Номер на апартамента" value={formData.number} onChange={handleChange} required />
                <input name="floor" placeholder="Етаж" value={formData.floor} onChange={handleChange} required />
                
                
                <div style={{ position: 'relative' }}>
                    <div style={{ position: 'relative', display: 'flex', alignItems: 'center' }}>
                        <span style={{ position: 'absolute', left: '10px', cursor: 'pointer' }}>🔍</span>
                        <input
                            type="text"
                            placeholder="Търси сграда по име..."
                            value={searchTerm}
                            disabled={!!buildingId}
                            
                            onChange={(e) => {
                                setSearchTerm(e.target.value);
                                setIsDropdownOpen(true);
                                // Изчистваме избраното ID, ако потребителят започне да трие и търси отново
                                if (formData.buildingId) setFormData({ ...formData, buildingId: "" });
                            }}
                            onFocus={() => setIsDropdownOpen(true)}
                            style={{ padding: '10px 10px 10px 35px', width: '100%', borderRadius: '4px', border: '1px solid #444', backgroundColor: '#333', color: 'white', boxSizing: 'border-box' }}
                        />
                    </div>

                    {/* Падащото меню с резултати */}
                    {isDropdownOpen && filteredBuildings.length > 0 && (
                        <ul style={{
                            position: 'absolute', top: '100%', left: 0, right: 0,
                            backgroundColor: '#fff', color: '#000',
                            border: '1px solid #ccc', borderRadius: '4px',
                            listStyle: 'none', padding: 0, margin: '5px 0 0 0',
                            maxHeight: '150px', overflowY: 'auto', zIndex: 1000
                        }}>
                            {filteredBuildings.map(b => (
                                <li
                                    key={b.id}
                                    onClick={() => handleBuildingSelect(b.id, b.name)}
                                    style={{ padding: '10px', cursor: 'pointer', borderBottom: '1px solid #eee' }}
                                    onMouseEnter={(e) => e.currentTarget.style.backgroundColor = '#f0f0f0'}
                                    onMouseLeave={(e) => e.currentTarget.style.backgroundColor = '#fff'}
                                >
                                    {b.name}
                                </li>
                            ))}
                        </ul>
                    )}
                    {isDropdownOpen && filteredBuildings.length === 0 && (
                        <div style={{ position: 'absolute', top: '100%', left: 0, right: 0, backgroundColor: '#fff', color: '#777', padding: '10px', border: '1px solid #ccc', borderRadius: '4px', zIndex: 1000 }}>
                            Няма намерени сгради.
                        </div>
                    )}
                </div>
                

                <button type="submit" style={{ padding: '8px', backgroundColor: '#007bff', color: 'white', border: 'none', cursor: 'pointer' }}>
                    Създай апартамент
                </button>
            </form>
        </div>
    );
}