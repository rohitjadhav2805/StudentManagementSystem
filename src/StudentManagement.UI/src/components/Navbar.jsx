import React from 'react';

export default function Navbar({ user, onLogout }) {
  return (
    <nav className="navbar navbar-expand-lg navbar-dark bg-dark shadow-sm">
      <div className="container">
        <a className="navbar-brand d-flex align-items-center gap-2 fw-bold" href="#">
          <i className="bi bi-mortarboard-fill text-primary"></i>
          <span>Zest Student Management</span>
        </a>
        <div className="d-flex align-items-center gap-3">
          {user && (
            <>
              <div className="d-flex align-items-center gap-2 text-light">
                <div className="avatar-badge">
                  {user.username ? user.username.charAt(0).toUpperCase() : 'U'}
                </div>
                <div className="d-none d-md-block text-end">
                  <div className="fw-semibold text-white">{user.username}</div>
                  <span className={`badge ${user.role === 'Admin' ? 'bg-danger' : 'bg-info'}`}>
                    {user.role}
                  </span>
                </div>
              </div>
              <button 
                className="btn btn-outline-light btn-sm d-flex align-items-center gap-1"
                onClick={onLogout}
              >
                <i className="bi bi-box-arrow-right"></i> Logout
              </button>
            </>
          )}
        </div>
      </div>
    </nav>
  );
}
