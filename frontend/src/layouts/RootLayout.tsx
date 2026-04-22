import { Link, NavLink, Outlet } from "react-router-dom";

export default function RootLayout() {
  return (
    <div className="min-h-screen flex flex-col bg-base-200">
      <header className="navbar bg-base-100 shadow-sm">
        <div className="flex-1">
          <Link to="/" className="btn btn-ghost text-xl">
            ChallengeTracker
          </Link>
        </div>
        <nav className="flex-none gap-2">
          <NavLink to="/create" className="btn btn-primary btn-sm">
            New challenge
          </NavLink>
          <NavLink to="/login" className="btn btn-ghost btn-sm">
            Log in
          </NavLink>
        </nav>
      </header>

      <main className="flex-1 container mx-auto px-4 py-6">
        <Outlet />
      </main>

      <footer className="footer footer-center p-4 text-sm opacity-70">
        Tracker Trio · ChallengeTracker
      </footer>
    </div>
  );
}
