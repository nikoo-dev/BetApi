// ── Config ──
const API_BASE = "http://localhost:5103/api"; // change port to match your dotnet run output

// ── Token helpers ──
const getToken   = ()        => localStorage.getItem("token");
const setToken   = (t)       => localStorage.setItem("token", t);
const getRefresh = ()        => localStorage.getItem("refreshToken");
const setRefresh = (t)       => localStorage.setItem("refreshToken", t);
const getUser    = ()        => JSON.parse(localStorage.getItem("user") || "null");
const setUser    = (u)       => localStorage.setItem("user", JSON.stringify(u));

function clearAuth() {
  localStorage.removeItem("token");
  localStorage.removeItem("refreshToken");
  localStorage.removeItem("user");
}

// ── Redirect helpers ──
function requireAuth() {
  if (!getToken()) { window.location.href = "index.html"; }
}

function redirectIfAuth() {
  if (getToken()) { window.location.href = "games.html"; }
}

// ── API fetch wrapper ──
async function apiFetch(path, options = {}) {
  const headers = { "Content-Type": "application/json", ...options.headers };
  const token = getToken();
  if (token) headers["Authorization"] = `Bearer ${token}`;

  const res = await fetch(`${API_BASE}${path}`, { ...options, headers });

  if (res.status === 401) {
    // Try to refresh token
    const refreshed = await tryRefresh();
    if (refreshed) {
      headers["Authorization"] = `Bearer ${getToken()}`;
      const retry = await fetch(`${API_BASE}${path}`, { ...options, headers });
      return retry;
    } else {
      clearAuth();
      window.location.href = "index.html";
      return;
    }
  }

  return res;
}

// ── Refresh token ──
async function tryRefresh() {
  const refreshToken = getRefresh();
  if (!refreshToken) return false;

  try {
    const res = await fetch(`${API_BASE}/auth/refresh`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ refreshToken })
    });

    if (!res.ok) return false;

    const data = await res.json();
    setToken(data.token);
    setRefresh(data.refreshToken);
    return true;
  } catch {
    return false;
  }
}

// ── Logout ──
async function logout() {
  const refreshToken = getRefresh();
  if (refreshToken) {
    try {
      await fetch(`${API_BASE}/auth/revoke`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ refreshToken })
      });
    } catch { /* ignore */ }
  }
  clearAuth();
  window.location.href = "index.html";
}

// ── Navbar balance ──
async function loadBalance() {
  const badge = document.getElementById("balanceBadge");
  if (!badge) return;

  try {
    const res  = await apiFetch("/users/me/balance");
    const data = await res.json();
    badge.textContent = `💰 ${parseFloat(data.balance).toFixed(2)} GEL`;
  } catch {
    badge.textContent = "💰 --";
  }
}

// ── Format date ──
function formatDate(iso) {
  const d = new Date(iso);
  return d.toLocaleDateString("en-GB", {
    day: "2-digit", month: "short", year: "numeric",
    hour: "2-digit", minute: "2-digit"
  });
}

// ── Show alert ──
function showAlert(id, message, type = "error") {
  const el = document.getElementById(id);
  if (!el) return;
  el.className = `alert alert-${type} show`;
  el.textContent = message;
  setTimeout(() => el.classList.remove("show"), 4000);
}
