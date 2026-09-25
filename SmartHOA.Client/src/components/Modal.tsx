import React from 'react';

interface ModalProps {
    isOpen: boolean;
    onClose: () => void;
    title: string;
    children: React.ReactNode;
}

export default function Modal({ isOpen, onClose, title, children }: ModalProps) {
    if (!isOpen) return null; 
    
    return (
        <div style={{
            position: 'fixed', top: 0, left: 0, right: 0, bottom: 0,
            backgroundColor: 'rgba(0, 0, 0, 0.5)',
            display: 'flex', justifyContent: 'center', alignItems: 'center',
            zIndex: 1000
        }}>
            <div style={{
                backgroundColor: 'white',
                padding: '20px',
                borderRadius: '8px',
                minWidth: '300px',
                maxWidth: '500px',
                boxShadow: '0 4px 6px rgba(0,0,0,0.1)',
                position: 'relative'
            }}>
                <button
                    onClick={onClose}
                    style={{ position: 'absolute', top: '10px', right: '15px', background: 'none', border: 'none', fontSize: '18px', cursor: 'pointer', color: 'darkgray'}}
                >
                    ✕
                </button>

                <h3 style={{ marginTop: 0, marginBottom: '20px' }}>{title}</h3>
                
                {children}
            </div>
        </div>
    );
}