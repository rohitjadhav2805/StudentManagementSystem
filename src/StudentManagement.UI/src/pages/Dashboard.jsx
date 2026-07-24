import React, { useState, useEffect } from 'react';
import Navbar from '../components/Navbar';
import StudentModal from '../components/StudentModal';
import { studentService } from '../services/api';

export default function Dashboard({ user, onLogout }) {
  const [students, setStudents] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [toastMessage, setToastMessage] = useState('');
  
  // Search & Filter
  const [searchTerm, setSearchTerm] = useState('');
  const [courseFilter, setCourseFilter] = useState('');

  // Modal State
  const [showModal, setShowModal] = useState(false);
  const [selectedStudent, setSelectedStudent] = useState(null);

  const isAdmin = user?.role === 'Admin';

  const fetchStudents = async () => {
    setLoading(true);
    setError('');
    try {
      let res;
      if (searchTerm || courseFilter) {
        res = await studentService.search(searchTerm, courseFilter);
      } else {
        res = await studentService.getAll();
      }
      if (res.success) {
        setStudents(res.data || []);
      }
    } catch (err) {
      console.error(err);
      if (err.statusCode === 401 || err.message?.includes('Unauthorized') || err.message?.includes('JWT')) {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        onLogout();
        return;
      }
      setError(err.message || 'Failed to load students list.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchStudents();
  }, [searchTerm, courseFilter]);

  const showToast = (msg) => {
    setToastMessage(msg);
    setTimeout(() => setToastMessage(''), 4000);
  };

  const handleCreateOrUpdate = async (formData) => {
    try {
      if (selectedStudent) {
        const res = await studentService.update(selectedStudent.id, formData);
        if (res.success) {
          showToast('Student details updated successfully!');
        }
      } else {
        const res = await studentService.create(formData);
        if (res.success) {
          showToast('New student created successfully!');
        }
      }
      setShowModal(false);
      setSelectedStudent(null);
      fetchStudents();
    } catch (err) {
      alert(err.message || (err.errors ? err.errors.join(', ') : 'Operation failed'));
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this student record?')) return;
    try {
      const res = await studentService.delete(id);
      if (res.success) {
        showToast('Student deleted successfully!');
        fetchStudents();
      }
    } catch (err) {
      alert(err.message || 'Failed to delete student.');
    }
  };

  return (
    <div>
      <Navbar user={user} onLogout={onLogout} />

      <div className="container py-4">
        {/* Toast Alert */}
        {toastMessage && (
          <div className="alert alert-success alert-dismissible fade show card-custom mb-4" role="alert">
            <i className="bi bi-check-circle-fill me-2"></i>
            {toastMessage}
          </div>
        )}

        {/* Header Stats & Controls */}
        <div className="row g-3 mb-4">
          <div className="col-md-4">
            <div className="card card-custom p-3 bg-white h-100 d-flex flex-row align-items-center gap-3">
              <div className="p-3 bg-primary bg-opacity-10 text-primary rounded-3">
                <i className="bi bi-people-fill fs-3"></i>
              </div>
              <div>
                <div className="text-muted small fw-semibold">Total Students</div>
                <div className="fs-3 fw-bold">{students.length}</div>
              </div>
            </div>
          </div>
          <div className="col-md-4">
            <div className="card card-custom p-3 bg-white h-100 d-flex flex-row align-items-center gap-3">
              <div className="p-3 bg-success bg-opacity-10 text-success rounded-3">
                <i className="bi bi-journal-bookmark-fill fs-3"></i>
              </div>
              <div>
                <div className="text-muted small fw-semibold">Active Courses</div>
                <div className="fs-3 fw-bold">
                  {new Set(students.map(s => s.course)).size}
                </div>
              </div>
            </div>
          </div>
          <div className="col-md-4">
            <div className="card card-custom p-3 bg-white h-100 d-flex flex-row align-items-center gap-3">
              <div className="p-3 bg-warning bg-opacity-10 text-warning rounded-3">
                <i className="bi bi-shield-lock-fill fs-3"></i>
              </div>
              <div>
                <div className="text-muted small fw-semibold">Current Role</div>
                <div className="fs-5 fw-bold text-dark">{user?.role}</div>
              </div>
            </div>
          </div>
        </div>

        {/* Action Bar */}
        <div className="card card-custom p-3 mb-4 bg-white">
          <div className="row g-3 align-items-center">
            <div className="col-md-5">
              <div className="input-group">
                <span className="input-group-text bg-light border-end-0">
                  <i className="bi bi-search"></i>
                </span>
                <input
                  type="text"
                  className="form-control border-start-0 bg-light"
                  placeholder="Search by name or email..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                />
              </div>
            </div>
            <div className="col-md-4">
              <input
                type="text"
                className="form-control bg-light"
                placeholder="Filter by course..."
                value={courseFilter}
                onChange={(e) => setCourseFilter(e.target.value)}
              />
            </div>
            <div className="col-md-3 text-md-end">
              {isAdmin ? (
                <button
                  className="btn gradient-btn w-100 d-flex align-items-center justify-content-center gap-2"
                  onClick={() => {
                    setSelectedStudent(null);
                    setShowModal(true);
                  }}
                >
                  <i className="bi bi-plus-circle-fill"></i> Add New Student
                </button>
              ) : (
                <span className="badge bg-secondary p-2 w-100">
                  <i className="bi bi-eye-fill me-1"></i> Read Only View
                </span>
              )}
            </div>
          </div>
        </div>

        {/* Students Data Table */}
        <div className="card card-custom bg-white">
          <div className="card-header bg-white border-0 py-3 d-flex justify-content-between align-items-center">
            <h5 className="fw-bold mb-0">Student Directory</h5>
            <button className="btn btn-outline-secondary btn-sm" onClick={fetchStudents}>
              <i className="bi bi-arrow-clockwise me-1"></i> Refresh
            </button>
          </div>
          <div className="table-responsive">
            {loading ? (
              <div className="text-center py-5">
                <div className="spinner-border text-primary" role="status"></div>
                <div className="mt-2 text-muted">Loading records...</div>
              </div>
            ) : error ? (
              <div className="alert alert-danger m-3">{error}</div>
            ) : students.length === 0 ? (
              <div className="text-center py-5 text-muted">
                <i className="bi bi-inbox fs-1 d-block mb-2"></i>
                No student records found.
              </div>
            ) : (
              <table className="table table-hover align-middle mb-0">
                <thead className="table-light">
                  <tr>
                    <th>#ID</th>
                    <th>Student Name</th>
                    <th>Email Address</th>
                    <th>Age</th>
                    <th>Course</th>
                    <th>Enrolled Date</th>
                    {isAdmin && <th className="text-end">Actions</th>}
                  </tr>
                </thead>
                <tbody>
                  {students.map((student) => (
                    <tr key={student.id}>
                      <td className="fw-bold text-secondary">#{student.id}</td>
                      <td>
                        <div className="fw-semibold text-dark">{student.name}</div>
                      </td>
                      <td>
                        <a href={`mailto:${student.email}`} className="text-decoration-none text-muted">
                          {student.email}
                        </a>
                      </td>
                      <td>
                        <span className="badge bg-light text-dark border">
                          {student.age} yrs
                        </span>
                      </td>
                      <td>
                        <span className="badge bg-primary bg-opacity-10 text-primary">
                          {student.course}
                        </span>
                      </td>
                      <td className="small text-muted">
                        {new Date(student.createdDate).toLocaleDateString()}
                      </td>
                      {isAdmin && (
                        <td className="text-end">
                          <button
                            className="btn btn-outline-primary btn-sm me-2"
                            onClick={() => {
                              setSelectedStudent(student);
                              setShowModal(true);
                            }}
                            title="Edit Student"
                          >
                            <i className="bi bi-pencil-square"></i>
                          </button>
                          <button
                            className="btn btn-outline-danger btn-sm"
                            onClick={() => handleDelete(student.id)}
                            title="Delete Student"
                          >
                            <i className="bi bi-trash-fill"></i>
                          </button>
                        </td>
                      )}
                    </tr>
                  ))}
                </tbody>
              </table>
            )}
          </div>
        </div>
      </div>

      <StudentModal
        show={showModal}
        onClose={() => {
          setShowModal(false);
          setSelectedStudent(null);
        }}
        onSave={handleCreateOrUpdate}
        student={selectedStudent}
      />
    </div>
  );
}
