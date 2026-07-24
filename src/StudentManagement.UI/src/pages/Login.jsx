import React, { useState } from 'react';
import { authService } from '../services/api';

export default function Login({ onLoginSuccess }) {
  const [isLoginView, setIsLoginView] = useState(true);
  const [email, setEmail] = useState('admin@zestindia.com');
  const [password, setPassword] = useState('Admin@123');
  const [username, setUsername] = useState('');
  const [role, setRole] = useState('Admin');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      if (isLoginView) {
        const response = await authService.login(email, password);
        if (response.success && response.data) {
          localStorage.removeItem('token');
          localStorage.removeItem('user');
          localStorage.setItem('token', response.data.token);
          localStorage.setItem('user', JSON.stringify({
            username: response.data.username,
            email: response.data.email,
            role: response.data.role
          }));
          onLoginSuccess(response.data);
        }
      } else {
        const response = await authService.register(username, email, password, role);
        if (response.success && response.data) {
          localStorage.setItem('token', response.data.token);
          localStorage.setItem('user', JSON.stringify({
            username: response.data.username,
            email: response.data.email,
            role: response.data.role
          }));
          onLoginSuccess(response.data);
        }
      }
    } catch (err) {
      console.error(err);
      if (err.errors && err.errors.length > 0) {
        setError(err.errors.join(', '));
      } else if (err.message) {
        setError(err.message);
      } else {
        setError('Authentication failed. Please check your credentials.');
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-container">
      <div className="auth-card p-4 p-md-5">
        <div className="text-center mb-4">
          <div className="d-inline-flex align-items-center justify-content-center bg-primary bg-opacity-10 text-primary p-3 rounded-circle mb-3">
            <i className="bi bi-mortarboard-fill fs-2"></i>
          </div>
          <h3 className="fw-bold">{isLoginView ? 'Welcome Back' : 'Create Account'}</h3>
          <p className="text-muted small">Student Management System • Zest India</p>
        </div>

        {error && (
          <div className="alert alert-danger py-2 small mb-3" role="alert">
            <i className="bi bi-exclamation-triangle-fill me-2"></i>
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit}>
          {!isLoginView && (
            <div className="mb-3">
              <label className="form-label small fw-semibold">Username</label>
              <input
                type="text"
                className="form-control"
                placeholder="Username"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                required
              />
            </div>
          )}

          <div className="mb-3">
            <label className="form-label small fw-semibold">Email Address</label>
            <input
              type="email"
              className="form-control"
              placeholder="name@example.com"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          </div>

          <div className="mb-3">
            <label className="form-label small fw-semibold">Password</label>
            <input
              type="password"
              className="form-control"
              placeholder="••••••••"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
          </div>

          {!isLoginView && (
            <div className="mb-3">
              <label className="form-label small fw-semibold">Assign Role</label>
              <select 
                className="form-select" 
                value={role} 
                onChange={(e) => setRole(e.target.value)}
              >
                <option value="Admin">Admin (Full Access)</option>
                <option value="User">User (Read Only)</option>
              </select>
            </div>
          )}

          <button
            type="submit"
            className="btn gradient-btn w-100 py-2 fw-semibold mb-3 d-flex align-items-center justify-content-center gap-2"
            disabled={loading}
          >
            {loading && <span className="spinner-border spinner-border-sm" role="status"></span>}
            {isLoginView ? 'Sign In' : 'Register Account'}
          </button>
        </form>

        <div className="text-center pt-2 border-top mt-3">
          <p className="small text-muted mb-0">
            {isLoginView ? "Don't have an account?" : "Already have an account?"}{' '}
            <button
              className="btn btn-link p-0 small fw-semibold text-primary text-decoration-none"
              onClick={() => {
                setIsLoginView(!isLoginView);
                setError('');
              }}
            >
              {isLoginView ? 'Register here' : 'Sign In'}
            </button>
          </p>
        </div>

        <div className="mt-4 p-3 bg-light rounded text-start fs-7 text-secondary">
          <div className="fw-bold mb-1 text-dark">Demo Credentials:</div>
          <div><strong>Admin:</strong> admin@zestindia.com / Admin@123</div>
          <div><strong>User:</strong> user@zestindia.com / User@123</div>
        </div>
      </div>
    </div>
  );
}
