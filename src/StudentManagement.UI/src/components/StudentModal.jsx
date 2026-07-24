import React, { useState, useEffect } from 'react';

export default function StudentModal({ show, onClose, onSave, student }) {
  const [formData, setFormData] = useState({
    name: '',
    email: '',
    age: '',
    course: ''
  });
  const [error, setError] = useState('');

  useEffect(() => {
    if (student) {
      setFormData({
        name: student.name || '',
        email: student.email || '',
        age: student.age || '',
        course: student.course || ''
      });
    } else {
      setFormData({ name: '', email: '', age: '', course: '' });
    }
    setError('');
  }, [student, show]);

  if (!show) return null;

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!formData.name || !formData.email || !formData.age || !formData.course) {
      setError('All fields are required.');
      return;
    }
    if (parseInt(formData.age, 10) <= 0) {
      setError('Age must be greater than 0.');
      return;
    }
    onSave({
      ...formData,
      age: parseInt(formData.age, 10)
    });
  };

  return (
    <div className="modal show d-block" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }} tabIndex="-1">
      <div className="modal-dialog modal-dialog-centered">
        <div className="modal-content card-custom">
          <div className="modal-header border-0 pb-0">
            <h5 className="modal-title fw-bold">
              {student ? 'Edit Student Details' : 'Add New Student'}
            </h5>
            <button type="button" className="btn-close" onClick={onClose}></button>
          </div>
          <form onSubmit={handleSubmit}>
            <div className="modal-body">
              {error && (
                <div className="alert alert-danger py-2" role="alert">
                  {error}
                </div>
              )}
              <div className="mb-3">
                <label className="form-label fw-semibold">Full Name</label>
                <input
                  type="text"
                  className="form-control"
                  placeholder="e.g. Rahul Sharma"
                  value={formData.name}
                  onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                  required
                />
              </div>
              <div className="mb-3">
                <label className="form-label fw-semibold">Email Address</label>
                <input
                  type="email"
                  className="form-control"
                  placeholder="e.g. rahul@example.com"
                  value={formData.email}
                  onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                  required
                />
              </div>
              <div className="mb-3">
                <label className="form-label fw-semibold">Age</label>
                <input
                  type="number"
                  className="form-control"
                  placeholder="e.g. 21"
                  value={formData.age}
                  onChange={(e) => setFormData({ ...formData, age: e.target.value })}
                  required
                  min="1"
                />
              </div>
              <div className="mb-3">
                <label className="form-label fw-semibold">Course</label>
                <input
                  type="text"
                  className="form-control"
                  placeholder="e.g. Computer Science"
                  value={formData.course}
                  onChange={(e) => setFormData({ ...formData, course: e.target.value })}
                  required
                />
              </div>
            </div>
            <div className="modal-footer border-0 pt-0">
              <button type="button" className="btn btn-secondary" onClick={onClose}>
                Cancel
              </button>
              <button type="submit" className="btn gradient-btn px-4">
                {student ? 'Save Changes' : 'Create Student'}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
